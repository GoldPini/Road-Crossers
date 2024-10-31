using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private Button replayButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    [SerializeField] private TextMeshProUGUI goldCollectedText;
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
        GameManager.OnLevelSuccess += GameManager_OnLevelSuccess;
        GameManager.OnGameReset += GameManager_OnGameReset; ;
    }

    private void GameManager_OnGameReset(object sender, System.EventArgs e)
    {
        hide();
    }

    private void GameManager_OnLevelSuccess(object sender, System.EventArgs e)
    {
        show();
    }

    private void show()
    {
        timeTakenText.text = GameStateManager.Instance.GetGameTimeSinceStart().ToString("F2");
        goldCollectedText.text = GameManager.Instance.GetCurrentPlayerCurrency() + "";
        ui.SetActive(true);
    }

    private void hide()
    {
        ui.SetActive(false);
    }
}
