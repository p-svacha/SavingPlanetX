using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class City : Building
{
    private const int MIN_DISTANCE_TO_OTHER_CITIES = 6;

    public Light CityLight;

    public string CityName;
    public float Relationship; // < 2 = enemy | < 1 = unfriendly | [-1,1] = neutral | > 1 = friendly | > 2 = allied

    public override void InitAttributes(GameModel model)
    {
        BuildingName = "City";
        BuildingDescription = "A permanent settlement of a big group of planet inhabitants.";
        BuildingIcon = model.Icons.Building_City;

        MaxHealth = model.GameSettings.City_MaxHealth;
        EmissionsPerCycle = model.GameSettings.City_Emissions;
        RepairCost = model.GameSettings.City_RepairCost;
    }

    public void ChangeRelationship(float amount)
    {
        Relationship += amount;
        UILabel.UpdatePanel();
    }

    public override bool CanBuildOn(Tile t)
    {
        return t.Type == TileType.Land && t.Topology != TileTopology.Mountains && !t.IsInRangeOfBuilding(MIN_DISTANCE_TO_OTHER_CITIES, typeof(City));
    }
    public override void OnBuild()
    {
        CityName = MarkovChainWordGenerator.GenerateWord("Province", 4);
        Relationship = 0;
        UILabel.UpdatePanel();
    }

    #region Getters

    public RelationshipStatus RelationshipStatus
    {
        get
        {
            if (Relationship <= -2f) return RelationshipStatus.Enemy;
            if (Relationship <= -1f) return RelationshipStatus.Unfriendly;
            if (Relationship <= 1f) return RelationshipStatus.Neutral;
            if (Relationship <= 2f) return RelationshipStatus.Friendly;
            return RelationshipStatus.Allied;
        }
    }

    public bool Discovered
    {
        get
        {
            return Tile.IsVisible;
        }
    }

    #endregion
}
