using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public enum GameState { Menu, Running, GameOver }
public enum GameEndState { Success, Fail }

public class GameManager : MonoBehaviour
{
    public GameState currentGameState;

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public GameObject StartPanel, GameOverPanel, GameplayPanel;

    public int succeedBallCount;
    public int score;
    public int scoreMultipler;

    public Text WinStateText, ScoreText, RemainingBallsCountText, StreakText;
    public List<Image> succeedBalls;

    public void AppearStreak(bool appear)
    {
        StreakText.text = "x " + scoreMultipler.ToString();
        StreakText.rectTransform.DOScale(appear ? 1 : 0, .2f);
    }

    public void UpdateRemainingBalls(int n)
    {
        RemainingBallsCountText.text ="X"+ n.ToString();
    }

    public void UpdateSucceedBalls(bool reset)
    {
        if (!reset)
            succeedBalls[succeedBallCount - 1].color = Color.green;
        else
        {
            foreach (var item in succeedBalls)
            {
                item.color = Color.white;
            }
        }
    }

    public void OnTapStart()
    {
        StartPanel.SetActive(false);
        GameplayPanel.SetActive(true);
        currentGameState = GameState.Running;
    }

    public void GameOver()
    {
        GameplayPanel.SetActive(false);

        currentGameState = GameState.GameOver;

        GameEndState state = succeedBallCount >= 3 ? GameEndState.Success : GameEndState.Fail;
        GameOverPanel.SetActive(true);

        WinStateText.text = "YOU " + (state == GameEndState.Success ? "WIN!" : "LOST!");
        ScoreText.text = score.ToString();

        LevelManager.Instance.ballRemaining = 5;
        succeedBallCount = 0;
        score = 0;
        scoreMultipler = 1;

}

}
