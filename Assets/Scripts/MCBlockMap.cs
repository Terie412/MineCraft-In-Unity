using System.Collections.Generic;

public class MCBlockMap
{
    public static MCBlockMap instance = new MCBlockMap();

    public MCBlockMap()
    {
        blockMap = new Dictionary<int, Dictionary<int, Dictionary<int, MCBlockData>>>();
    }

    private Dictionary<int, Dictionary<int, Dictionary<int, MCBlockData>>> blockMap;

    public bool TryGetValue(int x, int y, int z, out MCBlockData block)
    {
        if (!blockMap.TryGetValue(x, out var dicty) || !dicty.TryGetValue(y, out var dictz) || !dictz.TryGetValue(z, out var blockTmp))
        {
            block = null;
            return false;
        }

        block = blockTmp;
        return true;
    }

    public bool ContainsKey(int x, int y, int z)
    {
        if (!blockMap.TryGetValue(x, out var dicty) || !dicty.TryGetValue(y, out var dictz) || !dictz.TryGetValue(z, out var blockTmp))
        {
            return false;
        }

        return true;
    }
    
    public MCBlockData this[int x, int y, int z]
    {
        get
        {
            var res = TryGetValue(x, y, z, out var block);
            return block;
        }
        set
        {
            if (!blockMap.TryGetValue(x, out var dicty))
            {
                dicty = new Dictionary<int, Dictionary<int, MCBlockData>>();
                var dictz = new Dictionary<int, MCBlockData>();
                dicty[y] = dictz;
                dictz[z] = value;
                blockMap[x] = dicty;
            }
            else if (!dicty.TryGetValue(y, out var dictz))
            {
                dictz = new Dictionary<int, MCBlockData>();
                dictz[z] = value;
                dicty[y] = dictz;
            }
            else
            {
                dictz[z] = value;
            }
        }
    }
}