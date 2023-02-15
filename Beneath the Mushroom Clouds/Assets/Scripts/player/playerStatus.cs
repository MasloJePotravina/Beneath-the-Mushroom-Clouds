////////////////////////////////////////////
// File: playerStatusScr.cs               //
// Project: Beneath the Mushroom Clouds   //
// Author: Ondrej Kováč                   //
// Brief: Status of the Player Character  //
////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script holds all the variables that affect the status of the player character such as health, stamina, whether the character is crouching and so on.
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    public float playerHealth = 0.0f;
    public float playerStamina = 0.0f;
    public float playerHunger = 0.0f;
    public float playerThirst = 0.0f;
    public float playerTemp = 0.0f;
    public float playerTiredness = 0.0f;

    public float playerSpeed = 50.0f;
    public float crouchSpeed = 20.0f;
    public float walkSpeed = 50.0f;
    public float sprintSpeed = 100.0f;

    public bool playerHypothermia = false;
    public bool playerHyperthermia = false;
    public bool playerCrouched = false;
    public bool playerSprint = false;
    public bool playerAiming = false;
    public bool playerMoving = false;

    public float shooterAbility;

    public float baseStaminaDrain = 5;
    public float baseStaminaRegen = 7.5f;
    public float baseHungerDrain = 0.111f;      //Depletes completely from full to nothing after roughly 15 minutes of gameplay (15 hours in game)
    public float baseThirstDrain = 0.166f;      //Depletes completely from full to nothing after roughly 10 minutes of gameplay (10 hours in game)
    public float baseTirednessDrain = 0.0694f;  //Depletes completely from full to nothing after roughly 24 minutes of gameplay (24 hours in game)

    private bool staminaDepleted = false;

    private HUDController HUD;




    // Start is called before the first frame update
    void Start()
    {
        HUD = GameObject.Find("HUD").GetComponent<HUDController>();
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
