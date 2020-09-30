using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    public int Id;

    public abstract float GetProbability(GameModel model);
    public abstract void Cast(GameModel model);

    public void EndEvent(GameModel model)
    {
        model.EndEvent();
    }
}
