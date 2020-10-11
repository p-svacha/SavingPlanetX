using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MorningReport : MonoBehaviour
{
    // Elements
    public UI_MR_SummaryTab SummaryTab;
    public UI_MR_DisasterTab DisasterTab;
    public UI_MR_EventTab EventTab;

    public int Day;

    public void Init(GameModel model)
    {
        Day = model.Day;
        SummaryTab.Init(model);
        DisasterTab.Init(model);
        EventTab.Init(model);
    }
}
