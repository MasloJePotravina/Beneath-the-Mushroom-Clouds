////////////////////////////////////////////////////
// File: playerControlsScr.cs                     //
// Project: Beneath the Mushroom Clouds           //
// Author: Ondrej Kováč                           //
// Brief: Control Scheme of the Player Character  //
////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class playerControls : MonoBehaviour
{

    private Rigidbody2D playerRigidbody;
    public playerStatus status;
    private GameInputActions inputActions;

    private float crouchSpeed = 20.0f;
    private float walkSpeed = 50.0f;
    private float sprintSpeed = 100.0f;

    private Vector2 movementInput;

    private void Awake()
    {
        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Crouch.performed -= CrouchToggle;

        inputActions.Player.Sprint.started -= SprintToggle;
        inputActions.Player.Sprint.canceled -= SprintToggle;
    }


    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to the corouch event and call the crouch toggle function
        inputActions.Player.Crouch.performed += CrouchToggle;

        inputActions.Player.Sprint.started += SprintToggle;
        inputActions.Player.Sprint.canceled += SprintToggle;
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movementInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Get the world coordinates of the mouse
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(inputActions.Player.MousePosition.ReadValue<Vector2>());
        //Calculate the new direction the player should face
        Vector2 direction = (mouseWorldPos - (Vector2)transform.position).normalized;
        float angle = Vector2.SignedAngle(new Vector2(0, 1), direction);
        playerRigidbody.MoveRotation(angle);
        //Change the velocity of the player according to movement
        playerRigidbody.MovePosition(playerRigidbody.position + movementInput * status.playerSpeed * Time.deltaTime);




    }

    //Crouch when standing, stand up when crouching
    private void CrouchToggle(InputAction.CallbackContext context)
    {
        if (!status.playerCrouched)
        {
            status.playerCrouched = true;
            status.playerSpeed = crouchSpeed;
        }
        else
        {
            status.playerCrouched = false;

            if (status.playerSprint)
                status.playerSpeed = sprintSpeed;
            else
                status.playerSpeed = walkSpeed;
        }
    }

    private void SprintToggle(InputAction.CallbackContext context)
    {
        if (!status.playerSprint)
        {
            status.playerCrouched = false;
            status.playerSprint = true;
            status.playerSpeed = sprintSpeed;
        }
        else
        {
            status.playerSprint = false;

            if (status.playerCrouched)
                status.playerSpeed = crouchSpeed;
            else
                status.playerSpeed = walkSpeed;
        }

    }

}
