using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Ball : MonoBehaviour
{
    public bool collided;
    public bool isLastBall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (!collided) //to prevent colliding again.
        {
            collided = true;

            //if it's the danger zone, add force from center to outside to get rid of ball
            if (col.collider.tag == "Ground") 
            {
                GameManager.Instance.scoreMultipler = 1;

                var dx = (transform.position - col.transform.position);

                var direction = dx.normalized;
                var magnitude = dx.magnitude;

                GetComponent<Rigidbody>().AddForce(direction * (1 / magnitude) * 1000 + Vector3.up * 250);
                GameManager.Instance.AppearStreak(false);

            }
            if (col.collider.tag == "Hole") //if not, congz!!!
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<MeshRenderer>().material.DOFade(0, .3f); //make our soul fade away

                GameManager.Instance.succeedBallCount += 1;

                GameManager.Instance.score += GameManager.Instance.scoreMultipler;

                if (GameManager.Instance.succeedBallCount <= 3)
                {
                    GameManager.Instance.UpdateSucceedBalls(false); //Update UI balls (top)
                }

                GameManager.Instance.scoreMultipler += 1;
                GameManager.Instance.AppearStreak(true); //Apeear streak text. "e.g: x2"
            }

            if (isLastBall) //if we are the last ball alive, When we hit, game is over
            {
                LevelManager.Instance.LoadBall();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
