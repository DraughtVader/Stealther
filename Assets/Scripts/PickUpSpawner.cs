using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField]
    protected PickUpPrefab[] pickUps;

    [SerializeField]
    protected Vector2 spawnTimeRange = new Vector2(1.0f, 10f);

    [SerializeField]
    protected ParticleSystem revealPfx;

    private DateTime spawnTime, revealPfxTime;
    private bool pickUpSpawned, playedPfx;

    private void SetupSpawnTime()
    {
        revealPfxTime = DateTime.Now.AddSeconds(Random.Range(spawnTimeRange.x, spawnTimeRange.y));
        spawnTime = revealPfxTime.AddSeconds(1f);
        pickUpSpawned = playedPfx = false;
    }

    private void Update()
    {
        if (!pickUpSpawned && GameManager.Instance.GameState == GameManager.State.Playing)
        {
            if (DateTime.Now >= spawnTime)
            {
                pickUpSpawned = true;
                Instantiate(GetNextPickUp().Prefab, transform.position, Quaternion.identity);
            }
            else if (!playedPfx && DateTime.Now >= revealPfxTime)
            {
                playedPfx = true;
                revealPfx.Play();
            }
        }
    }

    private void Start()
    {
        GameManager.Instance.RoundStart += SetupSpawnTime;
    }

    private PickUpPrefab GetNextPickUp()
    {
        return pickUps[Random.Range(0, pickUps.Length)];
    }

    [Serializable]
    public class PickUpPrefab
    {
        public PickUp.Type Type;
        public GameObject Prefab;
    }
}
