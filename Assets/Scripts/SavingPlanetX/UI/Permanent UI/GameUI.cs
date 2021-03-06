﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameModel Model;

    // Prefabs
    public RectTransform Prefab_Panel;
    public UI_CityLabel Prefab_CityLabel;
    public UI_InfoBlob Prefab_InfoBlob;
    public UI_MorningReport Prefab_MorningReport;

    // UI Elements
    public Image AlertPanel;
    public RectTransform LabelsPanel;
    public RectTransform DialogPanel;
    public UI_Button EndDayButton;
    public Text TileInfoText;
    public RectTransform InstabilityPanel;
    public UI_BuildPanel BuildPanel;
    public UI_BuildingInfo BuildingInfo;
    public UI_ResourceInfo ResourceInfo;
    public UI_MenuPanel MenuPanel;

    // Variables
    private bool AlertFlash;
    private int TransparencyChange;
    private const float ALERT_FLASH_SPEED = 1f;

    // Dynamic UI Elements
    public List<UI_MorningReport> MorningReports;

    private void Update()
    {
        UpdateTileInfo();
        UpdateAlertPanel();
    }

    public void UpdateTileInfo()
    {
        TileInfoText.text = GetTileInfoText(Model.HoveredTile);
    }

    public void UpdateInstabilityPanel()
    {
        for (int i = 0; i < Model.GameSettings.MaxInstability; i++)
            InstabilityPanel.transform.GetChild(i).gameObject.SetActive(i < Model.InstabilityLevel);
    }

    #region Initilaize UI
    public void Initialize(GameModel model)
    {
        Model = model;
        InitEndDayButton();
        InitInstabilityPanel();
        BuildPanel.Init(Model);
        ResourceInfo.Init(Model);
        MenuPanel.Init(Model);
        BuildingInfo.gameObject.SetActive(false);

        UpdateInstabilityPanel();
    }

    private void InitInstabilityPanel()
    {
        float gap = 0.1f;
        float stepSize = (1f / (Model.GameSettings.MaxInstability));
        float gapSize = stepSize * gap;
        for (int i = 0; i < Model.GameSettings.MaxInstability; i++)
        {
            RectTransform levelPanel = Instantiate(Prefab_Panel, InstabilityPanel.transform);
            levelPanel.anchorMin = new Vector2(gapSize + i * stepSize, 0.05f);
            levelPanel.anchorMax = new Vector2(((i + 1) * stepSize) - gapSize, 0.95f);

            levelPanel.GetComponent<Image>().color = Model.ColorSettings.AlertColors[Mathf.Min(Model.ColorSettings.AlertColors.Length - 1, i / (Model.GameSettings.MaxInstability / (Model.ColorSettings.AlertColors.Length)))];
        }
    }

    #endregion

    #region Morning Report

    public UI_MorningReport InitAndShowMorningReport()
    {
        UI_MorningReport newReport = Instantiate(Prefab_MorningReport, DialogPanel);
        newReport.Init(Model);
        MorningReports.Add(newReport);
        return newReport;
    }

    #endregion

    #region End Turn Button
    private void InitEndDayButton()
    {
        EndDayButton.Button.onClick.AddListener(() => { Model.EndDay(); });
        EnableEndDayButton();
    }

    public void EnableEndDayButton()
    {
        EndDayButton.SetEnabled(true);
        EndDayButton.Text.text = "End Day";
    }

    public void DisableEndDayButton()
    {
        EndDayButton.SetEnabled(false);
        EndDayButton.Text.text = "Complete Event to End Day";
    }

    #endregion

    #region World Labels

    public void CreateInfoBlob(GameObject target, string blobText, Color textColor, Color bgColor)
    {
        UI_InfoBlob infoBlob = Instantiate(Prefab_InfoBlob, LabelsPanel);
        infoBlob.Initialize(target, blobText, textColor, bgColor);
    }

    #endregion

    #region Screen Overlays

    public void Alert_StartFlash()
    {
        AlertFlash = true;
        TransparencyChange = 1;
    }

    public void Alert_StopFlash()
    {
        AlertFlash = false;
        AlertPanel.color = Color.clear;
    }

    private void UpdateAlertPanel()
    {
        if(AlertFlash)
        {
            float a = AlertPanel.color.a;
            a += TransparencyChange * ALERT_FLASH_SPEED * Time.deltaTime;
            if (a >= 1 || a <= 0) TransparencyChange *= -1;
            AlertPanel.color = new Color(1f, 1f, 1f, a);
        }
    }

    #endregion

    private string GetTileInfoText(Tile tile)
    {
        if (tile == null) return "";
        else return tile.Biome.ToString() + " " + tile.Topology.ToString();
    }

    
}
