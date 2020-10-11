using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Headquarter : Building
{
    public override void InitAttributes(GameModel model)
    {
        BuildingName = "Headquarter";
        BuildingDescription = "The base camp where you control your actions from. Yields basic resources and is very robust.";
        BuildingIcon = model.Icons.Building_HQ;

        MaxHealth = model.GameSettings.Headquater_MaxHealth;
        MoneyPerCycle = model.GameSettings.Headquarter_Money;
        VisiblityRange = model.GameSettings.Headquarter_VisibilityRange;
        RepairCost = model.GameSettings.Headquarter_RepairCost;
    }

    public override bool CanBuildOn(Tile t)
    {
        return t != null && t.Type == TileType.Land && t.Topology != TileTopology.Mountains && t.Building == null;
    }
}
