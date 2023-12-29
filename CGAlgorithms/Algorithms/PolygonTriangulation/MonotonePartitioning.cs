using CGUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotonePartitioning : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            //List<Point> points_art = points;
            //List<Line> lines_art = new List<Line>();

            //// convert points to lines
            //for (int i = 0; i < points_art.Count; i++)
            //{
            //    int iplus1 = (i + 1) % points_art.Count;
            //    Line line_art = new Line(points_art[i], points_art[iplus1]);
            //    lines_art.Add(line_art);
            //}

            Point beginingPoint = GetPointWithMax_Y(lines);
            Point endingPoint = GetPointWithMin_Y(lines);
            List<Point> polygonPoints = GetPolygonPoints(lines, beginingPoint);

            List<Point> reversedpolygonPoints = new List<Point>();
            reversedpolygonPoints.Add(polygonPoints[0]);
            for (int i = polygonPoints.Count - 1; i > 0; i--)
            {
                reversedpolygonPoints.Add(polygonPoints[i]);
            }

            Point prevPoint = polygonPoints[polygonPoints.Count - 1];
            Point currPoint = polygonPoints[0];
            Point nexPoint = polygonPoints[1];

            Line lastlineinpolygon = new Line(prevPoint, currPoint);
            Enums.TurnType polygonrotation = HelperMethods.CheckTurn(lastlineinpolygon, nexPoint);
            if (polygonrotation == Enums.TurnType.Right) { polygonPoints.Clear(); polygonPoints.AddRange(reversedpolygonPoints); }
            
            List<Point> sortedPolygonPoints = polygonPoints.OrderByDescending(p => p.Y) // Sort by Y-coordinate in descending order
                                                           .ThenBy(p => p.X) // If Y-coordinates are equal, then sort by X-coordinate in ascending order
                                                           .ToList(); // Convert the sorted sequence to a List

            List<Line> T = new List<Line>();
            Dictionary<Line, Point> Helper = new Dictionary<Line, Point>();
            List<Line> Inserted_Diagonals = new List<Line>();

            List<Point> Start = new List<Point>();
            List<Point> End = new List<Point>();
            List<Point> Merge = new List<Point>();
            List<Point> Splite = new List<Point>();
            List<Point> Regular_Left = new List<Point>();
            List<Point> Regular_Right = new List<Point>();

            // Points Type assigning
            for (int i = 1; i < polygonPoints.Count + 1; i++)
            {
                Point previousPoint = polygonPoints[(i - 1) % polygonPoints.Count];
                Point currentPoint = polygonPoints[i % polygonPoints.Count];
                Point nextPoint = polygonPoints[(i + 1) % polygonPoints.Count];

                Line l = new Line(previousPoint, currentPoint);
                Enums.TurnType turn = HelperMethods.CheckTurn(l, nextPoint);

                if (turn == Enums.TurnType.Left) // angle < 180
                {
                    if (previousPoint.Y < currentPoint.Y && nextPoint.Y < currentPoint.Y) // neighbors below
                    {
                        Start.Add(currentPoint);
                    }
                    else if (previousPoint.Y > currentPoint.Y && nextPoint.Y > currentPoint.Y) // neighbors above
                    {
                        End.Add(currentPoint);
                    }
                    else if (previousPoint.Y > currentPoint.Y && nextPoint.Y < currentPoint.Y)
                    {
                        Regular_Left.Add(currentPoint);
                    }
                    else if (previousPoint.Y < currentPoint.Y && nextPoint.Y > currentPoint.Y)
                    {
                        Regular_Right.Add(currentPoint);
                    }
                }
                else if (turn == Enums.TurnType.Right) // angle > 180
                {
                    if (previousPoint.Y < currentPoint.Y && nextPoint.Y < currentPoint.Y) // neighbors below
                    {
                        Splite.Add(currentPoint);
                    }
                    else if (previousPoint.Y > currentPoint.Y && nextPoint.Y > currentPoint.Y) // neighbors above
                    {
                        Merge.Add(currentPoint);
                    }
                    else if (previousPoint.Y > currentPoint.Y && nextPoint.Y < currentPoint.Y)
                    {
                        Regular_Left.Add(currentPoint);
                    }
                    else if (previousPoint.Y < currentPoint.Y && nextPoint.Y > currentPoint.Y)
                    {
                        Regular_Right.Add(currentPoint);
                    }
                }
            }

            // loop that gets the Inserted Diagonals

            for (int i = 0; i < sortedPolygonPoints.Count - 1; i++)
            {
                if (Start.Contains(sortedPolygonPoints[i]))
                {
                    int Index = getindexinpolygon(polygonPoints, sortedPolygonPoints[i]);
                    Case_Start(sortedPolygonPoints[i], polygonPoints[Index + 1], T, Helper);
                }
                else if (End.Contains(sortedPolygonPoints[i]))
                {
                    Case1(sortedPolygonPoints[i], polygonPoints, T, Helper, Merge, Inserted_Diagonals);
                }
                else if (Merge.Contains(sortedPolygonPoints[i]))
                {
                    Case1(sortedPolygonPoints[i], polygonPoints, T, Helper, Merge, Inserted_Diagonals);
                    Case2_Merge(sortedPolygonPoints[i], T, Helper, Merge, Inserted_Diagonals);
                }
                else if (Splite.Contains(sortedPolygonPoints[i]))
                {
                    Case2_Split(sortedPolygonPoints[i], T, Helper, Inserted_Diagonals);
                    int Index = getindexinpolygon(polygonPoints, sortedPolygonPoints[i]);
                    Case_Start(sortedPolygonPoints[i], polygonPoints[Index + 1], T, Helper);
                }
                else if (Regular_Left.Contains(sortedPolygonPoints[i]))
                {
                    Case1(sortedPolygonPoints[i], polygonPoints, T, Helper, Merge, Inserted_Diagonals);
                    int Index = getindexinpolygon(polygonPoints, sortedPolygonPoints[i]);
                    Case_Start(sortedPolygonPoints[i], polygonPoints[Index + 1], T, Helper);
                }
                else if (Regular_Right.Contains(sortedPolygonPoints[i]))
                {
                    Case2_Regular(sortedPolygonPoints[i], T, Helper, Merge, Inserted_Diagonals);
                }
            }

            // Set the output

            foreach (Line line in lines)
            {
                outLines.Add(line);
            }
            foreach (Line line in Inserted_Diagonals)
            {
                outLines.Add(line);
            }

        }

        public override string ToString()
        {
            return "Monotone Partitioning";
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

        public int getindexinpolygon(List<Point> polygonpoints, Point v)
        {
            int index = 0;

            for (int i = 0; i < polygonpoints.Count; i++)
            {
                if (v == polygonpoints[i])
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public Point search_in_helper(Line e, Dictionary<Line, Point> helper)
        {
            int Indexo = -1;
            for (int i = 0; i < helper.Keys.Count; i++)
            {
                if (helper.Keys.ElementAt(i).Start.Equals(e.Start) && helper.Keys.ElementAt(i).End.Equals(e.End))
                {
                    Indexo = i;
                    break;
                }

            }

            Point ret_help = helper.Values.ElementAt(Indexo);
            return ret_help;
        }

        public void Case_Start(Point v1, Point v2, List<Line> t, Dictionary<Line, Point> helper)
        {
            Line e = new Line(v1, v2);
            t.Add(e);
            helper.Add(e, v1);
        }

        public void Case1(Point v, List<Point> polygonpoints, List<Line> t, Dictionary<Line, Point> helper, List<Point> merge, List<Line> inserted_diagonal)
        {
            int Index = getindexinpolygon(polygonpoints, v);
            if (Index == 0) { Index = polygonpoints.Count; }
            Line e_iminus1 = new Line(polygonpoints[Index - 1], v); // get line e(i-1)

            if (merge.Contains(search_in_helper(e_iminus1, helper)))
            {
                Line diagonal = new Line(v, search_in_helper(e_iminus1, helper));
                inserted_diagonal.Add(diagonal);
            }
            t.Remove(e_iminus1);
        }

        public void Case2_Regular(Point v, List<Line> t, Dictionary<Line, Point> helper, List<Point> merge, List<Line> inserted_diagonal)
        {
            // search for the left ej line
            Line ej = t[0];
            bool Flag = true;
            foreach (Line line in t)
            {
                if (line.Start.X < v.X && line.End.X < v.X)
                {
                    if ((line.Start.X < ej.Start.X && line.End.X < ej.End.X) || (line.Start.X < ej.End.X && line.End.X < ej.Start.X))
                    {
                        ej = line;
                        Flag = true;
                    }
                }
                else
                {
                    Flag = false;
                }
            }

            if (Flag == true)
            {
                if (merge.Contains(helper[ej]))
                {
                    Line diagonal = new Line(v, helper[ej]);
                    inserted_diagonal.Add(diagonal);
                    helper[ej] = v;
                }
            }
        }

        public void Case2_Merge(Point v, List<Line> t, Dictionary<Line, Point> helper, List<Point> merge, List<Line> inserted_diagonal)
        {
            // search for the left ej line
            Line ej = t[0];
            bool Flag = true;
            foreach (Line line in t)
            {
                if (line.Start.X < v.X && line.End.X < v.X)
                {
                    if ((line.Start.X < ej.Start.X && line.End.X < ej.End.X) || (line.Start.X < ej.End.X && line.End.X < ej.Start.X))
                    {
                        ej = line;
                        Flag = true;
                    }
                }
                else
                {
                    Flag = false;
                }
            }

            if (Flag == true)
            {
                if (merge.Contains(helper[ej]))
                {
                    Line diagonal = new Line(v, helper[ej]);
                    inserted_diagonal.Add(diagonal);
                }
                helper[ej] = v;
            }
        }

        public void Case2_Split(Point v, List<Line> t, Dictionary<Line, Point> helper, List<Line> inserted_diagonal)
        {
            // search for the left ej line
            Line ej = t[0];
            bool Flag = true;
            foreach (Line line in t)
            {
                if (line.Start.X < v.X && line.End.X < v.X)
                {
                    if ((line.Start.X < ej.Start.X && line.End.X < ej.End.X) || (line.Start.X < ej.End.X && line.End.X < ej.Start.X))
                    {
                        ej = line;
                        Flag = true;
                    }
                }
                else
                {
                    Flag = false;
                }
            }

            if (Flag == true)
            {
                Line diagonal = new Line(v, helper[ej]);
                inserted_diagonal.Add(diagonal);

                helper[ej] = v;
            }
        }
    }
}
