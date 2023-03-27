using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of the legs animation events.
/// </summary>
public class LegsAnimationEvents : MonoBehaviour
{   
    /// <summary>
    /// Reference to the player.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Reference to the Player Controls script.
    /// </summary>
    private PlayerControls playerControlsScript;

    /// <summary>
    /// On awake, initialize the Player Controls script.
    /// </summary>
    private void Awake()
    {
        playerControlsScript = player.GetComponent<PlayerControls>();
    }

    /// <summary>
    /// Called from the animation event. Enables crouching.
    /// </summary>
    public void setCrouchEnabled()
    {
        playerControlsScript.crouchEnabled = true;
    }

    /// <summary>
    /// Called from the animation event. Disables crouching.
    /// </summary>
    public void setCrouchDisabled()
    {
        playerControlsScript.crouchEnabled = false;
    }
}
