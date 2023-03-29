using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Implements variables describing the current status of the player and methods to manipulate these variables
/// </summary>
public class PlayerStatus : MonoBehaviour
{
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
    public float playerTemp = 0.0f;

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
    public float sprintSpeed = 100.0f;


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
    public bool playerCrouched = false;

    /// <summary>
    /// Whether the player is currently sprinting
    /// </summary>
    public bool playerSprint = false;

    /// <summary>
    /// Whether the player is currently aiming
    /// </summary>
    public bool playerAiming = false;

    /// <summary>
    /// Whether the player is currently moving
    /// </summary>
    public bool playerMoving = false;


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
    public float baseHungerDrain = 0.111f;      //Depletes completely from full to nothing after roughly 15 minutes of gameplay (15 hours in game)

    /// <summary>
    /// Base thirst drain
    /// </summary>
    public float baseThirstDrain = 0.166f;      //Depletes completely from full to nothing after roughly 10 minutes of gameplay (10 hours in game)

    /// <summary>
    /// Base tiredness drain
    /// </summary>
    public float baseTirednessDrain = 0.0694f;  //Depletes completely from full to nothing after roughly 24 minutes of gameplay (24 hours in game)

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
    [SerializeField] private HUDController HUD;




    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerStatusBars();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStatus();
        
    }

    public void ToggleSprintOn()
    {
        if(staminaDepleted)
            return;

        playerSprint = true;
        if (!playerCrouched)
        {
            playerSpeed = sprintSpeed;
        }
    }

    public void ToggleSprintOff()
    {
        playerSprint = false;
        if (!playerCrouched)
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
        playerHealth = 100.0f;
        playerStamina = 100.0f;
        playerHunger = 100.0f;
        playerThirst = 100.0f;
        playerTemp = 0.0f; //Temperatrure ranges between -100 and 100
        playerTiredness = 100.0f;

        return 0;
    }

    private void UpdateHunger(bool hypothermic, bool running)
    {
        float drain = baseHungerDrain;
        if (hypothermic)
            drain += 0.5f * drain;
        if (running)
            drain += 0.5f * drain;

        playerHunger -= (drain * Time.deltaTime);
    }

    public void IncreaseHunger(float amount)
    {
        playerHunger += amount;
    }

    private void UpdateThirst(bool hyperthermic, bool running)
    {
        float drain = baseThirstDrain;
        if (hyperthermic)
            drain += 0.5f * drain;
        if (running)
            drain += 0.5f * drain;

        playerThirst -= (drain * Time.deltaTime);
    }

    public void IncreaseThirst(float amount)
    {
        playerThirst += amount;
    }

    private void UpdateTiredness(bool running)
    {
        float drain = baseTirednessDrain;
        if (running)
            drain += 0.2f * drain;

        playerTiredness -= (drain * Time.deltaTime);
    }

    public void IncreaseTiredness(float amount)
    {
        playerTiredness += amount;
    }

    //TODO: Iplement weight debuff
    public void UpdateStamina(bool running)
    {
        float drain;
        if(running)
            drain = baseStaminaDrain;
        else
            drain = -baseStaminaRegen; //Regenerates stamina when not running

        if(drain > 0 && playerStamina <= 0)
        {
            playerStamina = 0;
            ToggleSprintOff();
            staminaDepleted = true;
            HUD.UpdateStamina(0);
            return;
        }else if(drain < 0 && playerStamina >= 100)
        {
            playerStamina = 100;
            HUD.UpdateStamina(0);
            return;
        }
        playerStamina -= (drain * Time.deltaTime);
        if (playerStamina > 20)
        {
            staminaDepleted = false;
        }
        HUD.UpdateStamina(drain);
        return;
    }


    private void UpdatePlayerStatus()
    {
        bool running = playerSprint && playerMoving;
        UpdateHunger(playerHypothermia, running);
        UpdateThirst(playerHyperthermia, running);
        UpdateTiredness(running);
        UpdateStamina(running);
    }
}
