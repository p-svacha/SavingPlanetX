﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Radar : Building
{
    public override void InitAttributes(GameModel model)
    {
        BuildingName = "Radar";
        BuildingDescription = "A building that provides vision of the surrounding area.";
        BuildingIcon = model.Icons.Building_Radar;

        MaxHealth = model.GameSettings.Radar_MaxHealth;
        BuildCost = model.GameSettings.Radar_BuildCost;
        RepairCost = model.GameSettings.Radar_RepairCost;
        VisiblityRange = model.GameSettings.Radar_VisiblityRange;
    }

    public override bool CanBuildOn(Tile t)
    {
        return t != null && t.Type == TileType.Land && t.Topology != TileTopology.Mountains && t.Building == null;
    }
}
