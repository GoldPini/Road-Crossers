using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneSpawnDelayController : MonoBehaviour
{
    [Tooltip("Sets the spawn delay for all the lanes in the scene")]
    [SerializeField] private float delay = 1f;

    private void FixedUpdate()
    {
        if(Lane.s_delay != delay)
            Lane.s_delay = delay;
    }
}
