using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCamOnAxis : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private Transform playerTransform;
    [Space]
    [SerializeField]
    private float camDistance = 8;


    private Vector3 startingPoint;
    private void Start()
    {
        startingPoint = playerTransform.position;
    }

    private void Update()
    {
        float movementOnX = startingPoint.x - playerTransform.position.x;
        //float x = virtualCamera.transform.position.x + movementOnX;
        float x = playerTransform.position.x;
        float y = virtualCamera.transform.position.y;
        float z = playerTransform.position.z - camDistance;

        virtualCamera.transform.position = new Vector3(x,y,z);        
    }
}
