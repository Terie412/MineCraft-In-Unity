using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameMgr : MonoBehaviour
{
    public Vector2 leftBottom, rightTop, heightRange;
    public GameObject chunkPrefab;
    public Transform terrain;
    public List<Material> availableMeterial;
    private static readonly FastNoise noise = new FastNoise();
    private static Random random;
    private const int chunkSize = 16;

    private List<GameObject> aliveChunks = new List<GameObject>();
    // public Transform testCube;
    
    private void Start()
    {
        Application.targetFrameRate = 30;
        noise.SetSeed((int) DateTime.Now.Ticks);
        random = new Random((int) DateTime.Now.Ticks);
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        int s_x = Mathf.FloorToInt(leftBottom.x);
        int e_x = Mathf.FloorToInt(rightTop.x);
        int s_y = Mathf.FloorToInt(leftBottom.y);
        int e_y = Mathf.FloorToInt(rightTop.y);

        for (int i = s_x; i < e_x; i+= chunkSize)
        {
            for (int j = s_y; j < e_y; j += chunkSize)
            {
                GenerateChunkInPos(i, j);
            }
        }
        
        foreach (var aliveChunk in aliveChunks)
        {
            aliveChunk.isStatic = true;
        }
    }

    private void GenerateChunkInPos(int x, int y)
    {
        GameObject chunk = Instantiate(chunkPrefab, terrain);
        MeshRenderer meshRenderer = chunk.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();

        // generate vertices
        Vector3[] vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];
        for (int i = 0; i < chunkSize + 1; i++)
        {
            for (int j = 0; j < chunkSize + 1; j++)
            {
                float height = (heightRange.y - heightRange.x) * noise.GetNoise((x + i) * (125.0f/ (rightTop.x - leftBottom.x)), (y + j) * (125/(rightTop.y - leftBottom.y))) + heightRange.x;
                vertices[j * (chunkSize + 1) + i] = new Vector3(x + i, height, y + j);
            }
        }
        
        // generate triangles
        int[] triangles = new int[6 * chunkSize * chunkSize];
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                int nowIndex = 6 * (j * chunkSize + i);
                triangles[nowIndex] = j * (chunkSize + 1) + i;
                triangles[nowIndex + 1] = (j + 1) * (chunkSize + 1) + i;
                triangles[nowIndex + 2] = (j + 1) * (chunkSize + 1) + i + 1;
                triangles[nowIndex + 3] = (j + 1) * (chunkSize + 1) + i + 1;
                triangles[nowIndex + 4] = j * (chunkSize + 1) + i + 1;
                triangles[nowIndex + 5] = j * (chunkSize + 1) + i;
            }
        }
        
        // generate uv
        Vector2[] uvs = new Vector2[(chunkSize + 1) * (chunkSize + 1)];
        for (int i = 0; i < chunkSize + 1; i++)
        {
            for (int j = 0; j < chunkSize + 1; j++)
            {
                uvs[j * (chunkSize + 1) + i] = new Vector2((float) (i + 1) / chunkSize, (float) (j + 1) / chunkSize);
            }
        }

        Mesh mesh = new Mesh {name = $"Chunk_{x}_{y}", vertices = vertices, triangles = triangles, uv = uvs};
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.material = availableMeterial[random.Next(0, availableMeterial.Count-1)];
        
        aliveChunks.Add(chunk);
    }

    private void Update()
    {
        // float y = noise.GetNoise(testCube.position.x, testCube.position.z) * 100;
        // Debug.Log($"y = {y}");
        // testCube.position = new Vector3(testCube.position.x, y, testCube.position.z);
    }
}
