using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CityLabel : UI_BuildingLabel
{
    public Text CityName;
    public Image RelationImage;
    public Image EmissionImage;

    private void Start()
    {
        BUILDING_LABEL_HEIGHT = 2.5f;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();
        City city = (City)Building;
        CityName.text = city.CityName;
        SetRelationshipImage(city);
        SetEmissionImage(city);
    }

    private void SetRelationshipImage(City city)
    {
        int relationship = (int)city.RelationshipStatus;
        RelationImage.sprite = Building.Model.Icons.Relationship[relationship];
        RelationImage.color = Building.Model.ColorSettings.AlertColors[4 - relationship];
    }

    private void SetEmissionImage(City city)
    {
        float imageGap = 0.015f;
        int index = Mathf.Clamp((int)(city.EmissionsPerCycle * (1f / imageGap)), 0, 4);
        EmissionImage.sprite = Building.Model.Icons.Emission[4 - index];
        EmissionImage.color = Building.Model.ColorSettings.AlertColors[index];
    }
}
