using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEventHandler
{
    private List<GameEvent> EventList = new List<GameEvent>()
    {
        new E001_StabilityIncrease(),
        new E002_StabilityDecrease(),
    };

    private GameModel Model;

    public GameEventHandler(GameModel model)
    {
        Model = model;
    }

    public GameEvent GetRandomEvent()
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
        return chosenEvent;
    }
    
}
