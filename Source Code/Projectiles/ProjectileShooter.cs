using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private ProjectileSO projectileSO;
    [SerializeField] private float detectionRadius;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Transform projectileSpawnPointTransfrom;
    [SerializeField] private float delayBetweenFirings = 2f;

    private float timeSinceLastFire = 0;

    private void Update()
    {
        if (timeSinceLastFire >= delayBetweenFirings)
        {
            Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, detectionRadius);
            foreach (var c in colliders)
            {
                if (c.gameObject == targetObject)
                    shootProjectile();
            }
        }
        timeSinceLastFire += Time.deltaTime;
    }

    private void shootProjectile()
    {
        Projectile projectilePrefab = Instantiate(projectileSO.Prefab, projectileSpawnPointTransfrom.position, Quaternion.identity,this.transform);
        projectilePrefab.TargetTransform = targetObject.transform;
        timeSinceLastFire = 0;                   
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(this.gameObject.transform.position, detectionRadius);
    }
}
