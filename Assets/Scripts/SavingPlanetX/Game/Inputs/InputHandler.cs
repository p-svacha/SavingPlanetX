using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler
{
    public GameModel Model;

    private Ray ray;
    private RaycastHit hit;

    private int TileLayerMask = 1 << 8;
    private int BuildingLayerMask = 1 << 9;

    public InputHandler(GameModel model)
    {
        Model = model;
    }

    public void HandleInputs()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HandleKeyInputs();
        HandleMouseInputs();
    }

    private void HandleKeyInputs()
    {
        switch(Model.GameState)
        {
            case GameState.Idle:
                if(Input.GetKeyDown(KeyCode.Alpha1)) // 1 - Build Radar
                {
                    Model.InitBuildMode(Model.BPC.Radar);
                }
                if(Input.GetKeyDown(KeyCode.R)) // R - Reveal / Unreveal map
                {
                    Model.RevealMap(!Model.Map.IsRevealed);
                }
                break;
        }
    }

    private void HandleMouseInputs()
    {
        HandleMouseMove();
        if (Input.GetMouseButtonDown(0)) HandleLeftClick();
        if (Input.GetMouseButtonDown(1)) HandleRightClick();
    }

    private void HandleLeftClick()
    {
        switch (Model.GameState)
        {
            case GameState.Idle: // Select building
                Building selectedBuilding = null;
                if (Physics.Raycast(ray, out hit, 100, BuildingLayerMask))
                    selectedBuilding = hit.transform.gameObject.GetComponentInParent<Building>();
                Model.SetSelectedBuilding(selectedBuilding);
                break;

            case GameState.BuildMode: // Place building
                Model.PlacePlannedBuilding();
                break;
        }
    }

    private void HandleRightClick()
    {
        switch(Model.GameState)
        {
            case GameState.BuildMode: // Cancel build mode
                Model.CancelBuildMode();
                break;
        }
    }

    private void HandleMouseMove()
    {
        // Always update hovered Tile
        Tile hoveredTile = null;
        if (Physics.Raycast(ray, out hit, 100, TileLayerMask))
        {
            hoveredTile = hit.transform.gameObject.GetComponent<Tile>();
            if (hoveredTile == null) hoveredTile = hit.transform.gameObject.GetComponentInParent<Tile>();
        }
        Model.SetHoveredTile(hoveredTile);

        switch(Model.GameState)
        {
            case GameState.BuildMode: // Update position of planned building
                if (hoveredTile != null)
                {
                    Model.PlannedBuildingObject.transform.position = hoveredTile.transform.position;
                    if (Model.PlannedBuildingObject.CanBuildOn(hoveredTile))
                        Model.PlannedBuildingObject.SetColor(Color.green);
                    else 
                        Model.PlannedBuildingObject.SetColor(Color.red);
                }
                break;
        }
    }

}
