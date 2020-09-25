using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private const int MAP_WIDTH = 60;
    private const int MAP_HEIGHT = 40;

    public int WidthTiles = MAP_WIDTH;
    public int HeightTiles = MAP_HEIGHT;

    public Tile[,] Tiles = new Tile[MAP_WIDTH, MAP_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        MapData data = MapGenerator.GenerateMap(MAP_WIDTH, MAP_HEIGHT);
        DrawTiles(data.Tiles);
    }



    public void DrawTiles(TileData[,] tileData)
    {
        foreach(TileData td in tileData)
            Tiles[td.X, td.Y] = td.DrawTile(this);
        for (int y = 0; y < MAP_HEIGHT; y++)
            for (int x = 0; x < MAP_WIDTH; x++)
                Tiles[x, y].SetNeighbours();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
