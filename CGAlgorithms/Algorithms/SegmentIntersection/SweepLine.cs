using System;
using System.Collections.Generic;
using System.Linq;
using CGUtilities;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class SweepLine : Algorithm
    {
        private enum EventType
        {
            Start,
            End,
            Intersection
        }

        private struct Event
        {
            public CGUtilities.Point Point { get; }
            public CGUtilities.Line Line { get; }
            public EventType Type { get; }

            public Event(CGUtilities.Point point, CGUtilities.Line line, EventType type)
            {
                Point = point;
                Line = line;
                Type = type;
            }
        }

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            HashSet<CGUtilities.Line> checkedLines = new HashSet<CGUtilities.Line>();
            List<CGUtilities.Point> intersections = new List<CGUtilities.Point>();
            List<Event> events = GenerateIntersectionEvents(lines);

            events = events.OrderBy(e => e.Point.X).ThenBy(e => e.Point.Y).ToList();

            List<CGUtilities.Line> activeLines = new List<CGUtilities.Line>();

            foreach (var ev in events)
            {
                ProcessEvent(ev, activeLines, ref intersections, ref outLines, checkedLines);
            }

            outPoints = intersections;
        }

        private List<Event> GenerateIntersectionEvents(List<CGUtilities.Line> lines)
        {
            List<Event> events = new List<Event>();
            int lineCount = lines.Count;

            for (int i = 0; i < lineCount - 1; i++)
            {
                for (int j = i + 1; j < lineCount; j++)
                {
                    CGUtilities.Line line1 = lines[i];
                    CGUtilities.Line line2 = lines[j];

                    events.Add(new Event(line1.Start, line1, EventType.Start));
                    events.Add(new Event(line1.End, line1, EventType.End));

                    events.Add(new Event(line2.Start, line2, EventType.Start));
                    events.Add(new Event(line2.End, line2, EventType.End));
                }
            }

            return events;
        }

        private void ProcessEvent(Event ev, List<CGUtilities.Line> activeLines, ref List<CGUtilities.Point> intersections, ref List<CGUtilities.Line> outLines, HashSet<CGUtilities.Line> checkedLines)
        {
            if (ev.Type == EventType.Start)
            {
                activeLines.Add(ev.Line);
                CheckEditOverlappingLines(ev.Line, checkedLines, activeLines, ev.Point, ref intersections, ref outLines);
            }
            else if (ev.Type == EventType.End)
            {
                CheckEditOverlappingLines(ev.Line, checkedLines, activeLines, ev.Point, ref intersections, ref outLines);
                activeLines.Remove(ev.Line);
            }
            else if (ev.Type == EventType.Intersection)
            {
                IntersectionCaseHandle(ev, activeLines, ref intersections, ref outLines);
            }

            ProcessIntersectionEvents(ev, activeLines, ref intersections, ref outLines);
        }

        private void ProcessIntersectionEvents(Event ev, List<CGUtilities.Line> activeLines, ref List<CGUtilities.Point> intersections, ref List<CGUtilities.Line> outLines)
        {
            foreach (var otherLine in activeLines.Where(line => line != ev.Line))
            {
                List<CGUtilities.Point> lineIntersections = CalculateIntersectionPoints(ev.Line, otherLine);
                intersections.AddRange(lineIntersections);

                outLines.AddRange(lineIntersections.Select(point => ev.Line));
                outLines.AddRange(lineIntersections.Select(point => otherLine));
            }
        }

        private void CheckEditOverlappingLines(CGUtilities.Line currentLine, HashSet<CGUtilities.Line> checkedLines, List<CGUtilities.Line> activeLines, CGUtilities.Point currentPoint, ref List<CGUtilities.Point> intersections, ref List<CGUtilities.Line> outLines)
        {
            foreach (var line in activeLines.ToList())
            {
                if (line != null && !checkedLines.Contains(line) && CheckOverlappingLines(currentLine, line))
                {
                    activeLines.Remove(line);
                    OverlappingCaseHandle(currentLine, line, ref intersections, ref outLines);
                    checkedLines.Add(line);
                }
            }
        }

        private bool CheckOverlappingLines(CGUtilities.Line line1, CGUtilities.Line line2)
        {
            if (line1 == null || line2 == null)
            {
                return false;
            }

            return (line1.Start.Y <= line2.Start.Y && line2.Start.Y <= line1.End.Y) ||
                   (line2.Start.Y <= line1.Start.Y && line1.Start.Y <= line2.End.Y);
        }

        private void OverlappingCaseHandle(CGUtilities.Line line1, CGUtilities.Line line2, ref List<CGUtilities.Point> intersections, ref List<CGUtilities.Line> outLines)
        {
            if (CheckParallelLines(line1, line2))
            {
                return;
            }

            List<CGUtilities.Point> intersectionPoints = CalculateIntersectionPoints(line1, line2);
            intersections.AddRange(intersectionPoints);

            outLines.AddRange(intersectionPoints.Select(point => line1));
            outLines.AddRange(intersectionPoints.Select(point => line2));
        }

        private bool CheckParallelLines(CGUtilities.Line line1, CGUtilities.Line line2)
        {
            if (line1.Start.X == line1.End.X && line2.Start.X == line2.End.X)
            {
                return true;
            }

            double denominator1 = line1.End.X - line1.Start.X;
            double denominator2 = line2.End.X - line2.Start.X;

            if (Math.Abs(denominator1) < 0 || Math.Abs(denominator2) < 0)
            {
                return true;
            }

            double slope1 = (line1.End.Y - line1.Start.Y) / denominator1;
            double slope2 = (line2.End.Y - line2.Start.Y) / denominator2;

            return Math.Abs(slope1 - slope2) < 0;
        }

        private void IntersectionCaseHandle(Event ev, List<CGUtilities.Line> activeLines, ref List<CGUtilities.Point> intersections, ref List<CGUtilities.Line> outLines)
        {
            CGUtilities.Line above = null;
            CGUtilities.Line below = null;

            foreach (var line in activeLines)
            {
                if (line.Start.Y <= ev.Point.Y && line.End.Y <= ev.Point.Y)
                {
                    below = line;
                }
                else if (line.Start.Y >= ev.Point.Y && line.End.Y >= ev.Point.Y)
                {
                    above = line;
                }
                else
                {
                    if (CheckOverlappingLines(line, ev.Line))
                    {
                        List<CGUtilities.Point> lineIntersections = CalculateIntersectionPoints(line, ev.Line);
                        intersections.AddRange(lineIntersections);

                        outLines.AddRange(lineIntersections.Select(point => line));
                        outLines.AddRange(lineIntersections.Select(point => ev.Line));
                    }
                }
            }

            if (above != null && below != null && above.Start.Y > below.Start.Y)
            {
                Swap(ref above, ref below);
            }

            if (above != null)
            {
                outLines.Add(above);
                outLines.Add(ev.Line);
            }

            if (below != null)
            {
                outLines.Add(below);
                outLines.Add(ev.Line);
            }

            activeLines.Remove(ev.Line);
        }

        private void Swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }

        private List<CGUtilities.Point> CalculateIntersectionPoints(CGUtilities.Line l1, CGUtilities.Line l2)
        {
            List<CGUtilities.Point> intersectionPoints = new List<CGUtilities.Point>();

            double x1 = l1.Start.X, y1 = l1.Start.Y;
            double x2 = l1.End.X, y2 = l1.End.Y;
            double x3 = l2.Start.X, y3 = l2.Start.Y;
            double x4 = l2.End.X, y4 = l2.End.Y;

            double m1 = (y2 - y1) / (x2 - x1);
            double b1 = y1 - m1 * x1;

            double m2 = (y4 - y3) / (x4 - x3);
            double b2 = y3 - m2 * x3;

            double intersectX = (b2 - b1) / (m1 - m2);
            double intersectY = m1 * intersectX + b1;

            CGUtilities.Point intersectionPoint = new CGUtilities.Point(intersectX, intersectY);
            if (DoesPointBelongToLine(intersectionPoint, l1) && DoesPointBelongToLine(intersectionPoint, l2))
            {
                intersectionPoints.Add(intersectionPoint);
            }

            return intersectionPoints;
        }

        private bool DoesPointBelongToLine(CGUtilities.Point point, CGUtilities.Line line)
        {
            double minX = Math.Min(line.Start.X, line.End.X);
            double maxX = Math.Max(line.Start.X, line.End.X);
            double minY = Math.Min(line.Start.Y, line.End.Y);
            double maxY = Math.Max(line.Start.Y, line.End.Y);

            return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
        }

        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}