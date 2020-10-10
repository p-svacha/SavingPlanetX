using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MR_SummaryTab : MonoBehaviour
{
    public Text Title;

    public void Init(GameModel model)
    {
        Title.text = "Summary for Day " + model.Day;
    }
}
