using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronoiEngine
{
    public class Voronoi
    {
        public List<Point> points;
        public List<Triangle> triangles;

        public Voronoi()
        {
            points = new List<Point>();
        }

        public Voronoi(List<Point> points)
        {
            this.points = points;
        }

        private void DelaunayTriangulate()
        {
            // Image that there are some random points scattered in the scene, so how to draw a mesh that looks "nice"?
            // One of the good choices is delaunay triangulation.
            // A delaunay triangle means there is no other point lies inside then circumcircle

            // Note that there is duality between the Voronoi diagram and the Delaunay triangle,
            // We will use delaunay triangle to help us build the voronoi diagram

            // FIRST: we will find a triangle containing all of our points and call this "super-triangle"
            Triangle superTriangle = GetSuperTriangle();
            points.Add(superTriangle.points[0]);
            points.Add(superTriangle.points[1]);
            points.Add(superTriangle.points[2]);

            triangles.Add(superTriangle);

            // SECOND: traverse all points and check if it lies inside any triangle's circumcircle
        }

        private Triangle GetSuperTriangle()
        {
            float up = float.MinValue;
            float right = float.MinValue;
            float down = float.MaxValue;
            float left = float.MaxValue;
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                up = Mathf.Max(point.y);
                right = Mathf.Max(point.x);
                down = Mathf.Min(point.y);
                left = Mathf.Min(point.x);
            }

            up += 1;
            right += 1;
            left += 1;
            down += 1;

            Point centerPoint = new Point((right + left) / 2, (up + down) / 2);
            float r = Mathf.Sqrt(Mathf.Pow(right - left, 2f) + Mathf.Pow(up - down, 2f));
            float sqrt3 = Mathf.Sqrt(3);
            List<Point> trianglePoints = new List<Point>();

            Point p1 = new Point(-sqrt3 * r, -r) + centerPoint;
            Point p2 = new Point(sqrt3 * r, -r) + centerPoint;
            Point p3 = new Point(0, 2 * r) + centerPoint;
            trianglePoints.Add(p1);
            trianglePoints.Add(p2);
            trianglePoints.Add(p3);

            return new Triangle(trianglePoints.ToArray());
        }
    }
}