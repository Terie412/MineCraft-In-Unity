using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VoronoiEngine;

public class VoronoiTest : MonoBehaviour
{
    public List<Vector2> points;

    public Vector2 vec;
    
    void Start()
    {
        var voronoi = new Voronoi();

        List<DCELPosition> poses = new List<DCELPosition>();
        // foreach (var vec in points)
        // {
        //     poses.Add(Vector2ToDCELPosition(vec));
        // }

        for (int i = 0; i < 1000; i++)
        {
            poses.Add(new DCELPosition(Random.value, Random.value));
        }
        
        string x = "";
        for (var i = 0; i < poses.Count; i++)
        {
            x += $"{poses[i].x},{poses[i].y}\n";
        }
        
        Debug.Log($"Insert positions: {x}");
        
        voronoi.Init(poses.ToArray());
        voronoi.Run();
        
        WriteDCELToFile(voronoi.dcel);
        
        // CircumcircleContainsPoint(new []
        // {
        //     new DCELPosition(0, 0),
        //     new DCELPosition(1, 0),
        //     new DCELPosition(0, 1),
        // }, Vector2ToDCELPosition(vec));
    }

    private void CircumcircleContainsPoint(DCELPosition[] tri, DCELPosition v)
    {
        var x1 = tri[0].x;
        var y1 = tri[0].y;
        var x2 = tri[1].x;
        var y2 = tri[1].y;
        var x3 = tri[2].x;
        var y3 = tri[2].y;
        var x4 = v.x;
        var y4 = v.y;

        // positive means outside of the circumcircle, negative means inside
        var res = MCMath.Determinant(new[]
        {
            new[] {x1, y1, x1 * x1 + y1 * y1, 1},
            new[] {x2, y2, x2 * x2 + y2 * y2, 1},
            new[] {x3, y3, x3 * x3 + y3 * y3, 1},
            new[] {x4, y4, x4 * x4 + y4 * y4, 1}
        });

        Debug.Log($"res = {res}");
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

    private void WriteDCELToFile(DCEL dcel)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var f in dcel.faceList)
        {
            var e = f.edge;
            DCELVertex v1 = e.ori;
            var v2 = e.suc.ori;
            var v3 = e.suc.suc.ori;
            sb.Append($"{DCELVertexToString(v1)}\n{DCELVertexToString(v2)}\n\n{DCELVertexToString(v2)}\n{DCELVertexToString(v3)}\n\n{DCELVertexToString(v3)}\n{DCELVertexToString(v1)}\n\n");
            // sb.Append($"{e.ori},{e.suc.ori},{e.suc.suc.ori}\n");
        }
        
        File.WriteAllText("DCEL.csv", sb.ToString());
    }

    private string DCELVertexToString(DCELVertex v)
    {
        return $"{v.position.x},{v.position.y}";
    }
}
