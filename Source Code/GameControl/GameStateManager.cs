using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    WaitingToStart, CountdownToStart, GamePlaying, GameOver
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    [SerializeField, Tooltip("Countdown duration in seconds")] private float countDownTime = 3f;

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnGameTimeOver;

    private GameState gameState;
    private float waitingToStartTimer = 1f;
    private float countdownTimer = 3f;
    private float gameDurationTimer;
    private bool isGamePaused = false;

    private void Awake()
    {
        Instance = this;
        gameState = GameState.WaitingToStart;

        countdownTimer = countDownTime;
    }

    private void Start()
    {
        GameInputManager.Instance.OnPauseAction += GameInput_OnPauseAction;
        GamePausedMenuUI.OnUIUnpausedAction += GamePausedMenuUI_OnUIUnpausedAction;

        GameManager.OnGameReset += GameManager_OnGameReset;
        GameManager.OnLevelSuccess += GameManager_GameOver;
        RigidBodyCharacterController.OnLevelFailure += GameManager_GameOver;

        gameDurationTimer = GameManager.Instance.TimeLimit;
    }

    private void GameManager_GameOver(object sender, EventArgs e)
    {
        gameState = GameState.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GamePausedMenuUI_OnUIUnpausedAction(object sender, EventArgs e)
    {
        togglePauseGame();
    }

    private void GameManager_OnGameReset(object sender, EventArgs e)
    {
        gameDurationTimer = 0;
        countdownTimer = countDownTime;
        gameDurationTimer = GameManager.Instance.TimeLimit;
        gameState = GameState.CountdownToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        togglePauseGame();
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0f)
                {
                    gameState = GameState.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.CountdownToStart:
                countdownTimer -= Time.deltaTime;
                if (countdownTimer <= 0f)
                {
                    gameState = GameState.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GamePlaying:
                gameDurationTimer -= Time.deltaTime;
                if (gameDurationTimer <= 0)
                {
                    gameDurationTimer = 0;
                    gameState = GameState.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    OnGameTimeOver?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return gameState == GameState.GamePlaying || gameState == GameState.GameOver;
    }

    public bool IsCountdownToStartActive()
    {
        return gameState == GameState.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownTimer;
    }

    public float GetGameTime()
    {
        return gameDurationTimer;
    }

    public float GetGameTimeSinceStart()
    {
        return GameManager.Instance.TimeLimit - gameDurationTimer;
    }

    public bool IsGameOver()
    {
        return gameState == GameState.GameOver;
    }

    private void togglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
