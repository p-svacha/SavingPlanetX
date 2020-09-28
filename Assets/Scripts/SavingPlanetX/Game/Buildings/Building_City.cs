using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building_City : Building
{
    private const int MIN_DISTANCE_TO_OTHER_CITIES = 2;

    public UI_CityLabel UILabel;

    public string Name;
    public float InstabilityFactor = 0.05f;
    public int Relation;

    public override bool CanBuildOn(Tile t)
    {
        return t.Type == TileType.Land && t.Topology != TileTopology.Mountains && !t.IsInRangeOfBuilding(MIN_DISTANCE_TO_OTHER_CITIES, typeof(Building_City));
    }
    public override void OnBuild()
    {
        InstabilityFactor = 0.05f;
        Health = Model.Settings.CityHealth;
        Relation = 3;
        Name = MarkovChainWordGenerator.GenerateWord("Province", 4);
    }

    public override void OnEndTurn()
    {
        Model.StarInstabilityLevel += InstabilityFactor;
    }
}
