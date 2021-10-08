public class MCSetting
{
    // Note that the maximum vertex number of a single mesh in Unity is 2^16
    // Then we can figure out that chunk size is floor(sqrt(2^16/(8 * 256))) = 5
    public static int MC_WORLD_HEIGHT = 256;
    public static int CHUNK_SIZE = 5;
    public static int MAP_RADIUS = 35; // We will render (MAP_RADIUS + 1)^2 chunks per frame
}