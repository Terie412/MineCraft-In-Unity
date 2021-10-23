using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VoronoiEngine
{
	public class DCEL
	{
		public List<DCELFace> faceList;
		public List<DCELHalfEdge> edgeList;
		public List<DCELVertex> vertexList;

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
			}

			if (e1.twin != null && e1.twin.face == null)
			{
				edgeList.Remove(e1.twin);
			}

			if (e2.twin != null && e2.twin.face == null)
			{
				edgeList.Remove(e2.twin);
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

		public bool ContainsPosition(DCELPosition position)
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
		public float GetPositionOrientation(DCELPosition position)
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
		public DCELPosition position;
		public DCELHalfEdge edge; // one of the output edge
		public bool dirty = false;

		public DCELVertex(float x, float y)
		{
			position = new DCELPosition(x, y);
		}

		public override string ToString()
		{
			return $"{id}:({position.x},{position.y})";
		}
	}

	public class DCELPosition
	{
		public float x;
		public float y;

		public DCELPosition(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static DCELPosition operator +(DCELPosition a, DCELPosition b)
		{
			return new DCELPosition(a.x + b.x, a.y + b.y);
		}

		public override string ToString()
		{
			return $"{x},{y}";
		}
	}
}