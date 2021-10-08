public struct MCPosition
{
    public int x, y, z; // All positions in Minecraft should be type integer

    public static MCPosition operator +(MCPosition a, MCPosition b)
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;
        return a;
    }
}

public class MCAdjacency
{
    public bool up;
    public bool down;
    public bool right;
    public bool left;
    public bool front;
    public bool back;
}