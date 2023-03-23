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

    public GameObject mainCamera;
    private InventoryController inventoryController;

    [SerializeField] private GameObject inventoryScreen;


    public GameObject playerTorso;
    public GameObject playerLegs;
    public GameObject playerHead;
    public GameObject playerHeadPivot;
    public GameObject playerWeapon;

    private Vector2 movementInput;
    private Vector2 mousePosition;

    private Animator torsoAnimator;
    private Animator legsAnimator;
    private Animator headAnimator;

    public GameObject firearmSprite;
    private Animator firearmAnimator;

    private Quaternion prevTorsoRotation;
    private FirearmScript firearmScript;
    private PlayerInput playerInput;

    
    

    public bool crouchEnabled;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        

        crouchEnabled = true;
        playerRigidbody = GetComponent<Rigidbody2D>();

        prevTorsoRotation = playerTorso.transform.rotation;
        torsoAnimator = playerTorso.GetComponent<Animator>();
        legsAnimator = playerLegs.GetComponent<Animator>();
        headAnimator = playerHead.GetComponent<Animator>();
        firearmAnimator = firearmSprite.GetComponent<Animator>();
        firearmScript = playerWeapon.GetComponent<FirearmScript>();

        inventoryController = mainCamera.GetComponent<InventoryController>();
    }

    void Update()
    {
        
    }

    // Update is called once per frame
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

        animateTorso(playerTorso);
        animateLegs(playerLegs);
        setFirearmAnimatorBools();
    }

    private void OnMove(InputValue value)
    {
        

        movementInput = value.Get<Vector2>();
        status.playerMoving = movementInput != Vector2.zero;
    }

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

    private void OnMousePosition(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }



    //Crouch when standing, stand up when crouching
    private void OnCrouch()
    {
        

        if (!status.playerCrouched && crouchEnabled)
        {
            status.playerCrouched = true;
            status.playerSpeed = status.crouchSpeed;
            torsoAnimator.SetTrigger("crouchedDown");
            legsAnimator.SetTrigger("crouchedDown");
        }
        else if(crouchEnabled)
        {
            status.playerCrouched = false;
            torsoAnimator.SetTrigger("stoodUp");
            legsAnimator.SetTrigger("stoodUp");

            if (status.playerSprint)
                status.playerSpeed = status.sprintSpeed;
            else
                status.playerSpeed = status.walkSpeed;
        }
    }

    
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

    private void OnFire(InputValue value)
    {
        firearmScript.PressTrigger(value);
    }

    private void OnReload()
    {
        firearmScript.ReloadButtonPressed();
    }

    private void OnCycleWeapon(InputValue value)
    { 
        //Mouse scroll is called twice when scrolling, once with 0.0f and once with the actual value
        if(value.Get<float>() == 0.0f)
            return;


        status.selectedWeapon = inventoryController.CycleWeapon(value);
        weaponEquipAnimation();
        
    }

    private void OnPrimaryWeapon()
    {
        status.selectedWeapon = inventoryController.SelectWeapon(1);
        weaponEquipAnimation();
    }

    private void OnSecondaryWeapon()
    {
        status.selectedWeapon = inventoryController.SelectWeapon(2);
        weaponEquipAnimation();
    }

    void weaponEquipAnimation()
    {
        if(status.selectedWeapon != null){
            firearmAnimator.runtimeAnimatorController = status.selectedWeapon.itemData.weaponAnimationController; 
            resetTorsoRotation();
            if(status.selectedWeapon.itemData.weaponLength == 0){
                torsoAnimator.ResetTrigger("weaponUnequipped");
                torsoAnimator.SetTrigger("shortWeaponEquipped");
                headAnimator.ResetTrigger("longWeaponEquipped");
                headAnimator.SetTrigger("weaponUnequipped");
            }else if(status.selectedWeapon.itemData.weaponLength == 1){
                torsoAnimator.ResetTrigger("weaponUnequipped");
                torsoAnimator.SetTrigger("longWeaponEquipped");
                headAnimator.ResetTrigger("weaponUnequipped");
                headAnimator.SetTrigger("longWeaponEquipped");
            }
            firearmAnimator.ResetTrigger("weaponUnequipped");
            firearmAnimator.SetTrigger("weaponEquipped");
        }else{
            torsoAnimator.ResetTrigger("longWeaponEquipped");
            torsoAnimator.ResetTrigger("shortWeaponEquipped");
            torsoAnimator.SetTrigger("weaponUnequipped");

            firearmAnimator.ResetTrigger("weaponEquipped");
            firearmAnimator.SetTrigger("weaponUnequipped");

            headAnimator.ResetTrigger("longWeaponEquipped");
            headAnimator.SetTrigger("weaponUnequipped");
            

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
        if(status.selectedWeapon == null){
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
        }
        

        setAnimatorBools(torsoAnimator);
        
    }

    //TODO: If no other issues come up with this, remove function and just use the line of code in update
    void animateLegs(GameObject legs)
    {
        if (movementInput == Vector2.zero)
        {
            legs.transform.rotation = playerTorso.transform.rotation;
        }
        else
        {
            legs.transform.up = movementInput;
        }    
        setAnimatorBools(legsAnimator);
    }

    void resetTorsoRotation()
    {
        playerTorso.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    void setFirearmAnimatorBools(){
        //PlayerCrouching
        if (status.playerCrouched)
        {
            firearmAnimator.SetBool("isCrouching", true);
        }
        else
        {
            firearmAnimator.SetBool("isCrouching", false);
        }
    }

    

}
