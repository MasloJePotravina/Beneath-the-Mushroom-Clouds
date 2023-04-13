using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFieldOfView : MonoBehaviour
{
    private NPCBehaviourHostile npcBehaviourHostile;
    private NPCStatus npcStatus;
    private PlayerStatus playerStatus;

    public LayerMask standingLayerMask;
    public LayerMask crouchingLayerMask;

    //Null if the player is not visible
    private Transform targetTransform = null;
    private Transform targetTransformPrevious = null;
    private Transform npcTransform;

    // Start is called before the first frame update
    void Awake()
    {
        npcBehaviourHostile = gameObject.transform.parent.GetComponent<NPCBehaviourHostile>();
        npcStatus = gameObject.transform.parent.GetComponent<NPCStatus>();
        npcTransform = gameObject.transform.parent;
        playerStatus = GameObject.FindObjectOfType<PlayerStatus>();

        standingLayerMask = LayerMask.GetMask("FullObstacle");
        crouchingLayerMask = LayerMask.GetMask("FullObstacle", "HalfObstacle");
    }

    // Update is called once per frame
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

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            targetTransform = other.gameObject.transform.root;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            targetTransform = null;
        }
    }

    private bool ObstacleBlock(){

        LayerMask layerMask = standingLayerMask;
        if(npcStatus.isCrouched || playerStatus.isCrouched){
            layerMask = crouchingLayerMask;
        }
        Vector3 targetPos = targetTransform.position;
        Vector3 startPos = npcTransform.position;
        Vector3 direction = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(targetPos, startPos);

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, distance, layerMask);

        if(hit.collider != null){
            return true;
        }else{
            return false;
        }
        
    }
}
