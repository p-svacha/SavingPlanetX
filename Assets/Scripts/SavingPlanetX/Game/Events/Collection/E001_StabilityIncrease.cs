using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E001_StabilityIncrease : GameEvent
{
    private const float BaseProbability = 1f;

    public override void Initialize(GameModel model)
    {
        base.Initialize(model);

        Id = 1;

        Text = "The star has unexpectedly stabilized by 2 points.";
        Options = new List<Tuple<string, Action>>
        {
            new Tuple<string, Action>("OK", () => ApplyEffect(model))
        };
    }

    public override float GetProbability(GameModel model)
    {
        return BaseProbability + (model.InstabilityLevel / 10f);
    }

    private void ApplyEffect(GameModel model)
    {
        Options.Clear();
        model.IncreaseStability(2);
        model.EventHandled();
    }
}
