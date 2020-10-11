using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemCollection : MonoBehaviour
{
    public ParticleSystem Hurricane;

    public static ParticleSystemCollection ParticleSystems
    {
        get
        {
            return GameObject.Find("GameModel").GetComponent<GameModel>().PSC;
        }
    }
}
