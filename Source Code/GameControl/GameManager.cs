using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

public class OnLevelProgressChangeEventArgs : EventArgs
{
    public float progress;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameDifficultySO defaultDifficulty;
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private RigidBodyCharacterController player;
    [SerializeField] private Player_brain playerBrain;
    [Space]
    [SerializeField] private CollectableItem[] collectableItems;



    public static event EventHandler OnGameReset;
    public static event EventHandler OnSuddenDifficultyChange;
    public static event EventHandler OnRestartDecision;
    public static event EventHandler OnLevelSuccess;
    public static event EventHandler<OnLevelProgressChangeEventArgs> OnLevelProgressChange;

    public static void ResetStaticData()
    {
        OnGameReset = null;
        OnSuddenDifficultyChange = null;
        OnRestartDecision = null;
        OnLevelSuccess = null;
        OnLevelProgressChange = null;
    }

    public static GameManager Instance 
    {
        get; private set;
    }

    private const int PLAYER_MASK = 9;
    private const int VEHICLE_MASK = 6;
    private const int VEHICLE_FRONT_MASK = 7;
    private const int VEHICLE_DESTROYER_MASK = 8;
    private const int VEHICLE_OBSTACLE_MASK = 11;
    private const int COLLECTABLES_LAYER_MASK = 10;
    private const int FINISHING_SEGMENT_LAYER_MASK = 13;

    public RigidBodyCharacterController Player{ get { return player; } }

    public static GameDifficultySO Difficulty { get; set; }
    public int PlyayerLayerMask { get { return PLAYER_MASK; } }
    public int VehicleLayerMask { get { return VEHICLE_MASK; } }
    public int VehicleFrontLayerMask { get { return VEHICLE_FRONT_MASK; } }
    public int VehicleDestroyerMask { get { return VEHICLE_DESTROYER_MASK; } }
    public int VehicleObstacleMask { get { return VEHICLE_OBSTACLE_MASK; } }
    public int CollectablesLayerMask { get { return COLLECTABLES_LAYER_MASK; } }
    public int FinishingSegmentLayerMask { get { return FINISHING_SEGMENT_LAYER_MASK; } }

    public static int RandomizationFactor { get; private set; }
    //in seconds. how often the game attemts to spawn a car
    public static float TimeInterval { get; private set; }

    public static bool RandomizeSpeed { get; private set; }
    public static float MinSpeed { get; private set; }
    public static float MaxSpeed { get; private set; }
    public static float TimeIntervalDecreaseFactor { get; private set; }
    public static float LowestTimeInterval { get; private set; }
    public static float SpeedIncreaseFactor { get; private set; }
    public float TimeLimit { get; private set; }
    public int BlockersEnableAttempts { get 
        {
            return (int)math.floor(blockersSpawnGraph.Evaluate((float)spawnedSegments / TotalRoadSegments));
        }}   
    public static int BlockerEnableChance { get; private set; }
    public static int TotalRoadSegments { get; private set; }


    public CollectableItem CollectableItem //TODO implement logic for picking a collectable
    {
        get { return collectableItems[rand.Next(collectableItems.Length)]; }
    }
    public int CollactablesSpawnCount { 
        get { return collactablesSpawnCount; } }
    public BaseVheicle VheiclePrefab { 
        get { return vehicles[rand.Next(vehicles.Length)]; } }

    private System.Random rand = new System.Random();
    private int collactablesSpawnCount;
    private float levelProgressNormalized = 0;
    private int passedSegments = 0;
    private int spawnedSegments = 0;
    private AnimationCurve blockersSpawnGraph;
    private BaseVheicle[] vehicles;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning("More than one game managers!");
        Instance = this;

        readDifficultySO();
    }

    private void Start()
    {
        RoadSegment.OnPlayerEnteredRoadSegment += RoadSegment_OnPlayerEnteredRoadSegment;
        RoadSegment.OnRoadSegmentSpawned += RoadSegment_OnRoadSegmentSpawned; ;
        player.OnPlayerRespawned += Player_OnPlayerRespawned;
        player.OnPlayerLevelSuccess += Player_OnPlayerLevelSuccess;
    }

    private void RoadSegment_OnRoadSegmentSpawned(object sender, EventArgs e)
    {
        spawnedSegments++;
    }

    private void Player_OnPlayerLevelSuccess(object sender, EventArgs e)
    {
        OnLevelSuccess?.Invoke(this, EventArgs.Empty);
    }

    private void Player_OnPlayerRespawned(object sender, RigidBodyCharacterController.PlayerRespawnedEventArgs e)
    {
        passedSegments = 0;
        spawnedSegments = 0;
        levelProgressNormalized = 0;
        OnLevelProgressChange?.Invoke(this, new OnLevelProgressChangeEventArgs { progress = levelProgressNormalized });
        OnGameReset?.Invoke(this, EventArgs.Empty);
    }

    private void RoadSegment_OnPlayerEnteredRoadSegment(object sender, EventArgs e)
    {
        levelProgressNormalized = (float)++passedSegments / ((float)TotalRoadSegments);
        OnLevelProgressChange?.Invoke(this, new OnLevelProgressChangeEventArgs {progress = levelProgressNormalized});
    }

    public void Restart()
    {
        OnRestartDecision?.Invoke(this, EventArgs.Empty);
    }

    public int GetCurrentPlayerCurrency()
    {
        return playerBrain.GetPlayerCurrency();
    }

    public void readDifficultySO()
    {
        if (Difficulty == null)
            Difficulty = defaultDifficulty;
        RandomizationFactor = Difficulty.randomizationFactor;
        TimeInterval = Difficulty.timeInterval;
        RandomizeSpeed = Difficulty.randomizeSpeed;
        MinSpeed = Difficulty.initialMinSpeed;
        MaxSpeed = Difficulty.initialMaxSpeed;
        TimeIntervalDecreaseFactor = Difficulty.timeIntervalDecreaseFactor;
        LowestTimeInterval = Difficulty.lowestTimeInterval;
        SpeedIncreaseFactor = Difficulty.speedIncreaseFactor;
        BlockerEnableChance = Difficulty.BlockerEnableChance; 
        collactablesSpawnCount = Difficulty.collactablesSpawnCount;
        TotalRoadSegments = Difficulty.TotalSegmentsCount;
        blockersSpawnGraph = Difficulty.roadBlocksOverProgress;
        TimeLimit = Difficulty.TimeLimit;
        vehicles = Difficulty.vheicles;
    }
}
