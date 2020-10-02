using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildPanel : MonoBehaviour
{
    public GameModel Model;

    public UI_Button BuildRadarButton;

    public void Init(GameModel model)
    {
        Model = model;
        BuildRadarButton.Button.onClick.AddListener(() => PlanBuild(Model.BPC.Radar));
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        BuildRadarButton.SetEnabled(Model.BPC.Radar.CanBuild(Model));
    }

    private void PlanBuild(Building b)
    {
        Model.InitBuildMode(b);
    }
}
