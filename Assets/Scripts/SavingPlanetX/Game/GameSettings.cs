using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameSettings : ScriptableObject
{
    public int NumCities = 15;
    public int MaxInstability = 20;

    public float CityStartEmissions = 0.05f;

    public int BuildingHealth = 5;
}
