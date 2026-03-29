public class MapData
{
    public int width;
    public int height;
    public TileData[,] tiles;

    public MapData(int w, int h)
    {
        width = w;
        height = h;
        tiles = new TileData[w, h];
    }
}