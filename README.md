# Computational Geometry Algorithms

This repository contains implementations of various fundamental computational geometry algorithms in C#. These algorithms are commonly used in fields such as computer graphics, geographic information systems, and robotics. Below is a list of algorithms included:

## Convex Hull Algorithms
- **Extreme Point**: Identifies extreme points of a given set of points to construct the convex hull.
- **Extreme Segment**: Finds extreme segments to form the convex hull efficiently.
- **Jarvis March (Gift Wrapping)**: A simple but inefficient algorithm to compute the convex hull.
- **Graham Scan**: A classic algorithm for finding the convex hull of a set of points in O(n log n) time.
- **Incremental**: Builds the convex hull incrementally by adding points one by one.
- **Divide and Conquer**: An efficient algorithm for computing the convex hull of a set of points by recursively dividing the points.

## Segment Intersection
- **Sweep Line**: Detects intersections among a set of line segments using a sweep line technique.

## Polygon Triangulation Algorithms
- **Inserting Diagonals**: Decomposes a polygon into triangles by inserting diagonals.
- **Subtracting Ears**: Identifies and removes "ears" from the polygon until it's triangulated.
- **Monotone Triangulation**: Triangulates a monotone polygon by partitioning it into monotone pieces.
- **Monotone Partitioning**: Partitions a polygon into monotone pieces for efficient triangulation.

These algorithms are implemented with a focus on readability and efficiency. Feel free to explore the code, contribute improvements, or use them in your projects.
