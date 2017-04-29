using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using InControl;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        PlayerScreen, Playing, RoundScore, MatchScore
    }

    public static GameManager Instance;

    public Action RoundStart, RoundEnd, MatchFinished;

    [SerializeField]
    protected int targerWins = 5;

    [SerializeField]
    protected GameObject splatterPfx;

    [SerializeField]
    protected NinjaBank ninjaBank;

    [SerializeField]
    protected SpawnManager spawnManager;

    [SerializeField]
    protected SlowMoController slowMoController;

    protected Dictionary<NinjaController, int> competingNinjas = new Dictionary<NinjaController, int>();
    protected List<NinjaController> aliveNinjas;
    private NinjaDescription[] ninjaDescriptions;

    public State GameState { get; set; }

    public void NinjaKilled(NinjaController ninja, Vector3 deathPosition)
    {
        aliveNinjas.Remove(ninja);
        if (aliveNinjas.Count == 1)
        {
            AddScore(aliveNinjas[0]);
        }

        var colliders = Physics2D.OverlapCircleAll(deathPosition, 2.0f);
        int length = colliders.Length,
                     count = 0;
        var splatter = Instantiate(splatterPfx, deathPosition, Quaternion.identity);
        var pfx = splatter.GetComponent<ParticleSystem>();
        for (var i = 0; i < length; i++)
        {
            if (colliders[i].GetComponent<Bloodable>())
            {
                pfx.trigger.SetCollider(count++, colliders[i]);
            }
        }
    }

    public void AddPlayer(NinjaController ninja)
    {
        competingNinjas.Add(ninja, 0);
        ninja.Description = ninjaDescriptions[competingNinjas.Count - 1];
        GameUiManager.Instance.AddPlayer(ninja);
    }

    public void AddScore(NinjaController ninja)
    {
        competingNinjas[ninja]++;
        if (competingNinjas[ninja] >= targerWins)
        {
            GameUiManager.Instance.DisplayFinalScores(competingNinjas);
            GameState = State.MatchScore;
        }
        else
        {
            GameUiManager.Instance.DisplayScores(competingNinjas);
            GameState = State.RoundScore;
        }
        if (RoundEnd != null)
        {
            RoundEnd();
        }
    }

    public NinjaController GetClosestNinja(Vector3 position)
    {
        if (aliveNinjas.Count <= 1)
        {
            return null;
        }

        int clostestIndex = 0,
            length = aliveNinjas.Count;
        var closestDistance = Vector2.Distance(position, aliveNinjas[0].transform.position);

        for (var i = 1; i < length; i++)
        {
            var currentDistance = Vector2.Distance(position, aliveNinjas[i].transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                clostestIndex = i;
            }
        }
        return aliveNinjas[clostestIndex];

    }

    public void StartRound()
    {
        RopeController.DestroyAllRopes();

        var spawnPoints = spawnManager.SpawnPoints;
        GameUiManager.Instance.HideAll();
        aliveNinjas = competingNinjas.Keys.ToList();
        var length = aliveNinjas.Count;
        for (var i = 0; i < length; i++)
        {
            var item = aliveNinjas[i];
            item.gameObject.SetActive(true);
            item.State = NinjaController.NinjaState.Alive;
            item.transform.position = spawnPoints[i].position;
        }

        if (RoundStart != null)
        {
            RoundStart();
        }
        GameState = State.Playing;
    }

    public void GameComplete()
    {
        if (MatchFinished != null)
        {
            MatchFinished();
        }

        RopeController.DestroyAllRopes();

        competingNinjas.Clear();
        ninjaDescriptions = ninjaBank.GetRandomNinjas(4);

        GameState = State.PlayerScreen;
        Destroy(BloodSplatterFX.bloodSpatterParent.gameObject);
        BloodSplatterFX.bloodSpatterParent = null;
    }

    protected virtual void Awake()
    {
        Instance = this;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    protected void Start()
    {
        ninjaDescriptions = ninjaBank.GetRandomNinjas(4);
    }
}
