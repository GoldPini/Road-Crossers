using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class RoadSegment : MonoBehaviour
{
    /// <summary>
    /// Fired only once when the player enters this road segment
    /// </summary>
    public static event EventHandler OnPlayerEnteredRoadSegment;
    public static event EventHandler OnRoadSegmentSpawned;

    public static void ResetStaticData()
    {
        OnPlayerEnteredRoadSegment = null;
        OnRoadSegmentSpawned = null;
    }

    [SerializeField] private Transform beginingPoint;
    [SerializeField] private Transform endingPoint;
    [SerializeField] private Transform centerPoint;
    [Tooltip("The minimum coordinates of the area where collectables can spawn")]
    [SerializeField] private Transform collectablesSpawnAreaMinCoords;
    [Tooltip("The maximum coordinates of the area where collectables can spawn")]
    [SerializeField] private Transform collectablesSpawnAreaMaxCoords;
    [SerializeField] private VheicleSpawner vheicleSpawner;

    [SerializeField] private GameObject collectablesContainer;

    [SerializeField] private Transform[] obstacles;

    [Tooltip("Countdown for the segment to despawn after the player leaves it"),SerializeField] 
    private float despawnTimer;    
    [SerializeField] private LayerMask playerMask;

    public Transform BeginingPoint 
    { get { return beginingPoint; } set { beginingPoint = value; } }
    public Transform EndingPoint 
    { get { return endingPoint; } set { endingPoint = value; } }
    public Transform CenterPoint 
    { get { return centerPoint; } set { centerPoint = value; } }
    public VheicleSpawner VheicleSpawner 
    { get { return vheicleSpawner; } set { vheicleSpawner = value; } }
    public Vector3 ForwardDirectionNormalized 
    { get; set; }

    private GameManager gameDifficultyController;

    private int blockersCount;
    private int blockEnableChance;

    private float minCollectablesAreaX;
    private float minCollectablesAreaZ;
    private float maxCollectablesAreaX;
    private float maxCollectablesAreaZ;
    private float CollectablesAreaY;

    private bool playerPassedSegment = false;

    private void Awake()
    {
        ForwardDirectionNormalized = (EndingPoint.position - BeginingPoint.position).normalized;
    }

    private void Start()
    {
        OnRoadSegmentSpawned?.Invoke(this, EventArgs.Empty);

        //setting the bounding box of the area that can spawn collactables
        minCollectablesAreaX = collectablesSpawnAreaMinCoords.position.x;
        minCollectablesAreaZ = collectablesSpawnAreaMinCoords.position.z;

        maxCollectablesAreaX = collectablesSpawnAreaMaxCoords.position.x;
        maxCollectablesAreaZ = collectablesSpawnAreaMaxCoords.position.z;
        CollectablesAreaY = centerPoint.position.y + 0.01f;

        gameDifficultyController = GameManager.Instance;
        readDifficulty();
        updateRoadBlockers();
        spawnCollectables();
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private void readDifficulty()
    {
        blockersCount = gameDifficultyController.BlockersEnableAttempts;
        blockEnableChance = GameManager.BlockerEnableChance;
    }

    private void spawnCollectables()
    {
        int collectablesToSpawn = gameDifficultyController.CollactablesSpawnCount;
        while (collectablesToSpawn-- > 0)
        {
            Transform collectableObject = gameDifficultyController.CollectableItem.Prefab;
            Transform collectable = Instantiate(collectableObject, collectablesContainer.transform);
            
            Vector3 position = new Vector3(UnityEngine.Random.Range(
                minCollectablesAreaX, maxCollectablesAreaX), CollectablesAreaY,
                UnityEngine.Random.Range(minCollectablesAreaZ, maxCollectablesAreaZ));

            collectable.position = position;
        }
    }

    private void updateRoadBlockers()
    {
        if (blockEnableChance == 0)
            return;
        int size = obstacles.Length;
        var rand = new System.Random();
        for (int i = 0; i < blockersCount; i++) 
        {
            if (rand.Next(0, 100 - blockEnableChance) == 0)
            {
                int chosenBlocker = rand.Next(0, size - 1);
                obstacles[chosenBlocker].gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameDifficultyController == null)
            return;
        if (other.gameObject.layer == gameDifficultyController.PlyayerLayerMask && !playerPassedSegment)
        {
            playerPassedSegment = true;
            OnPlayerEnteredRoadSegment?.Invoke(this, EventArgs.Empty);
        }
    }
}
