using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCollection : MonoBehaviour
{
    public Sprite Building_Radar;
    public Sprite Building_City;
    public Sprite Building_HQ;

    public Sprite[] Relationship = new Sprite[5];
    public Sprite[] Emission = new Sprite[5];

    public static IconCollection Icons
    {
        get
        {
            return GameObject.Find("GameModel").GetComponent<GameModel>().Icons;
        }
    }
}
