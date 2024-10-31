using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameLevelsUI levelsUI;
    [SerializeField] private SettingsUI settingsUI;
    [SerializeField] private GameObject UI;

    public event EventHandler OnNavigationToLevelsSelectionAction;
    public event EventHandler OnNavigationToSettingsAction;

    private void Awake()
    {
        Application.targetFrameRate = int.MaxValue;

        playButton.onClick.AddListener(() =>
        {
            hide();
            OnNavigationToLevelsSelectionAction?.Invoke(this, EventArgs.Empty);
        });
        
        settingsButton.onClick.AddListener(() =>
        {
            hide();
            OnNavigationToSettingsAction?.Invoke(this, EventArgs.Empty);
        });

        quitButton.onClick.AddListener(() =>
        {            
            Application.Quit();
        });
        show();
    }

    private void Start()
    {
        levelsUI.OnBackwardsNavigationAction += LevelsSelectionUI_OnBackwardsNavigationAction;
        settingsUI.OnBackwardsNavigationAction += SettingsUI_OnBackwardsNavigationAction;
    }

    private void SettingsUI_OnBackwardsNavigationAction(object sender, EventArgs e)
    {
        show();
    }

    private void LevelsSelectionUI_OnBackwardsNavigationAction(object sender, System.EventArgs e)
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
