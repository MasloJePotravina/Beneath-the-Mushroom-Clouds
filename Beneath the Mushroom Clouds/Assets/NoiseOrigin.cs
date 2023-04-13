using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOrigin : MonoBehaviour
{

    private LayerMask layerMask;


    void Awake(){
        layerMask = LayerMask.GetMask("NoiseReceiver");
    }


    public void GenerateNoise(float range){
        // Overlap a circle in front of the object and check for collisions on the specified layer
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, layerMask);

        // Loop through all colliders and call HeardNoise() method on the detected objects
        foreach (Collider2D collider in colliders)
        {
            collider.gameObject.GetComponent<NoiseReceiver>().HeardNoise(transform.position);
        }
    }
}
