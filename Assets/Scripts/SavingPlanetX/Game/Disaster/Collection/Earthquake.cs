using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : Disaster
{
    private const float RANGE_FACTOR = 2f;

    public Tile Epicenter;

    public Earthquake(GameModel model, Tile epicenter, int intensity) : base(model)
    {
        Intensity = intensity;
        Epicenter = epicenter;
        Center = epicenter;
    }

    public override void ApplyEffect()
    {
        State = DisasterState.Completed;
        int range = (int)(Intensity * RANGE_FACTOR);
        for(int i = 0; i < range; i++)
        {
            foreach (Tile t in Center.TilesWithDistance(i)) Model.DealDamage(t, Intensity - (int)(1f * i * (1f / RANGE_FACTOR)));
        }
        Debug.Log("Earthquake happened with intensity " + Intensity);
    }

    public override void CastVisualEffect()
    {
        Model.CameraHandler.Shake(2f, 1f * Intensity / 6f);
    }
}
