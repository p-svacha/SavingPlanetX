using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    public Light Sun;
    public Light Moon;

    public CameraHandler CameraHandler;
    public InputHandler InputHandler;
    public GameUI GameUI;

    public BuildingPrefabCollection BPC;
    public MaterialCollection MaterialCollection;
    public ParticleSystemCollection PSC;
    public IconCollection Icons;

    public GameSettings GameSettings;
    public ColorSettings ColorSettings;


    public GameState GameState = GameState.Initializing;

    public Map Map;
    public List<Building> Buildings = new List<Building>();

    public Tile HoveredTile { get; private set; }
    public Building SelectedBuilding { get; private set; }

    // Events
    public UI_MorningReport ActiveReport;
    public GameEventHandler EventHandler;
    public GameEvent DayEvent;

    // Disasters
    public DisasterHandler DisasterHandler;
    public List<Disaster> CycleDisasters = new List<Disaster>();
    public Disaster ActiveDisaster;
    public Queue<Disaster> DisastersToShow = new Queue<Disaster>();

    // Resources
    public int Money { get; private set; }
    public int Money_DayStart { get; private set; }

    public float InstabilityLevel { get; private set; }
    public float InstabilityLevel_DayStart { get; private set; }


    // Cycle values
    public int Cycle { get; private set; }
    private int _lastUpdateCycleTime;
    public int CycleTime { get; private set; }
    

    // Visual const
    private float CurCycleRealTime;
    private const float CYCLE_REAL_TIME = 5f;
    private const float MAX_MOON_INTENSITY = 0.35f;

    public const int CYCLE_OFFSET = 540; // The cycle starts at this time
    public const int CYCLE_TIME = 1440;

    // Timing values
    public const float DISASTER_ALERT_TIME = 2f; // Time before camera moves to disaster when one happens
    public const float DISASTER_START_TIME_OFFSET = 3f; // Time before disaster occurs after camera moved there
    public const float DISASTER_END_TIME_OFFSET = 4f; // Time before cycle continues after a diaster occured
    private float _disasterTime;

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.Initializing;
        StartNewGame(60, 45);
    }

    // Update is called once per frame
    void Update()
    {
        InputHandler.HandleInputs();
        CameraHandler.Update();
        DisasterHandler.Update();

        switch (GameState)
        {
            case GameState.Idle:
                break;

            case GameState.DayCycle:
                // Time update
                CurCycleRealTime += Time.deltaTime;
                _lastUpdateCycleTime = CycleTime;
                CycleTime = (int)(CurCycleRealTime / CYCLE_REAL_TIME * CYCLE_TIME);

                // Cycle actions
                foreach(Building b in Buildings)
                    if (b.Health > 0 && b.CycleActionTime < CycleTime && b.CycleActionTime >= _lastUpdateCycleTime) b.CycleAction();
                foreach (Disaster d in CycleDisasters)
                    if (d.CycleTime < CycleTime && d.CycleTime >= _lastUpdateCycleTime) DisastersToShow.Enqueue(d);

                if (DisastersToShow.Count > 0) ShowDisaster(DisastersToShow.Dequeue());

                // New day
                if (CurCycleRealTime >= CYCLE_REAL_TIME) StartNewDay();

                // Light adjustments
                Vector3 sunRotation = Vector3.Lerp(new Vector3(50, -30, 0), new Vector3(410, 330, 0), CurCycleRealTime / CYCLE_REAL_TIME);
                Sun.transform.rotation = Quaternion.Euler(sunRotation);
                if (sunRotation.x < 140) Moon.intensity = 0f;
                else if (sunRotation.x < 220) Moon.intensity = (sunRotation.x - 140) / 80 * MAX_MOON_INTENSITY;
                else if (sunRotation.x < 320) Moon.intensity = MAX_MOON_INTENSITY;
                else if(sunRotation.x < 400 ) Moon.intensity = MAX_MOON_INTENSITY - ((sunRotation.x - 320) / 80 * MAX_MOON_INTENSITY);
                else Moon.intensity = 0f;
                if ((int)sunRotation.x == 180) foreach (City city in Cities) city.CityLight.intensity = 1f;
                if ((int)sunRotation.x == 360) foreach (City city in Cities) city.CityLight.intensity = 0f;
                break;

            case GameState.AlertFlash:
                _disasterTime += Time.deltaTime;
                if(_disasterTime > DISASTER_ALERT_TIME)
                {
                    _disasterTime = 0f;
                    CameraHandler.MoveTo(ActiveDisaster.Center);
                    GameState = GameState.MoveToDisaster;
                }
                break;

            case GameState.MoveToDisaster:
                if (CameraHandler.State == CameraState.Idle)
                {
                    _disasterTime += Time.deltaTime;
                    if (_disasterTime > DISASTER_START_TIME_OFFSET)
                    {
                        ActiveDisaster.CastVisualEffect();
                        ActiveDisaster.ApplyEffect();
                        GameState = GameState.DisasterOccuring;
                        _disasterTime = 0f;
                    }
                }
                break;

            case GameState.DisasterOccuring:
                _disasterTime += Time.deltaTime;
                if(_disasterTime > DISASTER_END_TIME_OFFSET)
                {
                    GameUI.Alert_StopFlash();
                    GameState = GameState.DayCycle;
                }

                break;
        }
    }
    

    #region Game Commands
    

    private Building PlaceBuilding(Tile t, Building b, bool initializing = false)
    {
        Building newBuilding = GameObject.Instantiate(b);
        newBuilding.Tile = t;
        newBuilding.Model = this;
        newBuilding.Initialize(this);
        newBuilding.OnBuild();

        t.Building = newBuilding;
        newBuilding.transform.position = t.transform.position;
        Buildings.Add(newBuilding);

        if (!initializing)
        {
            RemoveGold(newBuilding.BuildCost, newBuilding);
            GameUI.ResourceInfo.UpdatePanel();
        }

        UpdateVisibility();

        return newBuilding;
    }
    public void DestroyBuilding(Building b)
    {
        b.OnDestroyed();
        Destroy(b.gameObject);
        Destroy(b.UILabel.gameObject);
        Buildings.Remove(b);
        b.Tile.Building = null;

        UpdateVisibility();
        GameUI.ResourceInfo.UpdatePanel();
    }
    public void RepairBuilding(Building b)
    {
        b.Repair();
        RemoveGold(b.RepairCost, b);
    }

    public int DealDamage(Tile t, int dmg)
    {
        if (dmg == 0) return 0;
        return t.TakeDamage(dmg);
    }

    public void IncreaseStability(float amount, Building source = null)
    {
        DoChangeInstability(-amount);
        if (amount != 0 && source != null && source.Tile.IsVisible) GameUI.CreateInfoBlob(source.gameObject, amount.ToString(), ColorSettings.UI_Text_Positive, Color.grey);
    }
    public void DecreaseStability(float amount, Building source = null)
    {
        DoChangeInstability(amount);
        if (amount != 0 && source != null && source.Tile.IsVisible) GameUI.CreateInfoBlob(source.gameObject, amount.ToString(), ColorSettings.UI_Text_Negative, Color.grey);
    }

    public void AddGold(int amount, Building source = null)
    {
        DoChangeGoldAmount(amount);
        if (amount != 0 && source != null && source.Tile.IsVisible) GameUI.CreateInfoBlob(source.gameObject, amount.ToString(), ColorSettings.UI_Text_Positive, Color.yellow);
    }
    public void RemoveGold(int amount, Building source = null)
    {
        DoChangeGoldAmount(-amount);
        if (amount != 0 && source != null && source.Tile.IsVisible) GameUI.CreateInfoBlob(source.gameObject, amount.ToString(), ColorSettings.UI_Text_Negative, Color.yellow);
    }

    public void ImproveRelationship(City city, float amount)
    {
        city.ChangeRelationship(amount);
    }
    public void WorsenRelationship(City city, float amount)
    {
        city.ChangeRelationship(-amount);
    }

    public void RevealMap(bool doReveal)
    {
        Map.IsRevealed = doReveal;
        UpdateVisibility();
    }

    #endregion

    #region Private Commands

    private void DoChangeGoldAmount(int amount)
    {
        Money += amount;
        GameUI.BuildPanel.UpdatePanel();
        if (GameUI.BuildingInfo.gameObject.activeSelf) GameUI.BuildingInfo.UpdatePanel();
        GameUI.ResourceInfo.UpdatePanel();
    }

    private void DoChangeInstability(float amount)
    {
        InstabilityLevel += amount;
        InstabilityLevel = Mathf.Clamp(InstabilityLevel, 0, GameSettings.MaxInstability);
        GameUI.UpdateInstabilityPanel();
    }

    private void UpdateVisibility()
    {
        foreach (Tile t in Map.Tiles) t.SetVisible(Map.IsRevealed);
        if (!Map.IsRevealed)
        {
            foreach (Building b in Buildings.Where(x => x.VisiblityRange > 0))
            {
                foreach (Tile t in b.Tile.TilesInRange(b.VisiblityRange)) t.SetVisible(true);
            }
        }
    }

    #endregion

    #region Cycle

    public void EndDay()
    {
        if (GameState != GameState.Idle) return;
        GameState = GameState.DayCycle;
        _lastUpdateCycleTime = 0;
        CurCycleRealTime = 0f;

        CycleDisasters = DisasterHandler.GetDisastersForCycle();

        // Distribute building cycle actions and disasters over the timeframe of the cycle
        foreach (Building b in Buildings) b.CycleActionTime = Random.Range(1, CYCLE_TIME);
        foreach (Disaster d in CycleDisasters) d.SetTime(Cycle, Random.Range(1, CYCLE_TIME));

        GameUI.MenuPanel.UpdatePanel();
    }

    private void StartNewDay()
    {
        GameState = GameState.Idle;
        GameUI.DisableEndDayButton();

        Cycle++;
        CycleTime = 0;

        Sun.transform.rotation = Quaternion.Euler(50, -30, 0);
        Moon.intensity = 0f;

        DayEvent = EventHandler.GetRandomEvent();
        DayEvent.Initialize(this);
        ActiveReport = GameUI.InitAndShowMorningReport();
        ActiveReport.EventTab.UpdateTabColor();

        ResetDailyValues();

        GameUI.MenuPanel.UpdatePanel();
    }

    private void ResetDailyValues()
    {
        InstabilityLevel_DayStart = InstabilityLevel;
        Money_DayStart = Money;
    }

    public void EventHandled()
    {
        DayEvent.EventHandled = true;
        GameUI.EnableEndDayButton();
        ActiveReport.EventTab.UpdateTabColor();
    }

    #endregion

    #region Disaster

    public void ShowDisaster(Disaster d)
    {
        if (d.Center.IsVisible)
        {
            ActiveDisaster = d;
            GameState = GameState.AlertFlash;
            GameUI.Alert_StartFlash();
            _disasterTime = 0f;
        }
        else
        {
            d.ApplyEffect();
        }
    }

    #endregion

    #region Build Mode
    public Building PlannedBuildingPrefab { get; private set; }
    public Building PlannedBuildingObject { get; private set; }

    public void InitBuildMode(Building b)
    {
        if (GameState != GameState.Idle) return;
        GameState = GameState.BuildMode;
        PlannedBuildingPrefab = b;
        PlannedBuildingObject = Instantiate(b);
    }

    public void PlacePlannedBuilding()
    {
        if (GameState != GameState.BuildMode) return;
        if (!PlannedBuildingPrefab.CanBuildOn(HoveredTile)) return;
        Destroy(PlannedBuildingObject.gameObject);
        PlaceBuilding(HoveredTile, PlannedBuildingPrefab);
        CameraHandler.UpdateBounds();
        GameState = GameState.Idle;
    }

    public void CancelBuildMode()
    {
        if (GameState != GameState.BuildMode) return;
        Destroy(PlannedBuildingObject.gameObject);
        GameState = GameState.Idle;
    }

    #endregion

    #region Menu UI

    public void ToggleReport()
    {
        ActiveReport.gameObject.SetActive(!ActiveReport.gameObject.activeSelf);
    }

    #endregion

    #region Initialize New Game
    public void StartNewGame(int mapWidth, int mapHeight)
    {
        // Generate map
        MapData data = MapGenerator.GenerateMap(mapWidth, mapHeight, false);
        Map.InitializeMap(this, data);

        // Init values
        InstabilityLevel = 1;

        // Init other elements
        CameraHandler = new CameraHandler(this);
        InputHandler = new InputHandler(this);
        EventHandler = new GameEventHandler(this);
        DisasterHandler = new DisasterHandler(this);
        MarkovChainWordGenerator.Init();

        // Initialize building attributes
        BPC.City.InitAttributes(this);
        BPC.HQ.InitAttributes(this);
        BPC.Radar.InitAttributes(this);

        // Place initial random cities
        PlaceRandomCities();

        // Choose random starting (next to a city) tile to place Headquarters
        List<Tile> startRadarCandidates = Map.TilesList.Where(x => BPC.HQ.CanBuildOn(x) && x.BuildingsInRange(GameSettings.Headquarter_VisibilityRange, typeof(City)).Count == 1).ToList();
        Tile startingTile = startRadarCandidates[Random.Range(0, startRadarCandidates.Count)];
        PlaceBuilding(startingTile, BPC.HQ, initializing: true);

        // Init visiblie tiles around starting radar
        foreach (Building b in Buildings) b.OnBuild();

        // Set camera position
        CameraHandler.FocusVisibleTiles();

        ResetDailyValues();

        GameUI.Initialize(this);

        GameState = GameState.Idle;
    }

    private void PlaceRandomCities()
    {
        //int tilesPerCity = UnityEngine.Random.Range(MIN_TILES_PER_CITY, MAX_TILES_PER_CITY);
        int numCities = GameSettings.NumCities; // Map.NumTiles / tilesPerCity;
        for (int i = 0; i < numCities; i++)
        {
            List<Tile> candidates = Map.TilesList.Where(x => BPC.City.CanBuildOn(x)).ToList();
            Tile cityTile = candidates[Random.Range(0, candidates.Count)];
            City newCity = (City)PlaceBuilding(cityTile, BPC.City, initializing: true);
        }
    }

    #endregion

    #region Setters
    public void SetHoveredTile(Tile t)
    {
        if (HoveredTile != null) HoveredTile.Unhover();
        HoveredTile = t;
        if (HoveredTile != null) HoveredTile.Hover();
    }

    public void SetSelectedBuilding(Building b)
    {
        if (SelectedBuilding != null)
        {
            SelectedBuilding.Unselect();
            GameUI.BuildingInfo.gameObject.SetActive(false);
        }
        SelectedBuilding = b;
        if (SelectedBuilding != null)
        {
            GameUI.BuildingInfo.gameObject.SetActive(true);
            GameUI.BuildingInfo.SetSelection(this, SelectedBuilding);
            SelectedBuilding.Select();
        }
    }

    #endregion

    #region Getters

    public List<City> Cities
    {
        get
        {
            return Buildings.Where(x => x.GetType() == typeof(City)).Select(x => (City)x).ToList();
        }
    }
    public List<Building> PlayerBuildings
    {
        get
        {
            return Buildings.Where(x => x.GetType() != typeof(City)).ToList();
        }
    }

    public int Day
    {
        get
        {
            return CycleTime > CYCLE_TIME - CYCLE_OFFSET ? Cycle + 1 : Cycle;
        }
    }
    public int Hour
    {
        get
        {
            return (CYCLE_OFFSET / 60 + CycleTime / 60) % 24;
        }
    }
    public int Minute
    {
        get
        {
            return CycleTime % 60;
        }
    }

    public int MoneyPerCycle
    {
        get
        {
            return Buildings.Sum(x => x.MoneyPerCycle);
        }
    }
    public float EmissionsPerCycle
    {
        get
        {
            return Buildings.Sum(x => x.EmissionsPerCycle);
        }
    }

    public int MoneyChangeThisCycle { get { return Money - Money_DayStart; } }
    public float InstabilityChangeThisCycle { get { return InstabilityLevel - InstabilityLevel_DayStart; } }

    #endregion

}
