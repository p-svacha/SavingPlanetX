using DelaunatorSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.VFX;

public class PlanetMeshGenerator : MonoBehaviour
{
    // Icosahedron
    private static List<Triangle> m_Triangles = new List<Triangle>();
    private static List<Vector3> m_SphereVertices = new List<Vector3>();

    // Stereographic projection & Delaunay triangulation
    private static List<Vector2> m_VerticesDelaunayPlane = new List<Vector2>();
    private static List<Vector3> m_VerticesDelaunaySphere = new List<Vector3>();
    private static int[] m_DelaunayTrianglesPlane;
    private static int[] m_DelaunayTrianglesSphere;
    private static Dictionary<int, HashSet<int>> d_VertexNeighbours = new Dictionary<int, HashSet<int>>(); // Saves for each vertex to which other vertices it is connected to
    private static Dictionary<Triangle, HashSet<Triangle>> d_TriangleNeighbours = new Dictionary<Triangle, HashSet<Triangle>>(); // Saves what triangles are neighbours to what

    // Hexagonization
    private static Dictionary<int, List<Triangle>> d_VertexTriangles = new Dictionary<int, List<Triangle>>(); // Saves for each vertex to which polygons it is connected to
    private static List<Hexagon> m_Hexagons = new List<Hexagon>();
    private static List<Vector3> m_HexagonVertices = new List<Vector3>();

    public static void CreatePlanetMesh(float radius, int n_subdivisions)
    {
        m_Triangles = new List<Triangle>();
        m_SphereVertices = new List<Vector3>();

        // An icosahedron has 12 vertices, and
        // since it's completely symmetrical the
        // formula for calculating them is kind of
        // symmetrical too:

        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        m_SphereVertices.Add(new Vector3(-1, t, 0).normalized);
        m_SphereVertices.Add(new Vector3(1, t, 0).normalized);
        m_SphereVertices.Add(new Vector3(-1, -t, 0).normalized);
        m_SphereVertices.Add(new Vector3(1, -t, 0).normalized);
        m_SphereVertices.Add(new Vector3(0, -1, t).normalized);
        m_SphereVertices.Add(new Vector3(0, 1, t).normalized);
        m_SphereVertices.Add(new Vector3(0, -1, -t).normalized);
        m_SphereVertices.Add(new Vector3(0, 1, -t).normalized);
        m_SphereVertices.Add(new Vector3(t, 0, -1).normalized);
        m_SphereVertices.Add(new Vector3(t, 0, 1).normalized);
        m_SphereVertices.Add(new Vector3(-t, 0, -1).normalized);
        m_SphereVertices.Add(new Vector3(-t, 0, 1).normalized);

        // And here's the formula for the 20 sides,
        // referencing the 12 vertices we just created.
        m_Triangles.Add(new Triangle(0, 11, 5));
        m_Triangles.Add(new Triangle(0, 5, 1));
        m_Triangles.Add(new Triangle(0, 1, 7));
        m_Triangles.Add(new Triangle(0, 7, 10));
        m_Triangles.Add(new Triangle(0, 10, 11));
        m_Triangles.Add(new Triangle(1, 5, 9));
        m_Triangles.Add(new Triangle(5, 11, 4));
        m_Triangles.Add(new Triangle(11, 10, 2));
        m_Triangles.Add(new Triangle(10, 7, 6));
        m_Triangles.Add(new Triangle(7, 1, 8));
        m_Triangles.Add(new Triangle(3, 9, 4));
        m_Triangles.Add(new Triangle(3, 4, 2));
        m_Triangles.Add(new Triangle(3, 2, 6));
        m_Triangles.Add(new Triangle(3, 6, 8));
        m_Triangles.Add(new Triangle(3, 8, 9));
        m_Triangles.Add(new Triangle(4, 9, 5));
        m_Triangles.Add(new Triangle(2, 4, 11));
        m_Triangles.Add(new Triangle(6, 2, 10));
        m_Triangles.Add(new Triangle(8, 6, 7));
        m_Triangles.Add(new Triangle(9, 8, 1));

        for (int i = 0; i < n_subdivisions; i++) Subdivide();
        FindTriangleNeighbours();
        FindHexagons();

        /*
        StereographicProjectionToPlane();
        DelaunayTriangulatePoints();
        StereographicProjectionToSphere();
        */
        
        
    }

    public static List<Mesh> GetIcosahedronTriangles()
    {
        List<Mesh> meshes = new List<Mesh>();

        foreach(Triangle t in m_Triangles)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(m_SphereVertices[t.a].x, m_SphereVertices[t.a].y, m_SphereVertices[t.a].z),
                new Vector3(m_SphereVertices[t.b].x, m_SphereVertices[t.b].y, m_SphereVertices[t.b].z),
                new Vector3(m_SphereVertices[t.c].x, m_SphereVertices[t.c].y, m_SphereVertices[t.c].z)
                };
            mesh.triangles = new int[] { 0, 1, 2 };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            meshes.Add(mesh);
        }
        return meshes;
    }

    public static List<Mesh> GetIcosahedronHexagons()
    {
        List<Mesh> meshes = new List<Mesh>();

        foreach (Hexagon hex in m_Hexagons)
        {
            if (hex.Vertices.Length == 6)
            {
                Mesh mesh = new Mesh();
            
                mesh.vertices = new Vector3[]
                {
                m_HexagonVertices[hex.Vertices[0]],
                m_HexagonVertices[hex.Vertices[1]],
                m_HexagonVertices[hex.Vertices[2]],
                m_HexagonVertices[hex.Vertices[3]],
                m_HexagonVertices[hex.Vertices[4]],
                m_HexagonVertices[hex.Vertices[5]],
                };
                mesh.triangles = new int[] {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5
            };
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                meshes.Add(mesh);
            }
        }
        return meshes;
    }

    public static List<Mesh> GetPlaneDelaunayTriangles()
    {
        List<Mesh> meshes = new List<Mesh>();

        for(int i = 0; i < m_DelaunayTrianglesPlane.Length; i+=3)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(m_VerticesDelaunayPlane[m_DelaunayTrianglesPlane[i]].x, -10, m_VerticesDelaunayPlane[m_DelaunayTrianglesPlane[i]].y),
                new Vector3(m_VerticesDelaunayPlane[m_DelaunayTrianglesPlane[i+1]].x, -10, m_VerticesDelaunayPlane[m_DelaunayTrianglesPlane[i+1]].y),
                new Vector3(m_VerticesDelaunayPlane[m_DelaunayTrianglesPlane[i+2]].x, -10, m_VerticesDelaunayPlane[m_DelaunayTrianglesPlane[i+2]].y)
                };
            mesh.triangles = new int[] { 0, 1, 2 };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            meshes.Add(mesh);
        }
        return meshes;
    }

    public static List<Mesh> GetSphereDelaunayTriangles()
    {
        List<Mesh> meshes = new List<Mesh>();
        for (int i = 0; i < m_DelaunayTrianglesSphere.Length; i += 3)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i]].x, m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i]].y, m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i]].z),
                new Vector3(m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i+1]].x, m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i+1]].y, m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i+1]].z),
                new Vector3(m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i+2]].x, m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i+2]].y, m_VerticesDelaunaySphere[m_DelaunayTrianglesSphere[i+2]].z)
                };
            mesh.triangles = new int[] { 0, 2, 1 };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            meshes.Add(mesh);
        }
        return meshes;
    }

    private static void Subdivide()
    {
        var midPointCache = new Dictionary<int, int>();

        var newTriangles = new List<Triangle>();
        foreach (var tri in m_Triangles)
        {
            // Use GetMidPointIndex to either create a
            // new vertex between two old vertices, or
            // find the one that was already created.
            int ab = GetMidPointIndex(midPointCache, tri.a, tri.b);
            int bc = GetMidPointIndex(midPointCache, tri.b, tri.c);
            int ca = GetMidPointIndex(midPointCache, tri.c, tri.a);
            // Create the four new polygons using our original
            // three vertices, and the three new midpoints.
            newTriangles.Add(new Triangle(tri.a, ab, ca));
            newTriangles.Add(new Triangle(tri.b, bc, ab));
            newTriangles.Add(new Triangle(tri.c, ca, bc));
            newTriangles.Add(new Triangle(ab, bc, ca));
        }
        // Replace all our old polygons with the new set of
        // subdivided ones.
        m_Triangles = newTriangles;
    }
    private static int GetMidPointIndex(Dictionary<int, int> cache, int indexA, int indexB)
    {
        // We create a key out of the two original indices
        // by storing the smaller index in the upper two bytes
        // of an integer, and the larger index in the lower two
        // bytes. By sorting them according to whichever is smaller
        // we ensure that this function returns the same result
        // whether you call
        // GetMidPointIndex(cache, 5, 9)
        // or...
        // GetMidPointIndex(cache, 9, 5)
        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << 16) + greaterIndex;
        // If a midpoint is already defined, just return it.
        int ret;
        if (cache.TryGetValue(key, out ret))
            return ret;
        // If we're here, it's because a midpoint for these two
        // vertices hasn't been created yet. Let's do that now!
        Vector3 p1 = m_SphereVertices[indexA];
        Vector3 p2 = m_SphereVertices[indexB];
        Vector3 middle = Vector3.Lerp(p1, p2, 0.5f).normalized;

        ret = m_SphereVertices.Count;
        m_SphereVertices.Add(middle);

        cache.Add(key, ret);
        return ret;
    }

    private static void FindHexagons()
    {
        m_Hexagons.Clear();
        m_HexagonVertices.Clear();

        for(int i = 0; i < m_Triangles.Count; i++)
        {
            m_Triangles[i].index = i;
            m_HexagonVertices.Add(GetTriangleCenter(m_Triangles[i]));
        }

        for(int i = 0; i < m_SphereVertices.Count; i++)
        {
            Vector3 v = m_SphereVertices[i];
            List<int> hexVertices = new List<int>();
            foreach(Triangle nt in d_VertexTriangles[i])
            {
                hexVertices.Add(nt.index);
            }
            Hexagon newHex = new Hexagon(hexVertices.ToArray());
            if (!m_Hexagons.Contains(newHex)) m_Hexagons.Add(newHex);
        }
    }

    private static void StereographicProjectionToPlane()
    {
        m_VerticesDelaunayPlane.Clear();

        for (int i = 0; i < m_SphereVertices.Count; i++)
        {
            Vector3 v = m_SphereVertices[i];
            if (v.y != 1) m_VerticesDelaunayPlane.Add(new Vector2(v.x / (1 - v.y), v.z / (1 - v.y)));
        }
    }

    private static void StereographicProjectionToSphere()
    {
        m_VerticesDelaunaySphere.Clear();

        foreach (Vector2 v in m_VerticesDelaunayPlane)
            m_VerticesDelaunaySphere.Add(new Vector3(
                (2 * v.x) / (1 + v.x * v.x + v.y * v.y),
                (-1 + v.x * v.x + v.y * v.y) / (1 + v.x * v.x + v.y * v.y),
                (2 * v.y) / (1 + v.x * v.x + v.y * v.y)));       
    }

    private static void DelaunayTriangulatePoints()
    {
        List<IPoint> delaunatorPoints = new List<IPoint>();
        foreach (Vector2 v in m_VerticesDelaunayPlane) delaunatorPoints.Add(new Point(v.x, v.y));
        Delaunator del = new Delaunator(delaunatorPoints.ToArray());
        m_DelaunayTrianglesPlane = del.Triangles;
        m_DelaunayTrianglesSphere = del.Triangles;
    }

    private static void FindTriangleNeighbours()
    {
        d_VertexTriangles.Clear();
        d_VertexNeighbours.Clear();
        d_TriangleNeighbours.Clear();

        // Create triangle list for each vertex
        foreach (Triangle triangle in m_Triangles)
        {
            // Add current triangle to each corner
            if (!d_VertexTriangles.ContainsKey(triangle.a)) d_VertexTriangles.Add(triangle.a, new List<Triangle>() { triangle });
            else d_VertexTriangles[triangle.a].Add(triangle);

            if (!d_VertexTriangles.ContainsKey(triangle.b)) d_VertexTriangles.Add(triangle.b, new List<Triangle>() { triangle });
            else d_VertexTriangles[triangle.b].Add(triangle);

            if (!d_VertexTriangles.ContainsKey(triangle.c)) d_VertexTriangles.Add(triangle.c, new List<Triangle>() { triangle });
            else d_VertexTriangles[triangle.c].Add(triangle);

            // Add other triangle points as neighbours of each vertex
            if (!d_VertexNeighbours.ContainsKey(triangle.a)) d_VertexNeighbours.Add(triangle.a, new HashSet<int>());
            if (!d_VertexNeighbours.ContainsKey(triangle.b)) d_VertexNeighbours.Add(triangle.b, new HashSet<int>());
            if (!d_VertexNeighbours.ContainsKey(triangle.c)) d_VertexNeighbours.Add(triangle.c, new HashSet<int>());

            d_VertexNeighbours[triangle.a].Add(triangle.b);
            d_VertexNeighbours[triangle.a].Add(triangle.c);
            d_VertexNeighbours[triangle.b].Add(triangle.a);
            d_VertexNeighbours[triangle.b].Add(triangle.c);
            d_VertexNeighbours[triangle.c].Add(triangle.a);
            d_VertexNeighbours[triangle.c].Add(triangle.b);
        }

        /*
        // Fill north pole hole by creating new north pole and connecting it to vertices with a missing triangle
        int northPoleIndex = m_VerticesDelaunaySphere.Count;
        m_VerticesDelaunaySphere.Add(new Vector3(0, 1, 0));

        List<int> northPoleConnectionVertices = new List<int>();
        foreach(int i in d_VertexTriangles.Keys)
        {
            if (d_VertexNeighbours[i].Count > d_VertexTriangles[i].Count)
                northPoleConnectionVertices.Add(i);
        }

        d_VertexNeighbours.Add(northPoleIndex, new HashSet<int>(northPoleConnectionVertices));
        d_VertexTriangles.Add(northPoleIndex, new List<Triangle>());
        
        List<int> newTriangles = new List<int>();
        List<int> skippedIndex = new List<int>();
        foreach(int i in northPoleConnectionVertices)
        {
            d_VertexNeighbours[i].Add(northPoleIndex);
            if (!skippedIndex.Contains(i))
            {
                foreach (int j in northPoleConnectionVertices)
                {
                    if (d_VertexNeighbours[i].Contains(j))
                    {
                        newTriangles.Add(northPoleIndex);
                        float angle_i = Vector2.SignedAngle(new Vector2(m_VerticesDelaunaySphere[i].x, m_VerticesDelaunaySphere[i].z), new Vector2(0, 1));
                        float angle_j = Vector2.SignedAngle(new Vector2(m_VerticesDelaunaySphere[j].x, m_VerticesDelaunaySphere[j].z), new Vector2(0, 1));
                        Triangle newTriangle = null;
                        if (angle_i < angle_j || (angle_i > 90 && angle_j < -90))
                        {
                            newTriangles.Add(j);
                            newTriangles.Add(i);
                            newTriangle = new Triangle(northPoleIndex, j, i);
                        }
                        else
                        {
                            newTriangles.Add(i);
                            newTriangles.Add(j);
                            newTriangle = new Triangle(northPoleIndex, i, j);
                        }

                        m_Triangles.Add(newTriangle);
                        d_VertexTriangles[northPoleIndex].Add(newTriangle);
                        d_VertexTriangles[i].Add(newTriangle);
                        d_VertexTriangles[j].Add(newTriangle);

                        skippedIndex.Add(j);
                    }
                }
            }
        }
        m_DelaunayTrianglesSphere = m_DelaunayTrianglesSphere.Concat(newTriangles).ToArray();

        // Find triangle neighbours
        foreach (Triangle triangle in m_Triangles)
        {
            d_TriangleNeighbours.Add(triangle, new HashSet<Triangle>());
            foreach(Triangle vertexTriangle in d_VertexTriangles[triangle.a])
            {
                if (vertexTriangle != triangle) d_TriangleNeighbours[triangle].Add(vertexTriangle);
            }
        }
        */
    }

    public static Vector3 GetTriangleCenter(Triangle t)
    {
        return Vector3.Lerp(m_SphereVertices[t.a], Vector3.Lerp(m_SphereVertices[t.b], m_SphereVertices[t.c], 0.5f), 0.5f);
    }


}

