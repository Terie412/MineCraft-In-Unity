using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronoiEngine
{
    public class Voronoi
    {
        public List<DCELPosition> unCheckedPoints;
        private DCEL dcel;
        
        public Voronoi()
        {
            dcel = new DCEL();
        }

        public void Init(DCELPosition[] points)
        {
            unCheckedPoints = new List<DCELPosition>(points);
        }

        public void Run()
        {
            DelaunayTriangulate();
        }
        
        private void DelaunayTriangulate()
        {
            // Image that there are some random points scattered in the scene, so how to draw a mesh that looks "nice"?
            // One of the good choices is delaunay triangulation.
            // A delaunay triangle means there is no other point lies inside then circumcircle

            // Note that there is duality between the Voronoi diagram and the Delaunay triangle,
            // We will use delaunay triangle to help us build the voronoi diagram

            // FIRST: we will find a triangle containing all of our points and call this "super-triangle"
            DCELPosition[] superTriangle = GetSuperTriangle(unCheckedPoints);
            DCELVertex v0 = new DCELVertex(superTriangle[0].x, superTriangle[0].y);
            DCELVertex v1 = new DCELVertex(superTriangle[1].x, superTriangle[1].y);
            DCELVertex v2 = new DCELVertex(superTriangle[2].x, superTriangle[2].y);
            DCELHalfEdge e01 = new DCELHalfEdge(v0, v1);
            DCELHalfEdge e12 = new DCELHalfEdge(v1, v2);
            DCELHalfEdge e20 = new DCELHalfEdge(v2, v0);
            DCELHalfEdge e10 = new DCELHalfEdge(v1, v0);
            DCELHalfEdge e21 = new DCELHalfEdge(v2, v1);
            DCELHalfEdge e02 = new DCELHalfEdge(v0, v2);
            e01.twinEdge = e10;
            e10.twinEdge = e01;
            e12.twinEdge = e21;
            e21.twinEdge = e12;
            e20.twinEdge = e02;
            e02.twinEdge = e20;
            DCELFace superFace = new DCELFace();
            superFace.edge = e01;
            v0.edge = e01;
            v1.edge = e12;
            v2.edge = e20;
            e01.preEdge = e20;
            e01.sucEdge = e12;
            e12.preEdge = e01;
            e12.sucEdge = e20;
            e20.preEdge = e12;
            e20.sucEdge = e01;
            e01.face = superFace;
            e12.face = superFace;
            e20.face = superFace;
            dcel.vertexList.Add(v0);
            dcel.vertexList.Add(v1);
            dcel.vertexList.Add(v2);
            dcel.edgeList.Add(e01);
            dcel.edgeList.Add(e10);
            dcel.edgeList.Add(e12);
            dcel.edgeList.Add(e21);
            dcel.edgeList.Add(e20);
            dcel.edgeList.Add(e02);
            dcel.faceList.Add(superFace);

            // SECOND: choose one point to add to the List first
            var firstPoint = unCheckedPoints[0];
            unCheckedPoints.Remove(firstPoint);
            
        }
        
        private void ConnectPointsToCreateTriangles(Point center, Point[] ps)
        {
            if (ps.Length < 2) return;
            
            for (var i = 0; i < ps.Length-1; i++)
            {
                Point[] triPoints = new Point[3];
                triPoints[0] = center;
                triPoints[1] = ps[i];
                triPoints[2] = ps[i+1];
                // triangles.Add(new Triangle(triPoints));    
            }
        }

        // Get a super triangle from given collection of Point
        private DCELPosition[] GetSuperTriangle(IEnumerable<DCELPosition> _points)
        {
            // To find out the super triangle, we can get the bound box of all these points
            // Then figure out the super triangle which incircle is also the circumcircle of the bound  
            float up = float.MinValue;
            float right = float.MinValue;
            float down = float.MaxValue;
            float left = float.MaxValue;
            // for (var i = 0; i < _points.Length; i++)
            foreach (var point in _points)
            {
                // var point = _points[i];
                up = Mathf.Max(point.y);
                right = Mathf.Max(point.x);
                down = Mathf.Min(point.y);
                left = Mathf.Min(point.x);
            }
            
            DCELPosition centerPoint = new DCELPosition((right + left) / 2, (up + down) / 2);
            float r = Mathf.Sqrt(Mathf.Pow(right - left, 2f) + Mathf.Pow(up - down, 2f));
            float sqrt3 = Mathf.Sqrt(3);
           
            DCELPosition p1 = new DCELPosition(-sqrt3 * r, -r) + centerPoint;
            DCELPosition p2 = new DCELPosition(sqrt3 * r, -r) + centerPoint;
            DCELPosition p3 = new DCELPosition(0, 2 * r) + centerPoint;
            return new[] {p1, p2, p3};
        }
    }
}