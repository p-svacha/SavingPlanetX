using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildPanel : MonoBehaviour
{
    public GameModel Model;

    public Button BuildRadarButton;

    public void Init(GameModel model)
    {
        Model = model;
        BuildRadarButton.onClick.AddListener(() => PlanBuild(Model.BPC.Radar));
    }

    private void PlanBuild(Building b)
    {
        Model.InitBuildMode(b);
    }
}
