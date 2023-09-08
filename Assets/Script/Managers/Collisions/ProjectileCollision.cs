using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Detected collision with: " + other.gameObject.name);

        if (other.CompareTag("Block"))
        {
            //Debug.Log("Projectile collided with Block: " + other.gameObject.name);
            Destroy(gameObject);
        }
    }
}
