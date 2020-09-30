using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Radar : Building
{
    public int Range = 3;

    public override void InitAttributes()
    {
        BuildingName = "Radar";
        BuildingDescription = "A building that provides vision of the surrounding area.";
    }

    public override bool CanBuildOn(Tile t)
    {
        return t != null && t.Type == TileType.Land && t.Topology != TileTopology.Mountains && t.Building == null;
    }

    public override void OnBuild()
    {
        foreach (Tile t in Tile.TilesInRange(Range)) t.IsInFogOfWar = false;
    }

    public override void OnEndTurn()
    {
        
    }
}
