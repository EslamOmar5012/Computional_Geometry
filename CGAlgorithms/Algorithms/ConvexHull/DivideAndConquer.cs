using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points.Sort((a, b) =>
            {
                if (a.X == b.X)
                    return a.Y.CompareTo(b.Y);
                return a.X.CompareTo(b.X);
            });

            outPoints = GetConvexHull(points);
        }

        List<Point> GetConvexHull(List<Point> points)
        {
            if (points.Count <= 3)
                return SortAntiClockwise(points);

            double meanX = points.Average(p => p.X);
            var (leftPoints, rightPoints) = SplitPoints(points, meanX);

            if (leftPoints.Count == 0)
                return new List<Point> { rightPoints[0], rightPoints.Last() };
            if (rightPoints.Count == 0)
                return new List<Point> { leftPoints[0], leftPoints.Last() };

            var leftHull = GetConvexHull(leftPoints);
            var rightHull = GetConvexHull(rightPoints);

            return Merge(leftHull, rightHull);
        }

        (List<Point> leftPoints, List<Point> rightPoints) SplitPoints(List<Point> points, double meanX)
        {
            var leftPoints = new List<Point>();
            var rightPoints = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < meanX)
                    leftPoints.Add(points[i]);
                else
                    rightPoints.Add(points[i]);
            }
            return (leftPoints, rightPoints);
        }

        List<Point> Merge(List<Point> leftHull, List<Point> rightHull)
        {
            double maxXValue = leftHull.Max(p => p.X);
            int maxXIndex = leftHull.FindIndex(p => p.X == maxXValue);

            double minXValue = rightHull.Min(p => p.X);
            int minXIndex = rightHull.FindIndex(p => p.X == minXValue);

            Tuple<int, int> lowerTangent = GetTangent(leftHull, rightHull, maxXIndex, minXIndex);
            Tuple<int, int> upperTangent = GetTangent(rightHull, leftHull, minXIndex, maxXIndex);

            List<Point> merged = new List<Point>();

            for (int i = lowerTangent.Item2; i != upperTangent.Item1; i = (i + 1) % rightHull.Count)
                merged.Add(rightHull[i]);

            merged.Add(rightHull[upperTangent.Item1]);

            for (int i = upperTangent.Item2; i != lowerTangent.Item1; i = (i + 1) % leftHull.Count)
                merged.Add(leftHull[i]);

            merged.Add(leftHull[lowerTangent.Item1]);

            for (int i = 0; i < merged.Count;)
            {
                int prev = (i - 1 + merged.Count) % merged.Count;
                int next = (i + 1) % merged.Count;
                if (HelperMethods.CheckTurn(new Line(merged[prev], merged[i]), merged[next]) == Enums.TurnType.Colinear)
                {
                    merged.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            return merged;
        }

        Tuple<int, int> GetTangent(List<Point> LHull, List<Point> RHull, int l, int r)
        {
            Line tangent = new Line(LHull[l], RHull[r]);

            while (true)
            {
                bool leftTangentFound = false;
                bool rightTangentFound = false;

                int lNext = (l + 1) % LHull.Count;
                int lPrev = (l - 1 + LHull.Count) % LHull.Count;
                int rNext = (r + 1) % RHull.Count;
                int rPrev = (r - 1 + RHull.Count) % RHull.Count;

                if (HelperMethods.CheckTurn(tangent, LHull[lNext]) == Enums.TurnType.Right ||
                    HelperMethods.CheckTurn(tangent, LHull[lPrev]) == Enums.TurnType.Right)
                {
                    l = (l - 1 + LHull.Count) % LHull.Count;
                    leftTangentFound = true;
                }

                if (HelperMethods.CheckTurn(tangent, RHull[rNext]) == Enums.TurnType.Right ||
                    HelperMethods.CheckTurn(tangent, RHull[rPrev]) == Enums.TurnType.Right)
                {
                    r = (r + 1) % RHull.Count;
                    rightTangentFound = true;
                }

                if (!leftTangentFound && !rightTangentFound)
                    break;

                tangent = new Line(LHull[l], RHull[r]);
            }

            return Tuple.Create(l, r);
        }

        List<Point> SortAntiClockwise(List<Point> points)
        {
            double minYValue = points.Min(p => p.Y);
            int minYIndex = points.FindIndex(p => p.Y == minYValue);

            Line referenceLine = new Line(points[minYIndex], new Point(points[minYIndex].X + 1000.0, points[minYIndex].Y));
            List<Tuple<double, int>> angleList = new List<Tuple<double, int>>();

            for (int i = 0; i < points.Count; i++)
            {
                if (i == minYIndex) continue;
                Point v1 = referenceLine.Start.Vector(referenceLine.End);
                Point v2 = referenceLine.Start.Vector(points[i]);

                double crossProduct = HelperMethods.CrossProduct(v1, v2);
                double dotProduct = v1.X * v2.X + v1.Y * v2.Y;
                double angle = Math.Atan2(crossProduct, dotProduct) * (180.00 / Math.PI);

                if (angle < 0)
                    angle += 360;

                angleList.Add(Tuple.Create(angle, i));
            }

            angleList.Sort((a, b) => {
                if (a.Item1 == b.Item1) return a.Item2.CompareTo(b.Item2);
                return a.Item1.CompareTo(b.Item1);
            });

            List<Point> sortedPoints = new List<Point>();
            sortedPoints.Add(points[minYIndex]);

            foreach (var pair in angleList)
                sortedPoints.Add(points[pair.Item2]);

            return sortedPoints;
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }
    }
}
