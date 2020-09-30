using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectionInfo : MonoBehaviour
{
    public Text Name;
    public Image Image;

    public Text Description;

    public void SetSelection(Building b)
    {
        Name.text = b.BuildingName;
        Description.text = b.BuildingDescription;
        Image.sprite = b.BuildingIcon;
    }
}
