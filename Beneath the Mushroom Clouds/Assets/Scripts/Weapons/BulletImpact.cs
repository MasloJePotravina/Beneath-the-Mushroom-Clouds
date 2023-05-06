using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the destruction of a bullet impact when its animation ends.
/// </summary>
public class BulletImpact : MonoBehaviour
{
    /// <summary>
    /// Destroys the bullet impact. Called by the animator of bullet impact.
    /// </summary>
    void DestroyBulletImpact()
    {
        Destroy(gameObject);
    }
}
