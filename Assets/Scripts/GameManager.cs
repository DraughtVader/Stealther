using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

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
    protected GameObject splatterPfx,
        accessoryDropPrefab;

    [SerializeField]
    protected SlowMoController slowMoController;

    [SerializeField]
    protected NinjaPickerManager ninjaPicker;

    [SerializeField]
    protected PregameManager pregameManger;

    [SerializeField]
    protected AudioClip start,
        end;

    protected Dictionary<NinjaController, int> competingNinjas = new Dictionary<NinjaController, int>();
    protected List<NinjaController> aliveNinjas;
    private AccessoryDescription[] ninjaBodies;
    private AudioSource audioSource;
    private Level currentLevel;

    public State GameState { get; set; }

    public Level[] levels;

    public void UpButtonPress()
    {
        if (GameState == State.PlayerScreen && competingNinjas.Count > 0)
        {
            PreviousLevel();
        }
    }

    public void DownButtonPress()
    {
        if (GameState == State.PlayerScreen && competingNinjas.Count > 0)
        {
            NextLevel();
        }
    }

    public void NinjaKilled(NinjaController ninja, Vector3 deathPosition)
    {
        aliveNinjas.Remove(ninja);
        if (aliveNinjas.Count == 1)
        {
            AddScore(aliveNinjas[0]);
        }

        var blood = Instantiate(splatterPfx, deathPosition, Quaternion.identity);
        blood.GetComponent<BloodSplatterFX>().SetUp(ninja.NinjaColor);
        var accessoryDrop = Instantiate(accessoryDropPrefab, deathPosition, Quaternion.identity);
        accessoryDrop.GetComponent<AccessoryDrop>().SetUp(ninja.HatSprite.sprite);
    }

    public void GetNextDescription(NinjaController ninja)
    {
        ninja.Description = ninjaPicker.GetNextDescription(ninja.Description);
        GameUiManager.Instance.UpdatePlayer(ninja);
    }

    public void GetPreviousDescription(NinjaController ninja)
    {
        ninja.Description = ninjaPicker.GetLastDescription(ninja.Description);
        GameUiManager.Instance.UpdatePlayer(ninja);
    }

    public void AddPlayer(NinjaController ninja)
    {
        competingNinjas.Add(ninja, 0);
        ninja.Description = ninjaPicker.GetDescription();
        ninja.HatSprite.sprite = ninjaPicker.GetNextHat().Sprite;
        ninja.SetUpBody(ninjaBodies[competingNinjas.Count - 1]);
        GameUiManager.Instance.AddPlayer(ninja);
        pregameManger.PlayerJoined(ninja);
        if (competingNinjas.Count == 1)
        {
            PickLevel();
        }
    }

    public void AddScore(NinjaController ninja)
    {
        competingNinjas[ninja]++;
        if (competingNinjas[ninja] >= targerWins)
        {
            GameUiManager.Instance.DisplayFinalScores(competingNinjas);
            GameState = State.MatchScore;
            audioSource.PlayOneShot(end);
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
        if (aliveNinjas == null || aliveNinjas.Count <= 1)
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
        foreach (var level in levels)
        {
            level.gameObject.SetActive(false);
        }
        currentLevel.gameObject.SetActive(true);
        pregameManger.End();

        RopeController.DestroyAllRopes();

        var spawnPoints = currentLevel.SpawnManager.SpawnPoints;
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

        audioSource.PlayOneShot(start);
    }

    public void GameComplete()
    {
        if (MatchFinished != null)
        {
            MatchFinished();
        }

        RopeController.DestroyAllRopes();

        foreach (var item in competingNinjas)
        {
            item.Key.transform.position = new Vector3(100, 100); //TODO tidy
            item.Key.Rigidbody.isKinematic = true;
            item.Key.Rigidbody.velocity = Vector2.zero;
        }

        competingNinjas.Clear();
        ninjaBodies = ninjaPicker.GetBodies(4);

        GameState = State.PlayerScreen;
        BloodSplatterFX.DestroyAll();
        currentLevel.gameObject.SetActive(false);
    }

    protected virtual void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    protected void Start()
    {
        ninjaBodies = ninjaPicker.GetBodies(4);
    }

    public void StartFirstRound()
    {
        BloodSplatterFX.DestroyAll();
        StartRound();
    }

    private void PickLevel()
    {
        currentLevel = levels[Random.Range(0, levels.Length)];
        GameUiManager.Instance.UpdateLevel(currentLevel);
    }

    private void NextLevel()
    {
        var index = Array.IndexOf(levels, currentLevel);
        currentLevel = levels[(index + 1) % levels.Length];
        GameUiManager.Instance.UpdateLevel(currentLevel);
    }

    private void PreviousLevel()
    {
        var index = Array.IndexOf(levels, currentLevel);
        currentLevel = index == 0  ? levels[levels.Length-1] : levels[index - 1];
        GameUiManager.Instance.UpdateLevel(currentLevel);
    }
}
