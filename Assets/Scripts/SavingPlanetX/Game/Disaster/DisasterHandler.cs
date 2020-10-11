using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisasterHandler
{
    private GameModel Model;

    private float EarthquakeChance;
    private float HurricaneChance;

    private List<Disaster> ActiveDisasters = new List<Disaster>();

    public DisasterHandler(GameModel model)
    {
        Model = model;

        EarthquakeChance = GameSettings.Settings.Earthquake_BaseChance;
        HurricaneChance = GameSettings.Settings.Hurricane_BaseChance;
    }

    public void Update()
    {
        foreach (Disaster d in ActiveDisasters) d.Update();
    }

    // Prepares and returns the disasters that will occur in the upcoming cycle
    public List<Disaster> GetDisastersForCycle()
    {
        ActiveDisasters = Model.CycleDisasters.Where(x => x.State == DisasterState.Active).ToList(); // Take active disasters from last cycle

        // Earthquake
        EarthquakeChance += GameSettings.Settings.Earthquake_BaseRise + Model.InstabilityLevel * GameSettings.Settings.Earthquake_RisePerInstability;
        if (Random.value < EarthquakeChance) ActiveDisasters.Add(GetRandomEarthquake());

        // Hurricane
        HurricaneChance += GameSettings.Settings.Hurricane_BaseRise + Model.InstabilityLevel * GameSettings.Settings.Hurricane_RisePerInstability;
        if (Random.value < HurricaneChance) ActiveDisasters.Add(GetRandomHurricane());

        return ActiveDisasters;
    }

    #region Earthquake
    private Earthquake GetRandomEarthquake()
    {
        EarthquakeChance = GameSettings.Settings.Earthquake_BaseChance;
        List<Tile> epicenterCandidates = GetEarthquakeCandidateTiles();
        Tile epicenter = epicenterCandidates[Random.Range(0, epicenterCandidates.Count)];
        int intensity = GetDisasterIntensity();
        return new Earthquake(Model, epicenter, intensity);
    }

    private List<Tile> GetEarthquakeCandidateTiles()
    {
        return Model.Map.LandTiles;
    }

    #endregion

    #region Hurricane
    private Hurricane GetRandomHurricane()
    {
        HurricaneChance = GameSettings.Settings.Hurricane_BaseChance;
        List<Tile> centerCandidates = GetHurricaneCandidateTiles();
        Tile center = centerCandidates[Random.Range(0, centerCandidates.Count)];
        int intensity = GetDisasterIntensity();
        return new Hurricane(Model, center, intensity);
    }

    private List<Tile> GetHurricaneCandidateTiles()
    {
        return Model.Map.TilesList.Where(x => x.Type == TileType.Water && x.Temperature >= 25).ToList();
    }


    #endregion

    #region Helper
    private int GetDisasterIntensity()
    {
        float baseIntensity = (Model.InstabilityLevel / Model.GameSettings.MaxInstability * 4) + 1;
        return Mathf.Clamp(Mathf.RoundToInt(NextNormalRandom(baseIntensity, GameSettings.Settings.Disaster_StandardDeviation)), 1, 5);
    }

    public static float NextNormalRandom(float mean, float standardDeviation)
    {
        float u, v, S;

        do
        {
            u = 2f * Random.value - 1f;
            v = 2f * Random.value - 1f;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        float fac = Mathf.Sqrt(-2f * Mathf.Log(S) / S);
        float normalizedVal = u * fac; // mean 0, standardDev 1
        return mean + standardDeviation * normalizedVal;
    }

    #endregion
}
