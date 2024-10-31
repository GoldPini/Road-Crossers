using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAnimationController : MonoBehaviour
{
    private const string VEHICLE_STOPPED_TRIGGER = "JustStoped";
    private const string VEHICLE_STOPPING_TRIGGER = "Stopping";
    private const string VEHICLE_EXPLODING = "Explode";
    private const string VEHICLE_VELOCITY = "VehicleVelocity";

    private Animator animator;
    private BaseVheicle vheicle;

    private void Start()
    {
        vheicle = GetComponent<BaseVheicle>();
        animator = GetComponentInChildren<Animator>();

        vheicle.OnVheicleStopped += Vheicle_OnVheicleStopped;
        vheicle.OnVehicleSpeedChanged += Vheicle_OnVehicleSpeedChanged;
        vheicle.OnVheicleExplode += Vheicle_OnVheicleExplode;
    }

    private void Vheicle_OnVheicleExplode(object sender, System.EventArgs e)
    {
        animator.SetTrigger(VEHICLE_EXPLODING);
    }

    private void Vheicle_OnVehicleSpeedChanged(object sender, RigidBodyVheicleController.OnVehicleSpeedChangedEventArgs e)
    {
        animator.SetFloat(VEHICLE_VELOCITY, e.speedNormalized);
    }

    private void Vheicle_OnVheicleStopped(object sender, System.EventArgs e)
    {
         
    }
}
