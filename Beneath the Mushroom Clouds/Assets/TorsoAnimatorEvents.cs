using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoAnimatorEvents : MonoBehaviour
{
    public GameObject player;
    private PlayerControls playerControlsScript;

    private void Awake()
    {
        playerControlsScript = player.GetComponent<PlayerControls>();
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
