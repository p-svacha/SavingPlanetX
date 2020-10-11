using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameSettings : ScriptableObject
{
    public int NumCities = 15;
    public int MaxInstability = 20;

    // Disasters
    public float Disaster_StandardDeviation = 0.6f; // Disaster intensity is calculated with a normal random distribution whereas the current instability level is the mean and this value is the standard deviation.

    public float Earthquake_BaseChance = 0f;
    public float Earthquake_BaseRise = 0.01f;
    public float Earthquake_RisePerInstability = 0.01f;

    public float Hurricane_BaseChance = 0f;
    public float Hurricane_BaseRise = 0.01f;
    public float Hurricane_RisePerInstability = 0.01f;

    // Buildings
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

    public static GameSettings Settings
    {
        get
        {
            return GameObject.Find("GameModel").GetComponent<GameModel>().GameSettings;
        }
    }
}
