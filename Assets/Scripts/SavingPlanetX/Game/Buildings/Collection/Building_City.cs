using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building_City : Building
{
    private const int MIN_DISTANCE_TO_OTHER_CITIES = 6;

    public Light CityLight;

    public string CityName;
    public float Emissions; // Adds to the star instability each cycle
    public int Relation;

    public override void InitAttributes()
    {
        BuildingName = "City";
        BuildingDescription = "A permanent settlement of a big group of planet inhabitants.";
    }

    public override bool CanBuildOn(Tile t)
    {
        return t.Type == TileType.Land && t.Topology != TileTopology.Mountains && !t.IsInRangeOfBuilding(MIN_DISTANCE_TO_OTHER_CITIES, typeof(Building_City));
    }
    public override void OnBuild()
    {
        Emissions = Model.Settings.CityStartEmissions;
        Relation = 3;
        CityName = MarkovChainWordGenerator.GenerateWord("Province", 4);
    }

    public override void CycleAction()
    {
        if(GetComponentInChildren<Renderer>().isVisible) Model.GameUI.CreateInfoBlob(gameObject, Emissions.ToString(), Color.black, Color.white);
        Model.DecreaseStability(Emissions);
    }
}
