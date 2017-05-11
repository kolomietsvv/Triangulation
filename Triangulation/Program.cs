using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Input;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Triangulation
{
    class Program
    {
        private static MainForm mainForm;
        private static List<Point> edges;
        private static List<PointF> points;
        private static List<TriangleData> trianglesData;
        private static int scale = 250;
        private static Image<Bgr, byte> img;
        private static Point mouseDownLocation;
        private static Rectangle roi;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            mainForm = new MainForm();
            edges = new List<Point>();
            points = new List<PointF>();
            trianglesData = new List<TriangleData>();
            mainForm.OpenToolStripMenuItem.Click += OpenFiles;
            mainForm.MouseWheel += Zoom;
            mainForm.ImgBox.MouseDoubleClick += UnZoom;
            mainForm.ImgBox.MouseMove += SelectROI;
            mainForm.ImgBox.MouseDown += RememberMouseDownLocation;
            mainForm.ImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            mainForm.Text = "Delaunay triangulation";
            Application.Run(mainForm);
        }

        private static void RememberMouseDownLocation(object sender, MouseEventArgs eventArgs)
        {
            if (img == null) return;
            if (img.Size.Width <= mainForm.ImgBox.Width && img.Size.Height <= mainForm.ImgBox.Height) return;

            mouseDownLocation = eventArgs.Location;
            roi = new Rectangle(
                (img.Width - mainForm.ImgBox.Width) / 2,
                (img.Height - mainForm.ImgBox.Height) / 2,
                mainForm.ImgBox.Width,
                mainForm.ImgBox.Height);
            img.ROI = roi;
            mainForm.ImgBox.Image = img;
        }

        private static void SelectROI(object sender, MouseEventArgs eventArgs)
        {
            if (img == null) return;
            if (img.Size.Width <= mainForm.ImgBox.Width && img.Size.Height <= mainForm.ImgBox.Height) return;
            if (!eventArgs.Button.HasFlag(MouseButtons.Left)) return;
        }

        private static void Zoom(object sender, MouseEventArgs eventArgs)
        {
            if (eventArgs.Delta < 0 && scale <= 100) return;
            if (!trianglesData.Any() || !points.Any()) return;

            if (eventArgs.Delta > 0)
                scale += 20;
            else if (eventArgs.Delta < 0)
                scale -= 20;
            else return;

            img = GetImg();

            if (img.Width <= mainForm.ImgBox.Width && img.Height <= mainForm.Height)
                roi = Rectangle.Empty;

            img.ROI = roi;
            mainForm.ImgBox.Image = img;
        }

        private static void UnZoom(object sender, EventArgs eventArgs)
        {
            if (!trianglesData.Any() || !points.Any()) return;
            scale = 250;
            roi = Rectangle.Empty;

            img = GetImg();
            img.ROI = roi;
            mainForm.ImgBox.Image = img;
            mainForm.Width = img.Size.Width + 20;
            mainForm.Height = img.Size.Height + 100;
        }

        private static void OpenFiles(object sender, EventArgs eventArgs)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var path = openFileDialog.FileName;
                var extensionIndex = path.LastIndexOf('.');
                path = path.Substring(0, extensionIndex);

                edges = GetEdgesFromFile(path + ".edge");
                points = GetPointsFromFile(path + ".node");
                trianglesData = GetTrianglesFromFile(path + ".ele");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: Could not read file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            img = GetImg();
            roi = Rectangle.Empty;
            mainForm.ImgBox.Image = img;
            mainForm.Width = img.Size.Width + 20;
            mainForm.Height = img.Size.Height + 100;
        }

        private static Image<Bgr, byte> GetImg()
        {
            var minX = points.Min(p => p.X * scale);
            var minY = points.Min(p => p.Y * scale);
            var maxX = points.Max(p => p.X * scale) - minX;
            var maxY = points.Max(p => p.Y * scale) - minY;
            var screenPoints = points.Select(p => new PointF(p.X * scale - minX, maxY - (p.Y * scale - minY))).ToList();

            var triangles = GetTriangles(screenPoints, trianglesData);
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

        private static List<PointF> GetPointsFromFile(string path)
        {
            var fileContent = File.ReadAllLines(path);
            var nodesCount = int.Parse(fileContent[0].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var points = new List<PointF>(nodesCount);

            for (int i = 1; i <= nodesCount; i++)
            {
                var lineContent = fileContent[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(lineContent[0]);
                var x = float.Parse(lineContent[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                var y = float.Parse(lineContent[2], NumberStyles.Any, CultureInfo.InvariantCulture);
                var point = new PointF(x, y);
                points.Add(point);
            }
            return points;
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

            var colors = new[] { new Bgr(
                Color.Gray), new Bgr(Color.Orange), new Bgr(Color.Blue), new Bgr(Color.Yellow), new Bgr(Color.Red), new Bgr(Color.RoyalBlue),new Bgr(Color.LawnGreen),
                new Bgr(Color.ForestGreen), new Bgr(Color.DarkGreen), new Bgr(Color.DarkBlue), new Bgr(Color.Cyan)};

            for (int i = 0; i < trianglesCount; i++)
            {
                var triangle = new Triangle2DF(points[trianglesData[i].v1 - 1], points[trianglesData[i].v2 - 1], points[trianglesData[i].v3 - 1]);
                var color = colors[trianglesData[i].colorID % (colors.Length)];
                triangles.Add(new KeyValuePair<Triangle2DF, Bgr>(triangle, color));
            }
            return triangles;
        }

        struct TriangleData
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
