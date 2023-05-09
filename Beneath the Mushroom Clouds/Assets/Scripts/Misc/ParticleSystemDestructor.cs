using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ensures the destruction of particle system after all of the particles were destroyed.
/// </summary>
public class ParticleSystemDestructor : MonoBehaviour
{
    /// <summary>
    /// Reference to the particle system.
    /// </summary>
    private ParticleSystem particleSystem;

    /// <summary>
    /// Gets the reference to the particle system when enabled.
    /// </summary>
    void OnEnable()
    {
        this.transform.parent = null;
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.rateOverDistance = 0f;
    }


    /// <summary>
    /// Each frame checks if the particle system is still alive (has live particles) and destroys the game object if not.
    /// </summary>
    void Update()
    {
        if(!particleSystem.IsAlive())
        {
            Destroy(this.gameObject);
        }
    }
}
