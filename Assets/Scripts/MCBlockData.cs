using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MCBlockData
{
    public MCChunkData chunk;
    public MCPosition position; // The position of a block is defined by one of its vertex

    public MCAdjacency _adjacency;
    public MCAdjacency adjacency // Tell if there is an adjacency in the certain direction
    {
        get
        {
            if (_adjacency == null)
            {
                _adjacency = new MCAdjacency();

                MCPosition originalPos = position + chunk.position;
                
                MCPosition pos = originalPos;
                pos.y += 1;
                if (pos.y >= MCSetting.MC_WORLD_HEIGHT || pos.y <= 0 || !MCBlockMap.instance.ContainsKey(pos.x, pos.y, pos.z))
                    adjacency.up = false;
                else
                    adjacency.up = true;
            
                pos = originalPos;
                pos.y -= 1;
                if (pos.y >= MCSetting.MC_WORLD_HEIGHT || pos.y <= 0 || !MCBlockMap.instance.ContainsKey(pos.x, pos.y, pos.z))
                    adjacency.down = false;
                else
                    adjacency.down = true;
            
                pos = originalPos;
                pos.x += 1;
                if (!MCBlockMap.instance.ContainsKey(pos.x, pos.y, pos.z))
                    adjacency.right = false;
                else
                    adjacency.right = true;
            
                pos = originalPos;
                pos.x -= 1;
                if (!MCBlockMap.instance.ContainsKey(pos.x, pos.y, pos.z))
                    adjacency.left = false;
                else
                    adjacency.left = true;
            
                pos = originalPos;
                pos.z += 1;
                if (!MCBlockMap.instance.ContainsKey(pos.x, pos.y, pos.z))
                    adjacency.front = false;
                else
                    adjacency.front = true;
            
                pos = originalPos;
                pos.z -= 1;
                if (!MCBlockMap.instance.ContainsKey(pos.x, pos.y, pos.z))
                    adjacency.back = false;
                else
                    adjacency.back = true;
            }

            return _adjacency;
        }
    }
}