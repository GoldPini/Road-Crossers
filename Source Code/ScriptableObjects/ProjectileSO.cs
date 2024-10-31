using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public enum ProjectileVehicleHitMode
{
    DestroyProjectile, ExplodeVehicle
}

[CreateAssetMenu(fileName = "ProjectileSO", menuName = "ScriptableObjects/Projectile SO")]
public class ProjectileSO : ScriptableObject
{
    [SerializeField] public Projectile Prefab;    

    [Space(10f), Header("Movement Settings")]
    [SerializeField] public float maxSpeed = 6f;
    [SerializeField, Range(0,1), 
        Tooltip("determines the projectile's ability to track. " +
        "0 - no tracking(projectiles follow a straight line). 1 - perfect tracking")]
    public float trackingAccuracy;
    [SerializeField, Range(0, 1), Tooltip("Determines how easly it is to lose the prjectile. the higher it is, the harder to shake off the missile")]
    public float missFactor;

    [Space(10f), Header("On Hit Settings")]
    [SerializeField] public float damage;
    [SerializeField, Tooltip("Determines the behivour of the projectile when it hits a vehicle that's not a target")] 
    public ProjectileVehicleHitMode VehicleHitReactionMode;
}
