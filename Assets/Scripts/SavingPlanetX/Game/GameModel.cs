using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    public CameraHandler CameraHandler;
    public GameUI GameUI;
    public BuildingPrefabCollection BPC;
    public ColorManager ColorManager;

    public GameSettings Settings;

    public GameState GameState = GameState.Initializing;

    public Map Map;
    public List<Building> Buildings = new List<Building>();

    public Tile HoveredTile;
    public Building SelectedBuilding;

    public float StarInstabilityLevel;

    RaycastHit hit;

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

        // Tile hover
        if (HoveredTile != null) HoveredTile.Unhover();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int tileLayerMask = 1 << 8;
        if (Physics.Raycast(ray, out hit, 100, tileLayerMask))
        {
            HoveredTile = hit.transform.gameObject.GetComponent<Tile>();
            if (HoveredTile == null) HoveredTile = hit.transform.gameObject.GetComponentInParent<Tile>();
            if (HoveredTile != null) HoveredTile.Hover();
        }

        // Building Selection
        if(Input.GetMouseButtonDown(0))
        {
            if (SelectedBuilding != null) SelectedBuilding.Unselect();
            int buildingLayerMask = 1 << 9;
            if (Physics.Raycast(ray, out hit, 100, buildingLayerMask))
            {
                SelectedBuilding = hit.transform.gameObject.GetComponentInParent<Building>();
                if (SelectedBuilding != null) SelectedBuilding.Select();
            }
        }

        if (GameState == GameState.Running)
            foreach (Tile t in Map.TilesList) t.UpdateTile();
    }

    public void EndTurn()
    {
        foreach (Building b in Buildings) b.OnEndTurn();
    }


    #region Initialize New Game
    public void StartNewGame(int mapWidth, int mapHeight)
    {
        // Generate map
        MapData data = MapGenerator.GenerateMap(mapWidth, mapHeight);
        Map.InitializeMap(data);
        foreach (Tile t in Map.TilesList)
            t.IsInFogOfWar = true;

        // Init values
        StarInstabilityLevel = 1;

        // Init other elements
        CameraHandler = new CameraHandler(Map);
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

        GameState = GameState.Running;
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

    public List<Building> Cities
    {
        get
        {
            return Buildings.Where(x => x.GetType() == typeof(Building_City)).ToList();
        }
    }

}
