using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    private float x;
    private float y;
    private float lastZ;

    private void Start()
    {
        RigidBodyCharacterController playerController = player.GetComponent<RigidBodyCharacterController>();
        playerController.OnPlayerRespawned += Player_OnPlayerRespawned;
        
        x = player.position.x;
        y = player.position.y;
        lastZ = player.position.z;
    }

    private void Player_OnPlayerRespawned(object sender, RigidBodyCharacterController.PlayerRespawnedEventArgs e)
    {
        this.transform.position = e.respawnPosition;
        lastZ = e.respawnPosition.z;
    }

    private void LateUpdate()
    {
        float z = player.position.z;
        lastZ = z > lastZ ? z : lastZ;
        this.transform.position = new Vector3(x, y, lastZ);
    }
}
