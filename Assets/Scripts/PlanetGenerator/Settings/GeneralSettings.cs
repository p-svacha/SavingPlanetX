using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu()]
public class GeneralSettings : ScriptableObject
{

    public float PlanetRadius;
    [Range(0, 3)]
    public int Subdivisions;

}
