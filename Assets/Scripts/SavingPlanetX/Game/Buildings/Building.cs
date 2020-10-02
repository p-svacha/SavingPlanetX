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

    public int CycleActionTime; // The time that NightAction() triggers during the cycle

    public int MaxHealth;
    public int Health;

    public UI_BuildingLabel UILabel_Prefab;
    public UI_BuildingLabel UILabel;

    public int RepairCost { get; protected set; }
    public int BuildCost { get; protected set; }

    public abstract void InitAttributes();

    public abstract bool CanBuildOn(Tile t);
    public virtual void CycleAction() { }
    public virtual void OnBuild() { }
    public virtual void OnDestroyed() { }
    

    public void Initialize(GameModel model)
    {
        Model = model;
        DefaultColor = GetComponentInChildren<Renderer>().material.color;
        SelectedColor = Model.ColorSettings.BuildingSelectedColor;
        Health = Model.Settings.BuildingHealth;
        MaxHealth = Model.Settings.BuildingHealth;
        UILabel = Instantiate(UILabel_Prefab, Model.GameUI.LabelsPanel);
        UILabel.Init(this);
        InitAttributes();
    }

    public void DealDamage(int dmg)
    {
        Health -= dmg;
        if (Health <= 0) Model.DestroyBuilding(this);
        else UILabel.UpdateHealthbar();
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
        for (int i = 0; i < transform.childCount; i++) if(transform.GetChild(i).GetComponent<Renderer>() != null) transform.GetChild(i).GetComponent<Renderer>().material.color = c;
    }

    public bool CanRepair()
    {
        return Model.Money >= RepairCost;
    }

    public bool CanBuild(GameModel model)
    {
        return model.Money >= BuildCost;
    }
}
