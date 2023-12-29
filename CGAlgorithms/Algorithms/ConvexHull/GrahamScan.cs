using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Create a copy of the input points list and removing duplicates
            List<Point> workingPoints = new List<Point>();
            foreach (Point point in points)
            {
                if (!workingPoints.Contains(point)) workingPoints.Add(point);
            }
            Stack<Point> gstack = new Stack<Point>();

            Point first_point = workingPoints[0];
            for (int i = 1; i < workingPoints.Count; i++)
            {
                if (workingPoints[i].Y < first_point.Y || (workingPoints[i].Y == first_point.Y && workingPoints[i].X < first_point.X))
                {
                    first_point = workingPoints[i];
                }
            }

            gstack.Push(first_point);
            workingPoints.Remove(first_point);

            var ordered_workingPoints = workingPoints.OrderBy(p => CalculateAngle(first_point, p)).ToList();
            int o_w_p_counter = 0;
            if (ordered_workingPoints.Count >= 1)
            {
                foreach (Point point in ordered_workingPoints) { System.Console.WriteLine("(" + point.X + "," + point.Y + ")"); }
                gstack.Push(ordered_workingPoints[o_w_p_counter]);
                o_w_p_counter++;
            }

            while (ordered_workingPoints.Count>1 && o_w_p_counter<ordered_workingPoints.Count)
            {
                Point top = gstack.Pop();
                Line segment = new Line(gstack.First(), top);

                while (gstack.Count > 1 && HelperMethods.CheckTurn(segment, ordered_workingPoints[o_w_p_counter]) != Enums.TurnType.Left)
                {
                    top = gstack.Pop();
                    segment = new Line(gstack.First(), top);
                }
                gstack.Push(top);
                gstack.Push(ordered_workingPoints[o_w_p_counter]);
                o_w_p_counter++;
            }
            if (ordered_workingPoints.Count>1)
            {
                Point top_special = gstack.Pop();
                Line segment_special = new Line(gstack.First(), top_special);
                if (HelperMethods.CheckTurn(segment_special, first_point) != Enums.TurnType.Colinear)
                {
                    gstack.Push(top_special);
                }
            }
            outPoints = gstack.ToList();
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }

        static double CalculateAngle(Point reference, Point point)
        {
            return Math.Atan2(point.Y - reference.Y, point.X - reference.X);
        }
    }
}
