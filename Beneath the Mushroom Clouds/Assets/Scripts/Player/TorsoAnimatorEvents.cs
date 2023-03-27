using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoAnimatorEvents : MonoBehaviour
{
    public GameObject player;
    private PlayerControls playerControlsScript;
    private Animator firearmAnimator;
    public GameObject firearmSprite;

    private void Awake()
    {
        playerControlsScript = player.GetComponent<PlayerControls>();
        firearmAnimator = firearmSprite.GetComponent<Animator>();
    }

    public void setCrouchEnabled()
    {
        playerControlsScript.crouchEnabled = true;
    }
    public void setCrouchDisabled()
    {
        playerControlsScript.crouchEnabled = false;
    }

}
