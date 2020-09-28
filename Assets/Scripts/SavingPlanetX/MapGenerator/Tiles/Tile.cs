using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Map Map;
    public GameObject FogOfWarObject;

    public int X;
    public int Y;

    public Color DefaultColor;
    public Color HoverColor;

    public TileType Type;
    public TileTopology Topology;
    public TileBiome Biome;

    public int WindDirection;
    public int Temperature;
    public int Precipitation;

    public Tile[] NeighbourTiles = new Tile[6];

    public bool IsInFogOfWar;

    public Building Building;

    public void Initialize(TileData data, Map map)
    {
        Map = map;
        X = data.X;
        Y = data.Y;
        Type = data.Type;
        Topology = data.Topology;
        Biome = data.Biome;

        WindDirection = data.WindDirection;
        Temperature = data.Temperature;
        Precipitation = data.Precipitation;

        // Visual
        transform.position = data.Position;
        FogOfWarObject.transform.position = new Vector3(data.Position.x, 0f, data.Position.z);
        DefaultColor = GetSimpleTileColor();
        HoverColor = new Color(DefaultColor.r - 0.3f, DefaultColor.g -0.3f, DefaultColor.b - 0.3f);
        SetColor(DefaultColor);
    }

    public void UpdateTile()
    {
        DrawTile();
    }

    public void SetNeighbours()
    {
        if (Y < Map.HeightTiles - 1 && (X > 0 || Y % 2 == 0)) NeighbourTiles[0] = Map.Tiles[Y % 2 == 0 ? X : X - 1, Y + 1]; // Northeast
        if (Y < Map.HeightTiles - 1 && (X < Map.WidthTiles - 1 || Y % 2 == 1)) NeighbourTiles[1] = Map.Tiles[Y % 2 == 0 ? X + 1 : X, Y + 1]; // Northwest
        if (X < Map.WidthTiles - 1) NeighbourTiles[2] = Map.Tiles[X + 1, Y]; // West
        if (Y > 0 && (X < Map.WidthTiles - 1 || Y % 2 == 1)) NeighbourTiles[3] = Map.Tiles[Y % 2 == 0 ? X + 1 : X, Y - 1]; // Southwest
        if (Y > 0 && (X > 0 || Y % 2 == 0)) NeighbourTiles[4] = Map.Tiles[Y % 2 == 0 ? X : X - 1, Y - 1]; // Northeast
        if (X > 0) NeighbourTiles[5] = Map.Tiles[X - 1, Y]; // East
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Color GetSimpleTileColor()
    {
        TilePrefabCollection TPC = GameObject.Find("TilePrefabCollection").GetComponent<TilePrefabCollection>();
        if (Type == TileType.Water)
        {
            if (Topology == TileTopology.Ocean) return TPC.OceanColor;
            else if (Topology == TileTopology.DeepOcean) return TPC.DeepOceanColor;
            else if (Topology == TileTopology.Lake) return TPC.LakeColor;
        }
        else
        {
            if (Biome == TileBiome.Desert) return TPC.DesertColor;
            else if (Biome == TileBiome.Grassland) return TPC.GrasslandColor;
            else if (Biome == TileBiome.Savanna) return TPC.SavannaColor;
            else if (Biome == TileBiome.Shrubland) return TPC.ShrublandColor;
            else if (Biome == TileBiome.Taiga) return TPC.TaigaColor;
            else if (Biome == TileBiome.Temperate) return TPC.TemperateColor;
            else if (Biome == TileBiome.TemperateRainForest) return TPC.TemperateRainforestColor;
            else if (Biome == TileBiome.Tropical) return TPC.TropicalColor;
            else if (Biome == TileBiome.TropicalRainForest) return TPC.TropicalRainforestColor;
            else if (Biome == TileBiome.Tundra) return TPC.TundraColor;
            else if (Biome == TileBiome.Ice) return TPC.IceColor;
        }
        return Color.black;
    }

    private void SetColor(Color c)
    {
        if (GetComponent<Renderer>() != null) GetComponent<Renderer>().material.color = c;
        else
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<Renderer>().material.color = c;
    }

    private void DrawTile()
    {
        gameObject.SetActive(!IsInFogOfWar);
        if (Building != null) Building.gameObject.SetActive(!IsInFogOfWar);
        FogOfWarObject.SetActive(IsInFogOfWar);
    }

    public void Hover()
    {
        SetColor(HoverColor);
    }

    public void Unhover()
    {
        SetColor(DefaultColor);
    }

    public Tile Tile_NorthEast() { return NeighbourTiles[0]; }
    public Tile Tile_NorthWest() { return NeighbourTiles[1]; }
    public Tile Tile_West() { return NeighbourTiles[2]; }
    public Tile Tile_SouthWest() { return NeighbourTiles[3]; }
    public Tile Tile_SouthEast() { return NeighbourTiles[4]; }
    public Tile Tile_East() { return NeighbourTiles[5]; }

    public List<Tile> TilesInRange(int range)
    {
        HashSet<Tile> tilesInRange = new HashSet<Tile>() { this };
        for(int i = 0; i < range; i++)
        {
            HashSet<Tile> tilesToAdd = new HashSet<Tile>();
            foreach(Tile t in tilesInRange)
                foreach (Tile n in t.NeighbourTiles.Where(x => x != null)) tilesToAdd.Add(n);
            foreach (Tile t in tilesToAdd) tilesInRange.Add(t);
        }
        return tilesInRange.ToList();
    }

    public bool IsInRangeOfBuilding(int range, Type type)
    {
        return TilesInRange(range).Any(x => x.Building != null && x.Building.GetType() == type);
    }
}
