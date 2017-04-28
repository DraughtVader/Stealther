using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{
    public static GameUiManager Instance;

    [SerializeField]
    protected GameObject scoresPanel,
                         startPanel;

    [SerializeField]
    protected Text scoreText,
                   countdownText;

    [SerializeField]
    protected Image[] playerImages;

    private int playerCount;
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
        scoreText.text = GetScoresAsString(competingNinjas);
        matchCompete = true;
        StartCountDown(ShowScorePanel, false, 2.0f);
    }

    private void ShowScorePanel()
    {
        scoresPanel.SetActive(true);
        if (matchCompete)
        {
            StartCountDown(Reset, false, 5.0f);
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
        playerImages[ninja.PlayerNumber].color = ninja.NinjaColor;
        playerImages[ninja.PlayerNumber].GetComponentInChildren<Text>().text = ninja.NinjaName;
        playerCount++;
        if (playerCount >= 2)
        {
            StartCountDown(GameManager.Instance.StartRound);
        }
    }

    private void Reset()
    {
        startPanel.SetActive(true);
        foreach (var item in playerImages)
        {
            item.color = new Color(1,1,1,0.25f);
            item.GetComponentInChildren<Text>().text = "PRESS A";
        }
        playerCount = 0;
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
        int size = 60;
        foreach (var entry in ordered)
        {
            scoresSb.AppendFormat("{3}<size={0}>{1} - {2}</size>{3}\n", size, entry.Key.NinjaName, entry.Value, string.Format(COLOUR_ID, entry.Key.Description.Color.ToHex()));
            size -= 10;
        }
        return scoresSb.ToString();
    }
}
