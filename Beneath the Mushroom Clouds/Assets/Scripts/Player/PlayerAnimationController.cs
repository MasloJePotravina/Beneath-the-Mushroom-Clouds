using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of the player animation controller. The player animation controller should be used to trigger different animations for the player from different scripts.
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{

    /// <summary>
    /// Reference to the player.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Reference to the player's torso.
    /// </summary>
    private GameObject playerTorso;

    /// <summary>
    /// Reference to the player's legs.
    /// </summary>
    private GameObject playerLegs;


    /// <summary>
    /// Reference to the torso animator.
    /// </summary>
    private Animator torsoAnimator;

    /// <summary>
    /// Reference to the firearm animator.
    /// </summary>
    private Animator firearmAnimator;

    /// <summary>
    /// Reference to the legs animator.
    /// </summary>
    private Animator legsAnimator;

    /// <summary>
    /// Reference to the player status script.
    /// </summary>
    private PlayerStatus playerStatus;

    /// <summary>
    /// Reference to the player controls script.
    /// </summary>
    private PlayerControls playerControls;

    /// <summary>
    /// Reference to the firearm script.
    /// </summary>
    private FirearmScript firearmScript;

    /// <summary>
    /// Rotation of the torso in the previous frame.
    /// </summary>
    private Quaternion prevTorsoRotation;


    /// <summary>
    /// On Start, initiliazes all of the needed references.
    /// </summary>
    void Start()
    {


        player = this.gameObject;
        playerTorso = player.transform.Find("Torso").gameObject;

        firearmScript = player.transform.Find("PlayerFirearm").GetComponent<FirearmScript>();
        playerLegs = playerTorso.transform.Find("Legs").gameObject;


        torsoAnimator = playerTorso.GetComponent<Animator>();
        firearmAnimator = playerTorso.transform.Find("FirearmSprite").GetComponent<Animator>();
        legsAnimator = playerLegs.GetComponent<Animator>();

        playerStatus = player.GetComponent<PlayerStatus>();
        playerControls = player.GetComponent<PlayerControls>();

        //Store the rotation of the torso in the first frame
        prevTorsoRotation = playerTorso.transform.rotation;
    }

    /// <summary>
    /// Sets appropriate animator booleans for the torso, firearm and head animators when the player equips or unequips a weapon.
    /// </summary>
    /// <param name="previousWeapon">Reference to the previous weapon that the player had equipped.</param>
    public void WeaponSelectAnimation(InventoryItem previousWeapon)
    {
        //Reset weapon swap booleans
        torsoAnimator.SetBool("weaponSwap", false);
        firearmAnimator.SetBool("weaponSwap", false);

        //If a new firearm is equipped
        if(firearmScript.selectedFirearm != null){
            resetTorsoRotation();

            //If the player had a weapon equipped before, play animations for weapon unequipping and delay firearm animation controller change
            if(previousWeapon != null){
                firearmAnimator.SetTrigger("weaponUnequipped");
                torsoAnimator.SetTrigger("weaponUnequipped");
                torsoAnimator.SetBool("weaponSwap", true);
                firearmAnimator.SetBool("weaponSwap", true);
                StartCoroutine(playerControls.DisableWeaponInteraction(true));
                StartCoroutine(DelayedWeaponControllerChange());
            }else{
            //If the player didn't have a weapon equipped before, change the animator to an override
                StartCoroutine(playerControls.DisableWeaponInteraction(false));
                firearmAnimator.runtimeAnimatorController = firearmScript.selectedFirearm.itemData.weaponAnimationController;
            }

            //Short weapon animations
            if(firearmScript.selectedFirearm.itemData.weaponLength == 0){
                if(previousWeapon == null)
                    torsoAnimator.ResetTrigger("weaponUnequipped");
                torsoAnimator.SetTrigger("shortWeaponEquipped");
            //Long weapon animations
            }else if(firearmScript.selectedFirearm.itemData.weaponLength == 1){ 
                if(previousWeapon == null)
                    torsoAnimator.ResetTrigger("weaponUnequipped");   
                torsoAnimator.SetTrigger("longWeaponEquipped");
            }
            if(previousWeapon == null)
                firearmAnimator.ResetTrigger("weaponUnequipped");
            firearmAnimator.SetTrigger("weaponEquipped");
        //If the player unequips a weapon
        }else{
            StartCoroutine(playerControls.DisableWeaponInteraction(false));
            torsoAnimator.ResetTrigger("longWeaponEquipped");
            torsoAnimator.ResetTrigger("shortWeaponEquipped");
            torsoAnimator.SetTrigger("weaponUnequipped");

            firearmAnimator.ResetTrigger("weaponEquipped");
            firearmAnimator.SetTrigger("weaponUnequipped");
        }
    }

    /// <summary>
    /// Coroutine for delayed spawn of the firearm animation controller.
    /// </summary>
    /// <returns>Reference to the coroutine.</returns>
    IEnumerator DelayedWeaponControllerChange(){
        yield return new WaitForSeconds(32f/60f);
        if(firearmScript.selectedFirearm != null)
            firearmAnimator.runtimeAnimatorController = firearmScript.selectedFirearm.itemData.weaponAnimationController; 
    }

    /// <summary>
    /// Resets torso rotation before equipping a weapon.
    /// </summary>
    private void resetTorsoRotation()
    {
        playerTorso.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Sets the basic movement booleans for a selected animator (called for the legs and torso animators).
    /// </summary>
    /// <param name="animator">Reference to the animator.</param>
    private void setAnimatorMovementBools(Animator animator)
    {
        //Player moving
        if (!playerStatus.playerMoving)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            if (playerStatus.playerSprint)
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
        if (playerStatus.playerCrouched)
        {
            animator.SetBool("isCrouching", true);
        }
        else
        {
            animator.SetBool("isCrouching", false);
        }
    }

    /// <summary>
    /// Triggers crouch down animation in the torso and legs animators.
    /// </summary>
    public void CrouchDownAnimation()
    {
        torsoAnimator.SetTrigger("crouchedDown");
        legsAnimator.SetTrigger("crouchedDown");
    }

    /// <summary>
    /// Trigger stand up animation in the torso and legs animators.
    /// </summary>
    public void StandUpAnimation(){
        torsoAnimator.SetTrigger("stoodUp");
        legsAnimator.SetTrigger("stoodUp");
    }

    //This function makes the head of the player have a leeway of 30 degrees before rotating the torso
    //It's meant to simulate the way humans look around, as most people also will first turn their neck and
    //only after a few degrees will start to rotate their torso

    /// <summary>
    /// Animates the torso of the player each frame. It is also used to give the player's head a leeway of 30 degrees before rotating the torso.
    /// </summary>
    /// <param name="torso"></param>
    public void animateTorso()
    {
        if(firearmScript.selectedFirearm == null){
            float localZRotation = playerTorso.transform.localRotation.eulerAngles.z;
            //If the torso is less than 30 degrees misaligned either way, do not rotate it (keep postion from previous frame)
            //If more then set the local position of the torso to either +30 or -30 (330) degrees
            if (localZRotation <= 30.0f && localZRotation >= 0.0f || localZRotation >= 330.0f && localZRotation <= 360.0f)
            {
                playerTorso.transform.rotation = prevTorsoRotation;
            }
            else
            {
                if (localZRotation >= 30.0f && localZRotation <= 180.0f)
                {
                    playerTorso.transform.localRotation = Quaternion.Euler(0, 0, 29.99f); //Sliglthly lower to ensure the head does not get stuck
                }
                else
                {
                    playerTorso.transform.localRotation = Quaternion.Euler(0, 0, 330.01f); //Same as above
                }
            }
            //Save current torso rotation for the next frame
            prevTorsoRotation = playerTorso.transform.rotation;
        }
        

        setAnimatorMovementBools(torsoAnimator);
        
    }

    /// <summary>
    /// Animates the legs of the player each frame. It also rotates the legs in the direction of movement.
    /// </summary>
    /// <param name="movementInput"></param>
    public void animateLegs(Vector2 movementInput)
    {
        if (!playerStatus.playerMoving)
        {
            playerLegs.transform.rotation = playerTorso.transform.rotation;
        }
        else
        {
            playerLegs.transform.up = movementInput;
        }    
        setAnimatorMovementBools(legsAnimator);
    }

    
    /// <summary>
    /// Sets the basic movement booleans for firearm animations.
    /// </summary>
    public void setFirearmAnimatorMovementBools(){
        //PlayerCrouching
        if (playerStatus.playerCrouched)
        {
            firearmAnimator.SetBool("isCrouching", true);
        }
        else
        {
            firearmAnimator.SetBool("isCrouching", false);
        }
    }

    //NOTE: The following methods are temporary solutions for temporary weapon animations. They will likely be entirely different when individual weapons receive individisual animations.

    /// <summary>
    /// Sets animator booleans for weapon racking. 
    /// </summary>
    /// <param name="firearm">Firearm to be racked.</param>
    public void RackAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        //Short weapon
        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponRack");
        //Long weapon
        }else{
            torsoAnimator.SetTrigger("longWeaponRack");
        }

        firearmAnimator.SetTrigger("weaponRack");
    }

    /// <summary>
    /// Plays the magazine unload animation.
    /// </summary>
    /// <param name="firearm">Firearm to be unloaded.</param>
    public void UnloadMagAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponUnloadMag");
        }else{
            torsoAnimator.SetTrigger("longWeaponUnloadMag");
        }

        firearmAnimator.SetTrigger("weaponUnloadMag");
    }

    /// <summary>
    /// Plays the magazine load animation.
    /// </summary>
    /// <param name="firearm">Firearm to be loaded.</param>
    public void LoadMagAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponLoadMag");
        }else{
            torsoAnimator.SetTrigger("longWeaponLoadMag");
        }

        firearmAnimator.SetTrigger("weaponLoadMag");
    }

    /// <summary>
    /// Plays the round load animation.
    /// </summary>
    /// <param name="firearm">Firearm to be loaded.</param>
    public void LoadRoundAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponLoadRound");
        }else{
            torsoAnimator.SetTrigger("longWeaponLoadRound");
        }

        firearmAnimator.SetTrigger("weaponLoadRound");
    }

    /// <summary>
    /// Quickly switches from idle animation state to equipped weapon state when the player swithces weapons in the inventory.
    /// </summary>
    /// <param name="firearm"></param>
    public void InventoryQuickWeaponChangeAnimation(InventoryItem firearm){
        if(firearm == null){
            torsoAnimator.SetTrigger("weaponQuickUnequip");
            firearmAnimator.SetTrigger("weaponQuickUnequip");
        }else{
            resetTorsoRotation();
            firearmAnimator.runtimeAnimatorController = firearm.itemData.weaponAnimationController;
            if(firearm.itemData.weaponLength == 0){
                torsoAnimator.SetTrigger("shortWeaponQuickEquip");
            }else{
                torsoAnimator.SetTrigger("longWeaponQuickEquip");
            }

            firearmAnimator.SetTrigger("weaponQuickEquip");
        }
    }

    //Fixes a bug which caused the player to hide their weapon immediately after equipping it
    //Its called after the inventory is closed
    /// <summary>
    /// Resets the triggers for quick weapon change (from inventory) after closing the inventory.
    /// </summary>
    public void ResetQuickWeaponChangeTriggers(){
        torsoAnimator.ResetTrigger("shortWeaponQuickEquip");
        torsoAnimator.ResetTrigger("longWeaponQuickEquip");
        torsoAnimator.ResetTrigger("weaponQuickUnequip");
        firearmAnimator.ResetTrigger("weaponQuickEquip");
        firearmAnimator.ResetTrigger("weaponQuickUnequip");
    }

    /// <summary>
    /// Disables all movement bools to stop movement animations when the inventory is open.
    /// </summary>
    public void DisableMovementBools(){
        torsoAnimator.SetBool("isWalking", false);
        torsoAnimator.SetBool("isRunning", false);
        torsoAnimator.SetBool("isCrouching", false);
        legsAnimator.SetBool("isWalking", false);
        legsAnimator.SetBool("isRunning", false);
        legsAnimator.SetBool("isCrouching", false);
    }
}
