using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
/// <summary>
/// Implements variables describing the current status of the player and methods to manipulate these variables
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    
    //////////References//////////

    /// <summary>
    /// Reference to the world status
    /// </summary>
    private WorldStatus worldStatus;

    /// <summary>
    /// Reference to the player's health status UI script in the Inventory Screen
    /// </summary>
    private HealthStatusUI healthStatusUI;

    /// <summary>
    /// Reference to the player's noise origin
    /// </summary>
    private NoiseOrigin noiseOrigin;

    /// <summary>
    /// Reference to the player's paricle system used for blood trail.
    /// </summary>
    private ParticleSystem bloodParticles;

    /// <summary>
    /// Reference to the Death Screen.
    /// </summary>
    private DeathScreen deathScreen;

    /// <summary>
    /// Reference to a dead body prefab.
    /// </summary>
    [SerializeField] private GameObject deadBodyPrefab;

    /// <summary>
    /// Reference to the weight info object in the Inventory Screen.
    /// </summary>
    private TextMeshProUGUI carryWeightText;

    /// <summary>
    /// Reference to the inventory controller.
    /// </summary>
    private InventoryController inventoryController;

    /// <summary>
    /// Reference to the HUD controller.
    /// </summary>
    private HUDController hudController;

    //////////Current Status Values//////////

    /// <summary>
    /// Health of the player
    /// </summary>
    public float playerHealth = 0.0f;

    /// <summary>
    /// Stamina of the player
    /// </summary>
    public float playerStamina = 0.0f;

    /// <summary>
    /// Hunger of the player
    /// </summary>
    public float playerHunger = 0.0f;
    /// <summary>
    /// Thirst of the player
    /// </summary>
    public float playerThirst = 0.0f;

    /// <summary>
    /// Body temperature of the player
    /// </summary>
    public float playerBodyTemp = 0.0f;

    /// <summary>
    /// Tiredness of the player (or rather the wakefulness of the player as the lower the value, the more tired the player is)
    /// </summary>
    public float playerTiredness = 0.0f;

    //////////Max Status Values//////////
    
    /// <summary>
    /// Maximum stamina of the player
    /// </summary>
    public float maxPlayerStamina = 100.0f;

    /// <summary>
    /// Maximum health of the player
    /// </summary>
    public float maxPlayerHealth = 100.0f;

    /// <summary>
    /// Maximum carry weight of the player
    /// </summary>
    public float maxCarryWeight = 40.0f;

    /// <summary>
    /// Current carry weight of the player
    /// </summary>
    public float currentCarryWeight = 0.0f;


    //////////Base Drain/Regen Values//////////
    
    /// <summary>
    /// Base health drain, used in the calculation of bleed damage
    /// </summary>
    public float baseHealthDrain = 0.027f;

    /// <summary>
    /// Base health regeneration rate when the character is Nourished
    /// </summary>
    public float baseHealthRegen = 0.02f;    

    /// <summary>
    /// Base stamina drain when running
    /// </summary>
    public float baseStaminaDrain = 0.25f;

    /// <summary>
    /// Base stamina regeneration when not running
    /// </summary>
    public float baseStaminaRegen = 0.375f;

    /// <summary>
    /// Base hunger drain
    /// </summary>
    public float baseHungerDrain = 0.00185f;      //Depletes completely from full to nothing after roughly 45 minutes of gameplay (15 hours in game)

    /// <summary>
    /// Base thirst drain
    /// </summary>
    public float baseThirstDrain = 0.00275f;      //Depletes completely from full to nothing after roughly 30 minutes of gameplay (10 hours in game)

    /// <summary>
    /// Base tiredness drain
    /// </summary>
    public float baseTirednessDrain = 0.00115f;  //Depletes completely from full to nothing after roughly 72 minutes of gameplay (24 hours in game)

    /// <summary>
    /// Base tiredness regeneration when resting
    /// </summary>
    public float baseTirednessRegen = 0.00345f;  //Regenerates completely from nothing to full after roughly 24 minutes of gameplay (8 hours in game)




    //////////Current Drain/Regen Values//////////
    
    /// <summary>
    /// Current health drain (regeneration when negative)
    /// </summary>
    public float healthDrain = 0.0f;

    /// <summary>
    /// Current stamina drain (regeneration when negative)
    /// </summary>
    public float staminaDrain = 0.0f;

    /// <summary>
    /// Current hunger drain (regeneration when negative)
    /// </summary>
    public float hungerDrain = 0.0f;

    /// <summary>
    /// Current thirst drain (regeneration when negative)
    /// </summary>
    public float thirstDrain = 0.0f;

    /// <summary>
    /// Current tiredness drain (regeneration when negative)
    /// </summary>
    public float tirednessDrain = 0.0f;

    /// <summary>
    /// Current difference between the body temperature and the environment temperature
    /// </summary>
    public float bodyTempDifference = 0.0f;

    //////////Player Speed Values//////////

    /// <summary>
    /// Current speed of the player
    /// </summary>
    public float playerSpeed = 50.0f;

    /// <summary>
    /// Crouch speed of the player
    /// </summary>
    public float crouchSpeed = 20.0f;

    /// <summary>
    /// Walk speed of the player
    /// </summary>
    public float walkSpeed = 50.0f;

    /// <summary>
    /// Sprint speed of the player
    /// </summary>
    public float runSpeed = 100.0f;

    //////////Current movement and stance//////////

    /// <summary>
    /// Whether the player is currently crouched
    /// </summary>
    public bool isCrouched = false;

    /// <summary>
    /// Whether the player is currently sprinting (the sprint key is pressed, this is true even if the player is not moving)
    /// </summary>
    public bool isRunning = false;

    /// <summary>
    /// Whether the player is currently aiming
    /// </summary>
    public bool isAiming = false;

    /// <summary>
    /// Whether the player is currently moving
    /// </summary>
    public bool isMoving = false;

    /// <summary>
    /// Whether the player is currently resting in bed
    /// </summary>
    public bool isResting = false;

    /// <summary>
    /// Whether the player's stamina is depleted. Used to block sprint after depleting the stamina and unlocking it again after regenerating it back to 20%
    /// </summary>
    private bool staminaDepleted = false;


    /// <summary>
    /// Ability of the player to handle firearms. 0 means the best possible (not achievable)
    /// </summary>
    public float shootingAbility = 0.1f;


    //////////Health Status Dictionaries//////////

    /// <summary>
    /// Dictionary for the status effects of the body parts
    /// </summary>
    private Dictionary<string, Dictionary<string, bool>> bodyPartStatusEffects;

    /// <summary>
    /// Dictionary for the status effects of the head
    /// </summary>
    private Dictionary<string, bool> headStatusEffects = new Dictionary<string, bool>();

    /// <summary>
    /// Dictionary for the status effects of the torso
    /// </summary>
    private Dictionary<string, bool> torsoStatusEffects = new Dictionary<string, bool>();

    /// <summary>
    /// Dictionary for the status effects of the left arm
    /// </summary>
    private Dictionary<string, bool> leftArmStatusEffects = new Dictionary<string, bool>();

    /// <summary>
    /// Dictionary for the status effects of the right arm
    /// </summary>
    private Dictionary<string, bool> rightArmStatusEffects = new Dictionary<string, bool>();

    /// <summary>
    /// Dictionary for the status effects of the left leg
    /// </summary>
    private Dictionary<string, bool> leftLegStatusEffects = new Dictionary<string, bool>();

    /// <summary>
    /// Dictionary for the status effects of the right leg
    /// </summary>
    private Dictionary<string, bool> rightLegStatusEffects = new Dictionary<string, bool>();



    /// <summary>
    /// How much infection is in each body part, capped at 100
    /// </summary>
    private Dictionary<string, float> infectionSpread = new Dictionary<string, float>(){
        {"Head", 0.0f},
        {"Torso", 0.0f},
        {"LeftArm", 0.0f},
        {"RightArm", 0.0f},
        {"LeftLeg", 0.0f},
        {"RightLeg", 0.0f}
    };

    /// <summary>
    /// Current chance to get infection to specific body parts when the infection roll happens
    /// </summary>
    private Dictionary<string, float> infectionChance = new Dictionary<string, float>(){
        {"Head", 0.0f},
        {"Torso", 0.0f},
        {"LeftArm", 0.0f},
        {"RightArm", 0.0f},
        {"LeftLeg", 0.0f},
        {"RightLeg", 0.0f}
    };


    /// <summary>
    /// Applies to stiched wounds, how healed the wound is, body part heals when this reaches 100
    /// </summary>
    private Dictionary<string, float> woundHealingStatus = new Dictionary<string, float>(){
        {"Head", 0.0f},
        {"Torso", 0.0f},
        {"LeftArm", 0.0f},
        {"RightArm", 0.0f},
        {"LeftLeg", 0.0f},
        {"RightLeg", 0.0f}
    };

    /// <summary>
    /// How much disinfectant is left in each body part, when this drops to 0 the body part is no longer disinfected
    /// </summary>
    private Dictionary<string, float> disinfectantRemaining = new Dictionary<string, float>(){
        {"Head", 0.0f},
        {"Torso", 0.0f},
        {"LeftArm", 0.0f},
        {"RightArm", 0.0f},
        {"LeftLeg", 0.0f},
        {"RightLeg", 0.0f}
    };

    /// <summary>
    /// Dictionary of global status effects such as Dehydrated, Starving, Over-encumbered, etc.
    /// </summary>
    private Dictionary<string, bool> globalStatusEffects = new Dictionary<string, bool>(){
        {"Nourished", false},
        {"Antibiotics", false},
        {"Painkillers", false},
        {"Hypothermia", false},
        {"Hyperthermia", false},
        {"Dehydrated", false},
        {"Starving", false},
        {"Over-encumbered", false},
        {"Tired", false}
        
    };

    /// <summary>
    /// Dictionary of the Inventory Item bandage references currently applied to each body part
    /// </summary>
    private Dictionary<string, InventoryItem> bandageItems = new Dictionary<string, InventoryItem>(){
        {"Head", null},
        {"Torso", null},
        {"LeftArm", null},
        {"RightArm", null},
        {"LeftLeg", null},
        {"RightLeg", null}
    };

    /// <summary>
    /// Timer for infection rolls
    /// </summary>
    private float infectionRollTimer = 0.0f;

    /// <summary>
    /// How long will the painkiller effect last, periodically decreases, when it reaches 0 the painkillers effect is removed
    /// </summary>
    private float painkillersEffect = 0.0f;

    /// <summary>
    /// How long will the antibiotics effect last, periodically decreases, when it reaches 0 the antibiotics effect is removed
    /// </summary>
    private float antibioticsEffect = 0.0f;

    /// <summary>
    /// Current bleeding strength, used for blood trail
    /// </summary>
    private int bleedStrength = 0;

    /// <summary>
    /// ItemData for the dirty bandage item, used when when bandage is swapped for a dirty bandage over itme
    /// </summary>
    [SerializeField] ItemData dirtyBandageItemData;

    /// <summary>
    /// List of possible death causes for the player
    /// </summary>
    private List<string> possibleDeathCauses = new List<string>();

    

    /// <summary>
    /// Gets all of the necessary resources and components on awake.
    /// </summary>
    void Awake()
    {
        worldStatus = GameObject.FindObjectOfType<WorldStatus>(true);
        noiseOrigin = GetComponent<NoiseOrigin>();
        bodyPartStatusEffects = new Dictionary<string, Dictionary<string, bool>>(){
            {"Head", headStatusEffects},
            {"Torso", torsoStatusEffects},
            {"LeftArm", leftArmStatusEffects},
            {"RightArm", rightArmStatusEffects},
            {"LeftLeg", leftLegStatusEffects},
            {"RightLeg", rightLegStatusEffects}
        };

        foreach (string bodyPart in bodyPartStatusEffects.Keys){
            bodyPartStatusEffects[bodyPart].Add("Bleeding", false);
            bodyPartStatusEffects[bodyPart].Add("BandageClean", false);
            bodyPartStatusEffects[bodyPart].Add("BandageDirty", false);
            bodyPartStatusEffects[bodyPart].Add("StitchedWound", false);
            bodyPartStatusEffects[bodyPart].Add("Infection", false);
            bodyPartStatusEffects[bodyPart].Add("Disinfected", false);
            bodyPartStatusEffects[bodyPart].Add("OpenWound", false);
            bodyPartStatusEffects[bodyPart].Add("Pain", false);
        }

        healthStatusUI = GameObject.FindObjectOfType<HealthStatusUI>(true);

        bloodParticles = GameObject.Find("BloodTrailEmitter").GetComponent<ParticleSystem>();
        deathScreen = GameObject.FindObjectOfType<DeathScreen>(true);
        inventoryController = GameObject.FindObjectOfType<InventoryController>(true);
        hudController = GameObject.FindObjectOfType<HUDController>(true);
    }

    //Note: I am fully aware that this way of continually resetting and checking for status effects
    //and resetting and adjusting variables accordingly is wasting resources, but working on a turn-on/turn-off
    //basis would likely introduce many bugs and edge cases (such as applying additional health drain when
    // a bleeding body part is shot again and the like), this will be reworked later but I would spend
    // a lot of time looking for bugs, so while this is wasteful, it is also more robust
    /// <summary>
    /// Each frame updates the current health status of the player.
    /// </summary>
    void Update()
    {
        UpdateStatusVariables();
        UpdatePlayerStatus();

        UpdateInfectionChance();
        UpdateInfectionSpread();
        UpdateDiminishingStatusEffects();
        BandageDeterioration();
        UpdateWoundHealing();

        healthStatusUI.UpdateHealthtext(playerHealth, maxPlayerHealth);
        healthStatusUI.UpdateHealthArrows(healthDrain, baseHealthDrain, baseHealthRegen);


        GenerateNoise();

        
    }

    /// <summary>
    /// Enables sprinting of the player.
    /// </summary>
    public void ToggleSprintOn()
    {
        if(staminaDepleted)
            return;

        isRunning = true;
        if (!isCrouched)
        {
            playerSpeed = runSpeed;
        }
    }

    /// <summary>
    /// Disables sprinting of the player.
    /// </summary>
    public void ToggleSprintOff()
    {
        isRunning = false;
        if (!isCrouched)
        {
            playerSpeed = walkSpeed;
        }
    }

    /// <summary>
    /// Updates the player's status based on their current global and local status effects.
    /// </summary>
    private void UpdateStatusVariables(){

        //Reset drainage and min/max variables to their default values
        ResetStatusVariables();

        if(isResting){
            tirednessDrain = -baseTirednessRegen;
        }else{
            tirednessDrain = baseTirednessDrain;
        }

        
        if(globalStatusEffects["Hypothermia"]){
            hungerDrain += 0.5f*baseHungerDrain;
            healthDrain += 0.5f*baseHealthDrain;
            possibleDeathCauses.Add("Hypothermia");
        }

        if(globalStatusEffects["Hyperthermia"]){
            thirstDrain += 0.5f*baseThirstDrain;
            healthDrain += 0.5f*baseHealthDrain;
            possibleDeathCauses.Add("Hyperthermia");
        }

        if(globalStatusEffects["Dehydrated"]){
            healthDrain += 0.5f*baseHealthDrain;
            bodyPartStatusEffects["Head"]["Pain"] = true;
            possibleDeathCauses.Add("Dehydration");
        }

        if(globalStatusEffects["Starving"]){
            healthDrain += 0.5f*baseHealthDrain;
            bodyPartStatusEffects["Torso"]["Pain"] = true;
            possibleDeathCauses.Add("Starvation");
        }

        if(globalStatusEffects["Tired"]){
            baseStaminaRegen -= 0.5f * baseStaminaRegen;
            shootingAbility += 0.15f;
        }

        //Hunger and thirst above 90
        if(globalStatusEffects["Nourished"]){
            healthDrain -= baseHealthRegen;
        }

        if(globalStatusEffects["Over-encumbered"]){
            runSpeed = walkSpeed;
        }else{
            runSpeed = 100f;
        }

        //Apply local status effect debuffs
        foreach(string bodyPart in bodyPartStatusEffects.Keys){
            if(bodyPartStatusEffects[bodyPart]["Bleeding"]){
                healthDrain += baseHealthDrain;
                bleedStrength += 5;
                possibleDeathCauses.Add("Bleeding");
            }
            if(bodyPartStatusEffects[bodyPart]["OpenWound"]){
                maxPlayerHealth -= 10f;
            }

            if(bodyPartStatusEffects[bodyPart]["StitchedWound"]){
                maxPlayerHealth -= 5f;
            }

            if(bodyPartStatusEffects[bodyPart]["Infection"]){
                maxPlayerStamina -= 10f;
            }

            if(bodyPartStatusEffects[bodyPart]["Pain"]){
                shootingAbility += 0.15f;
            }
        }

        //The player is currently running
        if(isMoving && isRunning){
            staminaDrain = baseStaminaDrain;
            hungerDrain += 0.5f * baseHungerDrain;
            thirstDrain += 0.5f * baseThirstDrain;
            tirednessDrain += 0.2f * baseTirednessDrain;
        }else{
            staminaDrain = -baseStaminaRegen;
        }


        //Avoids a regeneration arrow on full health in the hud
        if(playerHealth >= maxPlayerHealth && healthDrain <= 0f){
            healthDrain = 0f;
        }


    }

    /// <summary>
    /// Resets the status variables to their default values.
    /// </summary>
    private void ResetStatusVariables(){
        healthDrain = 0;
        staminaDrain = 0;
        hungerDrain = baseHungerDrain;
        thirstDrain = baseThirstDrain;
        tirednessDrain = baseTirednessDrain;
        maxPlayerHealth = 100f;
        maxPlayerStamina = 100f;
        bleedStrength = 0;
        shootingAbility = 0.1f;
        baseStaminaRegen = 0.375f;
        possibleDeathCauses.Clear();
    }

    /// <summary>
    /// Updates the overall status of the player.
    /// </summary>
    private void UpdatePlayerStatus()
    {
        UpdateHealth();
        UpdateStamina();
        UpdateHunger();
        UpdateThirst();
        UpdateTiredness();
        UpdateGlobalStatusEffects();
        BloodTrail();
    }

    /// <summary>
    /// Updates the player's hunger.
    /// </summary>
    private void UpdateHunger()
    {
        playerHunger -= (hungerDrain * Time.deltaTime * worldStatus.timeMultiplier);
        if(playerHunger < 0)
        {
            playerHunger = 0;
        }
    }

    /// <summary>
    /// Increases the player's hunger points by the given amount.
    /// </summary>
    /// <param name="amount">Amount by which the hunger points should be increased.</param>
    public void IncreaseHunger(float amount)
    {
        if(playerHunger + amount > 100)
        {
            playerHunger = 100;
        }
        else
        {
            playerHunger += amount;
        }
    }

    /// <summary>
    /// Updates the player's thirst.
    /// </summary>
    private void UpdateThirst()
    {
        playerThirst -= (thirstDrain * Time.deltaTime * worldStatus.timeMultiplier);

        if(playerThirst < 0)
        {
            playerThirst = 0;
        }
    }

    /// <summary>
    /// Increases the player's thirst points by the given amount.
    /// </summary>
    /// <param name="amount">Amount by which the thirst points should be increased.</param>
    public void IncreaseThirst(float amount)
    {
        if(playerThirst + amount > 100)
        {
            playerThirst = 100;
        }
        else
        {
            playerThirst += amount;
        }


    }

    /// <summary>
    /// Updates the player's tiredness.
    /// </summary>
    private void UpdateTiredness()
    {

        playerTiredness -= (tirednessDrain * Time.deltaTime * worldStatus.timeMultiplier);

        if(playerTiredness < 0)
        {
            playerTiredness = 0;
        }

        if(playerTiredness > 100)
        {
            playerTiredness = 100;
        }

        if(playerTiredness <= 0f && !isResting)
        {
            StartCoroutine(Faint());
        }
    }

    /// <summary>
    /// Increases the player's tiredness points by the given amount.
    /// </summary>
    /// <param name="amount">Amount by which the tiredness points should be increased.</param>
    public void IncreaseTiredness(float amount)
    {
        if(playerTiredness + amount > 100)
        {
            playerTiredness = 100;
        }
        else
        {
            playerTiredness += amount;
        }
    }

    /// <summary>
    /// Updates the player's stamina.
    /// </summary>
    public void UpdateStamina()
    {
 
        playerStamina -= (staminaDrain * Time.deltaTime * worldStatus.timeMultiplier);
        if (playerStamina > 20)
        {
            staminaDepleted = false;
        }

        if(playerStamina > maxPlayerStamina)
        {
            playerStamina = maxPlayerStamina;
        }
        else if(playerStamina < 0)
        {
            playerStamina = 0;
            ToggleSprintOff();
            staminaDepleted = true;
        }
            
        return;
    }

    /// <summary>
    /// Generates noise of the player when moving.
    /// </summary>
    private void GenerateNoise()
    {
        if (isMoving)
        {

            if(isCrouched){
                return;
            }

            if(isRunning)
            {
                noiseOrigin.GenerateNoise(50f);
            }
            else
            {
                noiseOrigin.GenerateNoise(30f);
            }         
        }
    }

    /// <summary>
    /// Updates global status effects.
    /// </summary>
    private void UpdateGlobalStatusEffects(){
        if(playerHunger > 80f && playerThirst > 80f)
        {
            EnableGlobalStatusEffect("Nourished");
        }else{
            DisableGlobalStatusEffect("Nourished");
        }

        if(playerHunger <= 0f)
        {
            EnableGlobalStatusEffect("Starving");
        }else{
            DisableGlobalStatusEffect("Starving");
        }

        if(playerThirst <= 0f)
        {
            EnableGlobalStatusEffect("Dehydrated");
        }else{
            DisableGlobalStatusEffect("Dehydrated");
        }

        if(playerTiredness <= 20f)
        {
            EnableGlobalStatusEffect("Tired");
        }else{
            DisableGlobalStatusEffect("Tired");
        }

    }

    /// <summary>
    /// Applies a bleed to a body part.
    /// </summary>
    /// <param name="bodyPart">Body part to which the bleed should be applied.</param>
    public void ApplyBleed(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Bleeding"] = true;
        bodyPartStatusEffects[bodyPart]["OpenWound"] = true;
        healthStatusUI.ApplyBleed(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }


    /// <summary>
    /// Applies a bandage to a body part.
    /// </summary>
    /// <param name="bodyPart">Body part to which the bandage should be applied.</param>
    /// <param name="bandage">Reference to the bandage item.</param>
    public void ApplyBandage(string bodyPart, InventoryItem bandage){
            
        bool isDirty = false;
        if(bandage.itemData.itemName == "Dirty Bandage"){
            bodyPartStatusEffects[bodyPart]["BandageDirty"] = true;
            isDirty = true;
        }else if(bandage.itemData.itemName == "Clean Bandage"){
            bodyPartStatusEffects[bodyPart]["BandageClean"] = true;
        }

        bodyPartStatusEffects[bodyPart]["Bleeding"] = false;
        healthStatusUI.ApplyBandage(bodyPart, isDirty);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
        bandageItems[bodyPart] = bandage;
    
    }

    /// <summary>
    /// Removes a bandage from a body part.
    /// </summary>
    /// <param name="bodyPart">Body part from which the bandage should be removed.</param>
    public void RemoveBandage(string bodyPart){
        if(bodyPartStatusEffects[bodyPart]["OpenWound"])
            ApplyBleed(bodyPart);

        bodyPartStatusEffects[bodyPart]["BandageDirty"] = false;
        bodyPartStatusEffects[bodyPart]["BandageClean"] = false;

        healthStatusUI.RemoveBandage(bodyPart, bodyPartStatusEffects[bodyPart]["OpenWound"]);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
        bandageItems[bodyPart] = null;
    }

    /// <summary>
    /// Swaps a clean bandage for a dirty one.
    /// </summary>
    /// <param name="bodyPart">Body part to which the bandage should be swapped.</param>
    public void DirtyBandage(string bodyPart){
        bodyPartStatusEffects[bodyPart]["BandageDirty"] = true;
        bodyPartStatusEffects[bodyPart]["BandageClean"] = false;

        healthStatusUI.DirtyBandage(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    /// <summary>
    /// Stitches a wound on a body part.
    /// </summary>
    /// <param name="bodyPart">Body part to which the wound should be stitched.</param>
    public void StitchWound(string bodyPart){
        bodyPartStatusEffects[bodyPart]["StitchedWound"] = true;
        bodyPartStatusEffects[bodyPart]["OpenWound"] = false;
        bodyPartStatusEffects[bodyPart]["Bleeding"] = false;
        healthStatusUI.StitchWound(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
        
    }

    /// <summary>
    /// Removes a stitched wound from a body part.
    /// </summary>
    /// <param name="bodyPart">Body part from which the stitched wound should be removed.</param>
    private void RemoveStitchedWound(string bodyPart){
        bodyPartStatusEffects[bodyPart]["StitchedWound"] = false;
        woundHealingStatus[bodyPart] = 0f;
        healthStatusUI.RemoveStitchedWound(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    /// <summary>
    /// Applies an infection to a body part.
    /// </summary>
    /// <param name="bodyPart">Body part to which the infection should be applied.</param>
    public void GetInfection(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Infection"] = true;

        healthStatusUI.GetInfection(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    /// <summary>
    /// Removes an infection from a body part.
    /// </summary>
    /// <param name="bodyPart">Body part from which the infection should be removed.</param>
    public void RemoveInfection(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Infection"] = false;

        healthStatusUI.RemoveInfection(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    /// <summary>
    /// Disinfects a body part.
    /// </summary>
    /// <param name="bodyPart">Body part to which the disinfectant should be applied.</param>
    public void Disinfect(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Disinfected"] = true;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
        disinfectantRemaining[bodyPart] = 100f;
    }

    /// <summary>
    /// Removes a disinfectant from a body part.
    /// </summary>
    /// <param name="bodyPart">Body part from which the disinfectant should be removed.</param>
    public void RemoveDisinfectant(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Disinfected"] = false;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    /// <summary>
    /// Applies pain to a body part.
    /// </summary>
    /// <param name="bodyPart">Body part to which the pain should be applied.</param>
    public void ApplyPain(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Pain"] = true;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    /// <summary>
    /// Removes pain from a body part.
    /// </summary>
    /// <param name="bodyPart">Body part from which the pain should be removed.</param>
    public void RemovePain(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Pain"] = false;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    /// <summary>
    /// Hets body parts relevant ot the currently held medical item. Used for highlighting.
    /// </summary>
    /// <param name="itemName">Name of the currently held medical item.</param>
    /// <returns></returns>
    public List<string> GetRelevantBodyParts(string itemName){
        List<string> relevantBodyParts = new List<string>();
        foreach(string bodyPart in bodyPartStatusEffects.Keys){
            if(itemName == "Clean Bandage" || itemName == "Dirty Bandage"){
                if((bodyPartStatusEffects[bodyPart]["OpenWound"] || 
                bodyPartStatusEffects[bodyPart]["StitchedWound"]) &&
                !bodyPartStatusEffects[bodyPart]["BandageClean"] &&
                !bodyPartStatusEffects[bodyPart]["BandageDirty"])
                {
                    relevantBodyParts.Add(bodyPart);
                }
            }
            else if(itemName == "Suture Needle"){
                if(bodyPartStatusEffects[bodyPart]["OpenWound"] && 
                !bodyPartStatusEffects[bodyPart]["StitchedWound"] &&
                !bodyPartStatusEffects[bodyPart]["BandageClean"] &&
                !bodyPartStatusEffects[bodyPart]["BandageDirty"])
                {
                    relevantBodyParts.Add(bodyPart);
                }
            }
            else if(itemName == "Antiseptic"){
                if(!bodyPartStatusEffects[bodyPart]["Disinfected"] &&
                (bodyPartStatusEffects[bodyPart]["OpenWound"] || bodyPartStatusEffects[bodyPart]["StitchedWound"]))
                {
                    relevantBodyParts.Add(bodyPart);
                }
            }
        }

        return relevantBodyParts;
    }

    /// <summary>
    /// Applies all effects associated with being shot to a body part.
    /// </summary>
    /// <param name="damage">Damage dealt by the shot.</param>
    /// <param name="bodyPart">Body part that was shot.</param>
    public void GotShot(float damage, string bodyPart){
        if(bodyPart == "Head"){
            DecreaseHealth(damage * 3.5f);
        }else if(bodyPart == "Torso"){
            DecreaseHealth(damage);
        }else{
            DecreaseHealth(damage * 0.5f);
        }

        ApplyBleed(bodyPart);
        bodyPartStatusEffects[bodyPart]["StitchedWound"] = false;

        if(!globalStatusEffects["Painkillers"]){
            ApplyPain(bodyPart);
        }

        if(bandageItems[bodyPart] != null){
            inventoryController.DestroyBandage(bodyPart);
        }

        if(playerHealth <= 0){
            Death("Gunshot Wound");
        }
    }

    /// <summary>
    /// Enables a global status effect.
    /// </summary>
    /// <param name="effectName">Name of the global status effect.</param>
    public void EnableGlobalStatusEffect(string effectName){
        globalStatusEffects[effectName] = true;
        healthStatusUI.AddGlobalStatusEffect(effectName);
        if(effectName == "Painkillers"){
            painkillersEffect = 100f;
        }

        if(effectName == "Antibiotics"){
            antibioticsEffect = 100f;
        }
    }

    /// <summary>
    /// Disables a global status effect.
    /// </summary>
    /// <param name="effectName">Name of the global status effect.</param>
    public void DisableGlobalStatusEffect(string effectName){
        globalStatusEffects[effectName] = false;
        healthStatusUI.RemoveGlobalStatusEffect(effectName);
    }

    
    /// <summary>
    /// Decreases the health of the player by a certain amount.
    /// </summary>
    /// <param name="amount">Amount of health to decrease.</param>
    public void DecreaseHealth(float amount)
    {
        playerHealth -= amount;
        if(playerHealth < 0)
        {
            playerHealth = 0;
        }
    }

    /// <summary>
    /// Updates the health of the player.
    /// </summary>
    public void UpdateHealth()
    {
        
        playerHealth -= (healthDrain * Time.deltaTime * worldStatus.timeMultiplier);

        if(playerHealth > maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;
        }

        if(playerHealth <= 0)
        {
            playerHealth = 0;
            if(possibleDeathCauses.Count > 0)
                Death(possibleDeathCauses[Random.Range(0, possibleDeathCauses.Count)]);
            else
                Death("Unknown");
        }

    }

    /// <summary>
    /// Updates the infection chance for individual body parts.
    /// </summary>
    private void UpdateInfectionChance(){
        if(infectionRollTimer > 0f){
            infectionRollTimer -= Time.deltaTime * worldStatus.timeMultiplier;
            return;
        }
        
        foreach(string bodyPart in bodyPartStatusEffects.Keys){
            if(!bodyPartStatusEffects[bodyPart]["Infection"]){
                //If the body part is disinfected, it cannot get infected
                //Disinfection also removes any previously accumulated infection chance
                if(bodyPartStatusEffects[bodyPart]["Disinfected"]){
                    infectionChance[bodyPart] = 0f;
                    continue;
                }
                if(bodyPartStatusEffects[bodyPart]["OpenWound"] || bodyPartStatusEffects[bodyPart]["StitchedWound"]){
                    float randomNum = Random.Range(0f, 1f);
                    if(randomNum < infectionChance[bodyPart]){
                        GetInfection(bodyPart);
                        infectionChance[bodyPart] = 0f;
                        continue;
                    }else{
                        //Only an open wound accumulates infection chance, however a stitched wound
                        // can still get infected
                        if(bodyPartStatusEffects[bodyPart]["OpenWound"]){
                            //Clean bandage lowers the rate at which the chance of infection increases
                            if(bodyPartStatusEffects[bodyPart]["BandageClean"])
                                infectionChance[bodyPart] += 0.005f;
                            else
                                infectionChance[bodyPart] += 0.02f;
                        }
                    }
                }
            }
        }

        infectionRollTimer = 600f;
    }

    /// <summary>
    /// Updates infection spread for individual body parts.
    /// </summary>
    private void UpdateInfectionSpread(){
        float infectionSpreadRate = 0.00277f; //Roughtly 10 in game hours for full infection
        if(globalStatusEffects["Antibiotics"]){
            infectionSpreadRate = -0.00554f; //Antibiotics remove infection completely in roughly 5 in game hours
        }
        foreach(string bodyPart in bodyPartStatusEffects.Keys){
            if(bodyPartStatusEffects[bodyPart]["Infection"]){
                infectionSpread[bodyPart] += infectionSpreadRate * Time.deltaTime * worldStatus.timeMultiplier;
                if(infectionSpread[bodyPart] >= 100f){
                    infectionSpread[bodyPart] = 0f;
                }else if(infectionSpread[bodyPart] <= 0f){
                    infectionSpread[bodyPart] = 0f;
                    RemoveInfection(bodyPart);
                }
            }
        }
    }

    /// <summary>
    /// Updates status effects which diminish over time.
    /// </summary>
    private void UpdateDiminishingStatusEffects(){
        foreach(string bodyPart in bodyPartStatusEffects.Keys){
            if(bodyPartStatusEffects[bodyPart]["Disinfected"]){
                //Disinfectant lasts for 10 in game hours if a clean bandage is applied with it
                //It lasts for 3 in game hours if a dirty bandage is applied with it
                //If no bandage is applied, it lasts for 1 in game hour
                if(bodyPartStatusEffects[bodyPart]["BandageClean"]){
                    disinfectantRemaining[bodyPart] -= 0.00277f * Time.deltaTime * worldStatus.timeMultiplier;
                }else if(bodyPartStatusEffects[bodyPart]["BandageDirty"]){
                    disinfectantRemaining[bodyPart] -= 0.00926f * Time.deltaTime * worldStatus.timeMultiplier;
                }else{
                    disinfectantRemaining[bodyPart] -= 0.027f * Time.deltaTime * worldStatus.timeMultiplier;
                }
                if(disinfectantRemaining[bodyPart] <= 0f){
                    RemoveDisinfectant(bodyPart);
                }
            }
        }

        //Painkillers last for 3 hours
        painkillersEffect -= 0.009259f * Time.deltaTime * worldStatus.timeMultiplier;
        if(painkillersEffect <= 0f){
            painkillersEffect = 0f;
            DisableGlobalStatusEffect("Painkillers");
            foreach(string bodyPart in bodyPartStatusEffects.Keys){
                if(bodyPartStatusEffects[bodyPart]["OpenWound"] || bodyPartStatusEffects[bodyPart]["StitchedWound"]){
                    ApplyPain(bodyPart);
                }
            }
        }

        //Antibiotics last for 3 hours
        antibioticsEffect -= 0.009259f * Time.deltaTime * worldStatus.timeMultiplier;
        if(antibioticsEffect <= 0f){
            antibioticsEffect = 0f;
            DisableGlobalStatusEffect("Antibiotics");
        }
    }

    /// <summary>
    /// Uses a global health item.
    /// </summary>
    /// <param name="itemData">Item Data of teh used health item.</param>
    public void UseHealthItem(ItemData itemData){
        if(itemData.itemName == "Painkillers"){
            EnableGlobalStatusEffect("Painkillers");
            foreach(string bodyPart in bodyPartStatusEffects.Keys){
                if(bodyPartStatusEffects[bodyPart]["Pain"]){
                    RemovePain(bodyPart);
                }
            }
        }

        if(itemData.itemName == "Antibiotics"){
            EnableGlobalStatusEffect("Antibiotics");
        }
    }

    /// <summary>
    /// Updates the healing of stitched wounds.
    /// </summary>
    private void UpdateWoundHealing(){
        foreach(string bodyPart in bodyPartStatusEffects.Keys){
            if(bodyPartStatusEffects[bodyPart]["StitchedWound"]){
                woundHealingStatus[bodyPart] += 0.0027f * Time.deltaTime * worldStatus.timeMultiplier; //10hr
                if(woundHealingStatus[bodyPart] >= 100f){
                    RemoveStitchedWound(bodyPart);
                    RemovePain(bodyPart);
                }
            }
        }
    }

    /// <summary>
    /// Updates the deterioration of bandages.
    /// </summary>
    private void BandageDeterioration(){
        List<string> bodyParts = new List<string>(bandageItems.Keys);
        foreach(string bodyPart in bodyParts){
            InventoryItem bandage = bandageItems[bodyPart];
            //Clean bandages last for 3 in game hours if covering an open wound
            if(bodyPartStatusEffects[bodyPart]["OpenWound"] && bandage != null){
                float currentValue = bandage.BandageDeterioration(0.00926f * Time.deltaTime * worldStatus.timeMultiplier);
                if(currentValue <= 0f){
                    SwapForDirtyBandage(bodyPart);
                }
            }
        }
    }

    /// <summary>
    /// Swaps a clean bandage for a dirty bandage.
    /// </summary>
    /// <param name="bodyPart">Body part to swap bandage for.</param>
    private void SwapForDirtyBandage(string bodyPart){
        InventoryController inventoryController = GameObject.FindObjectOfType<InventoryController>(true);
        inventoryController.SwapForDirtyBandage(bodyPart, dirtyBandageItemData);
    }

    /// <summary>
    /// Updates the emission of a blood trail.
    /// </summary>
    private void BloodTrail(){
        ParticleSystem.EmissionModule emission = bloodParticles.emission;
        if(bleedStrength <= 0f)
            emission.rateOverTime = 0f;
        else
            emission.rateOverTime = Random.Range(bleedStrength - 2, bleedStrength + 2);
    }

    /// <summary>
    /// Coroutine that simulates the player fainting.
    /// </summary>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator Faint(){
        isResting = true;
        TogglePlayerMovement(false);
        TogglePlayerVisibility(false);
        //This will appear as the player fainting
        GameObject deadBody = InstantiateDeadBody();
        float hoursToRest = (20f/baseTirednessRegen)/3600f;
        yield return new WaitForSeconds(2f);
        Rest(hoursToRest, deadBody);

    }

    /// <summary>
    /// Causes the player to die.
    /// </summary>
    /// <param name="causeOfDeath">Cause of death.</param>
    private void Death(string causeOfDeath){
        //Prevent this call from being made multiple times
        if(this.enabled == false)
            return;

        TogglePlayerMovement(false);
        deathScreen.ActivateDeathScreen(causeOfDeath);

        InstantiateDeadBody();
        
        //We do not want ot destroy the player game object as many other scripts depend on it
        //Therefore we make it invisible
        TogglePlayerVisibility(false);

        //Disable fov
        GameObject.Find("FOV").SetActive(false);

        //Disabling rigidbody makes sure enemies no longer target the player
        GetComponent<Rigidbody2D>().simulated = false;

        this.enabled = false;
    }

    /// <summary>
    /// Toggles the player's visibility.
    /// </summary>
    /// <param name="visible">Whether the player should be visible or not.</param>
    public void TogglePlayerVisibility(bool visible){

        GameObject torso = transform.Find("Torso").gameObject;
        if(visible){
            torso.GetComponent<SpriteRenderer>().enabled = true;
            torso.transform.Find("HeadPivot").Find("Head").gameObject.GetComponent<SpriteRenderer>().enabled = true;
            torso.transform.Find("Legs").gameObject.GetComponent<SpriteRenderer>().enabled = true;
            torso.transform.Find("FirearmSprite").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }else{
            torso.GetComponent<SpriteRenderer>().enabled = false;
            torso.transform.Find("HeadPivot").Find("Head").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            torso.transform.Find("Legs").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            torso.transform.Find("FirearmSprite").gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Instantiates a dead body at the position of the player with the right rotation.
    /// </summary>
    /// <returns></returns>
    public GameObject InstantiateDeadBody(){
        Vector2 direction = transform.up;
        Vector2 deadBodyPosition2D = new Vector2(transform.position.x + direction.x * 10f, transform.position.y + direction.y * 10f);
        Vector3 deadBodyPosition3D = new Vector3(deadBodyPosition2D.x, deadBodyPosition2D.y, -1);
        GameObject deadBody = Instantiate(deadBodyPrefab, deadBodyPosition3D, transform.rotation);
        if(Random.Range(0, 1) < 0.5f){
            deadBody.GetComponent<SpriteRenderer>().flipX = true;
        }

        return deadBody;
    }

    /// <summary>
    /// Whether an inventory was opened.
    /// </summary>
    public void InventoryOpened(){
        isRunning = false;
    }

    /// <summary>
    /// Implements resting for the player.
    /// </summary>
    /// <param name="hours">How many hours should the player rest for.</param>
    /// <param name="deadBody">Reference to the dead body game object. Used when fainting.</param>
    public void Rest(float hours, GameObject deadBody = null){
        StartCoroutine(RestTransition(hours, deadBody));
    }

    /// <summary>
    /// Transitions the player to the resting state.
    /// </summary>
    /// <param name="hours">How many hours should the player rest for.</param>
    /// <param name="deadBody">Reference to the dead body game object. Used when fainting.</param>
    /// <returns></returns>
    private IEnumerator RestTransition(float hours, GameObject deadBody = null){
        hudController.DeactivateRestMenu();
        worldStatus.StopTime();
        float timeMultiplier = CalculateRestTimeMultiplier(hours);
        StartCoroutine(hudController.ActivateRestScreen());
        yield return new WaitForSecondsRealtime(3f);
        isResting = true;
        worldStatus.SetTimeMultiplier(timeMultiplier);
        worldStatus.StartTime();
        yield return new WaitForSecondsRealtime(5f);
        if(deadBody != null){
            Destroy(deadBody);
            TogglePlayerVisibility(true);
        }
        worldStatus.StopTime();
        isResting = false;
        worldStatus.SetTimeMultiplier(20f);
        StartCoroutine(hudController.DeactivateRestScreen());
        yield return new WaitForSecondsRealtime(3f);
        worldStatus.StartTime();
        TogglePlayerMovement(true);
        
    }

    /// <summary>
    /// Calculates how much faster should the time flow when resting to always make it a 5 second transition.
    /// </summary>
    /// <param name="hours">How many hours should the player rest for.</param>
    /// <returns>The multiplier for the time flow.</returns>
    private float CalculateRestTimeMultiplier(float hours){
        float realSeconds = (hours) * 3600f;
        return (realSeconds/5f);

    }

    /// <summary>
    /// Toggles the movement ability of the player.
    /// </summary>
    /// <param name="toggle"></param>
    private void TogglePlayerMovement(bool toggle){
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if(toggle){
            playerInput.ActivateInput();
        }else{
            playerInput.DeactivateInput();
        }
    }



    
}
