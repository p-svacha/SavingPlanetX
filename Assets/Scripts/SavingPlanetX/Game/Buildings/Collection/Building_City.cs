using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building_City : Building
{
    private const int MIN_DISTANCE_TO_OTHER_CITIES = 6;

    public Light CityLight;

    public string CityName;
    public float Relationship; // [0-4]

    public override void InitAttributes(GameModel model)
    {
        BuildingName = "City";
        BuildingDescription = "A permanent settlement of a big group of planet inhabitants.";
        BuildingIcon = model.Icons.Building_City;

        MaxHealth = model.GameSettings.City_MaxHealth;
        EmissionsPerCycle = model.GameSettings.City_Emissions;
        RepairCost = model.GameSettings.City_RepairCost;
    }

    public override bool CanBuildOn(Tile t)
    {
        return t.Type == TileType.Land && t.Topology != TileTopology.Mountains && !t.IsInRangeOfBuilding(MIN_DISTANCE_TO_OTHER_CITIES, typeof(Building_City));
    }
    public override void OnBuild()
    {
        CityName = MarkovChainWordGenerator.GenerateWord("Province", 4);
        Relationship = 2;
        UILabel.UpdatePanel();
    }
}
