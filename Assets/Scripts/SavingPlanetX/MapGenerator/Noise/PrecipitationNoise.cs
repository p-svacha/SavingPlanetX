﻿using UnityEngine;

public class PrecipitationNoise : Noise
{
    private int PolePrecipitation;
    private int EquatorPrecipitation;

    private float BaseScale = 0.09f;
    private int OffsetX;
    private int OffsetY;

    public PrecipitationNoise(int polePrec, int equatorPrec)
    {
        OffsetX = Random.Range(-10000, 10000);
        OffsetY = Random.Range(-10000, 10000);

        PolePrecipitation = polePrec;
        EquatorPrecipitation = equatorPrec;
    }

    /// <summary>
    /// Returns the annual precipitation for the given position in mm
    /// </summary>
    public override float GetValue(float x, float y, MapData mapData)
    {
        float yEquator = mapData.MapHeightAbsolute / 2;
        float basePrec = PolePrecipitation + ((1 - (Mathf.Abs(y - yEquator) / yEquator)) * (EquatorPrecipitation - PolePrecipitation));
        float precMoidifer = Mathf.PerlinNoise(OffsetX + x * BaseScale * mapData.ScaleFactor, OffsetY + y * BaseScale * mapData.ScaleFactor);
        return basePrec * precMoidifer;
    }
}
