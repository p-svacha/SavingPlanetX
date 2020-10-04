using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingInfo : MonoBehaviour
{
    public Text Name;
    public Image Image;
    public UI_Button RepairButton;

    public Building DisplayedBuilding;
    
    public Text Description;

    public void SetSelection(GameModel model, Building b)
    {
        DisplayedBuilding = b;
        Name.text = b.BuildingName;
        Description.text = b.BuildingDescription;
        Image.sprite = b.BuildingIcon;
        RepairButton.SetEnabled(b.CanRepair());
        RepairButton.Button.onClick.RemoveAllListeners();
        RepairButton.Button.onClick.AddListener(() => model.RepairBuilding(DisplayedBuilding));
    }

    public void UpdatePanel()
    {
        RepairButton.SetEnabled(DisplayedBuilding.CanRepair());
    }
}
