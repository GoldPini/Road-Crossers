using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencyDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyDisplayText;

    private const string currencyIncreasedTrigger = "CurrencyIncreased";
    private uint currency;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Player_brain.OnPlayerCurrencyChanged += Player_brain_OnPlayerCurrencyChanged;
    }

    private void Player_brain_OnPlayerCurrencyChanged(object sender, OnPlayerCurrencyChangedEventArgs e)
    {
        currency = e.currency;
        currencyDisplayText.text = currency + ""; 
        animator.SetTrigger(currencyIncreasedTrigger);
    }
}
