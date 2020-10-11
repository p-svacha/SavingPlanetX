using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E002_StabilityDecrease : GameEvent
{
    private const float BaseProbability = 1f;

    public override void Initialize(GameModel model)
    {
        base.Initialize(model);

        Id = 2;

        Text = "An unknown astronomical event has destabilized the star by 2 points.";
        Options = new List<Tuple<string, Action>>
        {
            new Tuple<string, Action>("OK", () => ApplyEffect(model))
        };
    }

    public override float GetProbability(GameModel model)
    {
        return BaseProbability + ((GameSettings.Settings.MaxInstability - model.InstabilityLevel) / 10f);
    }

    private void ApplyEffect(GameModel model)
    {
        Options.Clear();
        model.DecreaseStability(2);
        model.EventHandled();
    }
}
