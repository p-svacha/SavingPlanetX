using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameModel Model;

    public int WidthTiles;
    public int HeightTiles;
    public int NumTiles;

    public Tile[,] Tiles;
    public List<Tile> TilesList = new List<Tile>();

    public bool IsRevealed;

    // Start is called before the first frame update
    public void InitializeMap(GameModel model, MapData data)
    {
        Model = model;

        WidthTiles = data.MapWidthTiles;
        HeightTiles = data.MapHeightTiles;
        NumTiles = data.NumTiles;

        Tiles = new Tile[WidthTiles, HeightTiles];

        DrawTiles(data.Tiles);

        for (int y = 0; y < HeightTiles; y++)
            for (int x = 0; x < WidthTiles; x++)
                Tiles[x, y].SetNeighbours();

        foreach (Tile t in Tiles) TilesList.Add(t);
    }



    public void DrawTiles(TileData[,] tileData)
    {
        foreach(TileData td in tileData)
            Tiles[td.X, td.Y] = td.DrawTile(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Getters

    public List<Tile> GetVisibleTiles()
    {
        return TilesList.Where(x => !x.IsInFogOfWar).ToList();
    }

    public List<Tile> LandTiles
    {
        get
        {
            return TilesList.Where(x => x.Type == TileType.Land).ToList();
        }
    }

    #endregion
}
