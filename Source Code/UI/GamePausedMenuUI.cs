using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePausedMenuUI : MonoBehaviour
{
    public static event EventHandler OnUIUnpausedAction;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    public static void ResetStaticData()
    {
        OnUIUnpausedAction = null;
    }

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            OnUIUnpausedAction?.Invoke(this, EventArgs.Empty);
        });
        restartButton.onClick.AddListener(() =>
        {
            hide();
            OnUIUnpausedAction?.Invoke(this, EventArgs.Empty);
            GameManager.Instance.Restart();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameStateManager.Instance.OnGamePaused += GameStateManager_OnGamePaused;
        GameStateManager.Instance.OnGameUnpaused += GameStateManager_OnGameUnpaused;
        hide();
    }

    private void GameStateManager_OnGameUnpaused(object sender, EventArgs e)
    {
        hide();
    }

    private void GameStateManager_OnGamePaused(object sender, EventArgs e)
    {
        show();
    }

    private void show()
    {
        gameObject.SetActive(true);
    }

    private void hide()
    {
        gameObject.SetActive(false);
    }
}
