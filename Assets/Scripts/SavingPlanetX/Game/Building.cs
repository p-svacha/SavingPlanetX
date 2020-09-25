using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public GameModel Model;
    public Tile Tile;

    public abstract void OnEndTurn();
    public abstract void OnBuild();

    public void Initialize(GameModel model)
    {
        Model = model;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
