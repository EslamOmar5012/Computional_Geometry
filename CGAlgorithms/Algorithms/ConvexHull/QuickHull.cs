using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            outPoints.Clear();
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            Point minX = points[0], maxX = minX, minY = minX, maxY = minX;

            for (int i = 1; i < points.Count; i++)
            {
                Point current = points[i];

                if (current.X < minX.X)
                    minX = current;
                if (current.X > maxX.X)
                    maxX = current;
                if (current.Y < minY.Y)
                    minY = current;
                if (current.Y > maxY.Y)
                    maxY = current;
            }

            outPoints.Add(maxX);
            outPoints.Add(minX);
            outPoints.Add(maxY);
            outPoints.Add(minY);

            Line line1 = new Line(minX, minY);
            Line line2 = new Line(minY, maxX);
            Line line3 = new Line(maxX, maxY);
            Line line4 = new Line(maxY, minX);

            List<Point> res1 = new List<Point>();
            List<Point> res2 = new List<Point>();
            List<Point> res3 = new List<Point>();
            List<Point> res4 = new List<Point>();

            foreach (Point point in points)
            {
                if (point.Equals(minX) || point.Equals(maxX) || point.Equals(minY) || point.Equals(maxY))
                    continue;

                if (HelperMethods.CheckTurn(line1, point) == Enums.TurnType.Right)
                    res1.Add(point);
                if (HelperMethods.CheckTurn(line2, point) == Enums.TurnType.Right)
                    res2.Add(point);
                if (HelperMethods.CheckTurn(line3, point) == Enums.TurnType.Right)
                    res3.Add(point);
                if (HelperMethods.CheckTurn(line4, point) == Enums.TurnType.Right)
                    res4.Add(point);
            }

            outPoints.AddRange(quick_hull(res1, line1));
            outPoints.AddRange(quick_hull(res2, line2));
            outPoints.AddRange(quick_hull(res3, line3));
            outPoints.AddRange(quick_hull(res4, line4));

            outPoints = outPoints.Distinct().ToList();
        }

        public static List<Point> quick_hull(List<Point> points, Line line)
        {
            if (points.Count == 0)
            {
                return new List<Point>();
            }

            Point farthestPoint = max_distance(points, line.Start, line.End);

            List<Point> external1 = new List<Point>();
            List<Point> external2 = new List<Point>();

            Line line1 = new Line(line.Start, farthestPoint);
            Line line2 = new Line(farthestPoint, line.End);

            for (int i = 0; i < points.Count; i++)
            {
                if (HelperMethods.CheckTurn(line1, points[i]) == Enums.TurnType.Right)
                {
                    external1.Add(points[i]);
                }
                if (HelperMethods.CheckTurn(line2, points[i]) == Enums.TurnType.Right)
                {
                    external2.Add(points[i]);
                }
            }

            List<Point> res1 = quick_hull(external1, line1);
            List<Point> res2 = quick_hull(external2, line2);

            int expectedSize = res1.Count + res2.Count + 1;
            List<Point> result = new List<Point>(expectedSize);

            result.AddRange(res1);
            result.AddRange(res2);
            result.Add(farthestPoint);

            return result;
        }


        public static Point max_distance(List<Point> points, Point start, Point end)
        {
            double maxDist = double.MinValue;
            Point maxPoint = null;

            double a = end.Y - start.Y;
            double b = start.X - end.X;
            double abSqrSum = a * a + b * b;

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                double dist = Math.Abs(a * p.X + b * p.Y - a * start.X - b * start.Y) / Math.Sqrt(abSqrSum);

                if (dist > maxDist)
                {
                    maxDist = dist;
                    maxPoint = p;
                }
            }
            return maxPoint;
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
