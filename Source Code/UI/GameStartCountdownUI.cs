using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    public static event EventHandler OnCountdownStarted;

    public static void ResetStaticData()
    {
        OnCountdownStarted = null;
    }

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        hide();
    }

    private void Update()
    {
        countdownText.text = Mathf.Ceil(GameStateManager.Instance.GetCountdownToStartTimer()) + "";
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameStateManager.Instance.IsCountdownToStartActive())
        {
            show();
        }
        else 
        { 
            hide(); 
        }
    }

    private void show()
    {
        OnCountdownStarted?.Invoke(this, EventArgs.Empty); 
        gameObject.SetActive(true);
    }

    private void hide()
    {
        gameObject.SetActive(false);
    }
}
