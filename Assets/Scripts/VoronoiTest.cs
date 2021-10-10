using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiEngine;

public class VoronoiTest : MonoBehaviour
{
    public Vector2 vec;
    // Start is called before the first frame update
    void Start()
    {
        Triangle tri = new Triangle(new[]
        {
            new Point(0, 0),
            new Point(0, 100),
            new Point(100, 0)
        });

        var res = tri.CircumCircleContainsPoint(new Point(vec.x, vec.y));
        Debug.Log($"res = {res}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
