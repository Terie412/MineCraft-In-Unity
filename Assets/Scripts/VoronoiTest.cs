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
        var engine = new Engine();

        List<Vector2> poses = new List<Vector2>();
        // foreach (var vec in points)
        // {
        //     poses.Add(Vector2ToDCELPosition(vec));
        // }

        for (int i = 0; i < 1000; i++)
        {
            poses.Add(new Vector2(Random.value, Random.value));
        }
        
        string x = "";
        for (var i = 0; i < poses.Count; i++)
        {
            x += $"{poses[i].x},{poses[i].y}\n";
        }
        
        Debug.Log($"Insert positions: {x}");
        
        engine.Init(poses.ToArray());
        engine.Run();
        
        WriteDCELToFile(engine.dcel);

        Voronoi voronoi = Voronoi.FromDCEL(engine.dcel);
        WriteVoronoiToFile(voronoi);
    }

    private void WriteVoronoiToFile(Voronoi voronoi)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var e in voronoi.edgeList)
        {
            sb.Append($"{e.start.x},{e.start.y}\n{e.end.x},{e.end.y}\n\n");
        }
        
        foreach (var f in voronoi.faceList)
        {
            // sb.Append($"{f.center.x},{f.center.y}\n\n");
        }
        
        File.WriteAllText("Voronoi.csv", sb.ToString());
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

        Debug.Log($"position to edge orientation {e1.GetPositionOrientation(new Vector2(1, 0.5f))}");
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

        Debug.Log($"face contains ? {f.ContainsPosition(new Vector2(1, 2f))}");
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
