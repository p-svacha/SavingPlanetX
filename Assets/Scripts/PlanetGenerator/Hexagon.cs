using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hexagon
{
    public int[] Vertices = new int[6];

    public Hexagon(int[] vertices)
    {
        Vertices = vertices;
    }

    public override bool Equals(object h)
    {
        Hexagon hex = (Hexagon)h;
        return Vertices.OrderBy(a => a).SequenceEqual(hex.Vertices.OrderBy(a => a));
    }

    public override int GetHashCode()
    {
        string s = "";
        foreach (int i in Vertices.OrderBy(a => a)) s += i;
        return int.Parse(s);
    }
}
