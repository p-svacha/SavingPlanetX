using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public MapData MapData;

    public const float TILE_SIZE = 1f;
    public readonly int X, Y;
    public readonly Vector3 Position;

    public TileType Type;
    public TileTopology Topology;
    public TileBiome Biome;

    public int WindDirection;
    public int Temperature;
    public int Precipitation;

    public TileData[] NeighbourTiles = new TileData[6];

    public TileData(int x, int y, MapData mapData)
    {
        MapData = mapData;

        float tileWidth = TILE_SIZE * Mathf.Sqrt(3);
        float tileHeight = TILE_SIZE;

        X = x;
        Y = y;
        Position = new Vector3(y % 2 == 1 ? x * tileWidth : x * tileWidth + tileWidth / 2f, 0, y * tileHeight * 1.5f);
    }

    public Tile DrawTile(Map map)
    {
        TilePrefabCollection TPC = GameObject.Find("TilePrefabCollection").GetComponent<TilePrefabCollection>();
        Tile tile = null;
        if(Topology == TileTopology.Ocean || Topology == TileTopology.Lake || Topology == TileTopology.DeepOcean) tile = GameObject.Instantiate(TPC.BasicTile_Water, map.transform);
        else if(Topology == TileTopology.Plains) tile = GameObject.Instantiate(TPC.BasicTile_Plains, map.transform);
        else if(Topology == TileTopology.Hills) tile = GameObject.Instantiate(TPC.BasicTile_Hills, map.transform);
        else if(Topology == TileTopology.Mountains) tile = GameObject.Instantiate(TPC.BasicTile_Mountains, map.transform);

        tile.FogOfWarObject = GameObject.Instantiate(TPC.FogOfWar, map.transform);

        tile.Initialize(this, map);
        return tile;
    }

    public void SetNeighbours()
    {
        if (Y < MapData.MapHeightTiles - 1 && (X > 0 || Y % 2 == 0)) NeighbourTiles[0] = MapData.Tiles[Y % 2 == 0 ? X : X - 1, Y + 1]; // Northeast
        if (Y < MapData.MapHeightTiles - 1 && (X < MapData.MapWidthTiles - 1 || Y % 2 == 1)) NeighbourTiles[1] = MapData.Tiles[Y % 2 == 0 ? X + 1 : X, Y + 1]; // Northwest
        if (X < MapData.MapWidthTiles - 1) NeighbourTiles[2] = MapData.Tiles[X + 1, Y]; // West
        if (Y > 0 && (X < MapData.MapWidthTiles - 1 || Y % 2 == 1)) NeighbourTiles[3] = MapData.Tiles[Y % 2 == 0 ? X + 1 : X, Y - 1]; // Southwest
        if (Y > 0 && (X > 0 || Y % 2 == 0)) NeighbourTiles[4] = MapData.Tiles[Y % 2 == 0 ? X : X - 1, Y - 1]; // Northeast
        if (X > 0) NeighbourTiles[5] = MapData.Tiles[X - 1, Y]; // East
    }

    public void SetBiome()
    {
        Biome = CalculateBiome();
    }

    private TileBiome CalculateBiome()
    {
        if (Temperature < -8) return TileBiome.Ice;
        else if (Temperature < -3) return TileBiome.Tundra;
        else if (Temperature < 1)
        {
            if (Precipitation < 200) return TileBiome.Grassland;
            else if (Precipitation < 300) return TileBiome.Shrubland;
            else return TileBiome.Tundra;
        }
        else if (Temperature < 8)
        {
            if (Precipitation < 300) return TileBiome.Grassland;
            else if (Precipitation < 400) return TileBiome.Shrubland;
            else return TileBiome.Taiga;
        }
        else if(Temperature < 14)
        {
            if (Precipitation < 500) return TileBiome.Grassland;
            else if (Precipitation < 800) return TileBiome.Shrubland;
            else if (Precipitation < 1700) return TileBiome.Temperate;
            else return TileBiome.TemperateRainForest;
        }
        else if (Temperature < 19)
        {
            if (Precipitation < 600) return TileBiome.Grassland;
            else if (Precipitation < 1100) return TileBiome.Shrubland;
            else if (Precipitation < 2000) return TileBiome.Temperate;
            else return TileBiome.TemperateRainForest;
        }
        else if(Temperature < 23)
        {
            if (Precipitation < 700) return TileBiome.Desert;
            else if (Precipitation < 1300) return TileBiome.Shrubland;
            else if (Precipitation < 2200) return TileBiome.Temperate;
            else return TileBiome.TemperateRainForest;
        }
        else
        {
            if (Precipitation < 800) return TileBiome.Desert;
            else if (Precipitation < 1400) return TileBiome.Savanna;
            else if (Precipitation < 2600) return TileBiome.Tropical;
            else return TileBiome.TropicalRainForest;
        }
    }
}
