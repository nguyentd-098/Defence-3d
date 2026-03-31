public enum TileType
{
    Road,
    Grass,
    Water,
    Mountain
}

public enum BiomeType
{
    Plain,
    Forest,
    Rocky,
    Wetland,
    Highland
}

public class TileData
{
    public TileType type;
    public BiomeType biome;
    public bool hasTree;
    public bool hasRock;
    public int height;
    public bool isRiverTile;   // dùng cho river system sau

    public TileData(TileType type, BiomeType biome = BiomeType.Plain)
    {
        this.type = type;
        this.biome = biome;
        hasTree = false;
        hasRock = false;
        isRiverTile = false;
        height = 0;
    }
}