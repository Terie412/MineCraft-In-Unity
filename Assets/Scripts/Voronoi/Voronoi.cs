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
            DCELHalfEdge e01 = new DCELHalfEdge{ori = v0};
            DCELHalfEdge e12 = new DCELHalfEdge{ori = v1};
            DCELHalfEdge e20 = new DCELHalfEdge{ori = v2};
            DCELHalfEdge e10 = new DCELHalfEdge{ori = v1};
            DCELHalfEdge e21 = new DCELHalfEdge{ori = v2};
            DCELHalfEdge e02 = new DCELHalfEdge{ori = v0};
            e01.twin = e10;
            e10.twin = e01;
            e12.twin = e21;
            e21.twin = e12;
            e20.twin = e02;
            e02.twin = e20;
            DCELFace superFace = new DCELFace();
            superFace.edge = e01;
            v0.edge = e01;
            v1.edge = e12;
            v2.edge = e20;
            e01.pre = e20;
            e01.suc = e12;
            e12.pre = e01;
            e12.suc = e20;
            e20.pre = e12;
            e20.suc = e01;
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

            // SECOND: choose one point to add to the List
            while (unCheckedPoints.Count > 0)
            {
                var point = unCheckedPoints[unCheckedPoints.Count - 1];
                unCheckedPoints.RemoveAt(unCheckedPoints.Count - 1);
                AddPointToDelaunay(point, out var unCheckedEdges, out var newVertex);
                foreach (var edge in unCheckedEdges)
                {
                    CheckEdge(edge, newVertex);
                }
            }

            Debug.Log($"{dcel}");
        }

        private void CheckEdge(DCELHalfEdge edge, DCELVertex vertex)
        {
            var e = edge;
            var p = vertex;
            var p1 = e.ori;
            var p2 = e.suc.ori;
            var p0 = e.twin.pre.ori;

            var x1 = p.position.x;
            var y1 = p.position.y;
            var x2 = p1.position.x;
            var y2 = p1.position.y;
            var x3 = p2.position.x;
            var y3 = p2.position.y;
            var x4 = p0.position.x;
            var y4 = p0.position.y;

            // positive means outside of the circumcircle, negative means inside
            var res = MCMath.Determinant(new[]
            {
                new[] {x1, y1, x1 * x1 + y1 * y1, 1},
                new[] {x2, y2, x2 * x2 + y2 * y2, 1},
                new[] {x3, y3, x3 * x3 + y3 * y3, 1},
                new[] {x4, y4, x4 * x4 + y4 * y4, 1}
            });

            if (res <= 0)
            {
                // dcel.edgeList.Remove(edge);
                FlipEdge(edge, out var newEdges);
                foreach (var newEdge in newEdges)
                {
                    if (newEdge.ori != vertex && newEdge.suc.ori != vertex)
                    {
                        CheckEdge(newEdge, vertex);
                    }
                }
            }

        }

        private void AddPointToDelaunay(DCELPosition position, out DCELHalfEdge[] unCheckedEdges, out DCELVertex newVertex)
        {
            // todo: Check if the point lies on one the edges
            newVertex = new DCELVertex(position.x, position.y);
            dcel.vertexList.Add(newVertex);
            
            // Add d point to delaunay will lead to one of the surfaces to tear, we have to check which face it is
            DCELFace f1 = GetFaceContainsPoint(position);

            DCELHalfEdge e1 = f1.edge;
            DCELHalfEdge e2 = e1.suc;
            DCELHalfEdge e3 = e2.suc;
            
            // 1. tear the face
            DCELFace f2 = new DCELFace();
            DCELFace f3 = new DCELFace();
            f2.edge = f1.edge.suc;
            f3.edge = f2.edge.suc;
            dcel.faceList.Add(f2);
            dcel.faceList.Add(f3);
            
            DCELVertex p1 = f1.edge.ori;
            DCELVertex p2 = f1.edge.suc.ori;
            DCELVertex p3 = f1.edge.suc.suc.ori;

            DCELHalfEdge e4 = new DCELHalfEdge();
            DCELHalfEdge e5 = new DCELHalfEdge();
            DCELHalfEdge e6 = new DCELHalfEdge();
            DCELHalfEdge e7 = new DCELHalfEdge();
            DCELHalfEdge e8 = new DCELHalfEdge();
            DCELHalfEdge e9 = new DCELHalfEdge();
            
            dcel.edgeList.Add(e4);
            dcel.edgeList.Add(e5);
            dcel.edgeList.Add(e6);
            dcel.edgeList.Add(e7);
            dcel.edgeList.Add(e8);
            dcel.edgeList.Add(e9);

            e4.ori = p2;
            e5.ori = newVertex;
            e6.ori = newVertex;
            e7.ori = p1;
            e8.ori = p3;
            e9.ori = newVertex;

            e4.suc = e6;
            e4.pre = e1;
            e5.suc = e2;
            e5.pre = e8;
            e6.suc = e1;
            e6.pre = e4;
            e7.suc = e9;
            e7.pre = e3;
            e8.suc = e5;
            e8.pre = e2;
            e9.suc = e3;
            e9.pre = e7;

            e4.twin = e5;
            e5.twin = e4;
            e6.twin = e7;
            e7.twin = e6;
            e8.twin = e9;
            e9.twin = e8;

            e2.face = f2;
            e3.face = f3;
            f2.edge = e2;
            f3.edge = e3;

            newVertex.edge = e6;

            unCheckedEdges = new[] {e1, e2, e3};
        }

        private void FlipEdge(DCELHalfEdge e, out DCELHalfEdge[] edges)
        {
            var e1 = e.suc;
            var e2 = e.suc.suc;
            var e3 = e.twin.suc;
            var e4 = e.twin.suc.suc;

            var p1 = e.ori;
            var p2 = e.suc.ori;
            var p3 = e.suc.suc.ori;
            var p4 = e3.suc.ori;

            var f1 = e.face;
            var f2 = e.twin.face;

            f1.edge = e;
            f2.edge = e.twin;

            p1.edge = e3;
            p2.edge = e1;
            p3.edge = e2;
            p4.edge = e4;

            e1.face = f2;
            e2.face = f1;
            e3.face = f1;
            e4.face = f2;

            e1.pre = e4;
            e1.suc = e.twin;
            e2.pre = e;
            e2.suc = e3;
            e3.pre = e2;
            e3.suc = e;
            e4.pre = e.twin;
            e4.suc = e1;
            e.pre = e3;
            e.suc = e2;
            e.twin.pre = e1;
            e.twin.suc = e4;

            edges = new[] {e1, e2, e3, e4};
        }

        private DCELFace GetFaceContainsPoint(DCELPosition position)
        {
            foreach (var dcelFace in dcel.faceList)
            {
                if (dcelFace.ContainsPosition(position))
                {
                    return dcelFace;
                }
            }

            return null;
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