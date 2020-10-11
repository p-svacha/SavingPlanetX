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

    public Text EventText;
    public Text IdText;
    public UI_Button[] Buttons;

    public RectTransform ContentPanel;

    public void Init(GameModel model)
    {
        Event = model.DayEvent;
        UpdateContent();
        UpdateTabColor();
    }

    public void UpdateContent()
    {
        EventText.text = Event.Text;
        IdText.text = Event.Id.ToString();
        Debug.Log(Event.Options.Count);
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].Button.onClick.RemoveAllListeners();
            if (i >= Event.Options.Count) Buttons[i].gameObject.SetActive(false);
            else
            {
                int j = i;
                Buttons[i].SetEnabled(true);
                Buttons[i].gameObject.SetActive(true);
                Buttons[i].Text.text = Event.Options[i].Item1;
                Buttons[i].Button.onClick.AddListener(() => { Event.Options[j].Item2.Invoke(); UpdateContent(); });
            }
        }
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
