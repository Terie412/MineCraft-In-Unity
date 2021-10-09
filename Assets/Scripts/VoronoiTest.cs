using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiEngine;
using Random = System.Random;

public class VoronoiTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Random rand = new Random();
        
        List<Point> points = new List<Point>();
        points.Add(new Point(rand.Next(0, 100), rand.Next(0, 100)));
        points.Add(new Point(rand.Next(0, 100), rand.Next(0, 100)));
        points.Add(new Point(rand.Next(0, 100), rand.Next(0, 100)));
        // points.Add(new Point(0, 0));
        // points.Add(new Point(0, 200));
        // points.Add(new Point(100, 0));
        Triangle tri = new Triangle(points.ToArray());

        Debug.Log($"tri = {tri}");

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(tri.points[0].x, tri.points[0].y, 0));
        vertices.Add(new Vector3(tri.points[1].x, tri.points[1].y, 0));
        vertices.Add(new Vector3(tri.points[2].x, tri.points[2].y, 0));
        vertices.Add(new Vector3(tri.circumcircle.center.x, tri.circumcircle.center.y, 0));
            
        foreach (var vector3 in vertices)
        {
            Debug.Log($"{vector3}");
        }
        
        List<int> triangles = new List<int>();
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(3);
        triangles.Add(1);
        triangles.Add(2);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
