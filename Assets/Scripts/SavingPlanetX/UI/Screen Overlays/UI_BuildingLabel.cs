using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingLabel : MonoBehaviour
{
    protected const float HEALTH_BAR_GAP_SIZE = 0.01f;
    protected float BUILDING_LABEL_HEIGHT = 1.7f;

    public Building Building;
    public RectTransform HealthpointPrefab;

    public RectTransform HealthBarFrame;

    public void Init(Building b)
    {
        Building = b;

        float stepSize = ((1f - ((Building.MaxHealth - 1) * HEALTH_BAR_GAP_SIZE)) / Building.MaxHealth);
        for (int i = 0; i < Building.MaxHealth; i++)
        {
            RectTransform healthPanel = Instantiate(HealthpointPrefab, HealthBarFrame.transform);
            healthPanel.anchorMin = new Vector2(i * stepSize + (i + 1) * HEALTH_BAR_GAP_SIZE, 0.1f);
            healthPanel.anchorMax = new Vector2((i + 1) * stepSize + (i - 1) * HEALTH_BAR_GAP_SIZE, 0.9f);
        }
        UpdateHealthbar();
    }

    public void UpdateHealthbar()
    {
        for (int i = 0; i < Building.MaxHealth; i++)
        {
            HealthBarFrame.transform.GetChild(i).gameObject.SetActive(i < Building.Health);
            int arrayLength = Building.Model.ColorSettings.AlertColors.Length;
            int colorIndex = (int)(arrayLength - ((1f * Building.Health / Building.MaxHealth) * arrayLength));
            HealthBarFrame.transform.GetChild(i).GetComponent<Image>().color = Building.Model.ColorSettings.AlertColors[colorIndex];
        }
    }

    protected void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(Building.transform.position.x, BUILDING_LABEL_HEIGHT, Building.transform.position.z));
    }

    public virtual void Update()
    {
        UpdatePosition();
    }
}
