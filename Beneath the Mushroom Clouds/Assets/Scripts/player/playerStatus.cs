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
public class PlayerStatus : MonoBehaviour
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

    public GameObject playerTorso;

    private Quaternion prevTorsoRotation;

    //This function makes the head of the player have a leeway of 30 degrees before rotating the torso
    //It's meant to simulate the way humans look around, as most people also will first turn their neck and
    //only after a few degrees will start to rotate their torso
    void adjustTorsoRotation(GameObject torso)
    {
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


    // Start is called before the first frame update
    void Start()
    {
        prevTorsoRotation = playerTorso.transform.rotation;
        LoadPlayerStatusBars(ref playerHealth, ref playerStamina, ref playerHunger, ref playerThirst, ref playerTemp, ref playerTired);
    }

    // Update is called once per frame
    void Update()
    {

        adjustTorsoRotation(playerTorso);
    }

    /// <summary>
    /// Loads player status bars from a save file
    /// For now only sets all values to full TODO
    /// </summary>
    /// <returns>0 when load is successful, 1 when unsuccessful</returns>
    int LoadPlayerStatusBars(ref float health, ref float stamina, ref float hunger, ref float thirst, ref float temp, ref float tired)
    {
        health = 100.0f;
        stamina = 100.0f;
        hunger = 100.0f;
        thirst = 100.0f;
        temp = 0.0f; //Temperatrure ranges between -100 and 100
        tired = 100.0f;

        return 0;
    }
}
