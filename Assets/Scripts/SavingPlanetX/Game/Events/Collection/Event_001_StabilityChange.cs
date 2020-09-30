using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_001_StabilityChange : GameEvent
{
    private const float Probability = 1f;
    private const string Title = "Stability Increase";
    private const string Text = "The star has unexpectedly stabilized by 2 points.";

public Event_001_StabilityChange()
    {
        Id = 1;
    }

    public override float GetProbability(GameModel model)
    {
        return Probability;
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
