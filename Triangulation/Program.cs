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

namespace Triangulation
{
    class Program
    {
        private static MainForm mainForm;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            mainForm = new MainForm();
            mainForm.OpenToolStripMenuItem.Click += OpenFiles;
            mainForm.ImgBox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            mainForm.Text = "Delaunay triangulation";
            Application.Run(mainForm);
        }

        private static void OpenFiles(object sender, EventArgs eventArgs)
        {
            List<Point> edges = new List<Point>();
            List<PointF> points = new List<PointF>();
            List<TriangleData> trianglesData = new List<TriangleData>();

            if (mainForm.OpenFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                var path = mainForm.OpenFileDialog.FileName;
                var extensionIndex = path.LastIndexOf('.');
                path = path.Substring(0, extensionIndex);

                edges = GetEdgesFromFile(path + ".edge");
                points = GetPointsFromFile(path + ".node");
                trianglesData = GetTrianglesFromFile(path + ".ele");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                return;
            }

            var scale = 250;
            var maxX = points.Max(p => p.X * scale);
            var maxY = points.Max(p => p.Y * scale);
            var screenPoints = points.Select(p => new PointF(p.X * scale, maxY - p.Y * scale)).ToList();

            var triangles = GetTriangles(screenPoints, trianglesData);
            var img = FEMMesh(triangles, (int)maxX, (int)maxY);


            mainForm.ImgBox.Image = img;
            mainForm.Size = img.Size;
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
