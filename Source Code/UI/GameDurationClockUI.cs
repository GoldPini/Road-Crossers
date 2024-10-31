using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDurationClockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;

    private void FixedUpdate()
    {
        clockText.text = GameStateManager.Instance.GetGameTime().ToString("F2");
    }
}
