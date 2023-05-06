using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements behaviour of footprints left by the characters in game.
/// </summary>
public class Footprints : MonoBehaviour
{   
    /// <summary>
    /// Reference to the particle system of the footprints.
    /// </summary>
    private ParticleSystem particleSystem;

    /// <summary>
    /// How far the player still has to walk to wipe the blood off their soles.
    /// </summary>
    private float bloodySoleDistance = 0f;

    /// <summary>
    /// Old position of the gameObject to which the footprints are attached (kegs of the character).
    /// </summary>
    private Vector3 oldPosition;
    
    /// <summary>
    /// Get reference to the particle system and save the old position.
    /// </summary>
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        oldPosition = transform.position;

    }

    /// <summary>
    /// Each update set the rotation of the footsteps amd update the color of the footsteps when bloody.
    /// </summary>
    void Update()
    {
        particleSystem.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 distanceMoved = transform.position - oldPosition;

        bloodySoleDistance -= distanceMoved.magnitude;
        if(bloodySoleDistance < 0f)
        {
            bloodySoleDistance = 0f;
        }
        oldPosition = transform.position;
        UpdateFootstepColor();
        
    }

    /// <summary>
    /// When the trigger on the legs enters a blood puddle, make the soles bloody.
    /// </summary>
    /// <param name="other">The object which triggered the legs collider.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "BloodPuddle")
        {
            bloodySoleDistance = 100f;    
        }
    }

    /// <summary>
    /// Updates the foorstep color each frame based on the blood on the soles.
    /// </summary>
    void UpdateFootstepColor(){
        float bloodMultiplier = 1 - (bloodySoleDistance / 100f);
        particleSystem.startColor = new Color(0.5f - bloodMultiplier * 0.5f, 0f , 0f, 0.9f - bloodMultiplier * 0.3f);
    }
}
