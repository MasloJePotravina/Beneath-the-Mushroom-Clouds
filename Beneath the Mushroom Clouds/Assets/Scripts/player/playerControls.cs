////////////////////////////////////////////////////
// File: playerControlsScr.cs                     //
// Project: Beneath the Mushroom Clouds           //
// Author: Ondrej Kováč                           //
// Brief: Control Scheme of the Player Character  //
////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControls : MonoBehaviour
{
    
    private Rigidbody2D playerRigidbody;

    private InputProvider playerInput;

    public playerStatus status;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = new InputProvider();
        playerInput.Enable();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get the world coordinates of the mouse
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(playerInput.MousePosition());
        //Calculate the new direction the player should face
        Vector2 direction = (mouseWorldPos - (Vector2)transform.position).normalized;
        transform.up = direction;
        //Change the velocity of the player according to movement
        playerRigidbody.velocity = playerInput.MoveInput() * status.playerSpeed;


    }
}
