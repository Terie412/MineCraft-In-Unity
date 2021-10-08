using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MCChunkData
{
    public GameObject gameObject;
    public MCPosition position; // Only the x and z is useful
    public List<MCBlockData> blocks;

    public MCChunkData()
    {
        blocks = new List<MCBlockData>();
    }
    
    public void Render(GameObject prefab, Transform parent, Material material = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        GameObject go = GameObject.Instantiate(prefab, parent);
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();
        MeshCollider meshCollider = go.GetComponent<MeshCollider>();
        go.transform.position = new Vector3(position.x, position.y, position.z);
        
        gameObject = go;
        
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        foreach (var block in blocks)
        {
            // Check the block can be rendered
            MCAdjacency adjacency = block.adjacency;
            if(adjacency.up && adjacency.down && adjacency.left && adjacency.right && adjacency.front && adjacency.back)
                continue;
            
            int vertexCount = vertices.Count;
            MCPosition pos = block.position;
            // Generate Vertices, 6 * 4 = 24 vertices for each block
            // up
            vertices.Add(new Vector3(pos.x, pos.y + 1, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y + 1, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y + 1, pos.z + 1));
            vertices.Add(new Vector3(pos.x, pos.y + 1, pos.z + 1));
            uvs.Add(new Vector2(0,0));
            uvs.Add(new Vector2(0,1));
            uvs.Add(new Vector2(1,1));
            uvs.Add(new Vector2(1,0));
            
            // down
            vertices.Add(new Vector3(pos.x, pos.y, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y, pos.z + 1));
            vertices.Add(new Vector3(pos.x, pos.y, pos.z + 1));
            uvs.Add(new Vector2(0,0));
            uvs.Add(new Vector2(0,1));
            uvs.Add(new Vector2(1,1));
            uvs.Add(new Vector2(1,0));

            // left
            vertices.Add(new Vector3(pos.x, pos.y, pos.z + 1));
            vertices.Add(new Vector3(pos.x, pos.y, pos.z));
            vertices.Add(new Vector3(pos.x, pos.y + 1, pos.z));
            vertices.Add(new Vector3(pos.x, pos.y + 1, pos.z + 1));
            uvs.Add(new Vector2(0,0));
            uvs.Add(new Vector2(0,1));
            uvs.Add(new Vector2(1,1));
            uvs.Add(new Vector2(1,0));
            
            // right
            vertices.Add(new Vector3(pos.x + 1, pos.y, pos.z + 1));
            vertices.Add(new Vector3(pos.x + 1, pos.y, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y + 1, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y + 1, pos.z + 1));
            uvs.Add(new Vector2(0,0));
            uvs.Add(new Vector2(0,1));
            uvs.Add(new Vector2(1,1));
            uvs.Add(new Vector2(1,0));
            
            // front
            vertices.Add(new Vector3(pos.x, pos.y, pos.z + 1));
            vertices.Add(new Vector3(pos.x + 1, pos.y, pos.z + 1));
            vertices.Add(new Vector3(pos.x + 1, pos.y + 1, pos.z + 1));
            vertices.Add(new Vector3(pos.x, pos.y + 1, pos.z + 1));
            uvs.Add(new Vector2(0,0));
            uvs.Add(new Vector2(0,1));
            uvs.Add(new Vector2(1,1));
            uvs.Add(new Vector2(1,0));
            
            // back
            vertices.Add(new Vector3(pos.x, pos.y, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y, pos.z));
            vertices.Add(new Vector3(pos.x + 1, pos.y + 1, pos.z));
            vertices.Add(new Vector3(pos.x, pos.y + 1, pos.z));
            uvs.Add(new Vector2(0,0));
            uvs.Add(new Vector2(0,1));
            uvs.Add(new Vector2(1,1));
            uvs.Add(new Vector2(1,0));
            
            // Check which face to be rendered
            if (!adjacency.up)
            {
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + 3);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + 3);
                triangles.Add(vertexCount + 2);
            }
            
            if (!adjacency.down)
            {
                triangles.Add(vertexCount + 7);
                triangles.Add(vertexCount + 4);
                triangles.Add(vertexCount + 6);
                triangles.Add(vertexCount + 6);
                triangles.Add(vertexCount + 4);
                triangles.Add(vertexCount + 5);
            }
            
            if (!adjacency.front)
            {
                triangles.Add(vertexCount + 17);
                triangles.Add(vertexCount + 18);
                triangles.Add(vertexCount + 16);
                triangles.Add(vertexCount + 16);
                triangles.Add(vertexCount + 18);
                triangles.Add(vertexCount + 19);
            }
            
            if (!adjacency.back)
            {
                triangles.Add(vertexCount + 20);
                triangles.Add(vertexCount + 23);
                triangles.Add(vertexCount + 21);
                triangles.Add(vertexCount + 21);
                triangles.Add(vertexCount + 23);
                triangles.Add(vertexCount + 22);
            }
            
            if (!adjacency.right)
            {
                triangles.Add(vertexCount + 13);
                triangles.Add(vertexCount + 14);
                triangles.Add(vertexCount + 12);
                triangles.Add(vertexCount + 12);
                triangles.Add(vertexCount + 14);
                triangles.Add(vertexCount + 15);
            }
            
            if (!adjacency.left)
            {
                triangles.Add(vertexCount + 8);
                triangles.Add(vertexCount + 11);
                triangles.Add(vertexCount + 9);
                triangles.Add(vertexCount + 9);
                triangles.Add(vertexCount + 11);
                triangles.Add(vertexCount + 10);
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.name = $"Chunk_{position.x}_{position.z}";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        
        // Debug.Log($"mesh is readable ? {mesh.isReadable}");
        
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        
        sw.Stop();
        MCPerfRecorder.chunkCount++;
        long costTime = sw.ElapsedMilliseconds;
        MCPerfRecorder.allChunkRenderTime += sw.ElapsedMilliseconds;
        MCPerfRecorder.chunkRenderTimeThisFrame += sw.ElapsedMilliseconds;
        MCPerfRecorder.minChunkRenderTime = MCPerfRecorder.minChunkRenderTime < costTime ? MCPerfRecorder.minChunkRenderTime : costTime;
        MCPerfRecorder.maxChunkRenderTime = MCPerfRecorder.maxChunkRenderTime > costTime ? MCPerfRecorder.maxChunkRenderTime : costTime;
        // go.isStatic = true;
    }
    
    public static void Dispose(MCChunkData chunk)
    {
        Object.Destroy(chunk.gameObject);
        chunk.blocks.Clear();
    }
}