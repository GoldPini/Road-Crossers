using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class VheicleSpawner : MonoBehaviour
{
    [SerializeField] private Lane[] lanes;
    [Space]
  
    private GameManager difficultyController;
    private float time;    
    private int randomizationFactor;
    private float timeInterval;
    private float minSpeed;
    private float maxSpeed;
    private float offsetSpeedRandomizer = 3f;
    private bool randomizeSpeed;
    private bool difficultyInitialized = false;


    private void Start()
    {
        difficultyController = GameManager.Instance;
        readDifficulty();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= timeInterval)
        {
            time = 0;
            attemptSpawn();
        }
    }

    private void attemptSpawn()
    {
        if (Random.Range(0, randomizationFactor) == 0)
        {
            BaseVheicle myCar = difficultyController.VheiclePrefab;
            if (myCar != null && difficultyInitialized)
            {                
                int lane = Random.Range(0, lanes.Length);

                bool hasSpace = true;//TODO implement
                if (hasSpace)
                {
                    float localMinSpeed;
                    float localMaxSpeed;
                    //fully randomizing the speed
                    if (randomizeSpeed)
                    {
                        localMinSpeed = minSpeed;
                        localMaxSpeed = maxSpeed;
                    }
                    else//slightly randomizing over the vheicle's pre-set speed
                    {
                        localMinSpeed = myCar.Speed - offsetSpeedRandomizer;
                        localMaxSpeed = myCar.Speed + offsetSpeedRandomizer;
                    }

                    myCar.Speed = Random.Range(localMinSpeed, localMaxSpeed);
                    lanes[lane].Spawn(myCar);
                    //Debug.Log("Min speed on spawn: " + minSpeed);
                    //Debug.Log("Max speed on spawn: " + maxSpeed);
                }
                else
                {
                    //Debug.Log($"No space to spawn {hit.collider.gameObject.name}");
                }
            }
        }
    }

    public void SetDifficulty(float timeInterval, float minSpeed, float maxSpeed)
    {
        this.timeInterval = timeInterval;
        this.minSpeed += minSpeed;
        this.maxSpeed += maxSpeed;
        difficultyInitialized = true;
    }

    public void readDifficulty()
    {
        randomizationFactor = GameManager.RandomizationFactor;
        timeInterval = GameManager.TimeInterval;
        randomizeSpeed = GameManager.RandomizeSpeed;
        minSpeed = GameManager.MinSpeed;
        maxSpeed = GameManager.MaxSpeed;
        difficultyInitialized = true;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //foreach (var lane in lanes) 
        //{
        //    Gizmos.DrawWireSphere(lane.transform.position + maxSpawningDistance * lane.transform.forward, spawningRadius);
        //}
    }
}




//spawning vheicle on a predefined spline lane
//myCar.splinePath = myCar.GetComponent<SplineAnimate>();

//if (splines != null)
//{
//    int lane = Random.Range(0, splines.Length);
//    //myCar.splinePath.Container = splines[lane];
//    Debug.Log($"Chosed lane: {lane} out of: {splines.Length}");
//}
