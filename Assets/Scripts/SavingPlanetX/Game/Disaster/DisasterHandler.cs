using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterHandler
{
    private const float INTENSITY_STANDARD_DEV = 0.5f;

    private const float BASE_EARTHQUAKE_CHANCE = 1f;
    private const float BASE_EARTHQUAKE_CHANCE_RISE = 0.01f;
    private const float EARTHQUAKE_CHANCE_RISE_PER_INSTABILITY = 0.01f;

    private GameModel Model;

    private float EarthquakeChance;

    public DisasterHandler(GameModel model)
    {
        Model = model;

        EarthquakeChance = BASE_EARTHQUAKE_CHANCE;
    }

    // Prepares and returns the disasters that will occur in the upcoming cycle
    public List<Disaster> GetDisastersForCycle()
    {
        List<Disaster> cycleDisasters = new List<Disaster>();

        // Earthquake
        EarthquakeChance += BASE_EARTHQUAKE_CHANCE_RISE + Model.StarInstabilityLevel * EARTHQUAKE_CHANCE_RISE_PER_INSTABILITY;
        if (Random.value < EarthquakeChance) cycleDisasters.Add(GetRandomEarthquake());

        return cycleDisasters;
    }

    private Earthquake GetRandomEarthquake()
    {
        EarthquakeChance = BASE_EARTHQUAKE_CHANCE;
        List<Tile> epicenterCandidates = Model.Map.LandTiles;
        Tile epicenter = epicenterCandidates[Random.Range(0, epicenterCandidates.Count)];
        int intensity = GetDisasterIntensity();
        return new Earthquake(Model, epicenter, intensity);
    }

    private int GetDisasterIntensity()
    {
        float baseIntensity = (Model.StarInstabilityLevel / Model.Settings.MaxInstability * 4) + 1;
        return Mathf.Clamp(Mathf.RoundToInt(NextNormalRandom(baseIntensity, INTENSITY_STANDARD_DEV)), 1, 5);
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
}
