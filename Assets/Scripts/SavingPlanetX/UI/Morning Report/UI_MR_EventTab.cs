using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MR_EventTab : MonoBehaviour
{
    public GameEvent Event;

    public Image ActiveTabBackground;
    public Image InactiveTabBackground;
    public Text TabText;

    public RectTransform ContentPanel;

    public void Init(GameModel model)
    {
        Event = model.DayEvent;
        RectTransform eventDialog = Event.GetEventDialog(model);
        eventDialog.SetParent(ContentPanel.transform);
        UpdateTabColor();
    }

    public void UpdateTabColor()
    {
        if(!Event.EventHandled)
        {
            ActiveTabBackground.color = ColorSettings.Colors.UI_Interactive_Enabled_Back;
            InactiveTabBackground.color = ColorSettings.Colors.UI_Interactive_Enabled_Back;
            TabText.color = ColorSettings.Colors.UI_Interactive_Enabled_Front;
        }
        else
        {
            ActiveTabBackground.color = Color.white;
            InactiveTabBackground.color = Color.white;
            TabText.color = Color.black;
        }
    }
}
