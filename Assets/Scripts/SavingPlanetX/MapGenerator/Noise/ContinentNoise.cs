using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentNoise : Noise
{
    private RidgedMultifractalNoise rmfn = new RidgedMultifractalNoise(1, 2, 6, Random.Range(int.MinValue, int.MaxValue));
    private float baseScale = 0.1f;

    /// <summary>
    /// Returns the land type for the given position:
    /// 0 = Water
    /// 1 = Land
    /// </summary>
    public override float GetValue(float x, float y, MapData mapData)
    {
        float val = (float)(rmfn.GetValue(x * baseScale * mapData.ScaleFactor, y * baseScale * mapData.ScaleFactor, 1));
        float distanceFromEdge = Mathf.Min(x, mapData.MapWidthAbsolute - x, y, mapData.MapHeightAbsolute - y);
        return (val > 0.2f || distanceFromEdge < 4f) ? 0f : 1f;
    }
}
