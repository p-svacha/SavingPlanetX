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

    public GameSettings Settings;
    public ColorSettings ColorSettings;
    

    public GameState GameState = GameState.Initializing;

    public Map Map;
    public List<Building> Buildings = new List<Building>();

    public Tile HoveredTile { get; private set; }
    public Building SelectedBuilding { get; private set; }

    public GameEventHandler EventHandler;
    public DisasterHandler DisasterHandler;
    public List<Disaster> CycleDisasters;
    public Disaster ActiveDisaster;

    // Game values
    public int Money { get; private set; }
    public float StarInstabilityLevel { get; private set; }


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
                    if (d.CycleTime < CycleTime && d.CycleTime >= _lastUpdateCycleTime) ShowDisaster(d);

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
                if ((int)sunRotation.x == 180) foreach (Building_City city in Cities) city.CityLight.intensity = 1f;
                if ((int)sunRotation.x == 360) foreach (Building_City city in Cities) city.CityLight.intensity = 0f;
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
                        ActiveDisaster.State = DisasterState.Occuring;
                        ActiveDisaster.CastVisualEffect();
                        ActiveDisaster.ApplyEffect();
                        GameState = GameState.DisasterOccuring;
                        _disasterTime = 0f;
                    }
                }
                break;

            case GameState.DisasterOccuring:
                if(ActiveDisaster.State != DisasterState.Occuring)
                {
                    _disasterTime += Time.deltaTime;
                    if(_disasterTime > DISASTER_END_TIME_OFFSET)
                    {
                        GameUI.Alert_StopFlash();
                        GameState = GameState.DayCycle;
                    }
                }

                break;
        }
    }
    

    #region Game Commands
    

    private void PlaceBuilding(Tile t, Building b)
    {
        Building newBuilding = GameObject.Instantiate(b);
        newBuilding.Tile = t;
        newBuilding.Model = this;
        newBuilding.Initialize(this);
        newBuilding.OnBuild();

        t.Building = newBuilding;
        newBuilding.transform.position = t.transform.position;
        Buildings.Add(newBuilding);
    }

    public void DestroyBuilding(Building b)
    {
        b.OnDestroyed();
        Destroy(b.gameObject);
        Destroy(b.UILabel.gameObject);
        Buildings.Remove(b);
        b.Tile.Building = null;
    }

    public void DealDamage(Tile t, int dmg)
    {
        t.TakeDamage(dmg);
    }

    public void IncreaseStability(float amount)
    {
        StarInstabilityLevel -= amount;
        if (StarInstabilityLevel < 0) StarInstabilityLevel = 0;
        GameUI.UpdateInstabilityPanel();
    }

    public void DecreaseStability(float amount)
    {
        StarInstabilityLevel += amount;
        GameUI.UpdateInstabilityPanel();
    }

    public void AddGold(int amount)
    {
        Money += amount;
        GameUI.BuildPanel.UpdatePanel();
        if (GameUI.BuildingInfo.gameObject.activeSelf) GameUI.BuildingInfo.UpdatePanel();
        GameUI.ResourceInfo.UpdatePanel();
    }

    public void RevealMap(bool doReveal)
    {
        Map.IsRevealed = doReveal;
        UpdateVisibility();
    }

    #endregion

    #region Visual Commands

    public void UpdateVisibility()
    {
        foreach (Tile t in Map.Tiles) t.UpdateVisibility();
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

    }

    private void StartNewDay()
    {
        Cycle++;
        CycleTime = 0;

        GameState = GameState.EventDialog;
        Sun.transform.rotation = Quaternion.Euler(50, -30, 0);
        Moon.intensity = 0f;

        EventHandler.CastRandomEvent();
    }

    public void EndEvent()
    {
        GameState = GameState.Idle;
    }

    #endregion

    #region Disaster

    public void ShowDisaster(Disaster d)
    {
        if (d.Center.IsVisible())
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

    # endregion

    #region Initialize New Game
    public void StartNewGame(int mapWidth, int mapHeight)
    {
        // Generate map
        MapData data = MapGenerator.GenerateMap(mapWidth, mapHeight, false);
        Map.InitializeMap(this, data);
        foreach (Tile t in Map.TilesList)
            t.IsInFogOfWar = true;

        // Init values
        StarInstabilityLevel = 1;

        // Init other elements
        CameraHandler = new CameraHandler(this);
        InputHandler = new InputHandler(this);
        EventHandler = new GameEventHandler(this);
        DisasterHandler = new DisasterHandler(this);
        MarkovChainWordGenerator.Init();

        // Place initial random cities
        PlaceRandomCities();

        // Choose random starting (next to a city) tile to place Radar
        List<Tile> startRadarCandidates = Map.TilesList.Where(x => BPC.Radar.CanBuildOn(x) && x.IsInRangeOfBuilding(BPC.Radar.Range, typeof(Building_City))).ToList();
        Tile startingTile = startRadarCandidates[Random.Range(0, startRadarCandidates.Count)];
        PlaceBuilding(startingTile, BPC.Radar);

        // Init visiblie tiles around starting radar
        foreach (Building b in Buildings) b.OnBuild();

        // Set camera position
        CameraHandler.FocusVisibleTiles();

        GameUI.Initialize(this);

        GameState = GameState.Idle;
    }

    private void PlaceRandomCities()
    {
        //int tilesPerCity = UnityEngine.Random.Range(MIN_TILES_PER_CITY, MAX_TILES_PER_CITY);
        int numCities = Settings.NumCities; // Map.NumTiles / tilesPerCity;
        for (int i = 0; i < numCities; i++)
        {
            List<Tile> candidates = Map.TilesList.Where(x => BPC.City.CanBuildOn(x)).ToList();
            Tile cityTile = candidates[Random.Range(0, candidates.Count)];
            PlaceBuilding(cityTile, BPC.City);
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
            GameUI.BuildingInfo.SetSelection(SelectedBuilding);
            SelectedBuilding.Select();
        }
    }

    #endregion

    #region Getters

    public List<Building> Cities
    {
        get
        {
            return Buildings.Where(x => x.GetType() == typeof(Building_City)).ToList();
        }
    }
    public List<Building> PlayerBuildings
    {
        get
        {
            return Buildings.Where(x => x.GetType() != typeof(Building_City)).ToList();
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

    #endregion

}
