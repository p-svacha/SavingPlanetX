using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.VFX;

public class PlanetMeshGenerator : MonoBehaviour
{
    private static List<Polygon> m_Polygons = new List<Polygon>();
    private static List<Vector3> m_SphereVertices = new List<Vector3>();

    private static List<Vector2> m_VerticesDelaunayPlane = new List<Vector2>();
    private static List<Vector3> m_VerticesDelaunaySphere = new List<Vector3>();

    private static int[] m_DelaunayTriangles;

    private static int i_NorthPole;

    public static Mesh GetIcosahedron(float radius, int n_subdivisions)
    {
        m_Polygons = new List<Polygon>();
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
        m_Polygons.Add(new Polygon(0, 11, 5));
        m_Polygons.Add(new Polygon(0, 5, 1));
        m_Polygons.Add(new Polygon(0, 1, 7));
        m_Polygons.Add(new Polygon(0, 7, 10));
        m_Polygons.Add(new Polygon(0, 10, 11));
        m_Polygons.Add(new Polygon(1, 5, 9));
        m_Polygons.Add(new Polygon(5, 11, 4));
        m_Polygons.Add(new Polygon(11, 10, 2));
        m_Polygons.Add(new Polygon(10, 7, 6));
        m_Polygons.Add(new Polygon(7, 1, 8));
        m_Polygons.Add(new Polygon(3, 9, 4));
        m_Polygons.Add(new Polygon(3, 4, 2));
        m_Polygons.Add(new Polygon(3, 2, 6));
        m_Polygons.Add(new Polygon(3, 6, 8));
        m_Polygons.Add(new Polygon(3, 8, 9));
        m_Polygons.Add(new Polygon(4, 9, 5));
        m_Polygons.Add(new Polygon(2, 4, 11));
        m_Polygons.Add(new Polygon(6, 2, 10));
        m_Polygons.Add(new Polygon(8, 6, 7));
        m_Polygons.Add(new Polygon(9, 8, 1));

        for (int i = 0; i < n_subdivisions; i++) Subdivide();

        StereographicProjectionToPlane();
        DelaunayTriangulatePoints();
        StereographicProjectionToSphere();
             
        Mesh mesh = new Mesh();
        mesh.vertices = m_SphereVertices.ToArray();
        List<int> triangles = new List<int>();
        foreach (Polygon p in m_Polygons) triangles.AddRange(p.m_Vertices);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    public static List<Mesh> GetPlaneDelaunayTriangles()
    {
        List<Mesh> meshes = new List<Mesh>();

        for(int i = 0; i < m_DelaunayTriangles.Length; i+=3)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(m_VerticesDelaunayPlane[m_DelaunayTriangles[i]].x, -10, m_VerticesDelaunayPlane[m_DelaunayTriangles[i]].y),
                new Vector3(m_VerticesDelaunayPlane[m_DelaunayTriangles[i+1]].x, -10, m_VerticesDelaunayPlane[m_DelaunayTriangles[i+1]].y),
                new Vector3(m_VerticesDelaunayPlane[m_DelaunayTriangles[i+2]].x, -10, m_VerticesDelaunayPlane[m_DelaunayTriangles[i+2]].y)
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

        for (int i = 0; i < m_DelaunayTriangles.Length; i += 3)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(m_VerticesDelaunaySphere[m_DelaunayTriangles[i]].x, m_VerticesDelaunaySphere[m_DelaunayTriangles[i]].y, m_VerticesDelaunaySphere[m_DelaunayTriangles[i]].z),
                new Vector3(m_VerticesDelaunaySphere[m_DelaunayTriangles[i+1]].x, m_VerticesDelaunaySphere[m_DelaunayTriangles[i+1]].y, m_VerticesDelaunaySphere[m_DelaunayTriangles[i+1]].z),
                new Vector3(m_VerticesDelaunaySphere[m_DelaunayTriangles[i+2]].x, m_VerticesDelaunaySphere[m_DelaunayTriangles[i+2]].y, m_VerticesDelaunaySphere[m_DelaunayTriangles[i+2]].z)
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

        var newPolys = new List<Polygon>();
        foreach (var poly in m_Polygons)
        {
            int a = poly.m_Vertices[0];
            int b = poly.m_Vertices[1];
            int c = poly.m_Vertices[2];
            // Use GetMidPointIndex to either create a
            // new vertex between two old vertices, or
            // find the one that was already created.
            int ab = GetMidPointIndex(midPointCache, a, b);
            int bc = GetMidPointIndex(midPointCache, b, c);
            int ca = GetMidPointIndex(midPointCache, c, a);
            // Create the four new polygons using our original
            // three vertices, and the three new midpoints.
            newPolys.Add(new Polygon(a, ab, ca));
            newPolys.Add(new Polygon(b, bc, ab));
            newPolys.Add(new Polygon(c, ca, bc));
            newPolys.Add(new Polygon(ab, bc, ca));
        }
        // Replace all our old polygons with the new set of
        // subdivided ones.
        m_Polygons = newPolys;
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

    private static void StereographicProjectionToPlane()
    {
        m_VerticesDelaunayPlane.Clear();

        for (int i = 0; i < m_SphereVertices.Count; i++)
        {
            Vector3 v = m_SphereVertices[i];
            if (v.y != 1) m_VerticesDelaunayPlane.Add(new Vector2(v.x / (1 - v.y), v.z / (1 - v.y)));
            else i_NorthPole = i; // save index for north pole vertex so we can fill the hole later
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

        // Fill north pole hole
        int newNorthPoleIndex = m_VerticesDelaunaySphere.Count;
        m_VerticesDelaunaySphere.Add(new Vector3(0, 1, 0));
        // Find vertices that need to be connected to north pole
        List<int[]> connectIds = new List<int[]>();
        foreach(Polygon p in m_Polygons)
            if (p.m_Vertices.Any(x => x == i_NorthPole)) connectIds.Add(p.m_Vertices.Where(x => x != i_NorthPole).ToArray());


    }

    private static void DelaunayTriangulatePoints()
    {
        List<IPoint> delaunatorPoints = new List<IPoint>();
        foreach (Vector2 v in m_VerticesDelaunayPlane) delaunatorPoints.Add(new Point(v.x, v.y));
        Delaunator del = new Delaunator(delaunatorPoints.ToArray());
        m_DelaunayTriangles = del.Triangles;

    }
}

