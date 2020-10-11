using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : Disaster
{
    private const float RANGE_FACTOR = 2f;

    public Earthquake(GameModel model, Tile center, int intensity) : base(model, center, intensity)
    {
        Name = "Earthquake " + (model.Day + 1);
    }

    public override void ApplyEffect()
    {
        DayDamage = 0;
        State = DisasterState.Completed;
        int range = (int)(Intensity * RANGE_FACTOR);
        for(int i = 0; i < range; i++)
        {
            foreach (Tile t in Center.TilesWithDistance(i)) DayDamage += Model.DealDamage(t, Intensity - (int)(1f * i * (1f / RANGE_FACTOR)));
        }
        TotalDamage += DayDamage;
        Day++;
    }

    public override void CastVisualEffect()
    {
        Model.CameraHandler.Shake(2f, 1f * Intensity / 6f);
    }
}
