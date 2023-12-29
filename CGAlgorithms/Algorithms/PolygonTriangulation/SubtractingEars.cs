using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Point beginingPoint = GetPointWithMax_Y(lines);
            List<Point> polygonpoints = GetPolygonPoints(lines, beginingPoint);
            Point minX = polygonpoints[0];

            if (polygonpoints.Count < 3)
            {
                outPoints = points;
            }
            else if (polygonpoints.Count == 3)
            {
                outPoints = points;
            }
            else
            {
                int index = 0;

                for (int i = 0; i < polygonpoints.Count; i++)
                {
                    Point current = polygonpoints[i];

                    if (current.X < minX.X)
                    {
                        minX = current;
                        index = i;
                    }
                }

                Point minX_next = getItem_circleList(polygonpoints, index + 1);
                Point minX_prev = getItem_circleList(polygonpoints, index - 1);

                Line test = new Line(minX_prev, minX);

                Enums.TurnType check = HelperMethods.CheckTurn(test, minX_next);

                if (check == Enums.TurnType.Right)
                {
                    while (polygonpoints.Count > 3)
                    {
                        for (int i = 0; i < polygonpoints.Count; i++)
                        {
                            Point pPrev = getItem_circleList(polygonpoints, i - 1);
                            Point pCurrent = getItem_circleList(polygonpoints, i);
                            Point pNext = getItem_circleList(polygonpoints, i + 1);

                            Line testLine = new Line(pPrev, pCurrent);

                            Enums.TurnType checkTriangle = HelperMethods.CheckTurn(testLine, pNext);
                            bool isEar = true;

                            if (checkTriangle == Enums.TurnType.Left)
                                continue;

                            for (int j = 0; j < polygonpoints.Count; j++)
                            {
                                if (polygonpoints[j] == pPrev || polygonpoints[j] == pCurrent || polygonpoints[j] == pNext)
                                    continue;

                                Enums.PointInPolygon checkPoint = HelperMethods.PointInTriangle(polygonpoints[j], pPrev, pCurrent, pNext);

                                if (checkPoint == Enums.PointInPolygon.Inside || checkPoint == Enums.PointInPolygon.OnEdge)
                                {
                                    isEar = false;
                                    break;
                                }
                            }

                            if (isEar)
                            {
                                Line triLine = new Line(pPrev, pNext);
                                polygonpoints.Remove(pCurrent);
                                outLines.Add(triLine);
                                if (outPoints.Contains(pPrev) || outPoints.Contains(pNext))
                                {

                                }
                                else
                                {
                                    outPoints.Add(pPrev);
                                    outPoints.Add(pNext);
                                }
                                break;
                            }
                        }
                    }
                    Line tri = new Line(polygonpoints[0], polygonpoints[2]);

                    outLines.Add(tri);
                    outPoints.Add(polygonpoints[0]);
                    outPoints.Add(polygonpoints[2]);
                }
                else if (check == Enums.TurnType.Left)
                {
                    while (polygonpoints.Count > 3)
                    {
                        for (int i = 0; i < polygonpoints.Count; i++)
                        {
                            Point pPrev = getItem_circleList(polygonpoints, i - 1);
                            Point pCurrent = getItem_circleList(polygonpoints, i);
                            Point pNext = getItem_circleList(polygonpoints, i + 1);

                            Line testLine = new Line(pPrev, pCurrent);

                            Enums.TurnType checkTriangle = HelperMethods.CheckTurn(testLine, pNext);
                            bool isEar = true;

                            if (checkTriangle == Enums.TurnType.Right)
                                continue;

                            for (int j = 0; j < polygonpoints.Count; j++)
                            {
                                if (polygonpoints[j] == pPrev || polygonpoints[j] == pCurrent || polygonpoints[j] == pNext)
                                    continue;

                                Enums.PointInPolygon checkPoint = HelperMethods.PointInTriangle(polygonpoints[j], pPrev, pCurrent, pNext);

                                if (checkPoint == Enums.PointInPolygon.Inside || checkPoint == Enums.PointInPolygon.OnEdge)
                                {
                                    isEar = false;
                                    break;
                                }
                            }

                            if (isEar)
                            {
                                Line triLine = new Line(pPrev, pNext);
                                polygonpoints.Remove(pCurrent);
                                outLines.Add(triLine);
                                if (outPoints.Contains(pPrev) || outPoints.Contains(pNext))
                                { }
                                else
                                {
                                    outPoints.Add(pPrev);
                                    outPoints.Add(pNext);
                                }
                                break;
                            }
                        }
                    }
                    Line tri = new Line(polygonpoints[0], polygonpoints[2]);

                    outLines.Add(tri);
                    outPoints.Add(polygonpoints[0]);
                    outPoints.Add(polygonpoints[2]);
                }
            }
        }
        public override string ToString()
        {
            return "Subtracting Ears";
        }

        public static Point getItem_circleList(List<Point> list, int index)
        {
            if (index >= list.Count)
            {
                return list[0];
            }
            else if (index < 0)
            {
                return list[list.Count - 1];
            }
            else
            {
                return list[index];
            }
        }

        public Point GetPointWithMax_Y(List<Line> Lines)
        {
            if (Lines == null || Lines.Count == 0)
            {
                throw new InvalidOperationException("Polygon does not contain any lines.");
            }

            Point maxPoint = Lines[0].Start;

            foreach (var line in Lines)
            {
                if (line.Start.Y > maxPoint.Y)
                {
                    maxPoint = line.Start;
                }

                if (line.End.Y > maxPoint.Y)
                {
                    maxPoint = line.End;
                }
            }

            return maxPoint;
        }

        public Point GetPointWithMin_Y(List<Line> Lines)
        {
            if (Lines == null || Lines.Count == 0)
            {
                throw new InvalidOperationException("Polygon does not contain any lines.");
            }

            Point minPoint = Lines[0].Start;

            foreach (var line in Lines)
            {
                if (line.Start.Y < minPoint.Y)
                {
                    minPoint = line.Start;
                }

                if (line.End.Y < minPoint.Y)
                {
                    minPoint = line.End;
                }
            }

            return minPoint;
        }

        public List<Point> GetPolygonPoints(List<Line> Lines, Point maxPoint)
        {
            List<Point> points = new List<Point>();

            bool foundMaxPoint1 = false;
            bool foundMaxPoint2 = false;
            int index = 0;

            // Iterate through lines starting from the line that contains maxPoint
            while (foundMaxPoint1 == false || foundMaxPoint2 == false)
            {
                index %= Lines.Count;
                if (Lines[index].Start.Equals(maxPoint) || Lines[index].End.Equals(maxPoint))
                {
                    foundMaxPoint1 = true;
                }

                // Stop iterating when the line containing maxPoint is reached again
                if ((Lines[index].Start.Equals(maxPoint)) || (Lines[index].End.Equals(maxPoint)))
                {
                    if (points.Count > 1 && foundMaxPoint1 == true)
                    {
                        foundMaxPoint2 = true;
                        continue;
                    }
                }

                if (foundMaxPoint1)
                {
                    // Add both start and end points of the line to the list if not exist
                    if (Lines[index].Start.Equals(maxPoint) && points.Count == 0)
                    {
                        points.Add(Lines[index].Start);
                        points.Add(Lines[index].End);
                    }
                    else if (Lines[index].End.Equals(maxPoint) && points.Count == 0)
                    {
                        points.Add(Lines[index].End);
                    }
                    else
                    {
                        points.Add(Lines[index].End);
                    }
                }

                index++;
            }

            return points;
        }
    }
}