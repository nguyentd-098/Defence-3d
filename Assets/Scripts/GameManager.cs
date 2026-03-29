using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int width = 12;
    public int height = 12;

    public MapRenderer mapRenderer;

    void Start()
    {
        MapGenerator gen = new MapGenerator();
        MapData map = gen.Generate(width, height);

        mapRenderer.Render(map);
    }
}