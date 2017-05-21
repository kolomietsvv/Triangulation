using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Triangulation
{
    public class Program
    {
        // Список цветов
        public static List<Bgr> Colors = new List<Bgr>(new[] { new Bgr(
            Color.Gray), new Bgr(Color.Orange), new Bgr(Color.Blue), new Bgr(Color.Yellow), new Bgr(Color.Red),
            new Bgr(Color.RoyalBlue),new Bgr(Color.LawnGreen),
            new Bgr(Color.ForestGreen), new Bgr(Color.DarkGreen), new Bgr(Color.DarkBlue), new Bgr(Color.Cyan)});

        // Переменные и сервисы, необходимые для корректного отображения курсора
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr LoadCursorFromFile(string fileName);

        public static Cursor GrabCursor;
        public static Cursor GrabbingCursor;

        [STAThread]
        private static void Main(string[] args)
        {
            // Инициализация курсоров
            IntPtr handleGrab = LoadCursorFromFile(@"..\..\Resources\grab.cur");
            GrabCursor = new Cursor(handleGrab);

            IntPtr handleGrabbing = LoadCursorFromFile(@"..\..\Resources\grabbing.cur");
            GrabbingCursor = new Cursor(handleGrabbing);

            // Создание основной формы и подписка на события
            var mainForm = new MainForm();
            mainForm.OpenToolStripMenuItem.Click += OpenBtnClick;
            mainForm.SaveAsToolStripMenuItem.Click += SaveAsBtnClick;
            mainForm.ClearToolStripMenuItem.Click += ClearBtnClick;
            mainForm.MouseWheel += Zoom;
            mainForm.ImgBox.MouseMove += OnMouseMove;
            mainForm.ImgBox.MouseDoubleClick += UnZoom;
            mainForm.ImgBox.MouseDown += RememberMouseDownLocation;
            mainForm.ImgBox.MouseClick += SelectColor;
            mainForm.ImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            foreach (ToolStripMenuItem item in mainForm.ColorsMenu.Items)
                item.Click += ChangeColor;
            mainForm.Text = "Delaunay triangulation (Main window)";
            Application.Run(mainForm);
        }

        // Функция для удаления изображения
        private static void ClearBtnClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var menu = menuItem.OwnerItem.Owner;
            var form = menu.Parent as MainForm;

            form.scale = 200;
            form.nodes = null;
            form.trianglesData = null;
            form.edges = null;
            form.img = null;
            form.ImgBox.Image = null;
            form.ROIOffset = Point.Empty;
            form.TrianglesCount.Text = string.Empty;
            form.CoordinateX.Text = string.Empty;
            form.CoordinateY.Text = string.Empty;
            form.EdgesCount.Text = string.Empty;
            form.NodesCount.Text = string.Empty;
            form.MinTriangleCount.Text = string.Empty;
            form.MaxTriangleCount.Text = string.Empty;
        }

        // Функция для изменения цвета треугольников. 
        private static void ChangeColor(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var menu = menuItem.Owner as ContextMenuStrip;
            var form = menu.SourceControl.Parent as MainForm;

            var newColorId = int.Parse(menuItem.Text);
            form.trianglesData = form.trianglesData.Select(t => ChangeColor(t, form.selectedColorId, newColorId)).ToList();

            var roi = form.img.ROI;
            form.img = GetImg(form);
            form.img.ROI = roi;
            form.ImgBox.Image = form.img;
        }
        // Перегрузка функции ChangeColor, заменяет цвет конкретного треугольника, если его текущий цвет равен currentColorId
        private static TriangleData ChangeColor(TriangleData triangleData, int currentColorId, int newColorId)
        {
            if (triangleData.colorID == currentColorId)
                triangleData.colorID = newColorId;
            return triangleData;
        }
        // Функция, задающая действия при движении мыши по изображению
        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;
            if (form.ImgBox.Image == null) return;

            if (form.img.IsROISet)
                Cursor.Current = GrabCursor;
            else
                Cursor.Current = Cursors.Default;

            SelectROI(form, e);
            ShowCoordinates(form, e);
        }
        //Функция, которая отображает координаты X и Y на нижней панели
        private static void ShowCoordinates(MainForm form, MouseEventArgs e)
        {
            var minX = form.nodes.Min(p => p.X);

            double decartX, decartY;
            if (form.img.IsROISet)
            {
                var maxY = form.nodes.Max(p => p.Y);
                decartX = (e.X + form.img.ROI.X) / (double)form.scale + minX;
                decartY = maxY - (e.Y + form.img.ROI.Y) / (double)form.scale;
            }
            else
            {
                var minY = form.nodes.Min(p => p.Y);
                decartX = (e.X - (form.ImgBox.Width - form.img.Width) / 2d) / form.scale + minX;
                decartY = (form.ImgBox.Height - (e.Y + (form.ImgBox.Height - form.img.Height) / 2d)) / form.scale + minY;
            }

            form.CoordinateX.Text = decartX.ToString("f6");
            form.CoordinateY.Text = decartY.ToString("f6");
        }
        // Функция, которая позволяет перемещаться по изображению, когда оно приближено и не помещается в форме
        private static void SelectROI(MainForm form, MouseEventArgs eventArgs)
        {
            if (!eventArgs.Button.HasFlag(MouseButtons.Left)) return;
            if (!form.img.IsROISet) return;

            Cursor.Current = GrabbingCursor;

            form.ROIOffset = new Point(
                (form.mouseDownLocation.X - eventArgs.X),
                (form.mouseDownLocation.Y - eventArgs.Y));

            var previousROI = form.img.ROI;

            var newX = previousROI.X + form.ROIOffset.X;
            var newY = previousROI.Y + form.ROIOffset.Y;
            newX = newX > 0 ? newX : 0;
            newY = newY > 0 ? newY : 0;

            form.img.ROI = Rectangle.Empty;
            var roi = new Rectangle(
                    newX, newY,
                    Math.Min(form.ImgBox.Width, form.img.Width),
                    Math.Min(form.ImgBox.Height, form.img.Height));

            form.img.ROI = Rectangle.Empty;
            if (roi.X >= 0 && roi.Y >= 0 && roi.X + roi.Width <= form.img.Width && roi.Y + roi.Height <= form.img.Height)
                form.img.ROI = roi;
            else
                form.img.ROI = previousROI;

            form.ImgBox.Image = form.img;

            form.mouseDownLocation = eventArgs.Location;
        }
        // Функция вызывает выпадающее меню со списком цветов по нажатию правой кнопки мыши
        private static void SelectColor(object sender, MouseEventArgs eventArgs)
        {
            if (!eventArgs.Button.HasFlag(MouseButtons.Right)) return;

            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;

            if (imgbox.Image == null) return;

            var trinagles = GetTriangles(form.nodes, form.trianglesData);
            var pointToCheck = new PointF(float.Parse(form.CoordinateX.Text), float.Parse(form.CoordinateY.Text));
            var triangle = trinagles.Find(t => CheckPointInTriangle(t.Key.V0, t.Key.V1, t.Key.V2, pointToCheck));

            if (triangle.Value.Blue == 0 && triangle.Value.Green == 0 && triangle.Value.Red == 0) return;
            form.selectedColorId = Colors.IndexOf(triangle.Value);

            form.ColorsMenu.Show(imgbox, eventArgs.Location);
        }
        // Функция запоминает положение мыши при зажатии левой кнопки
        private static void RememberMouseDownLocation(object sender, MouseEventArgs eventArgs)
        {
            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;
            if (form.img == null) return;
            if (!form.img.IsROISet) return;

            Cursor.Current = GrabbingCursor;
            form.mouseDownLocation = eventArgs.Location;
        }
        // Функция для приближения/отдаления изображения
        private static void Zoom(object sender, MouseEventArgs eventArgs)
        {
            var form = sender as MainForm;
            if (form == null) return;
            if (eventArgs.Delta < 0 && form.scale <= 50) return;
            if (!form.trianglesData.Any() || !form.nodes.Any()) return;

            var previousScale = form.scale;
            if (eventArgs.Delta > 0)
                form.scale += 20;
            else if (eventArgs.Delta < 0)
                form.scale -= 20;
            else return;

            var ratio = form.scale / (double)previousScale;

            var previousROI = form.img.ROI;

            form.img = GetImg(form);

            if (form.img.Width > form.ImgBox.Width || form.img.Height > form.ImgBox.Height)
            {
                var roi = Rectangle.Empty;
                var newX = 0;
                var newY = 0;

                if (form.ROIOffset.IsEmpty)
                {
                    newX = (form.img.Width - form.ImgBox.Width) / 2;
                    newY = (form.img.Height - form.ImgBox.Height) / 2;
                }
                else
                {
                    newX = (int)(previousROI.X * ratio);
                    newY = (int)(previousROI.Y * ratio);
                }

                roi = new Rectangle(
                    newX > 0 ? newX : 0,
                    newY > 0 ? newY : 0,
                    Math.Min(form.ImgBox.Width, form.img.Width),
                    Math.Min(form.ImgBox.Height, form.img.Height));

                if (roi.X >= 0 && roi.Y >= 0 && roi.X + roi.Width <= form.img.Width && roi.Y + roi.Height <= form.img.Height)
                    form.img.ROI = roi;
            }

            form.ImgBox.Image = form.img;

            ShowCoordinates(form, eventArgs);
        }
        // Функция, которая возвращает изображение к начальному размеру
        private static void UnZoom(object sender, EventArgs eventArgs)
        {
            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;
            if (!form.trianglesData.Any() || !form.nodes.Any()) return;
            form.scale = 200;

            form.img = GetImg(form);
            form.ImgBox.Image = form.img;
            form.ROIOffset = Point.Empty;
            form.Width = form.img.Size.Width + 20;
            form.Height = form.img.Size.Height + 100;
        }
        // Функция для сохранения цвета треугольников в файл
        private static void SaveTriangulationData(string path, List<TriangleData> data)
        {
            var index = 0;
            var lines = data.Select(d => { index++; return $"   {index}     {d.v1}   {d.v2}   {d.v3}  {d.colorID}"; });
            File.WriteAllText(path, $"{data.Count}  3  1{Environment.NewLine}");
            File.AppendAllLines(path, lines);
        }
        // Функция для сохранения информации о изображении или самого изображения
        private static void SaveAsBtnClick(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as ToolStripMenuItem;
            var menu = menuItem.OwnerItem.Owner;
            var form = menu.Parent as MainForm;

            if (form == null) return;
            if (form.img == null) return;

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Triangulation File(*.ele)| *.ele;|Image | *.gif; *.jpeg; *.png; *.bmp";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                if (Path.GetExtension(saveFileDialog.FileName) == ".ELE")
                    SaveTriangulationData(saveFileDialog.FileName, form.trianglesData);
                else
                    form.img.Save(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Функция для считывания данный из файлов с расширением .ele .edge .node 
        private static bool OpenFile(MainForm form)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Triangulation Files(*.ele; *.edge; *.node)| *.ele; *.edge; *.node";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return false;

            try
            {
                var path = openFileDialog.FileName;
                var extensionIndex = path.LastIndexOf('.');
                path = path.Substring(0, extensionIndex);

                form.edges = GetEdgesFromFile(path + ".edge");
                form.nodes = GetNodesFromFile(path + ".node");
                form.trianglesData = GetTrianglesFromFile(path + ".ele");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: Could not read file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        // Отрисовка изображения на форме
        private static bool BindImageToForm(MainForm form)
        {
            if (!OpenFile(form)) return false;
            form.img = GetImg(form);
            SetFooter(form);
            form.ImgBox.Image = form.img;
            form.Width = form.img.Size.Width + 20;
            form.Height = form.img.Size.Height + 100;
            return true;
        }
        // Обработчик нажатия на кнопку Файл->Открыть. Открывает изображение в текущей форме, если его нет, в противном случае 
        // получает ответ от пользователя, открыть ли файл в новом окне и выполняет соответствующее действие
        private static void OpenBtnClick(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as ToolStripMenuItem;
            var menu = menuItem.OwnerItem.Owner;
            var form = menu.Parent as MainForm;
            if (form.ImgBox.Image == null)
            {
                BindImageToForm(form);
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Открыть в новом окне?", "Открыть файл", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    MainForm secondForm = new MainForm();
                    if (!BindImageToForm(secondForm)) return;
                    secondForm.OpenToolStripMenuItem.Click += OpenBtnClick;
                    secondForm.SaveAsToolStripMenuItem.Click += SaveAsBtnClick;
                    secondForm.ClearToolStripMenuItem.Click += ClearBtnClick;
                    secondForm.MouseWheel += Zoom;
                    secondForm.ImgBox.MouseMove += OnMouseMove;
                    secondForm.ImgBox.MouseDoubleClick += UnZoom;
                    secondForm.ImgBox.MouseDown += RememberMouseDownLocation;
                    secondForm.ImgBox.MouseClick += SelectColor;
                    secondForm.ImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
                    foreach (ToolStripMenuItem item in secondForm.ColorsMenu.Items)
                        item.Click += ChangeColor;
                    secondForm.Text = "Delaunay triangulation";
                    secondForm.Show();
                }
                else if (dialogResult == DialogResult.No)
                {
                    BindImageToForm(form);
                }
            }
        }
        // Функция для выведения статичных данных на нижнюю панель
        private static void SetFooter(MainForm form)
        {
            form.TrianglesCount.Text = form.trianglesData.Count().ToString();
            form.EdgesCount.Text = form.edges.Count().ToString();
            form.NodesCount.Text = form.nodes.Count().ToString();
            var trianglesSizes = CalculateTriangleSizes(form);
            form.MinTriangleCount.Text = trianglesSizes.Min().ToString("f6");
            form.MaxTriangleCount.Text = trianglesSizes.Max().ToString("f6");
        }
        // Функция для приведения файловых координат к экранным
        private static Image<Bgr, byte> GetImg(MainForm form)
        {
            var minX = form.nodes.Min(p => p.X * form.scale);
            var minY = form.nodes.Min(p => p.Y * form.scale);
            var maxX = form.nodes.Max(p => p.X * form.scale) - minX;
            var maxY = form.nodes.Max(p => p.Y * form.scale) - minY;
            var screenPoints = form.nodes.Select(p => new PointF(p.X * form.scale - minX, maxY - (p.Y * form.scale - minY))).ToList();

            var triangles = GetTriangles(screenPoints, form.trianglesData);
            var img = FEMMesh(triangles, (int)maxX, (int)maxY);
            return img;
        }
        // Функция для отрисовки сетки
        private static Image<Bgr, byte> FEMMesh(List<KeyValuePair<Triangle2DF, Bgr>> triangles, int maxX, int maxY)
        {
            var white = new Bgr(255, 255, 255);

            var img = new Image<Bgr, byte>(maxX + 1, maxY + 1);

            foreach (var triangle in triangles)
            {
                img.Draw(triangle.Key, triangle.Value, -1);
                img.Draw(triangle.Key, white, 1);
            }
            return img;
        }
        // Функция для получения списка вершин из файла
        private static List<PointF> GetNodesFromFile(string path)
        {
            var fileContent = File.ReadAllLines(path);
            var nodesCount = int.Parse(fileContent[0].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var nodes = new List<PointF>(nodesCount);

            for (int i = 1; i <= nodesCount; i++)
            {
                var lineContent = fileContent[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(lineContent[0]);
                var x = float.Parse(lineContent[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                var y = float.Parse(lineContent[2], NumberStyles.Any, CultureInfo.InvariantCulture);
                var point = new PointF(x, y);
                nodes.Add(point);
            }
            return nodes;
        }
        // Функция для получения списка ребер из файла
        private static List<Point> GetEdgesFromFile(string path)
        {
            var fileContent = File.ReadAllLines(path);
            var edgesCount = int.Parse(fileContent[0].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var edges = new List<Point>(edgesCount);

            for (int i = 1; i <= edgesCount; i++)
            {
                var lineContent = fileContent[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(lineContent[0]);
                var pointFrom = int.Parse(lineContent[1]);
                var pointTo = int.Parse(lineContent[2]);
                var point = new Point(pointFrom - 1, pointTo - 1);
                edges.Add(point);
            }
            return edges;
        }
        // Функция для получения списка треугольников из файла
        private static List<TriangleData> GetTrianglesFromFile(string path)
        {
            var fileContent = File.ReadAllLines(path);
            var trianglesCount = int.Parse(fileContent[0].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var triangles = new List<TriangleData>(trianglesCount);

            for (int i = 1; i <= trianglesCount; i++)
            {
                var lineContent = fileContent[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(lineContent[0]);
                var point1 = int.Parse(lineContent[1]);
                var point2 = int.Parse(lineContent[2]);
                var point3 = int.Parse(lineContent[3]);
                var colorID = int.Parse(lineContent[4]);

                var triangleData = new TriangleData(point1, point2, point3, colorID);
                triangles.Add(triangleData);
            }
            return triangles;
        }
        // Функция для получения треугольников в удобном для программиста виде
        private static List<KeyValuePair<Triangle2DF, Bgr>> GetTriangles(List<PointF> points, List<TriangleData> trianglesData)
        {
            var trianglesCount = trianglesData.Count;
            var triangles = new List<KeyValuePair<Triangle2DF, Bgr>>(trianglesCount);

            for (int i = 0; i < trianglesCount; i++)
            {
                var triangle = new Triangle2DF(points[trianglesData[i].v1 - 1], points[trianglesData[i].v2 - 1], points[trianglesData[i].v3 - 1]);
                var color = Colors[trianglesData[i].colorID % (Colors.Count)];
                triangles.Add(new KeyValuePair<Triangle2DF, Bgr>(triangle, color));
            }
            return triangles;
        }
        // Функция для вычисления размеров всех треугольников
        private static List<double> CalculateTriangleSizes(MainForm form)
        {
            var trianglesSizes = new List<double>(form.trianglesData.Count);
            foreach (var triangle in form.trianglesData)
            {
                var edge1 = CalculateEdgeLength(form.nodes[triangle.v1 - 1], form.nodes[triangle.v2 - 1]);
                var edge2 = CalculateEdgeLength(form.nodes[triangle.v2 - 1], form.nodes[triangle.v3 - 1]);
                var edge3 = CalculateEdgeLength(form.nodes[triangle.v3 - 1], form.nodes[triangle.v1 - 1]);

                trianglesSizes.Add(Square(edge1, edge2, edge3));
            }
            return trianglesSizes;
        }
        // Функция для вычисления площади треугольника
        private static double Square(double edge1, double edge2, double edge3)
        {
            var halfPer = (edge1 + edge2 + edge3) / 2;
            return Math.Sqrt(halfPer * (halfPer - edge1) * (halfPer - edge2) * (halfPer - edge3));
        }
        // Функция для вычисления длины стороны треугольника
        private static double CalculateEdgeLength(PointF node1, PointF node2)
        {
            var XDifference = node1.X - node2.X;
            var YDifference = node1.Y - node2.Y;
            return Math.Sqrt(XDifference * XDifference + YDifference * YDifference);
        }
        // Функция для проверки принадлежности точки треугольнику
        private static bool CheckPointInTriangle(PointF v1, PointF v2, PointF v3, PointF pointToCheck)
        {
            var v2x = v2.X - v1.X;
            var v3x = v3.X - v1.X;
            var pointToCheckX = pointToCheck.X - v1.X;

            var v2y = v2.Y - v1.Y;
            var v3y = v3.Y - v1.Y;
            var pointToCheckY = pointToCheck.Y - v1.Y;


            var mu = (pointToCheckX * v2y - v2x * pointToCheckY) / (v3x * v2y - v2x * v3y);
            if (mu >= 0 && mu <= 1)
            {
                var lambda = (pointToCheckX - mu * v3x) / v2x;
                return lambda >= 0 && mu + lambda <= 1;
            }
            return false;
        }
        // Структура для хранения данных о треугольниках
        public struct TriangleData
        {
            public TriangleData(int v1, int v2, int v3, int colorID)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
                this.colorID = colorID;
            }
            public int v1, v2, v3, colorID;
        }
    }
}