using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundsManager : MonoBehaviour
{
    [SerializeField, Range(0, 300), Tooltip("How often (in seconds) the game plays a traffic ambient sound")] 
    private float trafficAmbientPlayInterval = 60f;
    [SerializeField, Range(0, 300), Tooltip("How often (in seconds) the game plays a car-passing ambient sound")] 
    private float carPassingAmbientPlayInterval = 20f;
    [SerializeField, Range(0, 30), Tooltip("A delay from the game's start for playing the first traffic ambient sound effect")] 
    private float trafficAmbientCountdownFromStart = 3f;
    [SerializeField, Range(0, 30), Tooltip("A delay from the game's start for playing the first car-passing ambient sound effect")] 
    private float carPassingAmbientCountdownFromStart = 7f;

    private float trafficAmbientTimer = 0f;
    private float carPassingAmbientTimer = 0f;

    private void Start()
    {
        trafficAmbientTimer = trafficAmbientPlayInterval - trafficAmbientCountdownFromStart;
        carPassingAmbientTimer = carPassingAmbientPlayInterval - carPassingAmbientCountdownFromStart;

        GameManager.OnGameReset += GameManager_OnGameReset;
    }

    private void GameManager_OnGameReset(object sender, System.EventArgs e)
    {
        trafficAmbientTimer = trafficAmbientPlayInterval - trafficAmbientCountdownFromStart;
        carPassingAmbientTimer = carPassingAmbientPlayInterval - carPassingAmbientCountdownFromStart;
    }

    private void Update()
    {
        trafficAmbientTimer += Time.deltaTime;
        carPassingAmbientTimer += Time.deltaTime;

        if (trafficAmbientTimer > trafficAmbientPlayInterval)
        {
            SoundManager.Instance.PlayTrafficAmbientSound();
            trafficAmbientTimer = 0;
        }

        if (carPassingAmbientTimer > carPassingAmbientPlayInterval)
        {
            SoundManager.Instance.PlayCarPassingAmbientSound();
            carPassingAmbientTimer = 0;
        }
    }
}
