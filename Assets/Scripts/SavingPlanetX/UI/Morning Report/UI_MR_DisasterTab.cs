using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MR_DisasterTab : MonoBehaviour
{
    public Text Title;

    public void Init(GameModel model)
    {
        Title.text = "Disaster Report for Day " + model.Day;
    }
}
