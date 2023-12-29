using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public int ComparePointsByXThenY(Point point1, Point point2) =>
            Math.Abs(point1.X - point2.X) <= Constants.Epsilon
                ? point1.Y.CompareTo(point2.Y)
                : point1.X.CompareTo(point2.X);

        public bool IsPointInLineSegment(Line line, Point point) =>
            point.X >= Math.Min(line.Start.X, line.End.X) && point.X <= Math.Max(line.Start.X, line.End.X) &&
            point.Y >= Math.Min(line.Start.Y, line.End.Y) && point.Y <= Math.Max(line.Start.Y, line.End.Y);

        public bool ArePointsEqual(Point point1, Point point2) =>
            Math.Abs(point1.X - point2.X) <= Constants.Epsilon && Math.Abs(point1.Y - point2.Y) <= Constants.Epsilon;

        public double CalculateDistanceSquared(Point point1, Point point2) =>
            Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2);

        public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons, ref List<Point> convexHullPoints, ref List<Line> convexHullLines, ref List<Polygon> convexHullPolygons)
        {
            inputPoints.Sort(ComparePointsByXThenY);

            int minIndex = inputPoints.FindIndex(p => ArePointsEqual(p, inputPoints[0]) || p.Y < inputPoints[0].Y);

            int currentIndex = minIndex;
            Point currentPoint = inputPoints[minIndex];

            List<Point> convexHullResult = new List<Point> { currentPoint };

            int size = inputPoints.Count;

            while (true)
            {
                int nextIndex = (currentIndex + 1) % size;

                while (ArePointsEqual(currentPoint, inputPoints[nextIndex]) && nextIndex != currentIndex)
                    nextIndex = (nextIndex + 1) % size;

                if (nextIndex == currentIndex)
                    break;

                Point nextPoint = inputPoints[nextIndex];
                Line currentSegment = new Line(currentPoint, nextPoint);

                for (int i = 0; i < size; i++)
                {
                    Point tempPoint = inputPoints[i];
                    Enums.TurnType position = HelperMethods.CheckTurn(currentSegment, tempPoint);

                    if (position == Enums.TurnType.Right || (position == Enums.TurnType.Colinear && CalculateDistanceSquared(currentPoint, tempPoint) > CalculateDistanceSquared(currentPoint, nextPoint)))
                    {
                        currentSegment.End = tempPoint;
                        nextIndex = i;
                        nextPoint = tempPoint;
                    }
                }

                if (nextIndex == minIndex)
                    break;

                convexHullResult.Add(nextPoint);
                currentPoint = nextPoint;
                currentIndex = nextIndex;
            }

            convexHullPoints = convexHullResult;
        }

        public override string ToString() => "Convex Hull - Jarvis March";
    }
}