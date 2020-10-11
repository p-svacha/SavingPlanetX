using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MR_SummaryTab : MonoBehaviour
{
    // Prefabs
    public UI_MR_SummaryContentLine ContentLine;

    // Elements
    public RectTransform ContentPanel;
    public Text Title;

    public void Init(GameModel model)
    {
        Title.text = "Summary for Day " + model.Day;

        UI_MR_SummaryContentLine emissionsLine = Instantiate(ContentLine, ContentPanel);
        float instabilityChange = model.InstabilityChangeThisCycle;

        emissionsLine.Init("Emissions", (instabilityChange >= 0 ? "+ " : "- ") + Mathf.Abs(instabilityChange).ToString("0.00"));

        UI_MR_SummaryContentLine moneyLine = Instantiate(ContentLine, ContentPanel);
        int moneyChange = model.MoneyChangeThisCycle;
        moneyLine.Init("Money", (moneyChange >= 0 ? "+ " : "- ") + Mathf.Abs(moneyChange).ToString());
    }
}
