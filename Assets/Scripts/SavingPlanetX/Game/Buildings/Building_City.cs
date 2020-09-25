using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_City : Building
{
    public int InstabilityFactor = 1;

    public override void OnBuild()
    {
        
    }

    public override void OnEndTurn()
    {
        Model.InstabilityLevel += InstabilityFactor;
    }
}
