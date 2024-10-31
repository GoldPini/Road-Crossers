using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MafiaVehicleDetectionAreaLogic : MonoBehaviour
{
    public event EventHandler OnPlayerEnteredDetectionArea;

    private GameManager gameController;

    private void Start()
    {
        gameController = GameManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (gameController == null)
            gameController = GameManager.Instance;
        if (other.gameObject.layer == gameController.PlyayerLayerMask)
        {
            OnPlayerEnteredDetectionArea?.Invoke(this, EventArgs.Empty);
        }
    }
}
