using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public enum ProjectileState
{
    Alive, Destroyed
}

public class Projectile : MonoBehaviour
{
    public static event EventHandler OnAnyProjectileFired;
    public static event EventHandler OnAnyProjectileExploded;
    public static event EventHandler OnAnyPojectileCollidesPlayer;

    public static void ResetStaticData()
    {
        OnAnyPojectileCollidesPlayer = null;
        OnAnyProjectileExploded = null;
        OnAnyProjectileFired = null;
    }

    [SerializeField] private ProjectileSO projectileSO;
    [SerializeField] private Transform projectileModel;
    [SerializeField] private Transform explosionParticlesObejct;
    [SerializeField] private VisualEffect smokeTrailVFX;

    private event EventHandler OnPojectileCollides;

    private Transform targetTransform;
    private Vector3 directionToTarget;
    private float maximumSpeed;
    private float currentSpeed;
    private float trackingAccuracy;
    private float distanceToTarget;
    private bool lockedOnTarget = true;
    private GameManager gameController;
    private ProjectileState projectileState;
    private ParticleSystem explosionParticleSystem;

    public Transform TargetTransform
    {
        set { targetTransform = value; }
    }       

    private void Start()
    {
        OnAnyProjectileFired?.Invoke(this, EventArgs.Empty);

        gameController = GameManager.Instance;
        projectileState = ProjectileState.Alive;

        explosionParticleSystem = explosionParticlesObejct.GetComponent<ParticleSystem>();
        explosionParticlesObejct.gameObject.SetActive(false);

        if (projectileSO == null)
            Debug.LogWarning("Projectile scriptable-object refrence is missing");
        if (targetTransform == null)
            Debug.LogWarning("Projectile has no target");
        if (projectileModel == null)
            Debug.LogWarning("Projectile Model refrence is missing");
        if (explosionParticlesObejct == null)
            Debug.LogWarning("Projectile explosion particles refrence is missing");

        maximumSpeed = projectileSO.maxSpeed;
        trackingAccuracy = projectileSO.trackingAccuracy;

        directionToTarget = targetTransform.position - transform.position;
        distanceToTarget = Vector3.Distance(targetTransform.position, transform.position);
        transform.forward = directionToTarget;

        currentSpeed = maximumSpeed;

        OnPojectileCollides += Projectile_OnPojectileCollides;
    }

    private void Projectile_OnPojectileCollides(object sender, EventArgs e)
    {
        explosionParticlesObejct.gameObject.SetActive(true);
        projectileModel.gameObject.SetActive(false);
        projectileState = ProjectileState.Destroyed;
        smokeTrailVFX.Stop();

        OnAnyProjectileExploded?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        switch (projectileState)
        {
            case ProjectileState.Alive:
                if (currentSpeed < maximumSpeed)
                {
                    currentSpeed += 2f;
                    currentSpeed = math.min(currentSpeed, maximumSpeed);
                }

                if (lockedOnTarget)
                {
                    directionToTarget = targetTransform.position - transform.position;
            
                    //rotation direction
                    transform.forward = Vector3.Slerp(transform.forward, directionToTarget, 13 * trackingAccuracy * Time.deltaTime);
            
                    //recalculating distance and breaking projectile lock on target if needed
                    float newDistanceToTarget = Vector3.Distance(targetTransform.position, transform.position);
                    if (newDistanceToTarget > distanceToTarget + projectileSO.missFactor)
                        lockedOnTarget = false;
                    distanceToTarget = newDistanceToTarget; 
                }

                //movement
                transform.position = transform.position + transform.forward * currentSpeed * Time.deltaTime;

                //destoying the projectile if it exits the screen
                if (!transform.position.IsInsideViewScreen(Camera.main))
                    destroySelf(3f);

                break;

            case ProjectileState.Destroyed:
                //destoying the projectile object only after the explosion particles are done
                if(!explosionParticleSystem.isEmitting && smokeTrailVFX.aliveParticleCount > 0) 
                {
                    destroySelf();
                }
                break;
        }
    }

    private void destroySelf(float countDown = 0)
    {
        Destroy(this.gameObject, countDown);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform == targetTransform)
        {
            OnAnyPojectileCollidesPlayer?.Invoke(this, EventArgs.Empty);
        }
        if (other.gameObject.layer == gameController.VehicleLayerMask)
            switch (projectileSO.VehicleHitReactionMode)
            {                
                case ProjectileVehicleHitMode.DestroyProjectile:
                    break;
                case ProjectileVehicleHitMode.ExplodeVehicle:
                    other.gameObject.GetComponentInParent<BaseVheicle>().Explode();
                    break;
            }            
        OnPojectileCollides?.Invoke(this, EventArgs.Empty);
    }
}
