using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Gun : MonoBehaviour
{
    public LineRenderer lr;

    public List<Vector3> points = new List<Vector3>();

    public float speed;

    bool isTouching;

    void Start()
    {
        DrawTrajectory();
    }

    void DrawTrajectory()
    {
        Vector3 point1 = transform.position;
        var bulletVelocity = transform.forward * speed; //render it towards our forward
        float resolution = .01f;

        points.Clear();

        //eğik atışı simüle etmek amacıyla velocity her seferinde gravity kadar yer değiştirir

        for (float i = 0; i < 1f; i += resolution)
        {

            bulletVelocity += Physics.gravity * resolution * 10;
            Vector3 point2 = point1 + bulletVelocity * resolution;
            points.Add(point1);

            point1 = point2;
            if (point1.y < points[0].y) //do not render unnecessary points
            {
                //apply positions to linerenderer
                lr.positionCount = points.Count; 
                lr.SetPositions(points.ToArray());
                break;
            }
        }

    }
    void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.Running)
        {
            if (Input.GetMouseButton(0))
            {
                DrawTrajectory();
                Vector3 newAngles = transform.eulerAngles;
                #region ANGLE CLAMPING
                newAngles += (Vector3.up * Input.GetAxis("Mouse X") + Vector3.right * Input.GetAxis("Mouse Y")) * 2;
                
                if (newAngles.y > 180)
                    newAngles.y = -360 + newAngles.y;

                var yRotation = Mathf.Clamp(newAngles.y, -40, 40);

                if (yRotation < 0) 
                    yRotation = 360 + yRotation;
                
                newAngles.y = yRotation;

                if (newAngles.x > 180)
                    newAngles.x = -360 + newAngles.x;

                var xRotation = Mathf.Clamp(newAngles.x, -85, -45);

                if (xRotation < 0)
                    xRotation = 360 + xRotation;

                newAngles.x = xRotation;
                #endregion
                transform.eulerAngles = newAngles;
                isTouching = true;
            }

            //Fire our bullet
            if (Input.GetMouseButtonUp(0) && isTouching && LevelManager.Instance.ballRemaining>0)
            {
                isTouching = false;
                if (LevelManager.Instance.ballRemaining > 0)
                    LevelManager.Instance.LoadBall();

                LevelManager.Instance.chosenBall.GetComponent<Collider>().enabled = true;
                LevelManager.Instance.chosenBall.transform.position = transform.position;
                LevelManager.Instance.chosenBall.GetComponent<Rigidbody>().useGravity = true;

                //Move ball on projectile
                LevelManager.Instance.chosenBall.GetComponent<Rigidbody>().velocity = getLaunchVelocity(points.Last());

            }

        }
    }

    Vector3 getLaunchVelocity(Vector3 target)
    {
        float angle = -transform.eulerAngles.x;
        var dir = target - transform.position;
        var h = dir.y;
        dir.y = 0;
        var dist = dir.magnitude;
        var a = angle * Mathf.Deg2Rad;
        dir.y = dist * Mathf.Tan(a);
        dist += h / Mathf.Tan(a);

        // angle of reach = v velocity'si ile d kadar gitmek için gereken a açısıdır. kaynak: wikipedia, https://en.wikipedia.org/wiki/Projectile_motion#Velocity
        // sin(2a) = g*d/(v^2). v'yi bulmak içi yalnız bırakırsak
        // v^2 = g*d/(sin(2a)). dolayısıyla v = sqrt(g*d/(sin(2a)))

        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return vel * dir.normalized;
    }

}