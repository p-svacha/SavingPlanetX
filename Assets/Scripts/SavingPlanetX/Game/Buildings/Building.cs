using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public string BuildingName;
    public string BuildingDescription;
    public Sprite BuildingIcon;

    public GameModel Model;
    public GameSettings GameSettings;
    public Tile Tile;

    public Color DefaultColor;
    public Color SelectedColor;

    public int CycleActionTime; // The time that NightAction() triggers during the cycle

    public int MaxHealth;
    public int Health;
    public int VisiblityRange;

    public int MoneyPerCycle;
    public float EmissionsPerCycle;

    public UI_BuildingLabel UILabel_Prefab;
    public UI_BuildingLabel UILabel;

    public int RepairCost { get; protected set; }
    public int BuildCost { get; protected set; }

    public abstract void InitAttributes(GameModel model);

    public abstract bool CanBuildOn(Tile t);

    public virtual void CycleAction()
    {
        Model.DecreaseStability(EmissionsPerCycle, this);
        Model.AddGold(MoneyPerCycle, this);
    }

    public virtual void OnBuild() { }
    public virtual void OnDestroyed() { }
    

    public void Initialize(GameModel model)
    {
        Model = model;
        GameSettings = Model.GameSettings;
        DefaultColor = GetComponentInChildren<Renderer>().material.color;
        SelectedColor = Model.ColorSettings.BuildingSelectedColor;
        InitAttributes(Model);
        Health = MaxHealth;
        UILabel = Instantiate(UILabel_Prefab, Model.GameUI.LabelsPanel);
        UILabel.Init(this);
    }

    /// <summary>
    /// Damages this building. Returns the amount of damage that was actually done.
    /// </summary>
    public int DealDamage(int dmg)
    {
        Health -= dmg;
        if (Health <= 0) Model.DestroyBuilding(this);
        else UILabel.UpdateHealthbar();
        return dmg;
    }

    public void Repair()
    {
        if (Health == MaxHealth) return;
        else Health += 1;
        UILabel.UpdateHealthbar();
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
        return Health < MaxHealth && Model.Money >= RepairCost;
    }

    public bool CanBuild(GameModel model)
    {
        return model.Money >= BuildCost;
    }
}
