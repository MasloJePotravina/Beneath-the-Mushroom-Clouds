using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements animator events for torso.
/// </summary>
public class TorsoAnimatorEvents : MonoBehaviour
{
    /// <summary>
    /// Reference to the player.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Reference to the player controls script.
    /// </summary>
    private PlayerControls playerControlsScript;

    /// <summary>
    /// Reference to the animator of the firearm.
    /// </summary>
    private Animator firearmAnimator;

    /// <summary>
    /// Rference to the firearm sprite.
    /// </summary>
    public GameObject firearmSprite;

    /// <summary>
    /// Gets the necessary references on awake.
    /// </summary>
    private void Awake()
    {
        playerControlsScript = player.GetComponent<PlayerControls>();
        firearmAnimator = firearmSprite.GetComponent<Animator>();
    }

    /// <summary>
    /// Enables crouching for the player.
    /// </summary>
    public void setCrouchEnabled()
    {
        playerControlsScript.crouchEnabled = true;
    }

    /// <summary>
    /// Disables crouching for the player.
    /// </summary>
    public void setCrouchDisabled()
    {
        playerControlsScript.crouchEnabled = false;
    }

}
