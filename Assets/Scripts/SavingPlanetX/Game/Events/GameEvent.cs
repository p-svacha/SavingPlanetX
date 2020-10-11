using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    public int Id;
    public bool EventHandled;

    public string Text;
    public List<Tuple<string, Action>> Options;

    public abstract float GetProbability(GameModel model); // Should stay in between 0-5
    public virtual void Initialize(GameModel model)
    {
        EventHandled = false;
    }
}
