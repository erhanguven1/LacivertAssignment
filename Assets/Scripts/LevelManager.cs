using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    void Awake()
    {
        Instance = this;
    }

    public int ballRemaining = 5;
    public Transform ballsTop;

    public GameObject ballPrefab;
    public List<GameObject> balls;
    public GameObject chosenBall;

    public bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBalls();
    }

    void InitializeBalls()
    {
        FindObjectOfType<HoleManager>().RepositionHole();

        GameManager.Instance.UpdateRemainingBalls(ballRemaining);
        GameManager.Instance.AppearStreak(false);

        //If it's first time, instantiate balls
        if (firstTime)
        {
            firstTime = false;
            for (int i = 0; i < 5; i++)
            {
                balls.Add(Instantiate(ballPrefab, ballsTop));
                balls.Last().transform.localPosition = Vector3.up * (i - 5);
            }

            return; //no need to continue
        }

        //reload balls (pooling)
        for (int i = 0; i < 5; i++)
        {
            balls[i].transform.parent = ballsTop;
            balls[i].GetComponent<Renderer>().material.color = Color.green;

            balls[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            balls[i].GetComponent<Collider>().enabled = false;
            balls[i].GetComponent<Rigidbody>().useGravity = false;
            balls[i].transform.localPosition = Vector3.up * (i - 5);

            balls[i].GetComponent<Ball>().collided = false;

        }

    }

    public void LoadBall()
    {
        if (ballRemaining==0)
        {
            GameManager.Instance.GameOver();
            return;
        }

        ballRemaining--;
        GameManager.Instance.UpdateRemainingBalls(ballRemaining);

        //move balls up
        ballsTop.DOMoveY(ballsTop.position.y + 1, .2f); 

        //choose the top ball
        chosenBall = balls[ballRemaining];
        chosenBall.transform.parent = null;

        if (ballRemaining==0)
        {
            chosenBall.GetComponent<Ball>().isLastBall = true;
        }
    }

    //Restart button tap
    public void ReloadScene()
    {

        GameManager.Instance.GameplayPanel.SetActive(true);
        GameManager.Instance.UpdateSucceedBalls(true);

        InitializeBalls();
        ballsTop.position -= Vector3.up * 5;
        
        GameManager.Instance.GameOverPanel.SetActive(false);

        GameManager.Instance.currentGameState = GameState.Running;
    }
}
