using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static event EventHandler OnProgress;
    public event EventHandler OnSegmentSpawned;

    public static void ResetStaticData()
    {
        OnProgress = null;
    }

    [SerializeField] private RigidBodyCharacterController player;
    [SerializeField] private RoadSegment firstSegment;
    [SerializeField] private RoadSegment segmentPrefab;
    [SerializeField] private Transform lastSegment;
    [SerializeField] private GameObject segmentsContainer;
    
    [Space,Tooltip("How many segments whould always be spawnd in front of the player"), SerializeField]
    private float minDistanceFromLastSegment = 100;
    [Tooltip("How many segments will be spawned at a time"), SerializeField]
    private uint spawnBatches = 1;
    [Space]
    [SerializeField,Tooltip("The maximum amount of segments allowed to exist in the scene at the same time")] 
    private int maxSegmentsAlive = 6;

    private GameManager gameManager;
    private float distance;
    private Vector3 lastSpawnPosition;
    //the transfrom of the road segment the player is currently on
    private Transform playerOnPosition = null;
    private float? playerFromLastSegmentDistance = null;
    private int totalSpawnedSegments = 1;
    private Queue<RoadSegment> segments = new Queue<RoadSegment>();
    private Vector3 firstSpawnPosition;
    private int segmentIndex = 0;
    private int totalSegments = 10;
    private bool finishedLevelGeneration = false;


    public int TotalSegments { get => totalSegments; set => totalSegments = value; }
    public float MinDistanceFromLastSegment { get => minDistanceFromLastSegment; set => MinDistanceFromLastSegment = value; }
    public int SpawnBatches { get { return (int)spawnBatches; }
        set { spawnBatches = value <= 0 ? 1 : (uint)value ;  } }

    private void Start()
    {
        gameManager = GameManager.Instance;
        totalSegments = GameManager.TotalRoadSegments;

        RoadSegment.OnPlayerEnteredRoadSegment += onPlayerEnteredRoadSegment;
        segments.Enqueue(firstSegment);
        firstSpawnPosition = firstSegment.transform.position;       

        reset();        

        player.OnPlayerRespawned += player_OnPlayerRespawned;        
    }    

    private void FixedUpdate()
    {
        //spawn based on player's distance from end segment                 
        if (playerOnPosition != null && playerFromLastSegmentDistance != null)
        {
            //if the player surpassed the min distanced (too closed to the end)
            if (playerFromLastSegmentDistance < minDistanceFromLastSegment)
            {
                if(totalSpawnedSegments < totalSegments)
                {
                    int batchCount = (totalSpawnedSegments + (int)spawnBatches) <= totalSegments
                        ? (int)spawnBatches : (totalSegments - totalSpawnedSegments);
                    spawn(batchCount);
                }
                else if(!finishedLevelGeneration)
                {
                    spawnLastSegment();
                }
                //recalculate the distance from the last segment
                playerFromLastSegmentDistance = Vector3.Distance(firstSegment.CenterPoint.position, playerOnPosition.position);
            }
        }        
    }

    private void spawnLastSegment()
    {
        Transform last = Instantiate(lastSegment, firstSegment.EndingPoint.position, Quaternion.identity, segmentsContainer.transform);
        finishedLevelGeneration = true;
    }

    private void player_OnPlayerRespawned(object sender, EventArgs e)
    {
        reset();
    }

    private void spawn(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            if (firstSegment != null)
            {
                distance = Vector3.Distance(firstSegment.BeginingPoint.position, firstSegment.EndingPoint.position);
                lastSpawnPosition = firstSegment.CenterPoint.position + firstSegment.ForwardDirectionNormalized * distance;
                
                firstSegment = Instantiate(segmentPrefab, lastSpawnPosition, Quaternion.identity, segmentsContainer.transform);
                firstSegment.name = "Segment"+ segmentIndex++;
                segments.Enqueue(firstSegment);
                totalSpawnedSegments++;


                Debug.Log("Level extended!");
            }
        }
        clearSegments();
    }

    private void reset()
    {
        totalSegments = GameManager.TotalRoadSegments;
        totalSpawnedSegments = 1;

        clearSegments(true);        
        segmentIndex = 0;

        firstSegment = Instantiate(segmentPrefab, firstSpawnPosition, Quaternion.identity, segmentsContainer.transform);
        firstSegment.name = "Segment" + segmentIndex++;
        segments.Enqueue(firstSegment);

        spawn(math.min(maxSegmentsAlive - 1, TotalSegments - 1));
    }

    private void clearSegments(bool fullReset = false)
    {
        int keepAliveCount = fullReset ? 0 : maxSegmentsAlive;
        if (segments.Count > keepAliveCount)
        {
            for (int i = segments.Count; i > keepAliveCount; --i)
            {
                segments.Dequeue().DestroySelf();
            }
        }
        Debug.Log("Current segment count: " + keepAliveCount);
    }

    private void onPlayerEnteredRoadSegment(object sender, System.EventArgs e)
    {
        if (sender is RoadSegment)
        {
            float distanceTmp;
            if (playerOnPosition != null)
            {
                distanceTmp = Vector3.Distance(firstSegment.CenterPoint.position, (sender as RoadSegment).CenterPoint.position);
                if (playerFromLastSegmentDistance == null || distanceTmp < playerFromLastSegmentDistance )
                {
                    playerOnPosition = (sender as RoadSegment).CenterPoint;
                    playerFromLastSegmentDistance = distanceTmp;
                }
            }
            else
            {
                playerOnPosition = (sender as RoadSegment).CenterPoint;
                playerFromLastSegmentDistance = Vector3.Distance(firstSegment.CenterPoint.position, playerOnPosition.position);
            }

        }
    }
}
