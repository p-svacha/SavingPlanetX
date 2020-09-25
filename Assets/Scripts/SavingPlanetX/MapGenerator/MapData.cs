using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public int MapWidthTiles;
    public int MapHeightTiles;

    public float MapWidthAbsolute;
    public float MapHeightAbsolute;

    public int NumTiles;
    public float ScaleFactor;

    public TileData[,] Tiles;
    public List<TileData> TilesList;

    public MapData(int width, int height)
    {
        MapWidthTiles = width;
        MapHeightTiles = height;
        Tiles = new TileData[width, height];

        NumTiles = width * height;
        ScaleFactor = (float)(Mathf.Sqrt(1500) / Mathf.Sqrt(MapWidthTiles * MapHeightTiles));

        MapWidthAbsolute = (width - 1) * Mathf.Sqrt(3);
        MapHeightAbsolute = (height - 1) * 1.5f;
    }

    public void CreateTilesList()
    {
        TilesList = new List<TileData>();
        foreach (TileData td in Tiles) TilesList.Add(td);
    }
}
