using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCollection : MonoBehaviour
{
    public Texture2D DamagedTexture;

    public static MaterialCollection Materials
    {
        get
        {
            return GameObject.Find("GameModel").GetComponent<GameModel>().MaterialCollection;
        }
    }
}
