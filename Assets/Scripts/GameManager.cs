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
        end,
        round;

    
    [SerializeField]
    protected ScoresManager[] scoreManagers;   
    
    private ScoresManager currentScoresManager;
    private AccessoryDescription[] ninjaBodies;
    private AudioSource audioSource;
    private Level currentLevel;
    private List<NinjaController> participatingNinjas = new List<NinjaController>();

    public State GameState { get; set; }

    public Level[] levels;

    public void UpButtonPress()
    {
        if (GameState == State.PlayerScreen && participatingNinjas.Count > 0)
        {
            PreviousLevel();
        }
    }

    public void DownButtonPress()
    {
        if (GameState == State.PlayerScreen && participatingNinjas.Count > 0)
        {
            NextLevel();
        }
    }
    
    public void LeftButtonPress()
    {
        if (GameState == State.PlayerScreen && participatingNinjas.Count > 0)
        {
            PreviousMode();
        }
    }

    public void RightButtonPress()
    {
        if (GameState == State.PlayerScreen && participatingNinjas.Count > 0)
        {
            NextMode();
        }
    }

    public void NinjaKilled(NinjaController killedNinja, Vector3 deathPosition, NinjaController killerNinja = null)
    {
        if (GameState == State.Playing)
        {
            currentScoresManager.NinjaKilled(killedNinja, killerNinja);
        }

        var blood = Instantiate(splatterPfx, deathPosition, Quaternion.identity);
        blood.GetComponent<BloodSplatterFX>().SetUp(killedNinja.NinjaColor);
        var accessoryDrop = Instantiate(accessoryDropPrefab, deathPosition, Quaternion.identity);
        accessoryDrop.GetComponent<AccessoryDrop>().SetUp(killedNinja.HatSprite.sprite);
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
        participatingNinjas.Add(ninja);
        ninja.Description = ninjaPicker.GetDescription();
        ninja.HatSprite.sprite = ninjaPicker.GetNextHat().Sprite;
        ninja.SetUpBody(ninjaBodies[participatingNinjas.Count - 1]);
        GameUiManager.Instance.AddPlayer(ninja);
        pregameManger.PlayerJoined(ninja);
        if ( participatingNinjas.Count == 1)
        {
            PickLevel();
        }
    }

    public void RoundEnded()
    {
        GameState = State.RoundScore;
        audioSource.PlayOneShot(round);
        if (RoundEnd != null)
        {
            RoundEnd();
        }
    }
    
    public void FinalRoundEnded()
    {
        GameState = State.MatchScore;
        audioSource.PlayOneShot(end);
        if (RoundEnd != null)
        {
            RoundEnd();
        }
    }

    public NinjaController GetClosestNinja(Vector3 position)
    {
        if (currentScoresManager.AliveNinjas == null || currentScoresManager.AliveNinjas.Count <= 1)
        {
            return null;
        }

        int clostestIndex = -1,
            length = currentScoresManager.AliveNinjas.Count;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < length; i++)
        {
            if (currentScoresManager.AliveNinjas[i].Shield != NinjaController.ShieldType.None)
            {
                continue;
            }
            var currentDistance = Vector2.Distance(position, currentScoresManager.AliveNinjas[i].transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                clostestIndex = i;
            }
        }
        return clostestIndex == -1 ? null : currentScoresManager.AliveNinjas[clostestIndex];
    }

    public void StartRound()
    {
        foreach (var level in levels)
        {
            level.gameObject.SetActive(false);
        }
        currentLevel.gameObject.SetActive(true);

        RopeController.DestroyAllRopes();

        GameUiManager.Instance.HideAll();
        currentScoresManager.Spawner = currentLevel.SpawnManager;
       
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

        ninjaBodies = ninjaPicker.GetBodies(4);

        GameState = State.PlayerScreen;
        BloodSplatterFX.DestroyAll();
        currentLevel.gameObject.SetActive(false);
        participatingNinjas.Clear();
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
        currentScoresManager.AddNinjas(participatingNinjas);
        BloodSplatterFX.DestroyAll();
        pregameManger.End();
        StartRound();
    }

    private void PickLevel()
    {
        currentLevel = levels[Random.Range(0, levels.Length)];
        GameUiManager.Instance.UpdateLevel(currentLevel);
        currentScoresManager = scoreManagers[0];
        GameUiManager.Instance.UpdateMode(currentScoresManager);
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
    
    private void NextMode()
    {
        var index = Array.IndexOf(scoreManagers, currentScoresManager);
        currentScoresManager = index == 0  ? scoreManagers[scoreManagers.Length-1] : scoreManagers[index - 1];
        GameUiManager.Instance.UpdateMode(currentScoresManager);
    }
    
    private void PreviousMode()
    {
        var index = Array.IndexOf(scoreManagers, currentScoresManager);
        currentScoresManager = scoreManagers[(index + 1) % scoreManagers.Length];
        GameUiManager.Instance.UpdateMode(currentScoresManager);
    }
}
