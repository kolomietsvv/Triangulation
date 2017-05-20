using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Triangulation
{
    public class Program
    {
        public static List<Bgr> Colors = new List<Bgr>(new[] { new Bgr(
            Color.Gray), new Bgr(Color.Orange), new Bgr(Color.Blue), new Bgr(Color.Yellow), new Bgr(Color.Red),
            new Bgr(Color.RoyalBlue),new Bgr(Color.LawnGreen),
            new Bgr(Color.ForestGreen), new Bgr(Color.DarkGreen), new Bgr(Color.DarkBlue), new Bgr(Color.Cyan)});

        [STAThread]
        private static void Main(string[] args)
        {
            var mainForm = new MainForm();
            mainForm.edges = new List<Point>();
            mainForm.nodes = new List<PointF>();
            mainForm.trianglesData = new List<TriangleData>();
            mainForm.OpenToolStripMenuItem.Click += OpemBtnClick;
            mainForm.SaveAsToolStripMenuItem.Click += SaveAsBtnClick;
            mainForm.MouseWheel += Zoom;
            mainForm.ImgBox.MouseMove += ShowCoordinates;
            mainForm.ImgBox.MouseDoubleClick += UnZoom;
            mainForm.ImgBox.MouseMove += SelectROI;
            mainForm.ImgBox.MouseDown += RememberMouseDownLocation;
            mainForm.ImgBox.MouseClick += SelectColor;
            mainForm.ImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            foreach (ToolStripMenuItem item in mainForm.ColorsMenu.Items)
                item.Click += ChangeColor;
            mainForm.Text = "Delaunay triangulation";
            Application.Run(mainForm);
        }

        private static void ChangeColor(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var menu = menuItem.Owner as ContextMenuStrip;
            var form = menu.SourceControl.Parent as MainForm;

            var newColorId = int.Parse(menuItem.Text);
            form.trianglesData = form.trianglesData.Select(t => ChangeColor(t, form.selectedColorId, newColorId)).ToList();

            form.img = GetImg(form);
            form.ImgBox.Image = form.img;
        }

        private static TriangleData ChangeColor(TriangleData triangleData, int currentColorId, int newColorId)
        {
            if (triangleData.colorID == currentColorId)
                triangleData.colorID = newColorId;
            return triangleData;
        }

        private static void ShowCoordinates(object sender, MouseEventArgs e)
        {
            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;

            if (imgbox.Image == null) return;

            var minX = form.nodes.Min(p => p.X);
            var minY = form.nodes.Min(p => p.Y);
            var decartX = (e.X - (form.ImgBox.Width - form.img.Width) / 2d) / form.scale + minX;
            var decartY = (form.ImgBox.Height - (e.Y + (form.ImgBox.Height - form.img.Height) / 2d)) / form.scale + minY;
            form.CoordinateX.Text = decartX.ToString("f6");
            form.CoordinateY.Text = decartY.ToString("f6");

        }

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

        private static void RememberMouseDownLocation(object sender, MouseEventArgs eventArgs)
        {
            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;
            if (form.img == null) return;
            if (form.img.Size.Width <= form.ImgBox.Width && form.img.Size.Height <= form.ImgBox.Height) return;

            form.mouseDownLocation = eventArgs.Location;
        }

        private static void SelectROI(object sender, MouseEventArgs eventArgs)
        {
            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;
            if (form.img == null) return;
            if (form.img.Size.Width <= form.ImgBox.Width && form.img.Size.Height <= form.ImgBox.Height) return;
            if (!eventArgs.Button.HasFlag(MouseButtons.Left)) return;
        }

        private static void Zoom(object sender, MouseEventArgs eventArgs)
        {
            var form = sender as MainForm;
            if (eventArgs.Delta < 0 && form.scale <= 100) return;
            if (!form.trianglesData.Any() || !form.nodes.Any()) return;

            var previousScale = form.scale;
            if (eventArgs.Delta > 0)
                form.scale += 20;
            else if (eventArgs.Delta < 0)
                form.scale -= 20;
            else return;

            var ratio = form.scale / previousScale;

            form.img = GetImg(form);

            form.ImgBox.Image = form.img;

            ShowCoordinates(form.ImgBox, eventArgs);
        }

        private static void UnZoom(object sender, EventArgs eventArgs)
        {
            var imgbox = sender as ImageBox;
            var form = imgbox.Parent as MainForm;
            if (!form.trianglesData.Any() || !form.nodes.Any()) return;
            form.scale = 200;

            form.img = GetImg(form);
            form.ImgBox.Image = form.img;
            form.Width = form.img.Size.Width + 20;
            form.Height = form.img.Size.Height + 100;
        }

        private static void SaveTriangulationData(string path, List<TriangleData> data)
        {
            var index = 0;
            var lines = data.Select(d => { index++; return $"   {index}     {d.v1}   {d.v2}   {d.v3}  {d.colorID}"; });
            File.WriteAllText(path, $"{data.Count}  3  1{Environment.NewLine}");
            File.AppendAllLines(path, lines);
        }

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

        private static void OpemBtnClick(object sender, EventArgs eventArgs)
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
                    secondForm.OpenToolStripMenuItem.Click += OpemBtnClick;
                    secondForm.MouseWheel += Zoom;
                    secondForm.ImgBox.MouseMove += ShowCoordinates;
                    secondForm.ImgBox.MouseDoubleClick += UnZoom;
                    secondForm.ImgBox.MouseMove += SelectROI;
                    secondForm.ImgBox.MouseDown += RememberMouseDownLocation;
                    secondForm.ImgBox.MouseClick += SelectColor;
                    secondForm.ImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
                    foreach (ToolStripMenuItem item in secondForm.ColorsMenu.Items)
                        item.Click += ChangeColor;
                    secondForm.Text = "Delaunay triangulation";
                    secondForm.MenuStrip.Hide();
                    secondForm.Show();
                }
                else if (dialogResult == DialogResult.No)
                {
                    BindImageToForm(form);
                }
            }
        }

        private static void SetFooter(MainForm form)
        {
            form.TrianglesCount.Text = form.trianglesData.Count().ToString();
            form.EdgesCount.Text = form.edges.Count().ToString();
            form.NodesCount.Text = form.nodes.Count().ToString();
            var trianglesSizes = CalculateTriangleSizes(form);
            form.MinTriangleCount.Text = trianglesSizes.Min().ToString("f6");
            form.MaxTriangleCount.Text = trianglesSizes.Max().ToString("f6");
        }

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

        private static double Square(double edge1, double edge2, double edge3)
        {
            var halfPer = (edge1 + edge2 + edge3) / 2;
            return Math.Sqrt(halfPer * (halfPer - edge1) * (halfPer - edge2) * (halfPer - edge3));
        }

        private static double CalculateEdgeLength(PointF node1, PointF node2)
        {
            var XDifference = node1.X - node2.X;
            var YDifference = node1.Y - node2.Y;
            return Math.Sqrt(XDifference * XDifference + YDifference * YDifference);
        }

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