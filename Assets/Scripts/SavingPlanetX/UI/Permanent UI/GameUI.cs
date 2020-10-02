using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameModel Model;

    // Prefabs
    public RectTransform PanelPrefab;
    public UI_CityLabel CityLabel;
    public UI_InfoBox InfoBox;
    public UI_InfoBlob InfoBlob;

    // UI Elements
    public Image AlertPanel;
    public RectTransform LabelsPanel;
    public RectTransform DialogPanel;
    public Button EndTurnButton;
    public Text TileInfoText;
    public RectTransform InstabilityPanel;
    public UI_BuildPanel BuildPanel;
    public UI_BuildingInfo BuildingInfo;
    public UI_ResourceInfo ResourceInfo;

    // Variables
    private bool AlertFlash;
    private int TransparencyChange;
    private const float ALERT_FLASH_SPEED = 1f;

    private void Update()
    {
        UpdateTileInfo();
        UpdateInstabilityPanel();
        UpdateAlertPanel();
    }

    public void UpdateTileInfo()
    {
         TileInfoText.text = GetTileInfoText(Model.HoveredTile);
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
        InitEndTurnButton();
        InitInstabilityPanel();
        BuildPanel.Init(Model);
        ResourceInfo.Init(Model);
        BuildingInfo.gameObject.SetActive(false);
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

            levelPanel.GetComponent<Image>().color = Model.ColorSettings.AlertColors[Mathf.Min(Model.ColorSettings.AlertColors.Length - 1, i / (Model.Settings.MaxInstability / (Model.ColorSettings.AlertColors.Length)))];
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

    #region World Labels

    public void CreateInfoBlob(GameObject target, string blobText, Color textColor, Color bgColor)
    {
        UI_InfoBlob infoBlob = Instantiate(InfoBlob, LabelsPanel);
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
