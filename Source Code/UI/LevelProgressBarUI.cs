using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class LevelProgressBarUI : MonoBehaviour
{
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private RectTransform playerIconTransform;

    private float actualProgressBarLength;
    private float progress = 0;
    private float originalPlayerIconPositionY;

    private void Awake()
    {        
        actualProgressBarLength = progressBar.rect.height;
        originalPlayerIconPositionY = playerIconTransform.transform.position.y;
    }

    public void Start()
    {
        GameManager.OnLevelProgressChange += GameManager_OnLevelProgressChange; ;   
    }

    private void GameManager_OnLevelProgressChange(object sender, OnLevelProgressChangeEventArgs e)
    {
        progress = e.progress;
        playerIconTransform.position = new Vector3(
            playerIconTransform.position.x, originalPlayerIconPositionY + progress * actualProgressBarLength);      
    }
}
