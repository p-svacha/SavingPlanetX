using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    public int Id;
    public bool EventHandled;

    public abstract float GetProbability(GameModel model);
    public abstract RectTransform GetEventDialog(GameModel model);
}
