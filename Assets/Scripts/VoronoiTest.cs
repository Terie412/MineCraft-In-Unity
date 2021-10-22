using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiEngine;

public class VoronoiTest : MonoBehaviour
{
    public List<Vector2> points;
    
    void Start()
    {
        var voronoi = new Voronoi();

        List<DCELPosition> poses = new List<DCELPosition>();
        foreach (var vec in points)
        {
            poses.Add(Vector2ToDCELPosition(vec));
        }
        
        voronoi.Init(poses.ToArray());
        voronoi.Run();
    }

    private DCELPosition Vector2ToDCELPosition(Vector2 vec)
    {
        return new DCELPosition(vec.x, vec.y);
    }
    
    private void TestPositionToEdgeOrientation()
    {
        var v1 = new DCELVertex(0, 0);
        var v2 = new DCELVertex(1, 1);
        var v3 = new DCELVertex(2, 2);

        var e1 = new DCELHalfEdge();
        var e2 = new DCELHalfEdge();
        e1.ori = v1;
        e2.ori = v2;
        e1.suc = e2;
        e2.pre = e1;

        Debug.Log($"position to edge orientation {e1.GetPositionOrientation(new DCELPosition(1, 0.5f))}");
    }

    private void TestFaceContainsPosition()
    {
        var v1 = new DCELVertex(0, 0);
        var v2 = new DCELVertex(3, 3);
        var v3 = new DCELVertex(6, 0);
        
        var e1 = new DCELHalfEdge();
        var e2 = new DCELHalfEdge();
        var e3 = new DCELHalfEdge();
        var e4 = new DCELHalfEdge();
        var e5 = new DCELHalfEdge();
        var e6 = new DCELHalfEdge();

        v1.edge = e1;
        v2.edge = e3;
        v3.edge = e5;

        e1.twin = e2;
        e2.twin = e1;
        e3.twin = e4;
        e4.twin = e3;
        e5.twin = e6;
        e6.twin = e5;

        e1.ori = v1;
        e3.ori = v2;
        e5.ori = v3;
        e6.ori = v1;
        e2.ori = v2;
        e4.ori = v3;

        e1.pre = e5;
        e2.pre = e4;
        e3.pre = e1;
        e4.pre = e6;
        e5.pre = e3;
        e6.pre = e2;
        
        e1.suc = e3;
        e2.suc = e6;
        e3.suc = e5;
        e4.suc = e2;
        e5.suc = e1;
        e6.suc = e4;
        
        var f = new DCELFace();
        
        e2.face = f;
        e4.face = f;
        e6.face = f;

        f.edge = e2;

        Debug.Log($"face contains ? {f.ContainsPosition(new DCELPosition(1, 2f))}");
    }
}
