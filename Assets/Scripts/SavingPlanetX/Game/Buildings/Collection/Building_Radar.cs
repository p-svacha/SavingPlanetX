using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Radar : Building
{
    public override void InitAttributes()
    {
        BuildingName = "Radar";
        BuildingDescription = "A building that provides vision of the surrounding area.";
        MaxHealth = GameSettings.Radar_MaxHealth;
        BuildCost = GameSettings.Radar_BuildCost;
        RepairCost = GameSettings.Radar_RepairCost;
        VisiblityRange = GameSettings.Radar_VisiblityRange;
    }

    public override bool CanBuildOn(Tile t)
    {
        return t != null && t.Type == TileType.Land && t.Topology != TileTopology.Mountains && t.Building == null;
    }
}
