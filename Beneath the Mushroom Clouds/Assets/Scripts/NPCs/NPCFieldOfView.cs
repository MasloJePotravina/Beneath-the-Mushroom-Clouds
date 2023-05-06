using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of the field of view of the NPC.
/// </summary>
public class NPCFieldOfView : MonoBehaviour
{
    /// <summary>
    /// Reference to the behaviour of the NPC.
    /// </summary>
    private NPCBehaviourHostile npcBehaviourHostile;

    /// <summary>
    /// Reference to the status of the NPC.
    /// </summary>
    private NPCStatus npcStatus;

    /// <summary>
    /// Reference to the status of the player
    /// </summary>
    private PlayerStatus playerStatus;

    /// <summary>
    /// Reference to the layer mask to be used for wight when both the NPC and the player are standing.
    /// </summary>
    public LayerMask standingLayerMask;
    /// <summary>
    /// Reference to the layer mask to be used for wight when either the NPC or the player is courching.
    /// </summary>
    public LayerMask crouchingLayerMask;

    //Null if the player is not visible
    /// <summary>
    /// Reference to the transform of the player. Null if the player is in visibility range.
    /// </summary>
    private Transform targetTransform = null;

    /// <summary>
    /// Transform of the player in the previous frame. While the transform itself does not change of course, this is used to determine whether the player was just lost.
    /// </summary>
    private Transform targetTransformPrevious = null;

    /// <summary>
    /// Transform of the NPC.
    /// </summary>
    private Transform npcTransform;

    /// <summary>
    /// Gets all necessary references on awake, sets layer masks for standing and crouching.
    /// </summary>
    void Awake()
    {
        npcBehaviourHostile = gameObject.transform.parent.GetComponent<NPCBehaviourHostile>();
        npcStatus = gameObject.transform.parent.GetComponent<NPCStatus>();
        npcTransform = gameObject.transform.parent;
        playerStatus = GameObject.FindObjectOfType<PlayerStatus>();

        standingLayerMask = LayerMask.GetMask("FullObstacle");
        crouchingLayerMask = LayerMask.GetMask("FullObstacle", "HalfObstacle");
    }

    /// <summary>
    /// Each frame, if the target is within the range of sight of the NPC, it is determined whether the NPC truly sees the player.
    /// </summary>
    void Update()
    {
        if(targetTransform != null){
            if(ObstacleBlock()){
                npcBehaviourHostile.LostTarget();
            }else{
                npcBehaviourHostile.SawTarget(targetTransform);
            }
        }else{
            if(targetTransformPrevious != null)
                npcBehaviourHostile.LostTarget();
        }

        targetTransformPrevious = targetTransform;
    }

    /// <summary>
    /// When the player enters the trigger collider of the NPC's field of view, the player is set as the target.
    /// </summary>
    /// <param name="other">What object entered the field of view collider.</param>
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            targetTransform = other.gameObject.transform.root;
        }
    }

    /// <summary>
    /// When the player exits the trigger collider of the NPC's field of view, the target is set to null.
    /// </summary>
    /// <param name="other">What object left the field of view collider.</param>
    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            targetTransform = null;
        }
    }

    /// <summary>
    /// Check of whether when the Player is within the fov of the NPC, there is an obstacle between them.
    /// </summary>
    /// <returns>True if there is an obstacle blocking the sight, false otherwise.</returns>
    private bool ObstacleBlock(){

        //Get the proper mask
        LayerMask layerMask = standingLayerMask;
        if(npcStatus.isCrouched || playerStatus.isCrouched){
            layerMask = crouchingLayerMask;
        }

        //Shoot a raycast between the NPC and the player
        Vector3 targetPos = targetTransform.position;
        Vector3 startPos = npcTransform.position;
        Vector3 direction = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(targetPos, startPos);
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, distance, layerMask);

        //If there was nothing between the NPC and the player, the NPC sees the player
        if(hit.collider != null){
            return true;
        }else{
            return false;
        }
        
    }
}
