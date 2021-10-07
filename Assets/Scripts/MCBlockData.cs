using System.Collections.Generic;

public class MCBlockData
{
    public static Dictionary<int, MCBlockData> blockMap = new Dictionary<int, MCBlockData>();
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
            
                MCPosition pos = position;
                pos.y += 1;
                if (pos.y >= MCSetting.MC_WORLD_HEIGHT || pos.y <= 0 || !blockMap.ContainsKey(GetPositionHash(pos)))
                    adjacency.up = false;
                else
                    adjacency.up = true;
            
                pos = position;
                pos.y -= 1;
                if (pos.y >= MCSetting.MC_WORLD_HEIGHT || pos.y <= 0 || !blockMap.ContainsKey(GetPositionHash(pos)))
                    adjacency.down = false;
                else
                    adjacency.down = true;
            
                pos = position;
                pos.x += 1;
                if (!blockMap.ContainsKey(GetPositionHash(pos)))
                    adjacency.right = false;
                else
                    adjacency.right = true;
            
                pos = position;
                pos.x -= 1;
                if (!blockMap.ContainsKey(GetPositionHash(pos)))
                    adjacency.left = false;
                else
                    adjacency.left = true;
            
                pos = position;
                pos.z += 1;
                if (!blockMap.ContainsKey(GetPositionHash(pos)))
                    adjacency.front = false;
                else
                    adjacency.front = true;
            
                pos = position;
                pos.z -= 1;
                if (!blockMap.ContainsKey(GetPositionHash(pos)))
                    adjacency.back = false;
                else
                    adjacency.back = true;
            }

            return _adjacency;
        }
    }

    public static int GetPositionHash(MCPosition pos)
    {
        // The fact is the efficiency of Dictionary.TryGetValue() and Dictionary.ContainsKey() is almost the same
        // The key is that program will greatly slow down when Dictionary try to compute a hash of a struct 
        return pos.x * MCSetting.MC_WORLD_HEIGHT * MCSetting.MC_WORLD_HEIGHT
               + pos.y * MCSetting.MC_WORLD_HEIGHT
               + pos.z;
    }
        
}