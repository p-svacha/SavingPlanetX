using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraHandler
{
    private Map Map;

    private Vector2 TargetPosition;
    private float CameraHeight;

    private float Angle;
    private float TurnSpeed = 0.8f;
    private float OffsetRadius = 4f;

    public CameraHandler(Map map)
    {
        Map = map;
    }

    public void FocusVisibleTiles()
    {
        List<Tile> visibleTiles = Map.GetVisibleTiles();
        float minX = visibleTiles.Min(x => x.transform.position.x);
        float maxX = visibleTiles.Max(x => x.transform.position.x);
        float minZ = visibleTiles.Min(x => x.transform.position.z);
        float maxZ = visibleTiles.Max(x => x.transform.position.z);

        TargetPosition.x = minX + (maxX - minX) / 2;
        CameraHeight = 2 + Mathf.Max(maxX - minX, maxZ - minZ) * 0.9f;
        TargetPosition.y = minZ + (maxZ - minZ) / 2;

        SetPosition();
    }

    public void HandleInput()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            Angle = Angle += TurnSpeed;
            SetPosition();
        }
        if (Input.GetKey(KeyCode.E))
        {
            Angle = Angle -= TurnSpeed;
            SetPosition();
        }
    }

    private void SetPosition()
    {
        float cameraOffsetX = Mathf.Sin(Mathf.Deg2Rad * Angle) * OffsetRadius;
        float cameraOffsetY = Mathf.Cos(Mathf.Deg2Rad * Angle) * OffsetRadius;

        Camera.main.transform.position = new Vector3(TargetPosition.x + cameraOffsetX, CameraHeight, TargetPosition.y + cameraOffsetY);
        Camera.main.transform.LookAt(new Vector3(TargetPosition.x, 0, TargetPosition.y));
    }
}
