using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VoronoiEngine
{
	public class Voronoi
	{
		public Dictionary<int, VoronoiFace> id_face;
		public Dictionary<int, int> dcelVertexID_faceID;
		public List<VoronoiFace> faceList;
		public List<VoronoiHalfEdge> edgeList;
		public List<VoronoiVertex> vertexList;

		public Voronoi()
		{
			id_face = new Dictionary<int, VoronoiFace>();
			dcelVertexID_faceID = new Dictionary<int, int>();
			faceList = new List<VoronoiFace>();
			edgeList = new List<VoronoiHalfEdge>();
			vertexList = new List<VoronoiVertex>();
		}
		
		public static Voronoi FromDCEL(DCEL dcel)
		{
			// Each vertex in Delaunay triangle refers to one face in Voronoi
			Voronoi voronoi = new Voronoi();
			foreach (var v in dcel.vertexList)
			{
				VoronoiFace f = new VoronoiFace();
				f.id = voronoi.faceList.Count + 1;
				voronoi.faceList.Add(f);
				voronoi.id_face[f.id] = f;
				voronoi.dcelVertexID_faceID[v.id] = f.id;
				f.center = v.position;
			}

			foreach (var f in dcel.faceList)
			{
				var c0 = f.GetCircumcircle();
				if (c0.x < 0 || c0.y < 0 || c0.x > 1 || c0.y > 1)
				{
					continue;
				}

				var voronoiV0 = new VoronoiVertex();
				voronoiV0.id = voronoi.vertexList.Count + 1;
				voronoiV0.position = new Vector2(c0.x, c0.y);
				voronoi.vertexList.Add(voronoiV0);

				var e1 = f.edge;
				var e2 = e1.suc;
				var e3 = e2.suc;

				var edgeList = new[] {e1, e2, e3};
				foreach (var e in edgeList)
				{
					if (e.twin?.face != null)
					{
						var c1 = e.twin.face.GetCircumcircle();
						if (c1.x > 0 && c1.x < 1 && c1.y > 0 && c1.y < 1)
						{
							var edge = new VoronoiHalfEdge();
							
							edge.start = new Vector2(c0.x, c0.y);
							edge.end = new Vector2(c1.x, c1.y);
							edge.ori = voronoiV0;
							voronoiV0.edge = edge;
							voronoi.edgeList.Add(edge);

							if (MCMath.GetPositionOrientationToEdge(new[] {e.ori.position.x, e.ori.position.y}, new []
							{
								new[] {c0.x, c0.y},
								new[] {c1.x, c1.y},
							}) > 0)
							{
								edge.face = voronoi.id_face[voronoi.dcelVertexID_faceID[e.ori.id]];
							}
							else
							{
								edge.face = voronoi.id_face[voronoi.dcelVertexID_faceID[e.twin.ori.id]];
							}
						}
					}
				}
			}

			return voronoi;
		}
	}

	public class VoronoiHalfEdge
	{
		public Vector2 start;
		public Vector2 end;

		public VoronoiVertex ori;
		public VoronoiHalfEdge pre;
		public VoronoiHalfEdge suc;

		public VoronoiFace face;
	}

	public class VoronoiFace
	{
		public int id;

		public Vector2 center;
	}

	public class VoronoiVertex
	{
		public int id;
		public Vector2 position;
		public VoronoiHalfEdge edge;
	}

	public class DCEL
	{
		public enum Mode
		{
			Delaunay,
			Voronoi
		}

		public List<DCELFace> faceList;
		public List<DCELHalfEdge> edgeList;
		public List<DCELVertex> vertexList;
		public Mode mode = Mode.Delaunay;

		public DCEL()
		{
			faceList = new List<DCELFace>();
			edgeList = new List<DCELHalfEdge>();
			vertexList = new List<DCELVertex>();
		}

		public int AddEdge(DCELHalfEdge e)
		{
			edgeList.Add(e);
			e.id = edgeList.Count;
			return e.id;
		}

		public int AddVertex(DCELVertex v)
		{
			vertexList.Add(v);
			v.id = vertexList.Count;
			return v.id;
		}

		public void TryRemoveVertex(DCELVertex v)
		{
			vertexList.Remove(v);
		}

		public void TryRemoveFace(DCELFace f)
		{
			faceList.Remove(f);
			var e = f.edge;
			var e1 = f.edge.suc;
			var e2 = e1.suc;

			if (e.twin != null)
			{
				e.twin.twin = null;
			}

			if (e1.twin != null)
			{
				e1.twin.twin = null;
			}

			if (e2.twin != null)
			{
				e2.twin.twin = null;
			}

			edgeList.Remove(e);
			edgeList.Remove(e1);
			edgeList.Remove(e2);

			if (e.twin != null && e.twin.face == null)
			{
				edgeList.Remove(e.twin);
				e.twin = null;
			}

			if (e1.twin != null && e1.twin.face == null)
			{
				edgeList.Remove(e1.twin);
				e1.twin = null;
			}

			if (e2.twin != null && e2.twin.face == null)
			{
				edgeList.Remove(e2.twin);
				e2.twin = null;
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Total faces: {faceList.Count}, total edges: {edgeList.Count}, total vertices: {vertexList.Count}\n");
			foreach (var f in faceList)
			{
				var e = f.edge;
				// if (!e.ori.dirty && !e.suc.ori.dirty && !e.suc.suc.ori.dirty)
				sb.Append($"{e.ori},{e.suc.ori},{e.suc.suc.ori}\n");
			}

			return sb.ToString();
		}
	}

	public class DCELFace
	{
		public int id;
		public DCELHalfEdge edge;

		public bool ContainsPosition(Vector2 position)
		{
			// Only if the point is to the same side (all to the right or all to the left)
			// of all three edges does that mean the face contains the point
			var e1 = edge;
			var e2 = e1.suc;
			var e3 = e2.suc;
			var res1 = e1.GetPositionOrientation(position);
			var res2 = e2.GetPositionOrientation(position);
			var res3 = e3.GetPositionOrientation(position);
			return (res1 < 0 && res2 < 0 && res3 < 0) || (res1 > 0 && res2 > 0 && res3 > 0);
		}

		/// (x,y) for center, z for radius
		public Vector3 GetCircumcircle()
		{
			var v1 = edge.ori;
			var v2 = edge.suc.ori;
			var v3 = edge.suc.suc.ori;

			var x0 = v1.position.x;
			var y0 = v1.position.y;
			var x1 = v2.position.x;
			var y1 = v2.position.y;
			var x2 = v3.position.x;
			var y2 = v3.position.y;

			var d = MCMath.Determinant(new[]
			{
				new[] {x0, y0, 1},
				new[] {x1, y1, 1},
				new[] {x2, y2, 1},
			}) * 2;

			var x = MCMath.Determinant(new[]
			{
				new[] {x0 * x0 + y0 * y0, y0, 1},
				new[] {x1 * x1 + y1 * y1, y1, 1},
				new[] {x2 * x2 + y2 * y2, y2, 1},
			}) / d;

			var y = MCMath.Determinant(new[]
			{
				new[] {x0, x0 * x0 + y0 * y0, 1},
				new[] {x1, x1 * x1 + y1 * y1, 1},
				new[] {x2, x2 * x2 + y2 * y2, 1},
			}) / d;

			var r = Mathf.Sqrt((x - x0) * (x - x0) + (y - y0) * (y - y0));
			return new Vector3(x, y, r);
		}
	}

	public class DCELHalfEdge
	{
		public int id;
		public DCELVertex ori; // The starting point of the edge

		public bool dirty = false;

		public DCELHalfEdge twin;
		public DCELFace face;

		public DCELHalfEdge pre;
		public DCELHalfEdge suc;

		/// <returns>return positive for left, negative for right</returns>
		public float GetPositionOrientation(Vector2 position)
		{
			float x1 = ori.position.x;
			float y1 = ori.position.y;
			float x2 = suc.ori.position.x;
			float y2 = suc.ori.position.y;
			float x = position.x;
			float y = position.y;
			return x1 * (y2 - y) + y1 * (x - x2) + x2 * y - x * y2;
		}

		public override string ToString()
		{
			return $"{id}:{ori.id}->{suc.ori.id}";
		}
	}

	public class DCELVertex
	{
		public int id;
		public Vector2 position;
		public DCELHalfEdge edge; // one of the output edge
		public bool dirty = false;

		public DCELVertex(float x, float y)
		{
			position = new Vector2(x, y);
		}

		public override string ToString()
		{
			return $"{id}:({position.x},{position.y})";
		}
	}
}