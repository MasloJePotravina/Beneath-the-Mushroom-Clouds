using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of the bed object.
/// </summary>
public class BedBehaviour : MonoBehaviour
{   
    /// <summary>
    /// Reference to the HUD controller.
    /// </summary>
    private HUDController hudController;

    /// <summary>
    /// Gets the reference to the HUD controller on awake.
    /// </summary>
    public void Awake(){
        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
    }

    /// <summary>
    /// Activates rest menu when the player interacts with the bed.
    /// </summary>
    public void Sleep(){
        hudController.ActivateRestMenu();
    }
}
