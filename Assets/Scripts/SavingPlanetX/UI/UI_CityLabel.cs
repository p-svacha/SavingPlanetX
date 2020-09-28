using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CityLabel : MonoBehaviour
{
    public Color[] HealthColors;

    public Building_City City;
    public RectTransform HealthpointPrefab;

    public Text CityName;
    public GameObject HealthPanel;
    public Image RelationImage;
    public Image StabilityImage;

    public void Init(Building_City city)
    {
        City = city;

        HealthColors = new Color[]
        {
            City.Model.ColorManager.AlertColors[4],
            City.Model.ColorManager.AlertColors[2],
            City.Model.ColorManager.AlertColors[0],
        };
        float gapSize = 0.04f;
        float stepSize = ((1f - ((City.Model.Settings.CityHealth - 1) * gapSize))  / City.Model.Settings.CityHealth);
        for (int i = 0; i < City.Model.Settings.CityHealth; i++)
        {
            RectTransform healthPanel = Instantiate(HealthpointPrefab, HealthPanel.transform);
            healthPanel.anchorMin = new Vector2(i * stepSize + (i+1) * gapSize, 0.1f);
            healthPanel.anchorMax = new Vector2((i + 1) * stepSize + (i-1) * gapSize, 0.9f);
        }

        UpdateHealthbar();
    }

    public void UpdateHealthbar()
    {
        for(int i = 0; i < City.Model.Settings.CityHealth; i++)
        {
            HealthPanel.transform.GetChild(i).gameObject.SetActive(i < City.Health);
            HealthPanel.transform.GetChild(i).GetComponent<Image>().color = HealthColors[City.Health - 1];
        }
    }
}
