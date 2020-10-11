using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class E003_StealFromCity : GameEvent
{
    private const float CaughtBeforeEscapeChance = 0.5f;

    private City City;
    private int MoneyAmount;
    private float CaughtChance;

    public override void Initialize(GameModel model)
    {
        base.Initialize(model);

        Id = 3;

        List<City> candidates = model.Cities.Where(x => x.Discovered).ToList();
        City = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        MoneyAmount = UnityEngine.Random.Range(3, 11) * 5; // 15 - 50
        int caughtChanceLevel = UnityEngine.Random.Range(0, 3);
        string caughtChanceString = "{chance}";
        if(caughtChanceLevel == 0)
        {
            caughtChanceString = "low";
            CaughtChance = 0.25f;
        }
        else if(caughtChanceLevel == 1)
        {
            caughtChanceString = "medium";
            CaughtChance = 0.5f;
        }
        else if (caughtChanceLevel == 2)
        {
            caughtChanceString = "high";
            CaughtChance = 0.75f;
        }

        Text = "A citizen from " + City.CityName + " has offered you to steal money from the city and give you some of it in exchange for shelter. The offered amount is " + MoneyAmount + ". There is a " + caughtChanceString + " chance that they will get caught during or after the operation resulting in a worsened relationship with the city. Do you accept the offer?";
        Options = new List<Tuple<string, Action>>
        {
            new Tuple<string, Action>("Accept", () => AcceptEffect(model)),
            new Tuple<string, Action>("Decline", () => DeclineEffect(model))
        };
    }

    public override float GetProbability(GameModel model)
    {
        return 5; // model.Cities.Where(x => x.Discovered).Count() / 3f;
    }

    private void AcceptEffect(GameModel model)
    {
        if(UnityEngine.Random.value < CaughtChance)
        {
            if(UnityEngine.Random.value < CaughtBeforeEscapeChance)
            {
                Text += "\n\nThe citizen has been caught before he could hand over the money. You gained nothing and your relationship with " + City.CityName + " has worsened by 1 point.";
                model.WorsenRelationship(City, 1);
            }
            else 
            {
                Text += "\n\nThe citizen has successfully stolen and handed over the money. However agents from " + City.CityName + " have found out that you were behind the operation. Your relationship with " + City.CityName + " has worsened by 1 point.";
                model.AddGold(MoneyAmount);
                model.WorsenRelationship(City, 1);
            }
        }
        else
        {
            Text += "\n\nThe citizen has successfully stolen the money without getting caught.";
            model.AddGold(MoneyAmount);
        }


        Options.Clear();
        model.EventHandled();
    }

    private void DeclineEffect(GameModel model)
    {
        model.EventHandled();
    }
}
