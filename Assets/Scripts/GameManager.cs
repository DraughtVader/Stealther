using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    protected int targerWins = 5;

    [SerializeField]
    protected NinjaBank ninjaBank;

    protected Dictionary<NinjaController, int> competingNinjas = new Dictionary<NinjaController, int>();
    protected List<NinjaController> aliveNinjas;
    private NinjaDescription[] ninjaDescriptions;

    public void NinjaKilled(NinjaController ninja)
    {
        aliveNinjas.Remove(ninja);
        if (aliveNinjas.Count == 1)
        {
            AddScore(aliveNinjas[0]);
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
            GameUiManager.Instance.DisplayFinalScores(GetScoresAsString());
            GameComplete();
        }
        else
        {
            GameUiManager.Instance.DisplayScores(GetScoresAsString());
            foreach (var entry in competingNinjas)
            {
                entry.Key.gameObject.SetActive(true);
                entry.Key.State = NinjaController.NinjaState.WaitingToPlay;
            }
        }
    }

    public void StartRound()
    {
        GameUiManager.Instance.HideAll();
        aliveNinjas = competingNinjas.Keys.ToList();
        foreach (var item in aliveNinjas)
        {
            item.gameObject.SetActive(true);
            item.State = NinjaController.NinjaState.Alive;
        }
    }

    protected void GameComplete()
    {
        foreach (var entry in competingNinjas)
        {
            entry.Key.SetToJoinable();
        }
        foreach (var item in FindObjectsOfType<RopeController>()) //TODO obvs
        {
            Destroy(item.gameObject);
        }
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

    protected string GetScoresAsString()
    {
        var scoresSb = new StringBuilder();
        var ordered = competingNinjas.OrderByDescending(x => x.Value);
        int size = 60;
        foreach (var entry in ordered)
        {
            scoresSb.AppendFormat("<size={0}>{1} - {2}</size>\n", size, entry.Key.NinjaName, entry.Value);
            size -= 10;
        }
        return scoresSb.ToString();
    }
}
