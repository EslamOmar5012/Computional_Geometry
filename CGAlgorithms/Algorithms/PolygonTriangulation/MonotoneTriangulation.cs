using CGUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            //List<Point> points_art = points;
            //List<Line> lines_art = new List<Line>();

            // convert points to lines
            //for (int i = 0; i < points_art.Count; i++)
            //{
            //    int iplus1 = (i + 1) % points_art.Count;
            //    Line line_art = new Line(points_art[i], points_art[iplus1]);
            //    lines_art.Add(line_art);
            //}

            Point beginingPoint = GetPointWithMax_Y(lines);
            Point endingPoint = GetPointWithMin_Y(lines);
            List<Point> polygonPoints = GetPolygonPoints(lines, beginingPoint);

            List<Point> RightMonotone = new List<Point>();
            List<Point> LeftMonotone = new List<Point>();

            Line lastlineinpolygon = new Line(polygonPoints[polygonPoints.Count - 1], polygonPoints[0]);
            Enums.TurnType side = HelperMethods.CheckTurn(lastlineinpolygon, polygonPoints[1]);
            if (side == Enums.TurnType.Right)
            {
                int r = 1;
                while (!polygonPoints[r].Equals(endingPoint))
                {
                    RightMonotone.Add(polygonPoints[r]);
                    r++;
                }

                int l = polygonPoints.Count - 1;
                while (!polygonPoints[l].Equals(endingPoint))
                {
                    LeftMonotone.Add(polygonPoints[l]);
                    l--;
                }
            }
            else if (side == Enums.TurnType.Left)
            {
                int l = 1;
                while (!polygonPoints[l].Equals(endingPoint))
                {
                    LeftMonotone.Add(polygonPoints[l]);
                    l++;
                }

                int r = polygonPoints.Count - 1;
                while (!polygonPoints[r].Equals(endingPoint))
                {
                    RightMonotone.Add(polygonPoints[r]);
                    r--;
                }
            }

            List<Point> sorted_points = MergeSortedLists(RightMonotone, LeftMonotone);

            List<Point> workingPoints = new List<Point>();
            workingPoints.Add(beginingPoint);
            workingPoints.AddRange(sorted_points);
            workingPoints.Add(endingPoint);

            Stack<Point> mstack = new Stack<Point>();

            List<Line> outputLines = new List<Line>();

            Point first_point = workingPoints[0];
            Point second_point = workingPoints[1];
            mstack.Push(first_point);
            mstack.Push(second_point);

            bool sameChain = true;
            int chainSide = 0; // (0) right side - (1) left side

            if (RightMonotone.Contains(second_point)) { chainSide = 0; }
            else if (LeftMonotone.Contains(second_point)) { chainSide = 1; }

            for (int i = 2; i < workingPoints.Count; i++)
            {
                Point current_vertex = workingPoints[i];
                // to get if the point is on the same chain or on the opposite chain
                if (RightMonotone.Contains(current_vertex))
                {
                    if (chainSide == 0)
                    {
                        sameChain = true;
                    }
                    else
                    {
                        sameChain = false;
                        chainSide = 0;
                    }
                }
                else if (LeftMonotone.Contains(current_vertex))
                {
                    if (chainSide == 1)
                    {
                        sameChain = true;
                    }
                    else
                    {
                        sameChain = false;
                        chainSide = 1;
                    }
                }

                if (sameChain) // same chain
                {
                    bool flag = false;
                    Point top_vertex = mstack.Pop();

                    while (mstack.Count > 0 && CheckVisibility(current_vertex, top_vertex, mstack.Peek(), chainSide) == true) // visible
                    {
                        flag = true;
                        Line inner_segment = new Line(mstack.Peek(), current_vertex);
                        if (!lines.Contains(inner_segment)) { outputLines.Add(inner_segment); }
                        top_vertex = mstack.Pop();
                    }

                    if (flag == false)
                    {
                        mstack.Push(top_vertex);
                    }

                    mstack.Push(current_vertex);
                }
                else //opposite chain
                {
                    Point top_vertex = mstack.Peek();

                    while (mstack.Count > 0)
                    {
                        Line inner_segment = new Line(mstack.Peek(), current_vertex);
                        if (!lines.Contains(inner_segment)) { outputLines.Add(inner_segment); }

                        mstack.Pop();
                    }

                    mstack.Push(top_vertex);
                    mstack.Push(current_vertex);
                }
            }

            // Set the output

            foreach (Line line in lines)
            {
                outLines.Add(line);
            }
            foreach (Line line in outputLines)
            {
                if (!outLines.Contains(line)) { outLines.Add(line); }
            }

        }

        public override string ToString()
        {
            return "Monotone Triangulation";
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

        static List<Point> MergeSortedLists(List<Point> list1, List<Point> list2)
        {
            List<Point> result = new List<Point>();
            int i = 0, j = 0;

            while (i < list1.Count && j < list2.Count)
            {
                if (list1[i].Y > list2[j].Y)
                {
                    result.Add(list1[i]);
                    i++;
                }
                else
                {
                    result.Add(list2[j]);
                    j++;
                }
            }

            // Add remaining elements from both lists
            while (i < list1.Count)
            {
                result.Add(list1[i]);
                i++;
            }

            while (j < list2.Count)
            {
                result.Add(list2[j]);
                j++;
            }

            return result;
        }

        static bool CheckVisibility(Point lastPoint, Point topPoint, Point pointinstack, int chainside)
        {
            Line l = new Line(lastPoint, topPoint);
            Enums.TurnType turn = HelperMethods.CheckTurn(l, pointinstack);

            if (chainside == 0) //right side
            {
                if (turn != Enums.TurnType.Left) // right or collinear
                { return false; }
                else // left
                { return true; }
            }
            else // left side
            {
                if (turn != Enums.TurnType.Right) // left or collinear
                { return false; }
                else // right
                { return true; }
            }
        }
    }
}
