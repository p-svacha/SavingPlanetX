using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MR_SummaryContentLine : MonoBehaviour
{
    public Text Label;
    public Text Value;

    public void Init(string label, string value)
    {
        Label.text = label;
        Value.text = value;
    }
}
