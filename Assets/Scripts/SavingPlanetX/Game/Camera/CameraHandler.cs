using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraHandler
{
    private Map Map;

    private const float TURN_SPEED = 0.8f;
    private const float MOVE_SPEED = 0.1f;
    private const float ZOOM_SPEED = 0.5f;
    private const float OFFSET_RADIUS_SCALE = 1f; // How far the camera is from the center depending on the height 
    private const float MIN_HEIGHT = 2f;
    private const float MAX_HEIGHT = 20f;

    private Vector2 Center;
    public float MaxHeight;

    private Vector2 TargetPosition;

    private float Angle;
    private float OffsetRadius;

    private float CurrentHeight;
    private float MinHeight = 5f;
    
    

    public CameraHandler(Map map)
    {
        Map = map;
        CurrentHeight = MinHeight;
    }

    public void FocusVisibleTiles()
    {
        UpdateBounds();
        TargetPosition = Center;
        SetPosition();
    }

    public void UpdateBounds()
    {
        List<Tile> visibleTiles = Map.GetVisibleTiles();
        float minX = visibleTiles.Min(x => x.transform.position.x);
        float maxX = visibleTiles.Max(x => x.transform.position.x);
        float minZ = visibleTiles.Min(x => x.transform.position.z);
        float maxZ = visibleTiles.Max(x => x.transform.position.z);

        Center.x = minX + (maxX - minX) / 2;
        Center.y = minZ + (maxZ - minZ) / 2;

        MaxHeight = 2 + Mathf.Max(maxX - minX, (maxZ - minZ) / 16 * 9) * 0.9f;
    }

    public void HandleInput()
    {
        if(Input.GetKey(KeyCode.Q)) // Q - Rotate camera anti-clockwise
        {
            Angle = Angle += TURN_SPEED;
            SetPosition();
        }
        if (Input.GetKey(KeyCode.E)) // E - Rotate camera clockwise
        {
            Angle = Angle -= TURN_SPEED;
            SetPosition();
        }

        if(Input.GetKey(KeyCode.W)) // W - Move camera up
        {
            TargetPosition.x -= MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * Angle);
            TargetPosition.y -= MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * Angle);
            SetPosition();
        }
        if (Input.GetKey(KeyCode.A)) // A - Move camera left
        {
            TargetPosition.x += MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * (Angle + 90));
            TargetPosition.y += MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * (Angle + 90));
            SetPosition();
        }
        if (Input.GetKey(KeyCode.S)) // S - Move camera down
        {
            TargetPosition.x += MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * Angle);
            TargetPosition.y += MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * Angle); ;
            SetPosition();
        }
        if (Input.GetKey(KeyCode.D)) // D - Move camera right
        {
            TargetPosition.x -= MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * (Angle + 90));
            TargetPosition.y -= MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * (Angle + 90));
            SetPosition();
        }

        if(Input.mouseScrollDelta.y < 0) // Scroll down - Zoom out
        {
            CurrentHeight += ZOOM_SPEED;
            SetPosition();
        }
        if (Input.mouseScrollDelta.y > 0) // Scroll up - Zoom in
        {
            CurrentHeight -= ZOOM_SPEED;
            SetPosition();
        }

    }

    private void SetPosition()
    {
        CurrentHeight = Mathf.Clamp(CurrentHeight, MIN_HEIGHT, MAX_HEIGHT);

        OffsetRadius = OFFSET_RADIUS_SCALE * CurrentHeight;

        float cameraOffsetX = Mathf.Sin(Mathf.Deg2Rad * Angle) * OffsetRadius;
        float cameraOffsetY = Mathf.Cos(Mathf.Deg2Rad * Angle) * OffsetRadius;

        Camera.main.transform.position = new Vector3(TargetPosition.x + cameraOffsetX, CurrentHeight, TargetPosition.y + cameraOffsetY);
        Camera.main.transform.LookAt(new Vector3(TargetPosition.x, 0, TargetPosition.y));
    }
}
