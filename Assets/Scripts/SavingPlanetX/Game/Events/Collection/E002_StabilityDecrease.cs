using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E002_StabilityDecrease : GameEvent
{
    private const float BaseProbability = 1f;
    private const string Title = "Instability Increase";
    private const string Text = "An unknown astronomical event has destabilized the star by 2 points.";

    public E002_StabilityDecrease()
    {
        Id = 2;
    }

    public override float GetProbability(GameModel model)
    {
        return BaseProbability + ((model.GameSettings.MaxInstability - model.InstabilityLevel) / 10f);
    }

    public override RectTransform GetEventDialog(GameModel model)
    {
        return model.GameUI.GetInfoBox(Title, Text, Id.ToString(), "OK", () => ApplyEffect(model) );
    }

    private void ApplyEffect(GameModel model)
    {
        model.DecreaseStability(2);
        model.EventHandled();
    }
}
