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

    [SerializeField]
    protected float recuringCoolDown = 5.0f;

    private DateTime spawnTime, revealPfxTime, recuringCoolDownTime;
    private bool pickUpSpawned, playedPfx, pickupActive;
    private bool recuring;
    

    public void PickUpUsed(PickUp pickup)
    {
        pickupActive = false;
    }

    private void SetupSpawnTime()
    {
        recuring = GameManager.Instance.CurrentGameMode == GameMode.Deathmatch;
        revealPfxTime = DateTime.Now.AddSeconds(Random.Range(spawnTimeRange.x, spawnTimeRange.y) + (recuring ? recuringCoolDown : 0));
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
                var pickup = Instantiate(GetNextPickUp().Prefab, transform.position, Quaternion.identity);
                pickup.GetComponent<PickUp>().AssignSpawner(this);
                pickupActive = true;
            }
            else if (!playedPfx && DateTime.Now >= revealPfxTime)
            {
                playedPfx = true;
                revealPfx.Play();
            }
            return;
        }
        if (recuring && pickUpSpawned && !pickupActive)
        {
            SetupSpawnTime();
        }
    }

    private void Awake()
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
