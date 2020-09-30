using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public const float BUILDING_LABEL_HEIGHT = 2.5f;

    public GameModel Model;

    // Prefabs
    public RectTransform LabelsPanel;
    public RectTransform DialogPanel;
    public RectTransform PanelPrefab;
    public UI_CityLabel CityLabel;
    public UI_InfoBox InfoBox;

    // UI Elements
    public Button EndTurnButton;
    public Text TileInfoText;
    public RectTransform InstabilityPanel;
    public UI_BuildPanel BuildPanel;
    public UI_SelectionInfo SelectionInfo;

    private void Update()
    {
        // Current hovered Tile
        UpdateTileInfo(Model.HoveredTile);

        // Stability Panel
        UpdateInstabilityPanel();

        // Building Labels
        foreach (Building_City city in Model.Cities) 
        {
            if (city.GetComponentInChildren<Renderer>().isVisible)
            {
                city.UILabel.gameObject.SetActive(true);
                city.UILabel.transform.position = Camera.main.WorldToScreenPoint(new Vector3(city.transform.position.x, BUILDING_LABEL_HEIGHT, city.transform.position.z));
            }
            else
                city.UILabel.gameObject.SetActive(false);
        }
    }

    public void UpdateTileInfo(Tile tile)
    {
         TileInfoText.text = GetTileInfoText(tile);
    }

    public void UpdateInstabilityPanel()
    {
        for (int i = 0; i < Model.Settings.MaxInstability; i++)
            InstabilityPanel.transform.GetChild(i).gameObject.SetActive(i < Model.StarInstabilityLevel);
    }

    #region Initilaize UI
    public void Initialize(GameModel model)
    {
        Model = model;
        InitCityLabels();
        InitEndTurnButton();
        InitInstabilityPanel();
        BuildPanel.Init(Model);
        SelectionInfo.gameObject.SetActive(false);
    }

    private void InitCityLabels()
    {
        foreach (Building_City city in Model.Cities)
        {
            city.UILabel = Instantiate(CityLabel, LabelsPanel);
            city.UILabel.CityName.text = "City of " + city.CityName;
            city.UILabel.Init(city);
        }
    }
    private void InitInstabilityPanel()
    {
        float gap = 0.1f;
        float stepSize = (1f / (Model.Settings.MaxInstability));
        float gapSize = stepSize * gap;
        for (int i = 0; i < Model.Settings.MaxInstability; i++)
        {
            RectTransform levelPanel = Instantiate(PanelPrefab, InstabilityPanel.transform);
            levelPanel.anchorMin = new Vector2(gapSize + i * stepSize, 0.05f);
            levelPanel.anchorMax = new Vector2(((i + 1) * stepSize) - gapSize, 0.95f);

            levelPanel.GetComponent<Image>().color = Model.ColorManager.AlertColors[Mathf.Min(Model.ColorManager.AlertColors.Length - 1, i / (Model.Settings.MaxInstability / (Model.ColorManager.AlertColors.Length)))];
        }
    }

    private void InitEndTurnButton()
    {
        EndTurnButton.onClick.AddListener(() => { Model.EndDay(); });
    }

    #endregion

    #region Dialog

    public void ShowInfoBox(string title, string text, string idText, string buttonText = "OK", Action buttonAction = null)
    {
        UI_InfoBox infoBox = Instantiate(InfoBox, DialogPanel);
        infoBox.Initialize(title, text, idText, buttonText, buttonAction);
    }

    #endregion

    private string GetTileInfoText(Tile tile)
    {
        if (tile == null) return "";
        else return tile.Biome.ToString() + " " + tile.Topology.ToString();
    }

    
}
