using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hurricane : Disaster
{
    private ParticleSystem HurricaneParticles;
    public List<Tile> Path;

    private const float BaseSize = 0.8f;
    private const float SizePerIntensity = 0.2f;
    private const float MoveSpeed = 0.008f;

    // Damage Mode
    // 1 | 1 1 = 2
    // 2 | 2 1 1 = 4
    // 3 | 3 2 1 = 6
    // 4 | 4 3 2 1 = 10
    // 5 | 5 4 3 2 = 14

    public Hurricane(GameModel model, Tile center, int intensity) : base(model, center, intensity)
    {
        Name = "Hurricane " + (model.Day + 1);

        GeneratePath();
    }

    public override void ApplyEffect()
    {
        DayDamage = 0;

        State = DisasterState.Active;
        Center = Path[Day];

        int range = (Intensity + 2) / 2;
        for (int i = 0; i <= range; i++)
        {
            foreach (Tile t in Center.TilesWithDistance(i)) DayDamage += Model.DealDamage(t, Mathf.Max(Intensity - i, 1));
        }
        TotalDamage += DayDamage;
        Day++;

        if(Day >= Path.Count)
        {
            State = DisasterState.Completed;
            HurricaneParticles.Stop();
            GameObject.Destroy(HurricaneParticles, 5);
        }
    }

    public override void CastVisualEffect()
    {
        switch(State)
        {
            case DisasterState.Planned:
                HurricaneParticles = GameObject.Instantiate(ParticleSystemCollection.ParticleSystems.Hurricane, Center.transform);
                float size = BaseSize + (Intensity - 1) * SizePerIntensity;
                HurricaneParticles.transform.localScale = new Vector3(size, size, size);
                break;
        }
    }

    public override void Update()
    {
        if(HurricaneParticles != null) HurricaneParticles.transform.position = Vector3.MoveTowards(HurricaneParticles.transform.position, Center.transform.position, MoveSpeed);
    }

    private void GeneratePath()
    {
        Path = new List<Tile>();
        Path.Add(Center);
        Tile lastPos = Center;
        while(lastPos.Type == TileType.Water)
        {
            List<Tile> candidates = lastPos.NeighbourTiles.Where(x => x!= null && x != lastPos).ToList();
            Path.Add(candidates[Random.Range(0, candidates.Count)]);
            lastPos = Path[Path.Count - 1];
        }
    }
}
