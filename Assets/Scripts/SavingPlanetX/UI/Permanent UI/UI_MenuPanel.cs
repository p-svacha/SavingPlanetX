using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuPanel : MonoBehaviour
{
    public GameModel Model;

    public Image Background;
    public UI_Button ReportButton;

    public void Init(GameModel model)
    {
        Model = model;
        Background.color = ColorSettings.Colors.UI_Darker;
        ReportButton.SetEnabled(true);
        ReportButton.Button.onClick.AddListener(() => Model.ToggleReport());
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        ReportButton.SetEnabled(Model.ActiveReport != null && Model.GameState == GameState.Idle);
    }
}
