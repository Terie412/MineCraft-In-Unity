using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class MCManager : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Transform terrain;
    public Transform player;
    public Material material;

    private static readonly FastNoise noise = new FastNoise();
    private static Random random;


    private List<MCChunkData> aliveChunks = new List<MCChunkData>();
    private Dictionary<MCPosition, MCChunkData> chunkMap = new Dictionary<MCPosition, MCChunkData>();

    private void Start()
    {
        Application.targetFrameRate = 30;
        noise.SetSeed((int) DateTime.Now.Ticks);
        random = new Random((int) DateTime.Now.Ticks);

        // RenderChunkInPosition(Convert.ToInt32(player.position.x), Convert.ToInt32(player.position.z));
    }

    private void Update()
    {
        MCPosition nowPos = new MCPosition {x = Convert.ToInt32(player.position.x), y = 0, z = Convert.ToInt32(player.position.z)};
        nowPos.x = Mathf.FloorToInt((float) nowPos.x / MCSetting.CHUNK_SIZE) * MCSetting.CHUNK_SIZE;
        nowPos.z = Mathf.FloorToInt((float) nowPos.z / MCSetting.CHUNK_SIZE) * MCSetting.CHUNK_SIZE;

        List<MCChunkData> chunkToBeRender = new List<MCChunkData>();

        // Check if we have all chunkData
        // We need to precompute one more circle chunk data for check a block's adjacency when rendering
        for (int i = -MCSetting.MAP_RADIUS - 1; i <= MCSetting.MAP_RADIUS + 1; i++)
        {
            for (int j = -MCSetting.MAP_RADIUS - 1; j <= MCSetting.MAP_RADIUS + 1; j++)
            {
                MCPosition chunkPos = new MCPosition {x = nowPos.x + i * MCSetting.CHUNK_SIZE, y = 0, z = nowPos.z + j * MCSetting.CHUNK_SIZE};
                if (!chunkMap.TryGetValue(chunkPos, out MCChunkData chunk))
                {
                    // Debug.Log($"check out {i} {j}");
                    chunk = PreComputeChunkDataInPosition(chunkPos);
                    aliveChunks.Add(chunk);
                    chunkMap[chunk.position] = chunk;

                    if (i > -MCSetting.MAP_RADIUS - 1 && i < MCSetting.MAP_RADIUS + 1 && j > -MCSetting.MAP_RADIUS - 1 && j < MCSetting.MAP_RADIUS + 1)
                    {
                        chunkToBeRender.Add(chunk);
                    }
                }
            }
        }

        // if(chunkToBeRender.Count > 0)
        // Debug.Log($"{chunkToBeRender.Count} to be Rendered");

        // Stopwatch sw = new Stopwatch();
        // sw.Start();
        // foreach (var chunk in chunkToBeRender)
        // {
        // chunk.Render(chunkPrefab, terrain);
        // }
        // sw.Stop();
        // if(chunkToBeRender.Count>0)
        // Debug.Log($"Render {chunkToBeRender.Count} blocks cost time: {sw.Elapsed}");

        if (chunkToBeRender.Count > 0)
            StartCoroutine(StartRenderChunks(chunkToBeRender));
    }

    private IEnumerator StartRenderChunks(List<MCChunkData> chunks)
    {
        foreach (var chunk in chunks)
        {
            // Stopwatch sw = new Stopwatch();
            // sw.Start();
            chunk.Render(chunkPrefab, terrain, material);
            // sw.Stop();
            // Debug.Log($"Render {chunk.blocks.Count} blocks cost time: {sw.Elapsed}");
            yield return null;
        }

        // foreach (var chunk in chunks)
        // {
        //     StaticBatchingUtility.Combine(chunk.gameObject);
        //     yield return null;
        // }
    }


    private MCChunkData PreComputeChunkDataInPosition(MCPosition position)
    {
        MCChunkData mcChunkData = new MCChunkData();
        mcChunkData.position = new MCPosition {x = position.x, y = 0, z = position.z};
        mcChunkData.blocks = new List<MCBlockData>();
        for (int i = 0; i < MCSetting.CHUNK_SIZE; i++)
        {
            for (int j = 0; j < MCSetting.CHUNK_SIZE; j++)
            {
                int height = Convert.ToInt32((MCSetting.MC_WORLD_HEIGHT / 4f) * noise.GetSimplex(i + position.x, j + position.z));
                for (int k = 0; k < height; k++)
                {
                    MCBlockData block = new MCBlockData();
                    block.position = new MCPosition {x = i, y = k, z = j};
                    mcChunkData.blocks.Add(block);
                    MCBlockData.blockMap[block.hashCode] = block;
                }
            }
        }

        return mcChunkData;
    }
}