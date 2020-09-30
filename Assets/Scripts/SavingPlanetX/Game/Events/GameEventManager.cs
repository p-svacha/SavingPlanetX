﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEventManager
{
    private List<GameEvent> EventList = new List<GameEvent>()
    {
        new Event_001_StabilityChange()
    };

    private GameModel Model;

    public GameEventManager(GameModel model)
    {
        Model = model;
    }

    public void CastEvent(GameEvent e)
    {
        e.Cast(Model);
    }

    public void CastRandomEvent()
    {
        GameEvent chosenEvent = null;
        float probSum = EventList.Sum(x => x.GetProbability(Model));
        float tmpSum = 0f;
        float rng = Random.Range(0, probSum);
        foreach(GameEvent e in EventList)
        {
            tmpSum += e.GetProbability(Model);
            if(rng < tmpSum)
            {
                chosenEvent = e;
                break;
            }
        }
        CastEvent(chosenEvent);
    }
    
}
