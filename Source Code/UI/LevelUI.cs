using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static event EventHandler OnLevelSelected;
    public static event EventHandler OnLevelUnselected;

    public static void ResetStaticData()
    {
        OnLevelSelected = null;
        OnLevelUnselected = null;
    }

    [SerializeField] public GameDifficultySO gameDifficultySO;
    [Space(15)]
    [Space(8), Header("UI Properties")]
    [SerializeField] private UnityEngine.UI.Image outlineBackgroundImage;
    [SerializeField] private UnityEngine.UI.Image notSelectedBackgroundImage;
    [SerializeField] private TextMeshProUGUI LevelNameText;
    [SerializeField] private Color selectedOutlineColor;
    [Space(8), Header("Level Difficulty Display")]
    [SerializeField] private StarsRatingUI difficultyStarsRating;
    [SerializeField] private StarsRatingUI lengthStarsRating;

    private bool mouseOver = false;
    private bool isSelected = false;
    private Color notSelectedOutlineColor;

    private void Awake()
    {
        notSelectedBackgroundImage.enabled = true;
        LevelNameText.text = gameDifficultySO.levelName;
        notSelectedOutlineColor = outlineBackgroundImage.color;
    }

    private void Start()
    {
        difficultyStarsRating.SetStarRating(gameDifficultySO.difficultyRating);
        lengthStarsRating.SetStarRating(gameDifficultySO.lengthRating);
        OnLevelSelected += LevelUI_OnLevelSelected;
    }

    private void LevelUI_OnLevelSelected(object sender, EventArgs e)
    {
        if((sender as LevelUI) != this)
        {
            isSelected = false;
        }
    }

    private void Update()
    {
        if(mouseOver || isSelected)
        {
            notSelectedBackgroundImage.enabled = false;
            outlineBackgroundImage.color = selectedOutlineColor;
        }
        else
        {
            notSelectedBackgroundImage.enabled = true;
            outlineBackgroundImage.color = notSelectedOutlineColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;
        if(isSelected)
        {
            OnLevelSelected?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnLevelUnselected?.Invoke(this, EventArgs.Empty);
        }
    }
}
