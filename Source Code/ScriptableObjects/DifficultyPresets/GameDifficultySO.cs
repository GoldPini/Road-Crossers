using UnityEngine;

public enum LevelMode
{
    Infinite, Finite
}

[CreateAssetMenu(fileName = "DifficultyPresetSO", menuName = "ScriptableObjects/Game Difficulty SO", order = 1)]
public class GameDifficultySO : ScriptableObject
{
    [Header("Vehicles Spawn Settings")]
    //chance to spawn a car upon attempt
    public int randomizationFactor = 3;
    //in seconds. how often the game attemts to spawn a car
    public float timeInterval = 5;
    public BaseVheicle[] vheicles;
    [Space(15f), Header("Vehicles Speed Settings")]
    public bool randomizeSpeed = true;
    public float initialMinSpeed = 10;
    public float initialMaxSpeed = 15;
    public float timeIntervalDecreaseFactor = 0.1f;
    public float lowestTimeInterval = 1f;
    public float speedIncreaseFactor = 2f;
    [Space(15f), Header("Road Blocks Settings")]
    [Range(0, 100), Tooltip("The chance of a road block to spawn opun an attempt in precentege " +
        "('0' = no chance, '100' = guranteed to spawn )")]
    public int BlockerEnableChance = 50;
    public AnimationCurve roadBlocksOverProgress;
    [Space(15f), Header("Collectable Items Settings")]
    public int collactablesSpawnCount = 5;
    public AnimationCurve collectableSpawnOverProgress;
    [Space(15f), Header("Road Generation Settings")]
    [Tooltip("The total amount of segments to be generated. leave 'int.MAX' for infinite level")]
    public int TotalSegmentsCount = 40;
    [Space(15f), Header("Time Settings")]
    [Range(0, 300), Tooltip("The time in seconds in which the player must finish the level")]
    public int TimeLimit = 60;
    [Space(15f), Header("UI Settings")]
    [Tooltip("The time in seconds in which the player must finish the level")]
    public string levelName = "e.g: Level 1";
    [Range(0,10)]
    public int difficultyRating = 1;
    [Range(0,10)]
    public int lengthRating = 1;
}
