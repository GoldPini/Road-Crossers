using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MafiaCar_Brain : MonoBehaviour
{
    [SerializeField] private float speedBoostOnDetecion = 20;
    [SerializeField] private Transform castingCenterPoint;
    [SerializeField] private MafiaVehicleDetectionAreaLogic mafiaVehicleDetectionAreaLogic;

    private GameManager gameController;
    private BaseVheicle vehicle;
    private void Start()
    {
        gameController = GameManager.Instance;
        vehicle = GetComponent<BaseVheicle>();
        mafiaVehicleDetectionAreaLogic.OnPlayerEnteredDetectionArea += OnPlayerEnteredDetectionArea;
        mafiaVehicleDetectionAreaLogic.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (this.transform.position.IsInsideViewScreen(Camera.main))
        {
            mafiaVehicleDetectionAreaLogic.gameObject.SetActive(true);
        }
    }

    private void OnPlayerEnteredDetectionArea(object sender, System.EventArgs e)
    {
        Vector3 dirToPlayer = gameController.Player.transform.position - castingCenterPoint.position;
        float distanceToPlayer = Vector3.Distance(gameController.Player.transform.position, castingCenterPoint.position);
        if (!Physics.Raycast(castingCenterPoint.position, dirToPlayer, distanceToPlayer - 0.5f))
        {
            Debug.Log("Player Detected By Mafia Car!!!!");
            vehicle.Speed += speedBoostOnDetecion;
        }
    }
}
