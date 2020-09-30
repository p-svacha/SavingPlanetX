using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public string BuildingName;
    public string BuildingDescription;
    public Sprite BuildingIcon;

    public GameModel Model;
    public Tile Tile;

    public Color DefaultColor;
    public Color SelectedColor;

    public int Health;

    public abstract void InitAttributes();

    public abstract bool CanBuildOn(Tile t);
    public abstract void OnEndTurn();
    public abstract void OnBuild();
    

    public void Initialize(GameModel model)
    {
        Model = model;
        DefaultColor = GetComponentInChildren<Renderer>().material.color;
        SelectedColor = Model.ColorManager.BuildingSelectedColor;
        InitAttributes();
    }

    public void Select()
    {
        SetColor(SelectedColor);
    }

    public void Unselect()
    {
        SetColor(DefaultColor);
    }

    public void SetColor(Color c)
    {
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<Renderer>().material.color = c;
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
