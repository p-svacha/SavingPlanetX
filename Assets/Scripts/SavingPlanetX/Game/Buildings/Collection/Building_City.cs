using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building_City : Building
{
    private const int MIN_DISTANCE_TO_OTHER_CITIES = 6;

    public Light CityLight;

    public string CityName;
    public int Relation;

    public override void InitAttributes()
    {
        BuildingName = "City";
        BuildingDescription = "A permanent settlement of a big group of planet inhabitants.";
        MaxHealth = GameSettings.City_MaxHealth;
        EmissionsPerCycle = GameSettings.City_Emissions;
        RepairCost = GameSettings.City_RepairCost;
    }

    public override bool CanBuildOn(Tile t)
    {
        return t.Type == TileType.Land && t.Topology != TileTopology.Mountains && !t.IsInRangeOfBuilding(MIN_DISTANCE_TO_OTHER_CITIES, typeof(Building_City));
    }
    public override void OnBuild()
    {
        Relation = 3;
        CityName = MarkovChainWordGenerator.GenerateWord("Province", 4);
    }
}
