using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstepSoundLogic : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float walkingFootstepSoundPlayInterval = 0.2f;
    [SerializeField, Range(0, 1)] private float runningFootstepSoundPlayInterval = 0.1f;

    private RigidBodyCharacterController characterController;
    private float timer = 0f;

    private void Start()
    {
        characterController = GetComponent<RigidBodyCharacterController>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (characterController.IsPlayerRunning())
        {
            if(timer > runningFootstepSoundPlayInterval)
            {
                timer = 0f;
                SoundManager.Instance.PlayFootStepSound(transform);
            }
        }
        else if(characterController.IsPlayerWalking())
        {
            if (timer > walkingFootstepSoundPlayInterval)
            {
                timer = 0f;
                SoundManager.Instance.PlayFootStepSound(transform);
            }
        }
    }
}
