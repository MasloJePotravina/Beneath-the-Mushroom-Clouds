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



public class PlayerControls : MonoBehaviour
{

    private Rigidbody2D playerRigidbody;
    public PlayerStatus status;
    private GameInputActions inputActions;

    public GameObject playerTorso;
    public GameObject playerLegs;
    public GameObject playerHeadPivot;

    private float crouchSpeed = 20.0f;
    private float walkSpeed = 50.0f;
    private float sprintSpeed = 100.0f;

    private Vector2 movementInput;

    private Animator torsoAnimator;
    private Animator legsAnimator;
    private Quaternion prevTorsoRotation;

    public bool crouchEnabled;

    private void Awake()
    {
        inputActions = new GameInputActions();
        crouchEnabled = true;
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

        prevTorsoRotation = playerTorso.transform.rotation;
        torsoAnimator = playerTorso.GetComponent<Animator>();
        legsAnimator = playerLegs.GetComponent<Animator>();
    }

    void Update()
    {
        movementInput = inputActions.Player.Move.ReadValue<Vector2>();
        animateTorso(playerTorso);
        animateLegs(playerLegs);
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
        playerHeadPivot.transform.rotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: direction);
        //Change the velocity of the player according to movement
        playerRigidbody.MovePosition(playerRigidbody.position + status.playerSpeed * Time.deltaTime * movementInput);

        if (inputActions.Player.Aiming.ReadValue<float>() > 0.1f)
        {
            status.playerAiming = true;
        }
        else
        {
            status.playerAiming = false;
        }

        



    }

    //Crouch when standing, stand up when crouching
    private void CrouchToggle(InputAction.CallbackContext context)
    {
        if (!status.playerCrouched && crouchEnabled)
        {
            status.playerCrouched = true;
            status.playerSpeed = crouchSpeed;
            torsoAnimator.SetTrigger("crouchedDown");
            legsAnimator.SetTrigger("crouchedDown");
        }
        else if(crouchEnabled)
        {
            status.playerCrouched = false;
            torsoAnimator.SetTrigger("stoodUp");
            legsAnimator.SetTrigger("stoodUp");

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
            //status.playerCrouched = false;
            status.playerSprint = true;
            if (!status.playerCrouched)
            {
                status.playerSpeed = sprintSpeed;
            }
           
        }
        else
        {
            status.playerSprint = false;

            if (!status.playerCrouched)
            {
                status.playerSpeed = walkSpeed;
            }
        }

    }



    void setAnimatorBools(Animator animator)
    {
        //Player moving
        if (movementInput == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            if (status.playerSprint)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
            animator.SetBool("isWalking", true);
        }
        //PlayerCrouching
        if (status.playerCrouched)
        {
            animator.SetBool("isCrouching", true);
        }
        else
        {
            animator.SetBool("isCrouching", false);
        }
    }

    //This function makes the head of the player have a leeway of 30 degrees before rotating the torso
    //It's meant to simulate the way humans look around, as most people also will first turn their neck and
    //only after a few degrees will start to rotate their torso
    void animateTorso(GameObject torso)
    {
        float localZRotation = torso.transform.localRotation.eulerAngles.z;
        //If the torso is less than 30 degrees misaligned either way, do not rotate it (keep postion from previous frame)
        //If more then set the local position of the torso to either +30 or -30 (330) degrees
        if (localZRotation <= 30.0f && localZRotation >= 0.0f || localZRotation >= 330.0f && localZRotation <= 360.0f)
        {
            torso.transform.rotation = prevTorsoRotation;
        }
        else
        {
            if (localZRotation >= 30.0f && localZRotation <= 180.0f)
            {
                torso.transform.localRotation = Quaternion.Euler(0, 0, 29.99f); //Sliglthly lower to ensure the head does not get stuck
            }
            else
            {
                torso.transform.localRotation = Quaternion.Euler(0, 0, 330.01f); //Same as above
            }
        }
        //Save current torso rotation for the next frame
        prevTorsoRotation = torso.transform.rotation;

        setAnimatorBools(torsoAnimator);
        
    }

    //TODO: If no other issues come up with this, remove function and just use the line of code in update
    void animateLegs(GameObject legs)
    {
        legs.transform.up = movementInput;
        setAnimatorBools(legsAnimator);
    }

}
