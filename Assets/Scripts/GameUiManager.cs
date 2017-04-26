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
    private string colourId = " <color=#{0}>*</color> ";

    public void DisplayScores(Dictionary<NinjaController, int> competingNinjas)
    {
        scoreText.text = GetScoresAsString(competingNinjas);
        scoresPanel.SetActive(true);
        StartCountDown(GameManager.Instance.StartRound);
    }

    public void DisplayFinalScores(Dictionary<NinjaController, int> competingNinjas)
    {
        scoreText.text = GetScoresAsString(competingNinjas);
        scoresPanel.SetActive(true);
        StartCountDown(Reset, false, 5.0f);
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
            onCountDownComplete = null;
            countdownText.gameObject.SetActive(false);
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
            scoresSb.AppendFormat("{3}<size={0}>{1} - {2}</size>{3}\n", size, entry.Key.NinjaName, entry.Value, string.Format(colourId, entry.Key.Description.Color.ToHex()));
            size -= 10;
        }
        return scoresSb.ToString();
    }
}
