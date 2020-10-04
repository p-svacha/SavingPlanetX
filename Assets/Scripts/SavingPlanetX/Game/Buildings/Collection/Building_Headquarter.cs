using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Headquarter : Building
{
    public override void InitAttributes()
    {
        BuildingName = "Headquarter";
        BuildingDescription = "The base camp where you control your actions from. Yields basic resources and is very robust.";
        MaxHealth = GameSettings.Headquater_MaxHealth;
        MoneyPerCycle = GameSettings.Headquarter_Money;
        VisiblityRange = GameSettings.Headquarter_VisibilityRange;
        RepairCost = GameSettings.Headquarter_RepairCost;
    }

    public override bool CanBuildOn(Tile t)
    {
        return t != null && t.Type == TileType.Land && t.Topology != TileTopology.Mountains && t.Building == null;
    }
}
