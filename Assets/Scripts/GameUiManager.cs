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
        countdownText,
        modeText;

    [SerializeField]
    protected PlayerUI[] playerUI;
    
    [SerializeField]
    protected ParticleSystem confettiParticleSystem;

    private float currentCountDown;
    private Action onCountDownComplete;
    private const string COLOUR_ID = " <color=#{0}>*</color> ";
    private bool matchCompete, newOnComplete;

    public void DisplayScores(List<ParticipatingNinja> participatingNinja)
    {
        PlayConfetti(GetWinner(participatingNinja));
        scoreText.text = GetScoresAsString(participatingNinja);
        matchCompete = false;
        StartCountDown(ShowScorePanel, false, 2.0f);
    }

    public void DisplayFinalScores(List<ParticipatingNinja> participatingNinja)
    {
        PlayConfetti(GetWinner(participatingNinja));
        scoreText.text = GetFinalScoresAsString(participatingNinja);
        matchCompete = true;
        StartCountDown(ShowScorePanel, false, 2.0f);
    }

    private void PlayConfetti(NinjaController winner)
    {
        var settings = confettiParticleSystem.main;
        settings.startColor = winner.NinjaColor;
        confettiParticleSystem.Play();
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
        newOnComplete = false;
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

    protected string GetScoresAsString(List<ParticipatingNinja> participatingNinjas)
    {
        var scoresSb = new StringBuilder();
        var ordered = participatingNinjas.OrderByDescending(x => x.Score);
        var size = 60;
        foreach (var participatingNinja in ordered)
        {
            var ninja = participatingNinja.Ninja;
            scoresSb.AppendFormat("{3}<size={0}>{1} - {2}</size>{3}\n", size, ninja.NinjaName, participatingNinja.Score, string.Format(COLOUR_ID, ninja.Description.Color.ToHex()));
            size -= 10;
        }
        scoresSb.Remove(scoresSb.Length - 2, 2); //remove the last "\n" added
        return scoresSb.ToString();
    }

    protected string GetFinalScoresAsString(List<ParticipatingNinja> participatingNinjas)
    {
        var scoresSb = new StringBuilder();
        var ordered = participatingNinjas.OrderByDescending(x => x.Score);
        foreach (var participatingNinja in ordered)
        {
            var ninja = participatingNinja.Ninja;
            scoresSb.AppendFormat("{2}<size={0}>{1} wins!</size>{2}", 60, ninja.NinjaName, string.Format(COLOUR_ID, ninja.Description.Color.ToHex()));
            break;
        }
        return scoresSb.ToString();
    }

    protected NinjaController GetWinner(List<ParticipatingNinja> participatingNinjas)
    {
        return participatingNinjas.OrderByDescending(x => x.Score).FirstOrDefault().Ninja;
    }

    public void StartGameCountDown()
    {
        StartCountDown(GameManager.Instance.StartFirstRound);
    }

    public void UpdateLevel(Level level)
    {
        levelTextPanel.GetComponentInChildren<Text>().text = level.Title;
    }
    
    public void UpdateMode(ScoresManager scoresManager)
    {
        modeText.text = scoresManager.Mode.ToString();
    }

    public void BackToTitle()
    {
        //TODO
       // SceneManager.LoadScene("TitleScreen");
    }
}
