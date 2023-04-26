using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    /// Base tiredness regeneration when sleeping
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

    private List<string> possibleDeathCauses = new List<string>();

    

    // Start is called before the first frame update
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

        bloodParticles = GetComponent<ParticleSystem>();
        deathScreen = GameObject.FindObjectOfType<DeathScreen>(true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatusVariables();
        UpdatePlayerStatus();

        UpdateInfectionChance();
        UpdateInfectionSpread();
        UpdateDiminishingStatusEffects();
        BandageDeterioration();
        //UpdateWoundHealing();

        healthStatusUI.UpdateHealthtext(playerHealth, maxPlayerHealth);
        healthStatusUI.UpdateHealthArrows(healthDrain, baseHealthDrain, baseHealthRegen);


        GenerateNoise();

        
    }

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

    public void ToggleSprintOff()
    {
        isRunning = false;
        if (!isCrouched)
        {
            playerSpeed = walkSpeed;
        }
    }

    private void UpdateStatusVariables(){

        //Reset drainage and min/max variables to their default values
        ResetStatusVariables();

        if(isResting){
            tirednessDrain = -baseTirednessRegen;
        }else{
            tirednessDrain = baseTirednessDrain;
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
            possibleDeathCauses.Add("Dehydration");
        }

        if(globalStatusEffects["Starving"]){
            healthDrain += 0.5f*baseHealthDrain;
            possibleDeathCauses.Add("Starvation");
        }

        //Hunger and thirst above 90
        if(globalStatusEffects["Nourished"]){
            healthDrain -= baseHealthRegen;
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

        //Avoids a regeneration arrow on full health in the hud
        if(playerHealth >= maxPlayerHealth && healthDrain <= 0f){
            healthDrain = 0f;
        }


    }

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
        possibleDeathCauses.Clear();
    }

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

    private void UpdateHunger()
    {
        playerHunger -= (hungerDrain * Time.deltaTime * worldStatus.timeMultiplier);
        if(playerHunger < 0)
        {
            playerHunger = 0;
        }
    }

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

    private void UpdateThirst()
    {
        playerThirst -= (thirstDrain * Time.deltaTime * worldStatus.timeMultiplier);

        if(playerThirst < 0)
        {
            playerThirst = 0;
        }
    }

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
        
    }

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

    //TODO: Iplement weight debuff
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

    }

    public void ApplyBleed(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Bleeding"] = true;
        bodyPartStatusEffects[bodyPart]["OpenWound"] = true;
        healthStatusUI.ApplyBleed(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }


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

    public void RemoveBandage(string bodyPart){
        if(bodyPartStatusEffects[bodyPart]["OpenWound"])
            ApplyBleed(bodyPart);

        bodyPartStatusEffects[bodyPart]["BandageDirty"] = false;
        bodyPartStatusEffects[bodyPart]["BandageClean"] = false;

        healthStatusUI.RemoveBandage(bodyPart, bodyPartStatusEffects[bodyPart]["OpenWound"]);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
        bandageItems[bodyPart] = null;
    }

    public void DirtyBandage(string bodyPart){
        bodyPartStatusEffects[bodyPart]["BandageDirty"] = true;
        bodyPartStatusEffects[bodyPart]["BandageClean"] = false;

        healthStatusUI.DirtyBandage(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    public void StitchWound(string bodyPart){
        bodyPartStatusEffects[bodyPart]["StitchedWound"] = true;
        bodyPartStatusEffects[bodyPart]["OpenWound"] = false;
        bodyPartStatusEffects[bodyPart]["Bleeding"] = false;
        healthStatusUI.StitchWound(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
        
    }

    public void GetInfection(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Infection"] = true;

        healthStatusUI.GetInfection(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    public void RemoveInfection(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Infection"] = false;

        healthStatusUI.RemoveInfection(bodyPart);
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    public void Disinfect(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Disinfected"] = true;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
        disinfectantRemaining[bodyPart] = 100f;
    }

    public void RemoveDisinfectant(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Disinfected"] = false;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    public void ApplyPain(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Pain"] = true;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }

    public void RemovePain(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Pain"] = false;
        healthStatusUI.UpdateStatusEffects(bodyPart, bodyPartStatusEffects[bodyPart]);
    }


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

    public void GotShot(float damage, string bodyPart){
        if(bodyPart == "Head"){
            DecreaseHealth(damage * 3.5f);
        }else if(bodyPart == "Torso"){
            DecreaseHealth(damage);
        }else{
            DecreaseHealth(damage * 0.5f);
        }

        RemoveBandage(bodyPart);
        ApplyBleed(bodyPart);

        if(!globalStatusEffects["Painkillers"]){
            ApplyPain(bodyPart);
        }

        if(playerHealth <= 0){
            Death("Gunshot Wound");
        }
    }

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

    public void DisableGlobalStatusEffect(string effectName){
        globalStatusEffects[effectName] = false;
        healthStatusUI.RemoveGlobalStatusEffect(effectName);
    }

    

    public void DecreaseHealth(float amount)
    {
        playerHealth -= amount;
        if(playerHealth < 0)
        {
            playerHealth = 0;
        }
    }

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

    private void SwapForDirtyBandage(string bodyPart){
        InventoryController inventoryController = GameObject.FindObjectOfType<InventoryController>(true);
        inventoryController.SwapForDirtyBandage(bodyPart, dirtyBandageItemData);
    }

    private void BloodTrail(){
        ParticleSystem.EmissionModule emission = bloodParticles.emission;
        if(bleedStrength <= 0f)
            emission.rateOverTime = 0f;
        else
            emission.rateOverTime = Random.Range(bleedStrength - 2, bleedStrength + 2);
    }

    private void Death(string causeOfDeath){
        //Prevent this call from being made multiple times
        if(this.enabled == false)
            return;
        deathScreen.ActivateDeathScreen(causeOfDeath);
        Vector2 direction = transform.up;
        Vector2 deadBodyPosition2D = new Vector2(transform.position.x + direction.x * 10f, transform.position.y + direction.y * 10f);
        Vector3 deadBodyPosition3D = new Vector3(deadBodyPosition2D.x, deadBodyPosition2D.y, -1);
        GameObject deadBody = Instantiate(deadBodyPrefab, deadBodyPosition3D, transform.rotation);
        if(Random.Range(0, 1) < 0.5f){
            deadBody.GetComponent<SpriteRenderer>().flipX = true;
        }
        
        //We do not want ot destroy the player game object as many other scripts depend on it
        //Therefore we make it invisible

        GameObject torso = transform.Find("Torso").gameObject;
        torso.GetComponent<SpriteRenderer>().enabled = false;
        torso.transform.Find("HeadPivot").Find("Head").gameObject.GetComponent<SpriteRenderer>().enabled = false;
        torso.transform.Find("Legs").gameObject.GetComponent<SpriteRenderer>().enabled = false;
        torso.transform.Find("FirearmSprite").gameObject.GetComponent<SpriteRenderer>().enabled = false;

        //Disable fov
        GameObject.Find("FOV").SetActive(false);

        //Disabling rigidbody makes sure enemies no longer target the player
        GetComponent<Rigidbody2D>().simulated = false;

        //Disable player controls
        Destroy(GetComponent<PlayerControls>());
        Destroy(GetComponent<UIControls>());
        this.enabled = false;
    }



    
}
