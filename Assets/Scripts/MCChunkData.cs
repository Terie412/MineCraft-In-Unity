using System.Collections.Generic;
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
        GameObject go = GameObject.Instantiate(prefab, parent);
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();
        MeshCollider meshCollider = go.GetComponent<MeshCollider>();

        gameObject = go;
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        foreach (var block in blocks)
        {
            // Check the block can be rendered
            MCAdjacency adjacency = block.adjacency;
            if(adjacency.up && adjacency.down && adjacency.left && adjacency.right && adjacency.front && adjacency.back)
                continue;
            
            int vertexCount = vertices.Count;
            // Generate Vertices
            // todo: For each vertex, we don't need to generate it if none of its 12 adjacent faces is rendered;
            vertices.Add(new Vector3(block.position.x, block.position.y, block.position.z));
            vertices.Add(new Vector3(block.position.x + 1, block.position.y, block.position.z));
            vertices.Add(new Vector3(block.position.x + 1, block.position.y, block.position.z + 1));
            vertices.Add(new Vector3(block.position.x, block.position.y, block.position.z + 1));
            vertices.Add(new Vector3(block.position.x, block.position.y + 1, block.position.z));
            vertices.Add(new Vector3(block.position.x + 1, block.position.y + 1, block.position.z));
            vertices.Add(new Vector3(block.position.x + 1, block.position.y + 1, block.position.z + 1));
            vertices.Add(new Vector3(block.position.x, block.position.y + 1, block.position.z + 1));
            
            // Check which face to be rendered
            if (!adjacency.up)
            {
                triangles.Add(vertexCount + 4);
                triangles.Add(vertexCount + 7);
                triangles.Add(vertexCount + 5);
                triangles.Add(vertexCount + 5);
                triangles.Add(vertexCount + 7);
                triangles.Add(vertexCount + 6);
            }
            
            if (!adjacency.down)
            {
                triangles.Add(vertexCount + 3);
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + 2);
                triangles.Add(vertexCount + 2);
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + 1);
            }
            
            if (!adjacency.front)
            {
                triangles.Add(vertexCount + 2);
                triangles.Add(vertexCount + 6);
                triangles.Add(vertexCount + 3);
                triangles.Add(vertexCount + 3);
                triangles.Add(vertexCount + 6);
                triangles.Add(vertexCount + 7);
            }
            
            if (!adjacency.back)
            {
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + 4);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + 4);
                triangles.Add(vertexCount + 5);
            }
            
            if (!adjacency.right)
            {
                triangles.Add(vertexCount + 1);
                triangles.Add(vertexCount + 5);
                triangles.Add(vertexCount + 2);
                triangles.Add(vertexCount + 2);
                triangles.Add(vertexCount + 5);
                triangles.Add(vertexCount + 6);
            }
            
            if (!adjacency.left)
            {
                triangles.Add(vertexCount + 3);
                triangles.Add(vertexCount + 7);
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + 0);
                triangles.Add(vertexCount + 7);
                triangles.Add(vertexCount + 4);
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.name = $"Chunk_{position.x}_{position.z}";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        
        // Debug.Log($"mesh is readable ? {mesh.isReadable}");
        
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;

        go.isStatic = true;
    }
    
    public static void Dispose(MCChunkData chunk)
    {
        Object.Destroy(chunk.gameObject);
        chunk.blocks.Clear();
    }
}