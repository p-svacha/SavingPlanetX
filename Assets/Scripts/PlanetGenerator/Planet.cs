﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public bool AutoUpdate;
    public GeneralSettings Settings;
    public ColorSettings ColorSettings;

    public Material DefaultMaterial;

    [HideInInspector]
    public bool GeneralSettingsFoldout;
    [HideInInspector]
    public bool ColorSettingsFoldout;

    public MeshFilter[] DelaunayTrianglesPlane;
    public MeshFilter[] DelaunayTrianglesSphere;

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    private void Initialize()
    {
        if(DelaunayTrianglesPlane.Length != 1274)
        {
            DelaunayTrianglesPlane = new MeshFilter[1274];
            for(int i = 0; i < DelaunayTrianglesPlane.Length; i++)
            {
                GameObject triangleObject = new GameObject("plane_triangle");
                triangleObject.transform.parent = transform;
                MeshRenderer renderer = triangleObject.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = DefaultMaterial;
                MeshFilter filter = triangleObject.AddComponent<MeshFilter>();
                DelaunayTrianglesPlane[i] = filter;
            }
        }

        if (DelaunayTrianglesSphere.Length != 1274)
        {
            DelaunayTrianglesSphere = new MeshFilter[1274];
            for (int i = 0; i < DelaunayTrianglesSphere.Length; i++)
            {
                GameObject triangleObject = new GameObject("sphere_triangle");
                triangleObject.transform.parent = transform;
                MeshRenderer renderer = triangleObject.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = DefaultMaterial;
                MeshFilter filter = triangleObject.AddComponent<MeshFilter>();
                DelaunayTrianglesSphere[i] = filter;
            }
        }
    }

    void GenerateMesh()
    {
        
        PlanetMeshGenerator.CreatePlanetMesh(Settings.PlanetRadius, Settings.Subdivisions);

        foreach (MeshFilter mf in DelaunayTrianglesPlane) mf.sharedMesh = null;
        foreach (MeshFilter mf in DelaunayTrianglesSphere) mf.sharedMesh = null;

        List<Mesh> planeTriangleMeshes = PlanetMeshGenerator.GetPlaneDelaunayTriangles();
        List<Mesh> sphereTriangleMeshes = PlanetMeshGenerator.GetSphereDelaunayTriangles();

        //Debug.Log(planeTriangleMeshes.Count);
        for(int i = 0; i < planeTriangleMeshes.Count; i++)
        {
            DelaunayTrianglesPlane[i].sharedMesh = planeTriangleMeshes[i];
            DelaunayTrianglesSphere[i].sharedMesh = sphereTriangleMeshes[i];
        }
    }

    void GenerateColours()
    {
        foreach(MeshFilter mf in DelaunayTrianglesSphere)
            mf.GetComponent<MeshRenderer>().sharedMaterial.color = ColorSettings.PlanetColor;
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
