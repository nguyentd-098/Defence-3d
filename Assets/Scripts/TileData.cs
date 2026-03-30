public enum TileType
{
    Road,
    Grass,
    Water
}

public class TileData
{
    public TileType type;
    public bool hasTree;
    public bool hasRock;

    public int height;
    public TileData(TileType type)
    {
        this.type = type;
        hasTree = false;
        hasRock = false;
    }
}