using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameSettings : ScriptableObject
{
    public int NumCities = 15;
    public int MaxInstability = 20;

    public int Headquater_MaxHealth = 5;
    public int Headquarter_VisibilityRange = 4;
    public int Headquarter_Money = 10;
    public int Headquarter_RepairCost = 10;

    public int City_MaxHealth = 5;
    public float City_Emissions = 0.05f;
    public int City_RepairCost = 10;

    public int Radar_MaxHealth = 3;
    public int Radar_VisiblityRange = 3;
    public int Radar_BuildCost = 30;
    public int Radar_RepairCost = 10;

    
}
