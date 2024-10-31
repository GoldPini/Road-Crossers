using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLevelsUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button playButton;
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private GameObject UI;

    public event EventHandler OnBackwardsNavigationAction;

    private GameDifficultySO chosenLevelDifficultySO = null;

    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            hide();
            OnBackwardsNavigationAction?.Invoke(this, EventArgs.Empty);
        });
        
        playButton.onClick.AddListener(() =>
        {
            hide();
            GameManager.Difficulty = chosenLevelDifficultySO;
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        });

        playButton.interactable = false;
        hide();
    }

    private void Start()
    {
        mainMenuUI.OnNavigationToLevelsSelectionAction += MainMenuUI_OnNavigationToLevelsSelectionAction;
        LevelUI.OnLevelSelected += LevelUI_OnLevelSelected;
        LevelUI.OnLevelUnselected += LevelUI_OnLevelUnselected;
    }

    private void Update()
    {
        if(chosenLevelDifficultySO == null)
        {
            playButton.interactable = false;
        }
        else
        {
            playButton.interactable = true;
        }
    }

    private void LevelUI_OnLevelUnselected(object sender, EventArgs e)
    {
        chosenLevelDifficultySO = null;
    }

    private void LevelUI_OnLevelSelected(object sender, EventArgs e)
    {
        chosenLevelDifficultySO = (sender as LevelUI).gameDifficultySO;
    }

    private void MainMenuUI_OnNavigationToLevelsSelectionAction(object sender, EventArgs e)
    {
        show();
    }

    private void show()
    {
        UI.SetActive(true);
    }

    private void hide()
    {
        UI.SetActive(false);
    }
}
