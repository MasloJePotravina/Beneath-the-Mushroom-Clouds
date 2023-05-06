using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of the human animation controller. The human animation controller should be used to trigger different animations for a human from different scripts.
/// </summary>
public class HumanAnimationController : MonoBehaviour
{

    /// <summary>
    /// Reference to the human.
    /// </summary>
    private GameObject human;

    /// <summary>
    /// Reference to the human's torso.
    /// </summary>
    private GameObject torso;

    /// <summary>
    /// Reference to the human's legs.
    /// </summary>
    private GameObject legs;

    /// <summary>
    /// Firearm sprite object.
    /// </summary>
    private GameObject firearmObject;


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
    /// Reference to the player status script in case the human is the player.
    /// </summary>
    private PlayerStatus playerStatus;

    /// <summary>
    /// Reference to the NPC status script in case the human is an NPC.
    /// </summary>
    private NPCStatus npcStatus;


    /// <summary>
    /// Rotation of the torso in the previous frame.
    /// </summary>
    private Quaternion prevTorsoRotation;

    /// <summary>
    /// Currently selected firearm.
    /// </summary>
    private InventoryItem selectedFirearm;

    /// <summary>
    /// Reference to the audio manager.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Whether the human this controller is attached to is the player.
    /// </summary>
    private bool isPlayer = false;


    /// <summary>
    /// On Start, initiliazes all of the needed references.
    /// </summary>
    void Start()
    {


        human = this.gameObject;
        torso = human.transform.Find("Torso").gameObject;
        legs = torso.transform.Find("Legs").gameObject;


        torsoAnimator = torso.GetComponent<Animator>();
        firearmAnimator = torso.transform.Find("FirearmSprite").GetComponent<Animator>();
        legsAnimator = legs.GetComponent<Animator>();

        //Select status script based on whether the human is the player or an NPC
        if(this.CompareTag("Player"))
        {
            isPlayer = true;
            playerStatus = human.GetComponent<PlayerStatus>();
        }
        else if(this.CompareTag("NPC"))
        {
            isPlayer = false;
            npcStatus = human.GetComponent<NPCStatus>();
        }
            

        //Store the rotation of the torso in the first frame
        prevTorsoRotation = torso.transform.rotation;

        audioManager = GameObject.FindObjectOfType<AudioManager>();

        firearmObject = transform.Find("Firearm").gameObject;
    }

    /// <summary>
    /// Sets appropriate animator booleans for the torso, firearm and head animators when the character equips or unequips a weapon.
    /// </summary>
    /// <param name="previousFirearm">Reference to the previous weapon that the character had equipped.</param>
    public void WeaponSelectAnimation(InventoryItem newFirearm, InventoryItem previousFirearm)
    {
        this.selectedFirearm = newFirearm;
        //Reset weapon swap booleans
        torsoAnimator.SetBool("weaponSwap", false);
        firearmAnimator.SetBool("weaponSwap", false);

        //If a new firearm is equipped
        if(selectedFirearm != null){
            ResetTorsoRotation();

            //If the player had a weapon equipped before, play animations for weapon unequipping and delay firearm animation controller change
            if(previousFirearm != null){
                firearmAnimator.SetTrigger("weaponUnequipped");
                torsoAnimator.SetTrigger("weaponUnequipped");
                torsoAnimator.SetBool("weaponSwap", true);
                firearmAnimator.SetBool("weaponSwap", true);
                audioManager.Play(previousFirearm.itemData.weaponType + "Unequip", firearmObject);
                StartCoroutine(DelayedWeaponControllerChange());
            }else{
            //If the player didn't have a weapon equipped before, change the animator to an override
                firearmAnimator.runtimeAnimatorController = selectedFirearm.itemData.weaponAnimationController;
            }

            //Short weapon animations
            if(selectedFirearm.itemData.weaponLength == 0){
                if(previousFirearm == null)
                    torsoAnimator.ResetTrigger("weaponUnequipped");
                torsoAnimator.SetTrigger("shortWeaponEquipped");
            //Long weapon animations
            }else if(selectedFirearm.itemData.weaponLength == 1){ 
                if(previousFirearm == null)
                    torsoAnimator.ResetTrigger("weaponUnequipped");   
                torsoAnimator.SetTrigger("longWeaponEquipped");
            }
            if(previousFirearm == null)
                firearmAnimator.ResetTrigger("weaponUnequipped");
            firearmAnimator.SetTrigger("weaponEquipped");
            //Wait in case a previous weapon was unequipped, if it was not, wait is ignored in audioManager
            audioManager.Play(selectedFirearm.itemData.weaponType + "Equip", firearmObject,  wait: true); 
        //If the player unequips a weapon
        }else{
            torsoAnimator.ResetTrigger("longWeaponEquipped");
            torsoAnimator.ResetTrigger("shortWeaponEquipped");
            torsoAnimator.SetTrigger("weaponUnequipped");
            audioManager.Play(previousFirearm.itemData.weaponType + "Unequip", firearmObject);

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
        if(selectedFirearm != null)
            firearmAnimator.runtimeAnimatorController = selectedFirearm.itemData.weaponAnimationController; 
    }

    /// <summary>
    /// Resets torso rotation before equipping a weapon.
    /// </summary>
    public void ResetTorsoRotation()
    {
        torso.transform.localRotation = Quaternion.Euler(0, 0, 0);
        prevTorsoRotation = torso.transform.rotation;
    }

    /// <summary>
    /// Sets the basic movement booleans for a selected animator (called for the legs and torso animators).
    /// </summary>
    /// <param name="animator">Reference to the animator.</param>
    private void setAnimatorMovementBools(Animator animator)
    {
        bool isMoving = false;
        bool isRunning = false;
        bool isCrouched = false;
        if(isPlayer){
            isMoving = playerStatus.isMoving;
            isRunning = playerStatus.isRunning;
            isCrouched = playerStatus.isCrouched;
        }else{
            isMoving = npcStatus.isMoving;
            isRunning = npcStatus.isRunning;
            isCrouched = npcStatus.isCrouched;
        }
        //Player moving
        if (!isMoving)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            if (isRunning)
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
        if (isCrouched)
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
    /// Animates the torso of the character each frame. It is also used to give the character's head a leeway of 30 degrees before rotating the torso.
    /// </summary>
    /// <param name="torso"></param>
    public void animateTorso()
    {
        if(selectedFirearm == null){
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
        

        setAnimatorMovementBools(torsoAnimator);
        
    }

    /// <summary>
    /// Animates the legs of the character each frame. It also rotates the legs in the direction of movement.
    /// </summary>
    /// <param name="movementInput"></param>
    public void animateLegs(Vector2 movementInput)
    {
        bool isMoving = false;
        if(isPlayer){
            isMoving = playerStatus.isMoving;
        }else{
            isMoving = npcStatus.isMoving;
        }
        if (!isMoving)
        {
            legs.transform.rotation = torso.transform.rotation;
        }
        else
        {
            legs.transform.up = movementInput;
        }    
        setAnimatorMovementBools(legsAnimator);
    }

    
    /// <summary>
    /// Sets the basic movement booleans for firearm animations.
    /// </summary>
    public void setFirearmAnimatorMovementBools(){
        //PlayerCrouching
        bool isCrouched = false;
        if(isPlayer){
            isCrouched = playerStatus.isCrouched;
        }else{
            isCrouched = npcStatus.isCrouched;
        }

        if (isCrouched)
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
    /// <param name="firearm">Equipped firearm.</param>
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
        if(selectedFirearm.shellInChamber){
            audioManager.Play(selectedFirearm.itemData.weaponType + "RackEject", firearmObject);
            
        }else{
            audioManager.Play(selectedFirearm.itemData.weaponType + "Rack", firearmObject);
        }
        

        firearmAnimator.SetTrigger("weaponRack");
    }

    /// <summary>
    /// Plays the magazine unload animation.
    /// </summary>
    /// <param name="firearm">Equipped firearm.</param>
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

        audioManager.Play(selectedFirearm.itemData.weaponType + "UnloadMagazine", firearmObject);
    }

    /// <summary>
    /// Plays the magazine load animation.
    /// </summary>
    /// <param name="firearm">Equipped firearm.</param>
    public void LoadMagAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponLoadMag");
        }else{
            torsoAnimator.SetTrigger("longWeaponLoadMag");
        }
        audioManager.Play(selectedFirearm.itemData.weaponType + "LoadMagazine", firearmObject);

        firearmAnimator.SetTrigger("weaponLoadMag");
    }

    //TODO: Add animation for bolt open, for now an aditional rack animation is played (this is done due to proper sound effects)
    /// <summary>
    /// Plays the bolt open animation.
    /// </summary>
    /// <param name="firearm">Equipped firearm.</param>
    public void OpenBoltAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponRack");
        }else{
            torsoAnimator.SetTrigger("longWeaponRack");
        }

        audioManager.Play(selectedFirearm.itemData.weaponType + "BoltOpen", firearmObject);
        if(selectedFirearm.shellInChamber){
            audioManager.Play(selectedFirearm.itemData.weaponType + "CasingDrop", firearmObject);
        }

        firearmAnimator.SetTrigger("weaponLoadRound");
    }

    //TODO: Add animation for bolt close, for now an aditional rack animation is played (this is done due to proper sound effects)
    /// <summary>
    /// Plays the bolt close animation.
    /// </summary>
    /// <param name="firearm">Equipped firearm.</param>
    public void CloseBoltAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponRack");
        }else{
            torsoAnimator.SetTrigger("longWeaponRack");
        }

        audioManager.Play(selectedFirearm.itemData.weaponType + "BoltClose", firearmObject);

        firearmAnimator.SetTrigger("weaponRack");
    }

    /// <summary>
    /// Plays the round load animation.
    /// </summary>
    /// <param name="firearm">Equipped firearm.</param>
    public void LoadRoundAnimation(InventoryItem firearm){
        if(firearm == null){
            return;
        }

        if(firearm.itemData.weaponLength == 0){
            torsoAnimator.SetTrigger("shortWeaponLoadRound");
        }else{
            torsoAnimator.SetTrigger("longWeaponLoadRound");
        }

        audioManager.Play(selectedFirearm.itemData.weaponType + "LoadRound", firearmObject);

        firearmAnimator.SetTrigger("weaponLoadRound");
    }

    /// <summary>
    /// Quickly switches from idle animation state to equipped weapon state when the player swithces weapons in the inventory.
    /// </summary>
    /// <param name="firearm">Equipped firearm.</param>
    public void InventoryQuickWeaponChangeAnimation(InventoryItem firearm){
        selectedFirearm = firearm;
        if(selectedFirearm == null){
            torsoAnimator.SetTrigger("weaponQuickUnequip");
            firearmAnimator.SetTrigger("weaponQuickUnequip");
        }else{
            ResetTorsoRotation();
            firearmAnimator.runtimeAnimatorController = selectedFirearm.itemData.weaponAnimationController;
            if(selectedFirearm.itemData.weaponLength == 0){
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
