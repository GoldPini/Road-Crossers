using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class Lane : MonoBehaviour
{
    //the parent game-object under which the vheicles will be spawned
    [SerializeField] private GameObject vheiclesContainer;
    [SerializeField] private GameObject spawnPlatform;    
    [SerializeField] private LayerMask mask;

    public static float s_delay = 1;

    private Queue<BaseVheicle> spawningQueue = new Queue<BaseVheicle>();
    private float time = 0f;
    private List<Collider> thingsInside = new List<Collider>();

    public void Spawn(BaseVheicle toSpawn)
    {
        spawningQueue.Enqueue(toSpawn);
    }


    private void Update()
    {
        emptyQueue();
    }

    private void emptyQueue()
    {
        time += Time.deltaTime;
        if (time >= s_delay && spawningQueue.Count != 0)
        {
            time = 0f;
            //if (!Physics.BoxCast(spawnPlatformCollider.bounds.center, spawnPlatform.transform.localScale * 0.5f,
            //    Vector3.up, spawnPlatform.transform.rotation, 10f, mask.value))
            if (thingsInside.Count == 0 || thingsInside.All(c => c == null))
            {
                BaseVheicle myCar = Instantiate(spawningQueue.Dequeue(), vheiclesContainer.transform);
                myCar.transform.position = this.transform.position;
                myCar.transform.forward = this.transform.forward;
                //Debug.Log("Spawned car with speed: " + myCar.Speed);
            }
            else
            {
                Debug.Log("Cannot spawn!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        thingsInside.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        thingsInside.Remove(other);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(spawnPlatform.transform.position + spawnPlatform.transform.forward * 5f, spawnPlatform.transform.localScale);
    }
}
