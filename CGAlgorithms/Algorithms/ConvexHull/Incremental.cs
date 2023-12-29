using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public int ComparePointsByXThenY(Point point1, Point point2)
        {
            if (point1.X == point2.X)
            {
                if (point1.Y < point2.Y)
                    return -1;
                return 1;
            }
            else if (point1.X < point2.X)
                return -1;
            return 1;
        }

        public bool ArePointsEqual(Point point1, Point point2) =>
            Math.Abs(point1.X - point2.X) <= Constants.Epsilon && Math.Abs(point1.Y - point2.Y) <= Constants.Epsilon;

        public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons, ref List<Point> convexHullPoints, ref List<Line> convexHullLines, ref List<Polygon> convexHullPolygons)
        {
            int pointCount = inputPoints.Count;

            if (pointCount < 3)
            {
                convexHullPoints = inputPoints.ToList();
                return;
            }


            inputPoints.Sort(ComparePointsByXThenY);

            int[] nextIndices = new int[pointCount];
            int[] prevIndices = new int[pointCount];

            int index = 1;

            for (; index < inputPoints.Count && ArePointsEqual(inputPoints[0], inputPoints[index]);)
                ++index;


            nextIndices[0] = index;
            prevIndices[0] = index;
            nextIndices[index] = 0;
            prevIndices[index] = 0;

            int currentHold = index;

            for (index = index + 1; index < inputPoints.Count; index++)
            {
                Point newPoint = inputPoints[index];

                if (ArePointsEqual(newPoint, inputPoints[currentHold]))
                    continue;

                if (newPoint.Y >= inputPoints[currentHold].Y)
                {
                    nextIndices[index] = nextIndices[currentHold];
                    prevIndices[index] = currentHold;
                }
                else
                {
                    nextIndices[index] = currentHold;
                    prevIndices[index] = prevIndices[currentHold];
                }

                nextIndices[prevIndices[index]] = index;
                prevIndices[nextIndices[index]] = index;

                while (true)
                {
                    Line segment = new Line(newPoint, inputPoints[nextIndices[index]]);
                    Point nextPoint = inputPoints[nextIndices[nextIndices[index]]];
                    Enums.TurnType turn = HelperMethods.CheckTurn(segment, nextPoint);

                    if (turn != Enums.TurnType.Left)
                    {
                        nextIndices[index] = nextIndices[nextIndices[index]];
                        prevIndices[nextIndices[index]] = index;

                        if (turn == Enums.TurnType.Colinear)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }

                while (true)
                {
                    Line segment = new Line(newPoint, inputPoints[prevIndices[index]]);
                    Point nextPoint = inputPoints[prevIndices[prevIndices[index]]];
                    Enums.TurnType turn = HelperMethods.CheckTurn(segment, nextPoint);

                    if (turn != Enums.TurnType.Right)
                    {
                        prevIndices[index] = prevIndices[prevIndices[index]];
                        nextIndices[prevIndices[index]] = index;

                        if (turn == Enums.TurnType.Colinear)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }

                currentHold = index;
            }

            int current = 0;
            while (true)
            {
                convexHullPoints.Add(inputPoints[current]);
                current = nextIndices[current];
                if (current == 0)
                    break;
            }
        }

        public override string ToString() => "Incremental Convex Hull Algorithm";
    }
}