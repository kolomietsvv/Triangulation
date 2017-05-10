﻿using Emgu.CV;
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

            var triangles = GetTrianglesFromFile("...\\...\\Resources\\rectan.1.ele", screenPoints);

            var img = new Image<Bgr, byte>((int)maxX + 1, (int)maxY + 1);
            foreach (var triangle in triangles)
            {
                img.Draw(triangle, new Bgr(255, 255, 0), -1);
                img.Draw(triangle, new Bgr(255, 255, 255), 1);
            }

            CvInvoke.Imshow("Triangulation", img);
            CvInvoke.WaitKey();
        }

        private static List<PointF> GetPointsFromFile(string path, float scale)
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

        private static List<Triangle2DF> GetTrianglesFromFile(string path, List<PointF> points)
        {
            var fileContent = File.ReadAllLines(path);
            var trianglesCount = int.Parse(fileContent[0].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var triangles = new List<Triangle2DF>(trianglesCount);

            for (int i = 1; i <= trianglesCount; i++)
            {
                var lineContent = fileContent[i].Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(lineContent[0]);
                var point1ID = int.Parse(lineContent[1]);
                var point2ID = int.Parse(lineContent[2]);
                var point3ID = int.Parse(lineContent[3]);
                var triangle = new Triangle2DF(points[point1ID - 1], points[point2ID - 1], points[point3ID - 1]);
                triangles.Add(triangle);
            }
            return triangles;
        }
    }
}