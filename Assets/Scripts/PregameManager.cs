using System.Collections.Generic;
using UnityEngine;

public class PregameManager : MonoBehaviour
{
    [SerializeField]
    protected SpawnManager spawnPoints;

    [SerializeField]
    protected SpriteRenderer[] blockers;

    [SerializeField]
    protected GameObject pregameLevel;

    [SerializeField]
    protected Target[] targets;

    public GameObject PregameLevel
    {
        get { return pregameLevel; }
    }

    private Dictionary<NinjaController, bool> pregamers = new Dictionary<NinjaController, bool>();
    private bool startedGame;

    public void PlayerJoined(NinjaController ninja)
    {
        blockers[ninja.PlayerNumber].color = new Color(1, 1, 1, 0);
        spawnPoints.SpawnNinja(ninja, ninja.PlayerNumber, NinjaState.Pregame);

        pregamers.Add(ninja, false);
    }

    public void SetBlockersActive(bool active)
    {
        foreach (var item in blockers)
        {
            item.color = new Color(1, 1, 1, active ? 1 : 0);
        }
    }

    public void PlayerReady(NinjaController ninja)
    {
        pregamers[ninja] = true;
        ninja.State = NinjaState.Ready;
        GameUiManager.Instance.SetNinjaAsReady(ninja.PlayerNumber);
        if (pregamers.Count <= 1)
        {
            return;
        }

        var ready = true;
        foreach (var item in pregamers)
        {
            if (item.Value)
            {
                continue;
            }
            ready = false;
            break;
        }

        if (ready && !startedGame)
        {
            GameUiManager.Instance.StartGameCountDown();
            startedGame = true;
        }
    }

    public void End()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.MatchFinished += OnMatchFinished;
    }

    private void OnMatchFinished()
    {
        gameObject.SetActive(true);
        SetBlockersActive(true);
        pregamers = new Dictionary<NinjaController, bool>();

        foreach (var item in targets)
        {
            item.gameObject.SetActive(true);
        }
        
        startedGame = false;
    }
}
