using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    public CameraHandler CameraHandler;
    public BuildingPrefabCollection BPC;

    private const int MIN_TILES_PER_CITY = 200;
    private const int MAX_TILES_PER_CITY = 200;

    public GameState GameState = GameState.Initializing;

    public Map Map;
    public List<Building> Buildings = new List<Building>();

    public int InstabilityLevel;

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.Initializing;
        StartNewGame(60, 45);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState == GameState.Running)
        {
            foreach (Tile t in Map.TilesList) t.UpdateTile();
        }
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
            t.IsInFogOfWar = false;

        // Init other elements
        CameraHandler = new CameraHandler(Map);

        // Place initial random cities
        PlaceRandomCities();

        // Choose random starting tile to place Radar
        List<Tile> startCandidates = Map.TilesList.Where(x => x.Type == TileType.Land).ToList();
        Tile startingTile = startCandidates[Random.Range(0, startCandidates.Count)];
        PlaceBuilding(startingTile, BPC.Radar);

        // Init visiblie tiles around starting radar
        foreach (Building b in Buildings) b.OnBuild();
        foreach (Tile t in Map.TilesList) t.UpdateTile();

        // Set camera position
        CameraHandler.FocusVisibleTiles();

        GameState = GameState.Running;
    }

    private void PlaceRandomCities()
    {
        int tilesPerCity = UnityEngine.Random.Range(MIN_TILES_PER_CITY, MAX_TILES_PER_CITY);
        int numCities = Map.NumTiles / tilesPerCity;
        for (int i = 0; i < numCities; i++)
        {
            List<Tile> candidates = Map.TilesList.Where(x => x.Type == TileType.Land && x.NeighbourTiles.All(n => n != null && n.Building == null)).ToList();
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
        newBuilding.OnBuild();

        t.Building = newBuilding;
        newBuilding.transform.position = t.transform.position;
        Buildings.Add(newBuilding);
    }

}
