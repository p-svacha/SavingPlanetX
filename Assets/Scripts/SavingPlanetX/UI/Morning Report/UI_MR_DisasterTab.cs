using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MR_DisasterTab : MonoBehaviour
{
    // Prefabs
    public UI_MR_DisasterContentLine ContentLine;

    // Elements
    public RectTransform ContentPanel;
    public Text Title;

    public void Init(GameModel model)
    {
        Title.text = "Disaster Report for Day " + model.Day;

        List<Disaster> cycleDisasters = model.CycleDisasters;

        if(cycleDisasters.Count == 0)
        {
            UI_MR_DisasterContentLine newLine = Instantiate(ContentLine, ContentPanel);
            newLine.Init("Nothing happened", "", "", "", "", "");
        }
        else
        {
            foreach(Disaster d in cycleDisasters)
            {
                UI_MR_DisasterContentLine newLine = Instantiate(ContentLine, ContentPanel);
                newLine.Init(d.Name,
                    d.Center.IsVisible ? "Near " + d.Center.NearestSettlement.CityName : "Unknown",
                    d.Intensity.ToString(),
                    d.DayDamage + " (" + d.TotalDamage + ")", 
                    d.Day.ToString(), 
                    d.State.ToString());
            }
        }
    }
}
