using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var points = GetPointsFromFile("...\\...\\Resources\\rectan.1.node", scale: 250);
            var edges = GetEdgesFromFile("...\\...\\Resources\\rectan.1.edge");

            var maxX = points.Max(p => p.X);
            var maxY = points.Max(p => p.Y);

            var screenPoints = points.Select(p => new PointF(p.X, maxY - p.Y)).ToList();

            var img = new Image<Bgr, byte>((int)maxX + 1, (int)maxY + 1);
            foreach (var edge in edges)
            {
                var line = new LineSegment2DF(screenPoints[edge.X], screenPoints[edge.Y]);
                img.Draw(line, new Bgr(255, 255, 255), 1);
            }
                        
            CvInvoke.Imshow("Triangulation", img);
            CvInvoke.WaitKey();
        }

        private static List<PointF> GetPointsFromFile(string path, float scale)
        {
            var fileContent = File.ReadAllLines(path);
            var nodesCount = int.Parse(fileContent[0].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var points = new List<PointF>(nodesCount);

            points.Add(Point.Empty);
            for (int i = 1; i <= nodesCount; i++)
            {
                var lineContent = fileContent[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(lineContent[0]);
                var x = float.Parse(lineContent[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                var y = float.Parse(lineContent[2], NumberStyles.Any, CultureInfo.InvariantCulture);
                var point = new PointF(x * scale, y * scale);
                points.Add(point);
            }
            return points;
        }

        private static List<Point> GetEdgesFromFile(string path)
        {
            var fileContent = File.ReadAllLines(path);
            var edgesCount = int.Parse(fileContent[0].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var edges = new List<Point>(edgesCount);

            edges.Add(Point.Empty);
            for (int i = 1; i <= edgesCount; i++)
            {
                var lineContent = fileContent[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(lineContent[0]);
                var pointFrom = int.Parse(lineContent[1]);
                var pointTo = int.Parse(lineContent[2]);
                var point = new Point(pointFrom, pointTo);
                edges.Add(point);
            }
            return edges;
        }
    }
}
