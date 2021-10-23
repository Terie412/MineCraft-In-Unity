using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoronoiEngine
{
	public class Voronoi
	{
		public List<DCELPosition> unCheckedPoints;

		public DCEL dcel;
		private List<DCELVertex> superTriangleVertices;

		public Voronoi()
		{
			dcel = new DCEL();
		}

		public void Init(DCELPosition[] points)
		{
			unCheckedPoints = new List<DCELPosition>(points);
			superTriangleVertices = new List<DCELVertex>();
		}

		public void Run()
		{
			if (unCheckedPoints.Count < 3)
			{
				Debug.LogError($"There has to be at least three points in voronoi diagram");
				return;
			}

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
			DCELHalfEdge e01 = new DCELHalfEdge {ori = v0};
			DCELHalfEdge e12 = new DCELHalfEdge {ori = v1};
			DCELHalfEdge e20 = new DCELHalfEdge {ori = v2};
			DCELHalfEdge e10 = new DCELHalfEdge {ori = v1};
			DCELHalfEdge e21 = new DCELHalfEdge {ori = v2};
			DCELHalfEdge e02 = new DCELHalfEdge {ori = v0};
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
			e12.pre = e01;
			e20.pre = e12;
			e10.pre = e21;
			e21.pre = e02;
			e02.pre = e10;

			e01.suc = e12;
			e12.suc = e20;
			e20.suc = e01;
			e10.suc = e02;
			e21.suc = e10;
			e02.suc = e21;

			e01.face = superFace;
			e12.face = superFace;
			e20.face = superFace;
			dcel.AddVertex(v0);
			dcel.AddVertex(v1);
			dcel.AddVertex(v2);
			dcel.AddEdge(e01);
			dcel.AddEdge(e10);
			dcel.AddEdge(e12);
			dcel.AddEdge(e21);
			dcel.AddEdge(e20);
			dcel.AddEdge(e02);
			dcel.faceList.Add(superFace);

			superTriangleVertices.Add(v0);
			superTriangleVertices.Add(v1);
			superTriangleVertices.Add(v2);

			v0.dirty = true;
			v1.dirty = true;
			v2.dirty = true;

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

			var preRemoveVertices = dcel.vertexList.Where(v => v.dirty).ToList();
			foreach (var v in preRemoveVertices)
			{
				dcel.TryRemoveVertex(v);
			}

			var preRemoveFaces = new List<DCELFace>();
			foreach (var f in dcel.faceList)
			{
				var e = f.edge;
				var e1 = e.suc;
				var e2 = e1.suc;

				if (e.ori.dirty || e1.ori.dirty || e2.ori.dirty)
				{
					preRemoveFaces.Add(f);
				}
			}

			foreach (var f in preRemoveFaces)
			{
				dcel.TryRemoveFace(f);
			}

			Debug.Log($"{dcel}");
		}

		private void CheckEdge(DCELHalfEdge edge, DCELVertex vertex)
		{
			Debug.Log($"CheckEdge {edge} {vertex}");

			if (edge.face == null || edge.twin.face == null) return;

			var e = edge;
			var p = vertex;
			var p1 = e.ori;
			var p2 = e.suc.ori;
			var p0 = e.twin.pre.ori;

			var x1 = p2.position.x;
			var y1 = p2.position.y;
			var x2 = p.position.x;
			var y2 = p.position.y;
			var x3 = p1.position.x;
			var y3 = p1.position.y;
			var x4 = p0.position.x;
			var y4 = p0.position.y;

			// Vector3 vec1 = new Vector3(p.position.x - p2.position.x, p.position.x - p2.position.y);
			// Vector3 vec2 = new Vector3(p1.position.x - p.position.x, p1.position.x - p.position.y);
			//
			// Vector3 cross = Vector3.Cross(vec1, vec2);
			// Debug.Log($"cross = {cross}");

			// var res = MCMath.Determinant(new[]
			// {
			// 	new[] {x1, y1, x1 * x1 + y1 * y1, 1},
			// 	new[] {x2, y2, x2 * x2 + y2 * y2, 1},
			// 	new[] {x3, y3, x3 * x3 + y3 * y3, 1},
			// 	new[] {x4, y4, x4 * x4 + y4 * y4, 1}
			// });

			var isInCircle = MCMath.InCircumcircleTest(new[]
			{
				new[] {x1, y1},
				new[] {x2, y2},
				new[] {x3, y3},
			}, new[] {x4, y4});

			if (isInCircle)
			{
				// dcel.edgeList.Remove(edge);
				FlipEdge(edge, vertex, out var newEdges);
				foreach (var newEdge in newEdges)
				{
					CheckEdge(newEdge, vertex);
				}
			}
		}
		
		private void AddPointToDelaunay(DCELPosition position, out DCELHalfEdge[] unCheckedEdges, out DCELVertex newVertex)
		{
			Debug.Log($"AddPointToDelaunay {position}");
			// todo: Check if the point lies on one the edges
			newVertex = new DCELVertex(position.x, position.y);
			dcel.AddVertex(newVertex);

			// Add d point to delaunay will lead to one of the surfaces to tear, we have to check which face it is
			DCELFace f1 = GetFaceContainsPoint(position);
			if (f1 == null)
			{
				// The current point falls on one of the halfedges, and we choose to ignore it
				Debug.LogWarning($"Ignore point {position}");
				unCheckedEdges = new DCELHalfEdge[0];
				newVertex = null;
				return;
			}

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

			dcel.AddEdge(e4);
			dcel.AddEdge(e5);
			dcel.AddEdge(e6);
			dcel.AddEdge(e7);
			dcel.AddEdge(e8);
			dcel.AddEdge(e9);

			e4.ori = p2;
			e5.ori = newVertex;
			e6.ori = newVertex;
			e7.ori = p1;
			e8.ori = p3;
			e9.ori = newVertex;

			e4.suc = e6;
			e6.pre = e4;
			e4.pre = e1;
			e1.suc = e4;
			e5.suc = e2;
			e2.pre = e5;
			e5.pre = e8;
			e8.suc = e5;
			e6.suc = e1;
			e1.pre = e6;
			e7.suc = e9;
			e9.pre = e7;
			e7.pre = e3;
			e3.suc = e7;
			e8.pre = e2;
			e2.suc = e8;
			e9.suc = e3;
			e3.pre = e9;

			e4.twin = e5;
			e5.twin = e4;
			e6.twin = e7;
			e7.twin = e6;
			e8.twin = e9;
			e9.twin = e8;

			e1.face = e4.face = e6.face = f1;
			e2.face = e8.face = e5.face = f2;
			e3.face = e7.face = e9.face = f3;

			f2.edge = e2;
			f3.edge = e3;

			newVertex.edge = e6;

			unCheckedEdges = new[] {e1, e2, e3};
		}

		private void FlipEdge(DCELHalfEdge e, DCELVertex v, out DCELHalfEdge[] edges)
		{
			Debug.Log($"FlipEdge {e}");
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
			e.face = f1;
			e.twin.face = f2;

			e.ori = p4;
			e.twin.ori = p3;

			e1.pre = e4;
			e1.suc = e.twin;
			e.twin.pre = e1;
			e.twin.suc = e4;
			e4.pre = e.twin;
			e4.suc = e1;
			e.pre = e3;
			e.suc = e2;
			e2.pre = e;
			e2.suc = e3;
			e3.pre = e2;
			e3.suc = e;

			p1.edge = e3;
			p2.edge = e1;
			p3.edge = e2;
			p4.edge = e4;

			edges = new[] {e1, e2, e3, e4}.Where(_e => _e.ori != v && _e.twin.ori != v).ToArray();
			var a = 1;
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

			Debug.Log($"Fail to find a face that contains point {position}");
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
				up = Mathf.Max(point.y, up);
				right = Mathf.Max(point.x, right);
				down = Mathf.Min(point.y, down);
				left = Mathf.Min(point.x, left);
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