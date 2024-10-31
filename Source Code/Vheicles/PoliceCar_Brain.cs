using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCar_Brain : MonoBehaviour
{
    private const string FIRING_TRIGGER = "Fireing";
    private const string FIRING_STATE_NAME = "Firing";

    [SerializeField] private ProjectileSO projectileSO;
    [SerializeField] private Transform cannon;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float delayBetweenFirings = 2f;
    [SerializeField] private float maxDetectionDistance;
    [SerializeField] private Animator cannonAnimator;

    private BaseVheicle containingVehicle;
    private Transform target;
    private GameManager gameController;
    private float timeSinceLastFire = 0;


    private void Start()
    {
        gameController = GameManager.Instance;
        target = gameController.Player.ProjectileFocusPoint;
        containingVehicle = GetComponent<BaseVheicle>();
    }

    private void Update()
    {
        bool vehicleCanFire = containingVehicle.State == VheicleState.Stationed || containingVehicle.State == VheicleState.Moving;
        if (vehicleCanFire)
        {
            Vector3 dirToPlayer = target.position - projectileSpawnPoint.position;
            cannon.forward = Vector3.Slerp(cannon.forward, dirToPlayer, 5f * Time.deltaTime);
            float distanceToPlayer = Vector3.Distance(target.position, projectileSpawnPoint.position);

            if (timeSinceLastFire > delayBetweenFirings)
            {
                //firing if the player is within the distance AND no obscuring object is in the way
                if (distanceToPlayer < maxDetectionDistance && !Physics.Raycast(projectileSpawnPoint.position, 
                    dirToPlayer, distanceToPlayer - 1))
                {
                    fireProjectile();
                } 
            }
        }
        timeSinceLastFire += Time.deltaTime;
    }

    private void fireProjectile()
    {
        Projectile projectile = Instantiate(projectileSO.Prefab);
        projectile.transform.position = projectileSpawnPoint.position;
        projectile.TargetTransform = target;
        timeSinceLastFire = 0;
        cannonAnimator.Play(FIRING_STATE_NAME);
    }
}
