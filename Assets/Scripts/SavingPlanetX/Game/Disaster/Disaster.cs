﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Disaster
{
    private int DisasterId = 1;

    public GameModel Model;

    public int Id;
    public string Name;
    public int Intensity;
    public Tile Center;

    public int Cycle;
    public int CycleTime;

    public DisasterState State;

    public Disaster(GameModel model)
    {
        Model = model;
        Id = DisasterId++;
        State = DisasterState.Planned;
    }

    public void SetTime(int cycle, int time)
    {
        Cycle = cycle;
        CycleTime = time;
    }

    public abstract void ApplyEffect();
    public abstract void CastVisualEffect();
}
