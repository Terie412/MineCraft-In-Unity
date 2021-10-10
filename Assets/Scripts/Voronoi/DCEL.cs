using System.Collections.Generic;
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
    }

    public class DCELFace
    {
        public DCELHalfEdge edge;
    }
    
    public class DCELHalfEdge
    {
        public DCELVertex head;
        public DCELVertex tail;

        public DCELHalfEdge twinEdge;
        public DCELFace face;

        public DCELHalfEdge preEdge;
        public DCELHalfEdge sucEdge;

        public DCELHalfEdge(DCELVertex head, DCELVertex tail)
        {
            this.head = head;
            this.tail = tail;
        }
    }
    
    public class DCELVertex
    {
        public DCELPosition position;
        public DCELHalfEdge edge; // one of the output edge

        public DCELVertex(float x, float y)
        {
            position = new DCELPosition(x, y);
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
    }
}