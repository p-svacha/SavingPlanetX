using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Radar : Building
{
    public int Range = 3;

    public override void OnBuild()
    {
        foreach (Tile t in Tile.TileIsRange(Range)) t.IsInFogOfWar = false;
    }

    public override void OnEndTurn()
    {
        
    }
}
