using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as an origin for noises the NPCs can hear.
/// </summary>
public class NoiseOrigin : MonoBehaviour
{
    /// <summary>
    /// LayerMask for Noise Receivers.
    /// </summary>
    private LayerMask layerMask;

    /// <summary>
    /// When the script is activated the layer mask for noise receivers is set.
    /// </summary>
    void Awake(){
        layerMask = LayerMask.GetMask("NoiseReceiver");
    }

    /// <summary>
    /// Generates a noise at the position of the object with the specified range. Notifies all Noise Receivers in the range.
    /// </summary>
    /// <param name="range">How far does the sound reach,</param>
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
