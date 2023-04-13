using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Implements variables describing the current status of the player and methods to manipulate these variables
/// </summary>
public class PlayerStatus : MonoBehaviour
{

    private WorldStatus worldStatus;

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


    /// <summary>
    /// Current speed of the player
    /// </summary>
    public float playerSpeed = 50.0f;

    /// <summary>
    /// Default crouch speed of the player
    /// </summary>
    public float crouchSpeed = 20.0f;

    /// <summary>
    /// Default walk speed of the player
    /// </summary>
    public float walkSpeed = 50.0f;

    /// <summary>
    /// Default sprint speed of the player
    /// </summary>
    public float runSpeed = 100.0f;


    /// <summary>
    /// Whether the player is currently hypothermic
    /// </summary>
    public bool playerHypothermia = false;

    /// <summary>
    /// Whether the player is currently hyperthermic
    /// </summary>
    public bool playerHyperthermia = false;

    /// <summary>
    /// Whether the player is currently crouched
    /// </summary>
    public bool isCrouched = false;

    /// <summary>
    /// Whether the player is currently sprinting
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
    /// Ability of the player to handle firearms. 0 means the best possible
    /// </summary>
    public float shooterAbility;

    /// <summary>
    /// Base stamina drain when running
    /// </summary>
    public float baseStaminaDrain = 5;

    /// <summary>
    /// Base stamina regeneration when not running
    /// </summary>
    public float baseStaminaRegen = 7.5f;

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

    public float baseTirednessRegen = 0.00345f;  //Regenerates completely from nothing to full after roughly 24 minutes of gameplay (8 hours in game)
    public float healthDrain = 0.0f;
    public float staminaDrain = 0.0f;

    public float hungerDrain = 0.0f;
    public float thirstDrain = 0.0f;
    public float tirednessDrain = 0.0f;

    public float bodyTempDifference = 0.0f;

    public bool isResting = false;



    /// <summary>
    /// Base health drain, used in the calculation of bleed
    /// </summary>
    public float baseHealthDrain = 1.0f;        //While of course, healthy character does not periodically lose health, this is used in the calculation for health drain arrows in the HUD
                                                //to simulate the rate of potential bleeds

    /// <summary>
    /// Base health regeneration rate
    /// </summary>
    public float baseHealthRegen = 0.5f;        

    /// <summary>
    /// Whether the player's stamina is depleted. Used to block sprint after depleting the stamina and unlocking it again after regenerating it back to 20%
    /// </summary>
    private bool staminaDepleted = false;

    /// <summary>
    /// Reference to the HUD controller
    /// </summary>
    private HUDController HUD;

    private NoiseOrigin noiseOrigin;




    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerStatusBars();
        worldStatus = GameObject.FindObjectOfType<WorldStatus>();
        HUD = GameObject.FindObjectOfType<HUDController>();
        noiseOrigin = GetComponent<NoiseOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStatus();
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


    /// <summary>
    /// Loads player status bars from a save file
    /// For now only sets all values to full TODO
    /// </summary>
    /// <returns>0 when load is successful, 1 when unsuccessful</returns>
    int LoadPlayerStatusBars()
    {
        /*playerHealth = 100.0f;
        playerStamina = 100.0f;
        playerHunger = 100.0f;
        playerThirst = 100.0f;
        playerBodyTemp = 0.0f; //Temperatrure ranges between -100 and 100
        playerTiredness = 100.0f;*/

        return 0;
    }

    private void UpdateHunger(bool hypothermic, bool running)
    {
        hungerDrain = baseHungerDrain;
        if (hypothermic)
            hungerDrain += 0.5f * baseHungerDrain;
        if (running)
            hungerDrain += 0.5f * baseHungerDrain;

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

    private void UpdateThirst(bool hyperthermic, bool running)
    {
        thirstDrain = baseThirstDrain;
        if (hyperthermic)
            thirstDrain += 0.5f * baseThirstDrain;
        if (running)
            thirstDrain += 0.5f * baseThirstDrain;
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

    private void UpdateTiredness(bool running)
    {
        if(isResting)
        {
            playerTiredness += (baseTirednessRegen * Time.deltaTime * worldStatus.timeMultiplier);
            if(playerTiredness > 100)
            {
                playerTiredness = 100;
            }
        }else{
            tirednessDrain = baseTirednessDrain;
            if (running)
                tirednessDrain += 0.2f * baseTirednessDrain;

            playerTiredness -= (tirednessDrain * Time.deltaTime * worldStatus.timeMultiplier);

            if(playerTiredness < 0)
            {
                playerTiredness = 0;
            }
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
    public void UpdateStamina(bool running)
    {
        if(running)
            staminaDrain = baseStaminaDrain;
        else if(!running && playerStamina < 100)
            staminaDrain = -baseStaminaRegen;
        else
            staminaDrain = 0;

        if(staminaDrain > 0 && playerStamina <= 0)
        {
            playerStamina = 0;
            ToggleSprintOff();
            staminaDepleted = true;
            return;
        }else if(staminaDrain < 0 && playerStamina >= 100)
        {
            playerStamina = 100;
            return;
        }
        playerStamina -= (staminaDrain * Time.deltaTime);
        if (playerStamina > 20)
        {
            staminaDepleted = false;
        }
        return;
    }


    private void UpdatePlayerStatus()
    {
        bool running = isRunning && isMoving;
        UpdateHunger(playerHypothermia, running);
        UpdateThirst(playerHyperthermia, running);
        UpdateTiredness(running);
        UpdateStamina(running);
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
}
