using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{
    [Serializable]
    public class PlayerUI
    {
        public Text NinjaName;

        public GameObject JoinText,
            readyIcon,
            leftArrow,
            rightArrow;

        public void SetNinjaName(string name, bool unready = true)
        {
            NinjaName.text = name;
            NinjaName.gameObject.SetActive(true);
            JoinText.SetActive(false);
            if (unready)
            {
                readyIcon.SetActive(false);
                rightArrow.SetActive(true);
                leftArrow.SetActive(true);
            }
        }

        public void SetAsReady()
        {
            readyIcon.SetActive(true);
            rightArrow.SetActive(false);
            leftArrow.SetActive(false);
        }

        public void SetJoinText()
        {
            NinjaName.gameObject.SetActive(false);
            JoinText.SetActive(true);
        }
    }

    public static GameUiManager Instance;

    [SerializeField]
    protected GameObject scoresPanel,
        startPanel,
        title,
        levelTextPanel;

    [SerializeField]
    protected Text scoreText,
        countdownText;

    [SerializeField]
    protected PlayerUI[] playerUI;

    private float currentCountDown;
    private Action onCountDownComplete;
    private const string COLOUR_ID = " <color=#{0}>*</color> ";
    private bool matchCompete, newOnComplete;

    public void DisplayScores(Dictionary<NinjaController, int> competingNinjas)
    {
        scoreText.text = GetScoresAsString(competingNinjas);
        matchCompete = false;
        StartCountDown(ShowScorePanel, false, 2.0f);
    }

    public void DisplayFinalScores(Dictionary<NinjaController, int> competingNinjas)
    {
        scoreText.text = GetFinalScoresAsString(competingNinjas);
        matchCompete = true;
        StartCountDown(ShowScorePanel, false, 2.0f);
    }

    private void ShowScorePanel()
    {
        scoresPanel.SetActive(true);
        if (matchCompete)
        {
            StartCountDown(Reset, true, 5.0f);
        }
        else
        {
            StartCountDown(GameManager.Instance.StartRound);
        }
    }

    public void HideAll()
    {
        scoresPanel.SetActive(false);
        startPanel.SetActive(false);
    }

    public void StartCountDown(Action onComplete, bool display = true, float duration = 3.0f)
    {
        onCountDownComplete = onComplete;
        currentCountDown = duration;
        countdownText.gameObject.SetActive(display);
        newOnComplete = true;
    }

    public void AddPlayer(NinjaController ninja)
    {
        title.SetActive(false);
        levelTextPanel.SetActive(true);
        playerUI[ninja.PlayerNumber].SetNinjaName(ninja.NinjaName);
    }

    public void UpdatePlayer(NinjaController ninja)
    {
        playerUI[ninja.PlayerNumber].SetNinjaName(ninja.NinjaName, false);
    }

    public void SetNinjaAsReady(int index)
    {
        playerUI[index].SetAsReady();
    }

    private void Reset()
    {
        startPanel.SetActive(true);
        scoresPanel.SetActive(false);
        title.SetActive(true);
        levelTextPanel.SetActive(false);
        foreach (var item in playerUI)
        {
            item.SetJoinText();

        }
        GameManager.Instance.GameComplete();
    }

    private void Update()
    {
        if (onCountDownComplete != null)
        {
            currentCountDown -= Time.deltaTime;
            countdownText.text = currentCountDown.ToString("f1");
            if (currentCountDown > 0)
            {
                return;
            }
            onCountDownComplete();
            if (!newOnComplete)
            {
                onCountDownComplete = null;
                countdownText.gameObject.SetActive(false);
            }
            newOnComplete = false;
        }
    }

    protected virtual void Awake()
    {
        Instance = this;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    protected string GetScoresAsString(Dictionary<NinjaController, int> competingNinjas)
    {
        var scoresSb = new StringBuilder();
        var ordered = competingNinjas.OrderByDescending(x => x.Value);
        var size = 60;
        foreach (var entry in ordered)
        {
            scoresSb.AppendFormat("{3}<size={0}>{1} - {2}</size>{3}\n", size, entry.Key.NinjaName, entry.Value, string.Format(COLOUR_ID, entry.Key.Description.Color.ToHex()));
            size -= 10;
        }
        scoresSb.Remove(scoresSb.Length - 2, 2); //remove the last "\n" added
        return scoresSb.ToString();
    }

    protected string GetFinalScoresAsString(Dictionary<NinjaController, int> competingNinjas)
    {
        var scoresSb = new StringBuilder();
        var ordered = competingNinjas.OrderByDescending(x => x.Value);
        foreach (var entry in ordered)
        {
            scoresSb.AppendFormat("{2}<size={0}>{1} wins!</size>{2}", 60, entry.Key.NinjaName, string.Format(COLOUR_ID, entry.Key.Description.Color.ToHex()));
            break;
        }
        return scoresSb.ToString();
    }

    public void StartGameCountDown()
    {
        StartCountDown(GameManager.Instance.StartFirstRound);
    }

    public void UpdateLevel(Level level)
    {
        levelTextPanel.GetComponentInChildren<Text>().text = level.Title;
    }

    public void BackToTitle()
    {
        //TODO
       // SceneManager.LoadScene("TitleScreen");
    }
}
