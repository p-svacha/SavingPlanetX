using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public bool AutoUpdate;

    public PlanetGeneratorSettings Settings;
    public ColorSettings ColorSettings;

    [HideInInspector]
    public bool GeneralSettingsFoldout;
    [HideInInspector]
    public bool ColorSettingsFoldout;

    [HideInInspector]
    public MeshFilter PlanetMesh;

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    private void Initialize()
    {
        if (PlanetMesh == null)
        {
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;
            PlanetMesh = meshObj.AddComponent<MeshFilter>();
        }
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, Settings.PlanetRadius), new Vector3(Settings.PlanetRadius, 0, 0), new Vector3(Settings.PlanetRadius, 0, Settings.PlanetRadius) };
        mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        PlanetMesh.sharedMesh = mesh;
    }

    void GenerateColours()
    {
        PlanetMesh.GetComponent<MeshRenderer>().sharedMaterial.color = ColorSettings.PlanetColor;
    }

    public void OnColorSettingsUpdated()
    {
        if (AutoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }
    public void OnGeneralSettingsUpdated()
    {
        if (AutoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }
}
