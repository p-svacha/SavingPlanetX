﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Color[] AlertColors = new Color[5];

    public Color UI_Lighter;
    public Color UI_Darker;

    public Color UI_Text_Positive;
    public Color UI_Text_Negative;

    public Color UI_Interactive_Enabled_Back;
    public Color UI_Interactive_Enabled_Front;
    public Color UI_Interactive_Disabled_Back;
    public Color UI_Interactive_Disabled_Front;

    public Color BuildingSelectedColor;

    public static ColorSettings Colors
    {
        get
        {
            return GameObject.Find("GameModel").GetComponent<GameModel>().ColorSettings;
        }
    }
}
