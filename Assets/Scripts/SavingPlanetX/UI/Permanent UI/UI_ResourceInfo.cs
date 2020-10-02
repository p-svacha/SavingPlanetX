using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResourceInfo : MonoBehaviour
{
    private GameModel Model;

    public Image MoneyIcon;
    public Text MoneyText;

    public void Init(GameModel model)
    {
        Model = model;
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        MoneyText.text = Model.Money + "";
    }

}
