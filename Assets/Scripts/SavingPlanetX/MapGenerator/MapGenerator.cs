using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MapGenerator
{
    private static MapData MapData;

    private static bool Visualize;
    private static NoiseTester NoiseTester;

    public static MapData GenerateMap(int width, int height, bool visualize)
    {
        MapData = new MapData(width, height);

        Visualize = visualize;
        if(Visualize) NoiseTester = GameObject.Find("MapNoiseTester").GetComponent<NoiseTester>();

        GenerateInitialTiles();
        GenerateContinents();
        IdentifyLakes();
        GenerateLandTopology();
        GenerateWaterTopology();
        GenerateWind();
        GenerateTemperature();
        GeneratePrecipitation();

        ApplyBiomes();
        DebugMapInformation();

        return MapData;
    }

    private static void GenerateInitialTiles()
    {
        for (int y = 0; y < MapData.MapHeightTiles; y++)
        {
            for (int x = 0; x < MapData.MapWidthTiles; x++)
            {
                TileData tileData = new TileData(x, y, MapData);
                MapData.Tiles[x, y] = tileData;
            }
        }
        MapData.CreateTilesList();
        foreach (TileData td in MapData.Tiles) td.SetNeighbours();
    }

    private static void ApplyBiomes()
    {
        foreach (TileData td in MapData.Tiles) td.SetBiome();
    }


    private static void GenerateContinents()
    {
        ContinentNoise noise = new ContinentNoise();

        foreach(TileData td in MapData.Tiles)
        {
            float noiseValue = noise.GetValue(td.Position.x, td.Position.z, MapData);
            if (noiseValue == 0) td.Type = TileType.Water;
            else td.Type = TileType.Land;
        }

        if (Visualize) NoiseTester.DisplayNoise(noise, NoiseTester.ContinentPlane, MapData);
    }

    private static void GenerateLandTopology()
    {
        LandTopologyNoise noise = new LandTopologyNoise();

        foreach (TileData td in MapData.Tiles)
        {
            float noiseValue = noise.GetValue(td.Position.x, td.Position.z, MapData);

            if (td.Type == TileType.Water) continue;
            else if (noiseValue == 0) td.Topology = TileTopology.Plains;
            else if (noiseValue == 1) td.Topology = TileTopology.Hills;
            else if (noiseValue == 2) td.Topology = TileTopology.Mountains;
            else if (noiseValue == 3)
            {
                if (td.NeighbourTiles.All(x => x != null && x.Type == TileType.Land))
                {
                    td.Type = TileType.Water;
                    td.Topology = TileTopology.Lake;
                }
                else td.Topology = TileTopology.Plains;
            }
        }

        if (Visualize) NoiseTester.DisplayNoise(noise, NoiseTester.LandTopologyPlane, MapData, 0, 3);
    }

    private static void GenerateWaterTopology()
    {
        WaterTopologyNoise noise = new WaterTopologyNoise();

        foreach (TileData td in MapData.Tiles)
        {
            float noiseValue = noise.GetValue(td.Position.x, td.Position.z, MapData);

            if (td.Type == TileType.Land || td.Topology == TileTopology.Lake) continue;
            else if (noiseValue == 1 && (td.NeighbourTiles.All(x => x == null || x.Type == TileType.Water)))
            {
                td.Topology = TileTopology.DeepOcean;
            }
            else td.Topology = TileTopology.Ocean;
        }

        if (Visualize) NoiseTester.DisplayNoise(noise, NoiseTester.WaterTopologyPlane, MapData, 0, 1);
    }


    private static void GenerateWind()
    {
        WindNoise noise = new WindNoise();

        foreach (TileData td in MapData.Tiles)
        {
            float noiseValue = noise.GetValue(td.Position.x, td.Position.z, MapData);
            td.WindDirection = (int)noiseValue;
        }

        if (Visualize) NoiseTester.DisplayNoise(noise, NoiseTester.WindPlane, MapData, 0, 360);
    }

    private static void GenerateTemperature()
    {
        float poleTemperature = -12f;
        float equatorTemperature = 35f;
        float temperatureModifyRange = 15f;
        TemperatureNoise noise = new TemperatureNoise(poleTemperature, equatorTemperature, temperatureModifyRange);

        foreach (TileData td in MapData.Tiles)
        {
            float noiseValue = noise.GetValue(td.Position.x, td.Position.z, MapData);
            td.Temperature = (int)noiseValue;
        }

        if (Visualize) NoiseTester.DisplayNoise(noise, NoiseTester.TemperaturePlane, MapData, poleTemperature - temperatureModifyRange, equatorTemperature + temperatureModifyRange);
    }

    private static void GeneratePrecipitation()
    {
        int polePrecipitation = 0;
        int equatorPrecipitation = 5000;
        PrecipitationNoise noise = new PrecipitationNoise(polePrecipitation, equatorPrecipitation);

        foreach (TileData td in MapData.Tiles)
        {
            float noiseValue = noise.GetValue(td.Position.x, td.Position.z, MapData);
            td.Precipitation = (int)noiseValue;
        }

        if (Visualize) NoiseTester.DisplayNoise(noise, NoiseTester.PrecipitationPlane, MapData, polePrecipitation, equatorPrecipitation);
    }

    private static void IdentifyLakes()
    {
        List<TileData> waterTiles = MapData.TilesList.Where(x => x.Type == TileType.Water).ToList();
        bool[,] checkedTiles = new bool[MapData.MapWidthTiles, MapData.MapHeightTiles];

        Queue<TileData> tilesToCheck = new Queue<TileData>();
        tilesToCheck.Enqueue(MapData.Tiles[0,0]);
        checkedTiles[0, 0] = true;
        while(tilesToCheck.Count > 0)
        {
            TileData td = tilesToCheck.Dequeue();
            waterTiles.Remove(td);
            foreach(TileData n in td.NeighbourTiles.Where(x => x != null && x.Type == TileType.Water && !checkedTiles[x.X, x.Y]))
            {
                checkedTiles[n.X, n.Y] = true;
                tilesToCheck.Enqueue(n);
            }
        }

        foreach (TileData td in waterTiles) td.Topology = TileTopology.Lake;
    }

    private static void DebugMapInformation()
    {
        // Types
        Dictionary<TileType, int> tilesByType = new Dictionary<TileType, int>();
        foreach (TileType type in Enum.GetValues(typeof(TileType)))
            tilesByType.Add(type, MapData.TilesList.Where(x => x.Type == type).Count());
        tilesByType = tilesByType.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        string typeString = "Tiles by Type:";
        foreach (KeyValuePair<TileType, int> kvp in tilesByType)
            typeString += "\n" + kvp.Key + ": " + kvp.Value;
        Debug.Log(typeString);

        // Topology
        Dictionary<TileTopology, int> tilesByTopology = new Dictionary<TileTopology, int>();
        foreach (TileTopology topology in Enum.GetValues(typeof(TileTopology)))
            tilesByTopology.Add(topology, MapData.TilesList.Where(x => x.Topology == topology).Count());
        tilesByTopology = tilesByTopology.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        string topologyString = "Tiles by Topology:";
        foreach (KeyValuePair<TileTopology, int> kvp in tilesByTopology)
            topologyString += "\n" + kvp.Key + ": " + kvp.Value;
        Debug.Log(topologyString);

        // Biomes
        Dictionary<TileBiome, int> tilesByBiome = new Dictionary<TileBiome, int>();
        foreach (TileBiome biome in Enum.GetValues(typeof(TileBiome)))
            tilesByBiome.Add(biome, MapData.TilesList.Where(x => x.Type == TileType.Land && x.Biome == biome).Count());
        tilesByBiome = tilesByBiome.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        string biomeString = "Tiles by Biome:";
        foreach(KeyValuePair<TileBiome, int> kvp in tilesByBiome)
            biomeString += "\n" + kvp.Key + ": " + kvp.Value;
        Debug.Log(biomeString);
    }

}
