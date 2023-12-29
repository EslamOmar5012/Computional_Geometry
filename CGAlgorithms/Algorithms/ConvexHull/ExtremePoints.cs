using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Create a copy of the input points list and removing duplicates
            List<Point> workingPoints = new List<Point>();
            foreach (Point point in points)
            {
                if (!workingPoints.Contains(point)) workingPoints.Add(point);
            }
            List<Point> pointsToRemove = new List<Point>();

            int Count = workingPoints.Count;
            for (int a = 0; a < Count; a++)
            {
                for (int b = a + 1; b < Count; b++)
                {
                    for (int c = b + 1; c < Count; c++)
                    {
                        for (int p = 0; p < Count; p++)
                        {
                            if (p == a || p == b || p == c) { continue; }

                            Enums.PointInPolygon position = HelperMethods.PointInTriangle(workingPoints[p], workingPoints[a], workingPoints[b], workingPoints[c]);
                            if (position != Enums.PointInPolygon.Outside)
                            {
                                if (!pointsToRemove.Contains(workingPoints[p])) pointsToRemove.Add(workingPoints[p]);
                            }
                        }
                    }
                }
            }

            foreach (Point point in pointsToRemove)
            {
                workingPoints.Remove(point);
            }

            outPoints = workingPoints;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
