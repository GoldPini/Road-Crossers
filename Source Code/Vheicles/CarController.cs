using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 10;

    //private float maxCollisionDistance = 4f;

    private void Update()
    {
        Vector3 facingDirection = transform.forward;

        float moveDistance = moveSpeed * Time.deltaTime;
        float objectHeight = transform.position.y;
        transform.position += facingDirection * moveDistance;

        //bool canMove = !Physics.BoxCast(transform.position, transform.position + Vector3.up * objectHeight, facingDirection, Quaternion.Euler(Vector3.zero), maxCollisionDistance);

        //if (canMove)
        //{
        //    
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "CarDestroyerTrigger")
        {
            Destroy(this.gameObject);
        }
    }
}
