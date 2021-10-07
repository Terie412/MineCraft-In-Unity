using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MCBlockData
{
    public static Dictionary<int, MCBlockData> blockMap = new Dictionary<int, MCBlockData>();
    public MCChunkData chunk;
    public MCPosition position; // The position of a block is defined by one of its vertex

    // GetHashCode for a struct can greatly slow down the efficiency of dictionary
    // We assume that there are no more than 2^31 blocks in the scene
    private static readonly int HASH_CODE_MAX = Int32.MaxValue; 
    private int _hashCode = 0;
    public int hashCode
    {
        get
        {
            if (_hashCode == 0)
            {
                Random random = new Random();
                while (true)
                {
                    int next = random.Next(1, HASH_CODE_MAX);
                    if (!blockMap.ContainsKey(next))
                    {
                        _hashCode = next;
                        break;
                    }
                }
            }

            return _hashCode;
        }
    }
    
    public MCAdjacency _adjacency;
    public MCAdjacency adjacency // Tell if there is an adjacency in the certain direction
    {
        get
        {
            if (_adjacency == null)
            {
                _adjacency = new MCAdjacency();
            
                MCPosition pos = position;
                pos.y += 1;
                if (pos.y >= MCSetting.MC_WORLD_HEIGHT || pos.y <= 0 || !blockMap.ContainsKey(hashCode))
                    adjacency.up = false;
                else
                    adjacency.up = true;
            
                pos = position;
                pos.y -= 1;
                if (pos.y >= MCSetting.MC_WORLD_HEIGHT || pos.y <= 0 || !blockMap.ContainsKey(hashCode))
                    adjacency.down = false;
                else
                    adjacency.down = true;
            
                pos = position;
                pos.x += 1;
                if (!blockMap.ContainsKey(hashCode))
                    adjacency.right = false;
                else
                    adjacency.right = true;
            
                pos = position;
                pos.x -= 1;
                if (!blockMap.ContainsKey(hashCode))
                    adjacency.left = false;
                else
                    adjacency.left = true;
            
                pos = position;
                pos.z += 1;
                if (!blockMap.ContainsKey(hashCode))
                    adjacency.front = false;
                else
                    adjacency.front = true;
            
                pos = position;
                pos.z -= 1;
                if (!blockMap.ContainsKey(hashCode))
                    adjacency.back = false;
                else
                    adjacency.back = true;
            }

            return _adjacency;
        }
    }

    // x, y, z is world space position of a block
    public static int GetBlockPositionHashCode(int x, int y, int z)
    {
        // The fact is the efficiency of Dictionary.TryGetValue() and Dictionary.ContainsKey() is almost the same
        // The key is that program will greatly slow down when Dictionary try to compute a hash of a struct 
        return x * MCSetting.MC_WORLD_HEIGHT * MCSetting.MC_WORLD_HEIGHT
               + y * MCSetting.MC_WORLD_HEIGHT
               + z;
    }
        
}