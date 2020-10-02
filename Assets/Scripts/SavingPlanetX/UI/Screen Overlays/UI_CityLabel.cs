using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CityLabel : UI_BuildingLabel
{
    public Text CityName;
    public Image RelationImage;
    public Image StabilityImage;

    private void Start()
    {
        BUILDING_LABEL_HEIGHT = 2.5f;
    }

    public override void Update()
    {
        base.Update();
        CityName.text = ((Building_City)Building).CityName;
    }
}
