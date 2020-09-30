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
    public ColorManager ColorManager;

    public GameSettings Settings;

    public GameState GameState = GameState.Initializing;

    public Map Map;
    public List<Building> Buildings = new List<Building>();

    public Tile HoveredTile { get; private set; }
    public Building SelectedBuilding { get; private set; }

    public GameEventManager EventManager;
    public float StarInstabilityLevel;

    // Visual const
    private float CurDayCycleTime;
    private const float DAY_CYCLE_TIME = 5f;
    private const float MAX_MOON_INTENSITY = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.Initializing;
        StartNewGame(60, 45);
    }

    // Update is called once per frame
    void Update()
    {
        CameraHandler.HandleInput();
        InputHandler.HandleInputs();

        switch (GameState)
        {
            case GameState.Idle:
                foreach (Tile t in Map.TilesList) t.UpdateTile();
                break;

            case GameState.DayCycle:
                CurDayCycleTime += Time.deltaTime;
                if (CurDayCycleTime >= DAY_CYCLE_TIME) StartNewDay();
                else
                {
                    Vector3 sunRotation = Vector3.Lerp(new Vector3(50, -30, 0), new Vector3(410, 330, 0), CurDayCycleTime / DAY_CYCLE_TIME);
                    Sun.transform.rotation = Quaternion.Euler(sunRotation);
                    if (sunRotation.x < 140) Moon.intensity = 0f;
                    else if (sunRotation.x < 220) Moon.intensity = (sunRotation.x - 140) / 80 * MAX_MOON_INTENSITY;
                    else if (sunRotation.x < 320) Moon.intensity = MAX_MOON_INTENSITY;
                    else if(sunRotation.x < 400 ) Moon.intensity = MAX_MOON_INTENSITY - ((sunRotation.x - 320) / 80 * MAX_MOON_INTENSITY);
                    else Moon.intensity = 0f;
                    if ((int)sunRotation.x == 180) foreach (Building_City city in Cities) city.CityLight.intensity = 1f;
                    if ((int)sunRotation.x == 360) foreach (Building_City city in Cities) city.CityLight.intensity = 0f;
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

    public void IncreaseStability(float amount)
    {
        StarInstabilityLevel -= amount;
        if (StarInstabilityLevel < 0) StarInstabilityLevel = 0;
        GameUI.UpdateInstabilityPanel();
    }

    #endregion

    public void EndDay()
    {
        if (GameState != GameState.Idle) return;
        GameState = GameState.DayCycle;
        CurDayCycleTime = 0f;
        foreach (Building b in Buildings) b.OnEndTurn();
    }

    private void StartNewDay()
    {
        GameState = GameState.EventDialog;
        Sun.transform.rotation = Quaternion.Euler(50, -30, 0);
        Moon.intensity = 0f;

        EventManager.CastRandomEvent();
    }

    public void EndEvent()
    {
        GameState = GameState.Idle;
    }

    #region Day Cycle

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
        Map.InitializeMap(data);
        foreach (Tile t in Map.TilesList)
            t.IsInFogOfWar = true;

        // Init values
        StarInstabilityLevel = 1;

        // Init other elements
        CameraHandler = new CameraHandler(this);
        InputHandler = new InputHandler(this);
        EventManager = new GameEventManager(this);
        MarkovChainWordGenerator.Init();

        // Place initial random cities
        PlaceRandomCities();

        // Choose random starting (next to a city) tile to place Radar
        List<Tile> startRadarCandidates = Map.TilesList.Where(x => BPC.Radar.CanBuildOn(x) && x.IsInRangeOfBuilding(BPC.Radar.Range, typeof(Building_City))).ToList();
        Tile startingTile = startRadarCandidates[Random.Range(0, startRadarCandidates.Count)];
        PlaceBuilding(startingTile, BPC.Radar);

        // Init visiblie tiles around starting radar
        foreach (Building b in Buildings) b.OnBuild();
        foreach (Tile t in Map.TilesList) t.UpdateTile();

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
            GameUI.SelectionInfo.gameObject.SetActive(false);
        }
        SelectedBuilding = b;
        if (SelectedBuilding != null)
        {
            GameUI.SelectionInfo.gameObject.SetActive(true);
            GameUI.SelectionInfo.SetSelection(SelectedBuilding);
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

    #endregion

}
