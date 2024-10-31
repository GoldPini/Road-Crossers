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

public class MainMenuVheicleSpawner : MonoBehaviour
{
    [SerializeField] private Lane[] lanes;
    [SerializeField] private BaseVheicle[] vehiclePrefabs;
    [Space]
  
    private float time;        
    private float timeInterval = 2f;
    private float minSpeed = 20;
    private float maxSpeed = 25;    

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
        BaseVheicle myCar = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
        int lane = Random.Range(0, lanes.Length);
        myCar.Speed = Random.Range(minSpeed, maxSpeed);
        lanes[lane].Spawn(myCar);
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
