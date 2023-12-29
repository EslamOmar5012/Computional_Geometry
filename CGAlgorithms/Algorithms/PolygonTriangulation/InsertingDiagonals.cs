using CGUtilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            Point beginingPoint = GetPointWithMax_Y(lines);
            List<Point> polygonpoints = GetPolygonPoints(lines, beginingPoint);
            Point minX = polygonpoints[0];

            if (polygonpoints.Count <= 3)
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

                if (check == Enums.TurnType.Left)
                {
                    outLines = insertDiagonals(polygonpoints, Enums.TurnType.Left);
                }
                else if (check == Enums.TurnType.Right)
                {
                    outLines = insertDiagonals(polygonpoints, Enums.TurnType.Right);
                }
            }
        }

        public override string ToString()
        {
            return "Inserting Diagonals";
        }


        public static List<Line> insertDiagonals(List<Point> points, Enums.TurnType turn)
        {
            if (points.Count > 3)
            {
                int flag = 1;
                List<Line> result = new List<Line>();
                Point prev = new Point(0, 0);
                Point current = new Point(0, 0);
                Point next = new Point(0, 0);
                Line testLine = new Line(new Point(0, 0), new Point(0, 0));
                //get convex points
                for (int i = 0; i < points.Count; i++)
                {
                    prev = getItem_circleList(points, i - 1);
                    current = getItem_circleList(points, i);
                    next = getItem_circleList(points, i + 1);

                    testLine.Start = prev;
                    testLine.End = current;

                    Enums.TurnType isConvex = HelperMethods.CheckTurn(testLine, next);

                    if (isConvex != turn)
                    {
                        flag = 0;
                        continue;
                    }
                    else
                    {
                        flag = 1;
                        break;
                    }
                }
                //check if there is convx points or not
                if (flag == 0)
                    return new List<Line>();

                //get max point in triangle if exist
                Point maxPoint = new Point(0, 0);
                double max_dis = 0, dis = 0;

                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i] == prev || points[i] == current || points[i] == next)
                        continue;

                    dis = get_distance(prev, next, points[i]);

                    Enums.PointInPolygon checkPoint = HelperMethods.PointInTriangle(points[i], prev, current, next);
                    if (checkPoint == Enums.PointInPolygon.Inside || checkPoint == Enums.PointInPolygon.OnEdge)
                        if (max_dis < dis)
                        {
                            flag = 0;
                            max_dis = dis;
                            maxPoint = points[i];
                        }
                }
                //add the diagonal , divide the polygon into two polygons
                int i1 = 0, i2 = 0;
                if (flag == 1)
                {
                    result.Add(new Line(prev, next));
                    i1 = points.IndexOf(next);
                    i2 = points.IndexOf(prev);
                }
                else
                {
                    result.Add(new Line(current, maxPoint));
                    i1 = points.IndexOf(current);
                    i2 = points.IndexOf(maxPoint);
                }

                List<Point> points1 = new List<Point>();
                List<Point> points2 = new List<Point>();

                int s = Math.Min(i1, i2);
                int e = Math.Max(i1, i2);

                for (int i = e; i != s; i = (i + 1) % points.Count)
                    points1.Add(points[i]);

                for (int i = s; i != e; i = (i + 1) % points.Count)
                    points2.Add(points[i]);

                points1.Add(points[s]); points2.Add(points[e]);
                result.AddRange(insertDiagonals(points1, turn));
                result.AddRange(insertDiagonals(points2, turn));

                return result;
            }
            return new List<Line>();
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

        public static double get_distance(Point p1, Point p2, Point p3)
        {
            double result = (Math.Abs(((p2.X - p1.X) * (p1.Y - p3.Y)) - ((p1.X - p3.X) * (p2.Y - p1.Y)))) / (Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2)));
            return result;
        }
    }
}