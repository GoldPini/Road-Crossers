using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFailedUI : MonoBehaviour
{
    [SerializeField] private Button replayButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI failMessageText;
    [SerializeField] private GameObject ui;

    private void Awake()
    {
        replayButton.onClick.AddListener(() =>
        {
            hide();
            GameManager.Instance.Restart();
        });
        exitButton.onClick.AddListener(() =>
        {
            hide();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
        hide();
    }

    private void Start()
    {
        RigidBodyCharacterController.OnLevelFailure += GameManager_OnLevelFailure;
        GameManager.OnGameReset += GameManager_OnGameReset; ;
    }

    private void GameManager_OnLevelFailure(object sender, OnGameFailureEventArgs e)
    {
        failMessageText.text = e.failureMessage;
        show();
    }

    private void GameManager_OnGameReset(object sender, System.EventArgs e)
    {
        hide();
    }

    private void show()
    {
        ui.SetActive(true);
    }

    private void hide()
    {
        ui.SetActive(false);
    }
}
