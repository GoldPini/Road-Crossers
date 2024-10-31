using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public event EventHandler OnBackwardsNavigationAction;

    [SerializeField] private Button backButton;
    [SerializeField] private GameObject UI;
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [Header("Rebinding keys options")]
    [SerializeField] private Button moveForwardButton;
    [SerializeField] private Button moveBackButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button runButton;
    [SerializeField] private Button pauseButton;

    [SerializeField] private TextMeshProUGUI moveForwardButtonText;
    [SerializeField] private TextMeshProUGUI moveBackButtonText;
    [SerializeField] private TextMeshProUGUI moveRightButtonText;
    [SerializeField] private TextMeshProUGUI moveLeftButtonText;
    [SerializeField] private TextMeshProUGUI runButtonText;
    [SerializeField] private TextMeshProUGUI pauseButtonText;



    private void Awake()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        vsyncToggle.isOn = QualitySettings.vSyncCount == 1;

        backButton.onClick.AddListener(() =>
        {
            hide();
            OnBackwardsNavigationAction?.Invoke(this, EventArgs.Empty);
        });

        fullscreenToggle.onValueChanged.AddListener((bool state) =>
        {
            if (state)
            {
                Screen.SetResolution(1920, 1080, true);
                Debug.Log("Fullscreen on");
            }
            else
            {
                Screen.SetResolution(1920, 1080, false);
                Debug.Log("Fullscreen off");
            }
        });

        vsyncToggle.onValueChanged.AddListener((bool state) =>
        {
            if (state)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        });

        masterVolumeSlider.onValueChanged.AddListener((float level) =>
        {
            SoundMixerManager.Instance.SetMasterVolume(level);
        });
        
        musicVolumeSlider.onValueChanged.AddListener((float level) =>
        {
            SoundMixerManager.Instance.SetMusicVolume(level);
        });
        
        sfxVolumeSlider.onValueChanged.AddListener((float level) =>
        {
            SoundMixerManager.Instance.SetSFXVolume(level);
        });

        hide();
    }

    private void Start()
    {
        mainMenuUI.OnNavigationToSettingsAction += MainMenuUI_OnNavigationToSettingsAction; ;
    }

    private void MainMenuUI_OnNavigationToSettingsAction(object sender, EventArgs e)
    {
        show();
    }

    private void show()
    {
        UI.SetActive(true);
        masterVolumeSlider.value = SoundMixerManager.Instance.GetMasterVolume();
        musicVolumeSlider.value = SoundMixerManager.Instance.GetMusicVolume();
        sfxVolumeSlider.value = SoundMixerManager.Instance.GetSFXVolume();
    }

    private void hide()
    {
        UI.SetActive(false);
    }
}
