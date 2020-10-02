using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DayInfo : MonoBehaviour
{
    public GameModel Model;

    public Text DayText;
    public Text TimeText;
    
    public RectTransform ProgressBar;
    public float PanelWidth;

    void Start()
    {
        PanelWidth = GetComponent<RectTransform>().rect.width;
    }

    void Update()
    {
        DayText.text = "Day " + Model.Day;
        TimeText.text = Model.Hour.ToString("00") + ":" + Model.Minute.ToString("00");
        ProgressBar.offsetMax = new Vector2(- (PanelWidth - (1f * Model.CycleTime / GameModel.CYCLE_TIME * PanelWidth)), 1f);
    }
}
