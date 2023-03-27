using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Implements the behaviour of the Player control scheme. Contains methods to which the messages from the InputSystem are sent and redirects those messages to other relevant scripts.
/// </summary>
public class PlayerControls : MonoBehaviour
{

    /// <summary>
    /// Reference to the player's GameObject.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Reference to the Rigidbody2D component of the player.
    /// </summary>
    private Rigidbody2D playerRigidbody;

    /// <summary>
    /// Reference to the PlayerStatus script.
    /// </summary>
    public PlayerStatus status;

    /// <summary>
    /// Reference to the Inventory Screen, which contains the Inventory Controller script.
    /// </summary>
    [SerializeField] private GameObject inventoryScreen;

    /// <summary>
    /// Reference to the Inventory Controller script.
    /// </summary>
    private InventoryController inventoryController;

    /// <summary>
    /// Reference to the Player Animation Controller script.
    /// </summary>
    private PlayerAnimationController playerAnimationController;

    /// <summary>
    /// Reference to the main camera.
    /// </summary>
    [SerializeField] GameObject mainCamera;

    /// <summary>
    /// Reference to the Camera Movement script.
    /// </summary>
    private CameraMovement cameraMovement;



    /// <summary>
    /// Reference to the player's torso.
    /// </summary>
    public GameObject playerTorso;
    /// <summary>
    /// Reference to the player's legs.
    /// </summary>
    public GameObject playerLegs;
    /// <summary>
    /// Reference to the player's head pivot.
    /// </summary>
    public GameObject playerHeadPivot;
    /// <summary>
    /// Reference to the player's weapon.
    /// </summary>
    public GameObject playerWeapon;

    /// <summary>
    /// Current movement input.
    /// </summary>
    private Vector2 movementInput;

    /// <summary>
    /// Current mouse position.
    /// </summary>
    private Vector2 mousePosition;

    /// <summary>
    /// Time of weapon equip animations in frames.
    /// </summary>
    private float equipAnimationTime = 32f;

    /// <summary>
    /// Time for the pre crouch and post crouch animations in frames. These animations are played before/after the player equips/unequips a weapon while crouching.
    /// </summary>
    private float crouchAnimationPaddingTime = 16f;

    /// <summary>
    /// Reference to the Firearm Script of the player's weapon.
    /// </summary>
    private FirearmScript firearmScript;

    /// <summary>
    /// Reference to the Player Input script.
    /// </summary>
    private PlayerInput playerInput;

    /// <summary>
    /// Whether the player can change weapons.
    /// </summary>
    public bool weaponChangeEnabled;

    /// <summary>
    /// Whether the player can crouch.
    /// </summary>
    public bool crouchEnabled;

    /// <summary>
    /// At Start initializes all og the needed references.
    /// </summary>
    void Start()
    {
        player = this.gameObject;
        playerInput = GetComponent<PlayerInput>();
        playerAnimationController = player.GetComponent<PlayerAnimationController>();


        cameraMovement = mainCamera.GetComponent<CameraMovement>();
        

        crouchEnabled = true;
        weaponChangeEnabled = true;
        playerRigidbody = GetComponent<Rigidbody2D>();
        firearmScript = playerWeapon.GetComponent<FirearmScript>();

        inventoryController = inventoryScreen.GetComponent<InventoryController>();
    }


    /// <summary>
    /// Each frame handles the rotation, movement and animations of the player.
    /// </summary>
    void FixedUpdate()
    {
        //If the inventory screen is active, do not update the player's position and rotation
        //Removing this causes a bug wherethe player will always look to lower left corner of the screen in inventory
        //Also wastes resources for no reason
        if(inventoryScreen.activeSelf)
            return;
        
        //Get the world coordinates of the mouse
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        //Calculate the new direction the player should face
        Vector2 direction = (mouseWorldPos - (Vector2)transform.position).normalized;
        float angle = Vector2.SignedAngle(new Vector2(0, 1), direction);
        playerRigidbody.MoveRotation(angle);
        playerHeadPivot.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        //Change the velocity of the player according to movement
        playerRigidbody.MovePosition(playerRigidbody.position + status.playerSpeed * Time.deltaTime * movementInput);

        
    }

    /// <summary>
    /// Each frame after everything else is updated, handles the animations of the player. This is done because of the counterrotation of the torso implemented in the PlayerAnimationController script.
    /// </summary>
    void LateUpdate(){
        playerAnimationController.animateTorso();
        playerAnimationController.animateLegs(movementInput);
        playerAnimationController.setFirearmAnimatorMovementBools();
    }

    /// <summary>
    /// Method called form the Input System when the player moves. Uses the value to move the character.
    /// </summary>
    /// <param name="value">Normalized Vector2 of the movement input.</param>
    private void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
        status.playerMoving = (movementInput != Vector2.zero);
    }

    /// <summary>
    /// Method called from the Input System when the player aims. Uses the value to set the aiming status of the player.
    /// </summary>
    /// <param name="value">Float value of the aiming input. 0 if not aiming, above 0 if aiming.</param>
    private void OnAiming(InputValue value)
    {

        if (value.Get<float>() > 0.1f)
        {
            status.playerAiming = true;
        }
        else
        {
            status.playerAiming = false;
        }
    }

    /// <summary>
    /// Method called from the Input System when the player moves the mouse. Uses the value to determine where the player is looking and sends the value to the CameraMovement script.
    /// </summary>
    /// <param name="value">Vector2 value of the mouse position.</param>
    private void OnMousePosition(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
        cameraMovement.SetMousePosition(mousePosition);
    }



    /// <summary>
    /// Method called from the Input System when the player crouches or stands up. Used to toggle the crouch status of the player and play appropriate animations.
    /// </summary>
    private void OnCrouch()
    {
        

        if (!status.playerCrouched && crouchEnabled)
        {
            status.playerCrouched = true;
            status.playerSpeed = status.crouchSpeed;
            playerAnimationController.CrouchDownAnimation();
        }
        else if(crouchEnabled)
        {
            status.playerCrouched = false;
            playerAnimationController.StandUpAnimation();

            if (status.playerSprint)
                status.playerSpeed = status.sprintSpeed;
            else
                status.playerSpeed = status.walkSpeed;
        }
    }

    /// <summary>
    /// Method called from the Input System when the player presses the sprint button. Used to toggle the sprint status of the player.
    /// </summary>
    /// <param name="value"></param>
    private void OnSprint(InputValue value)
    {
        

        if (value.isPressed)
        {
            status.ToggleSprintOn();
        }
        else
        {
            status.ToggleSprintOff();
        }

    }

    /// <summary>
    /// Method called from the Input System when the player presses the fire button. Used to call the PressTrigger method of the FirearmScript.
    /// </summary>
    /// <param name="value">Input value of the fire button.</param>
    private void OnFire(InputValue value)
    {
        firearmScript.PressTrigger(value);
    }

    /// <summary>
    /// Method called from the Input System when the player presses the reload button. Used to call the ReloadButtonPressed method of the FirearmScript.
    /// </summary>
    private void OnReload()
    {
        firearmScript.ReloadButtonPressed();
    }

    /// <summary>
    /// Method called from the Input System when the player presses the switch firemode button. Used to call the SwitchFiremode method of the FirearmScript.
    /// </summary>
    private void OnSwitchFiremode()
    {
        firearmScript.SwitchFiremode();
    }

    /// <summary>
    /// Method called from the Input System when the player tries to cycle weapons with the mouse wheel. Used to call the CycleWeapon method of the InventoryController.
    /// </summary>
    /// <param name="value"></param>
    private void OnCycleWeapon(InputValue value)
    { 
        
        //Mouse scroll is called twice when scrolling, once with 0.0f and once with the actual value
        if(value.Get<float>() == 0.0f)
            return;

        if(!weaponChangeEnabled)
            return;

        InventoryItem previousWeapon = firearmScript.selectedFirearm;
        firearmScript.selectedFirearm = inventoryController.CycleWeapon(value);
        //If a weapon was equipped, unequipped or changed to differento one, play the animation
        if(previousWeapon != null || firearmScript.selectedFirearm != null)
            playerAnimationController.WeaponSelectAnimation(previousWeapon);
        
        
    }

    /// <summary>
    /// Method called from the Input System when the player presses the primary weapon button. Used to call the SelectWeapon method of the InventoryController with the value for the primary weapon.
    /// </summary>
    private void OnPrimaryWeapon()
    {
        if(!weaponChangeEnabled)
            return;
        InventoryItem previousWeapon = firearmScript.selectedFirearm;
        firearmScript.selectedFirearm = inventoryController.SelectWeapon(1);
        if(previousWeapon != null || firearmScript.selectedFirearm != null)
            playerAnimationController.WeaponSelectAnimation(previousWeapon);
        
    }

    /// <summary>
    /// Method called from the Input System when the player presses the secondary weapon button. Used to call the SelectWeapon method of the InventoryController with the value for the secondary weapon.
    /// </summary>
    private void OnSecondaryWeapon()
    {
        if(!weaponChangeEnabled)
            return;
        InventoryItem previousWeapon = firearmScript.selectedFirearm;
        firearmScript.selectedFirearm = inventoryController.SelectWeapon(2);
        if(previousWeapon != null || firearmScript.selectedFirearm != null)
            playerAnimationController.WeaponSelectAnimation(previousWeapon);
    }


    /// <summary>
    /// Coroutine that disables the ability to change weapons for a certain amount of time. Used to prevent the player from changing weapons while the animation is playing.
    /// </summary>
    /// <param name="weaponSwap">True if the player is switching weapons, false if the player is equipping or unequipping a weapon.</param>
    /// <returns>Reference to the coroutine.</returns>
    public IEnumerator DisableWeaponInteraction(bool weaponSwap){
        weaponChangeEnabled = false;
        firearmScript.SetFirearmActive(false);
        float timeToWait = 0f;
        if(weaponSwap)
        {
            timeToWait = 2*equipAnimationTime;
        }else{
            if(status.playerCrouched)
                timeToWait = equipAnimationTime + crouchAnimationPaddingTime;
            else
                timeToWait = equipAnimationTime;
        }

        timeToWait += 8f; //Extra frame of aimation padding to avoid any inconsistencies
        timeToWait /= 60f; //Convert to seconds
        yield return new WaitForSeconds(timeToWait);

        if(firearmScript.selectedFirearm != null)
            firearmScript.SetFirearmActive(true);
        weaponChangeEnabled = true;
    }

}
