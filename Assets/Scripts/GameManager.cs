using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
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

    protected Dictionary<NinjaController, int> competingNinjas = new Dictionary<NinjaController, int>();
    protected List<NinjaController> aliveNinjas;
    private NinjaDescription[] ninjaDescriptions;

    public void NinjaKilled(NinjaController ninja, Vector3 deathPosition)
    {
        aliveNinjas.Remove(ninja);
        if (aliveNinjas.Count == 1)
        {
            AddScore(aliveNinjas[0]);
        }
        Instantiate(splatterPfx, deathPosition, Quaternion.identity);
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
            GameComplete();
        }
        else
        {
            GameUiManager.Instance.DisplayScores(competingNinjas);
            if (RoundEnd != null)
            {
                RoundEnd();
            }
            RopeController.DestroyAllRopes();
        }
    }

    public void StartRound()
    {
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
    }

    protected void GameComplete()
    {
        if (MatchFinished != null)
        {
            MatchFinished();
        }

        RopeController.DestroyAllRopes();

        competingNinjas.Clear();
        ninjaDescriptions = ninjaBank.GetRandomNinjas(4);
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
