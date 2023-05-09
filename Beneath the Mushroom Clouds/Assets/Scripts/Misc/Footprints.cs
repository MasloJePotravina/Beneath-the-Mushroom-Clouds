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
    /// Whether the particle system should emit snow footprints.
    /// </summary>
    private bool snowFootPrintsEnabled = true;

    /// <summary>
    /// Emission module of the particle system.
    /// </summary>
    ParticleSystem.EmissionModule emission;



    
    /// <summary>
    /// Get reference to the particle system and save the old position.
    /// </summary>
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        oldPosition = transform.position;
        emission = particleSystem.emission;

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

        if(!snowFootPrintsEnabled && bloodySoleDistance <= 0f){
            emission.enabled = false;
        }else{
            emission.enabled = true;
        }

        
    }

    /// <summary>
    /// When the trigger on the footprint emitter enters a blood puddle, make the soles bloody. When it leaves wooden floor, enable snow footprints.
    /// </summary>
    /// <param name="other">The collider of the object which left the area of the footprint emitter's collider.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "BloodPuddle")
        {
            bloodySoleDistance = 100f;    
        }

        if (other.gameObject.CompareTag("WoodenFloor"))
        {
            snowFootPrintsEnabled = true;
        }

    }

    /// <summary>
    /// While the footprint emitter of the character are within a wooden floor, the foorprints will only show if the character stepped in blood.
    /// </summary>
    /// <param name="other">The collider of the object which is currently within the area of the footprint emitter's collider.</param>
    void OnTriggerStay2D(Collider2D other)
    {

        
        if (other.gameObject.CompareTag("WoodenFloor"))
        {
            snowFootPrintsEnabled = false;
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
