using Cinemachine;
using UnityEngine;
using static RigidBodyCharacterController;

public class CemeraFollowLogic : MonoBehaviour
{
    [SerializeField] private RigidBodyCharacterController player;

    [SerializeField] private float delayAfterOverrun = 2f;

    private float timer = 0f;    
    private CinemachineVirtualCamera cam;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (timer != 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if(cam != null) 
                    cam.enabled = false;
                if (timer < 0)
                    timer = 0f;
            }
        }            
    }
}
