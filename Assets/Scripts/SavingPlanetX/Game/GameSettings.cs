using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameSettings : ScriptableObject
{
    public int NumCities = 15;
    public int MaxInstability = 20;
    public int CityHealth = 3;
    public int BuildingHealth = 3;
}
