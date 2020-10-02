using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E001_StabilityIncrease : GameEvent
{
    private const float BaseProbability = 1f;
    private const string Title = "Stability Increase";
    private const string Text = "The star has unexpectedly stabilized by 2 points.";

    public E001_StabilityIncrease()
    {
        Id = 1;
    }

    public override float GetProbability(GameModel model)
    {
        return BaseProbability + (model.StarInstabilityLevel / 10f);
    }

    public override void Cast(GameModel model)
    {
        model.GameUI.ShowInfoBox(Title, Text, Id.ToString(), "Cool", () => ApplyEffect(model) );
    }

    private void ApplyEffect(GameModel model)
    {
        model.IncreaseStability(2);
        EndEvent(model);
    }
}
