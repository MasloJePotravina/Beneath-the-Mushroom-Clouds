////////////////////////////////////////////
// File: playerStatusScr.cs               //
// Project: Beneath the Mushroom Clouds   //
// Author: Ondrej Kováč                   //
// Brief: Status of the Player Character  //
////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script holds all the variables that affect the status of the player character such as health, stamina, whether the character is crouching and so on.
/// </summary>
public class playerStatus : MonoBehaviour
{
    public float playerHealth = 0.0f;
    public float playerStamina = 0.0f;
    public float playerHunger = 0.0f;
    public float playerThirst = 0.0f;
    public float playerTemp = 0.0f;
    public float playerTired = 0.0f;
    public float playerSpeed = 50.0f;
    public bool playerCrouched = false;
    public bool playerSprint = false;
    public bool playerAiming = false;

    public float shooterAbility;

    /// <summary>
    /// Loads player status bars from a save file
    /// For now only sets all values to full TODO
    /// </summary>
    /// <returns>0 when load is successful, 1 when unsuccessful</returns>
    int loadPlayerStatusBars(ref float health, ref float stamina, ref float hunger, ref float thirst, ref float temp, ref float tired){
        health = 100.0f;
        stamina = 100.0f;
        hunger = 100.0f;
        thirst = 100.0f;
        temp = 0.0f; //Temperatrure ranges between -100 and 100
        tired = 100.0f;

        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {

        loadPlayerStatusBars(ref playerHealth, ref playerStamina, ref playerHunger, ref playerThirst, ref playerTemp, ref playerTired);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
