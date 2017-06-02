using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    protected Transform[] points;

    [SerializeField]
    protected NinjaSpawner ninjaSpawnerPrefab;

    [SerializeField]
    protected float safetyCastRadius = 1.0f;

    [SerializeField]
    protected LayerMask unsafeLayers;

    public Transform[] Points
    {
        get { return points; }
    }

    public Transform[] SpawnPoints
    {
        get
        {
            points.ShuffleInPlace();
            return points;
        }
    }

    public void SpawnNinja(NinjaController ninja, NinjaState ninjaStateOnSpawn = NinjaState.Alive, bool shieldOnSpawn = false)
    {
        points.ShuffleInPlace();
        Vector2 spawnPoint = points[0].position; 
        int length = points.Length;
        for (int i = 0; i < length; i++)
        {
            if (Physics2D.OverlapCircle(points[i].position, safetyCastRadius, unsafeLayers) != null)
            {
                continue;
            }
            spawnPoint = points[i].position;
            break;
        }
        
        var spawner = Instantiate(ninjaSpawnerPrefab, spawnPoint, Quaternion.identity);
        spawner.SetUp(ninja, ninjaStateOnSpawn);
        if (shieldOnSpawn)
        {
            ninja.StartShield(3.0f);
        }
    }
    
    public void SpawnNinja(NinjaController ninja, int spawnPosition, NinjaState ninjaStateOnSpawn)
    {
        var spawner = Instantiate(ninjaSpawnerPrefab, Points[spawnPosition].position, Quaternion.identity);
        spawner.SetUp(ninja, ninjaStateOnSpawn);
    }
}
