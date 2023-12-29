using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Create a copy of the input points list and removing duplicates
            List<Point> workingPoints = new List<Point>();
            foreach (Point point in points)
            {
                if (!workingPoints.Contains(point)) workingPoints.Add(point);
            }

            List<Point> PointsToRemove = new List<Point>();
            List<Point> resultPoints = new List<Point>();
            List<Line> resultLines = new List<Line>();

            if (workingPoints.Count == 1)
            {
                resultPoints.Add(workingPoints[0]);
                outPoints = resultPoints;
                return;
            }

            for (int i = 0; i < workingPoints.Count; i++)
            {
                for (int j = 0; j < workingPoints.Count; j++)
                {
                    if (j == i) { continue; }

                    Line segment = new Line(workingPoints[i], workingPoints[j]);
                    int stat = 0;
                    for (int k = 0; k < workingPoints.Count; k++)
                    {
                        if (k == i || k == j) { continue; }

                        Enums.TurnType side = HelperMethods.CheckTurn(segment, workingPoints[k]);
                        if (side == Enums.TurnType.Right)
                        {
                            stat = 1;
                            break;
                        }
                    }

                    if (stat == 0)
                    {
                        resultLines.Add(segment);

                        if (!resultPoints.Contains(workingPoints[i])) resultPoints.Add(workingPoints[i]);
                        if (!resultPoints.Contains(workingPoints[j])) resultPoints.Add(workingPoints[j]);

                    }
                }
            }

            foreach(Line line in resultLines)
            {
                for(int i =0;i<resultPoints.Count;i++)
                {
                    if (resultPoints[i] == line.Start || resultPoints[i] == line.End) continue;
                    if ((CalcDist(line.Start, resultPoints[i])+ CalcDist(resultPoints[i],line.End))== CalcDist(line.Start, line.End)
                        && (HelperMethods.CheckTurn(line, resultPoints[i]) == Enums.TurnType.Colinear))
                    {
                        if (!PointsToRemove.Contains(resultPoints[i])) PointsToRemove.Add(resultPoints[i]);
                    }
                }
            }
            foreach(Point point in PointsToRemove)
            {
                if (resultPoints.Contains(point)) resultPoints.Remove(point);
            }

            outPoints = resultPoints;
            outLines = resultLines;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
        static double CalcDist(Point point1, Point point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance;
        }
    }
}