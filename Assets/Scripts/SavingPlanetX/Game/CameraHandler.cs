using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraHandler
{
    private Map Map;
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

        float cameraX = minX + (maxX - minX) / 2;
        float cameraY = 2 + Mathf.Max(maxX - minX, maxZ - minZ) * 0.9f;
        float cameraZ = minZ + (maxZ - minZ) / 2;
        Camera.main.transform.position = new Vector3(cameraX, cameraY, cameraZ);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);

    }
}
