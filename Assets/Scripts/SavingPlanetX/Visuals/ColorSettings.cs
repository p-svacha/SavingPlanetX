using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Color[] AlertColors = new Color[5];

    public Color UI_Lighter;
    public Color UI_Darker;
    public Color UI_Interactive;

    public Color BuildingSelectedColor;
}
