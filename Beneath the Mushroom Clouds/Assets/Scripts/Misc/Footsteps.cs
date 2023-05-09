using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the audio of footsteps of player and NPCs.
/// </summary>
public class Footsteps : MonoBehaviour
{
    /// <summary>
    /// Reference to the audio manager.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to the player status, if this is the player.
    /// </summary>
    private PlayerStatus playerStatus = null;

    /// <summary>
    /// Reference to the NPC status, if this is an NPC.
    /// </summary>
    private NPCStatus npcStatus = null;

    /// <summary>
    /// Current floor material the character is walking on.
    /// </summary>
    private string floorMaterial = "Snow";

    /// <summary>
    /// Timer to keep track of the time between steps.
    /// </summary>
    private float betweenStepsTimer = 0f;

    /// <summary>
    /// Time between steps.
    /// </summary>
    private float betweenStepsTime = 0.6f;

    /// <summary>
    /// Reference to the character.
    /// </summary>
    private GameObject character = null;

    /// <summary>
    /// Gets all necessary references on start.
    /// </summary>
    void Start()
    {

        audioManager = GameObject.FindObjectOfType<AudioManager>();
        character = this.transform.parent.parent.gameObject;
        if(character.CompareTag("Player"))
        {
            playerStatus = character.GetComponent<PlayerStatus>();
        }
        else
        {
            npcStatus = character.GetComponent<NPCStatus>();
        }
    }

    /// <summary>
    /// Each frame emits a sound of a footstep based on time between steps, material of the floor and whether the character is moving.
    /// </summary>
    void Update()
    {

        bool isMoving = false;
        float speedMultiplier = 1f;
        if(character.CompareTag("Player")){
            isMoving = playerStatus.isMoving;
            if(playerStatus.isCrouched)
            {
                speedMultiplier = 2f;
            }else if(playerStatus.isRunning)
            {
                speedMultiplier = 0.5f;
            }
        }else{
            //Note: enemies do not run or crouch in this version, tehrefore there is no need
            //to calculate the speedMultiplier for them.
            isMoving = npcStatus.isMoving;
        }

        betweenStepsTime = 0.6f * speedMultiplier;

        if(isMoving)
        {
            betweenStepsTimer += Time.deltaTime;
            if(betweenStepsTimer > betweenStepsTime)
            {
                betweenStepsTimer = 0f;
                audioManager.Play("Footstep" + floorMaterial + Random.Range(1, 4), this.gameObject);
            }
        }

    }

    /// <summary>
    /// When the player leaves a wooden floor, the footstep sound is switched back to snow.
    /// </summary>
    /// <param name="other">The collider of the object which left the area of the player's collider.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("WoodenFloor"))
        {
            floorMaterial = "Snow";
        }

    }

    /// <summary>
    /// While the player is within a wooden floor, the footstep sound is switched to wood.
    /// </summary>
    /// <param name="other">The collider of the object which is currently within the area of the  player's collider.</param>
    void OnTriggerStay2D(Collider2D other)
    {    
        if (other.gameObject.CompareTag("WoodenFloor"))
        {
            floorMaterial = "Wood";
        }
    }
}
