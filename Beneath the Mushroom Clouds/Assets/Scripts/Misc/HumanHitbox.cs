using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of a human hitbox.
/// </summary>
public class HumanHitbox : MonoBehaviour
{

    /// <summary>
    /// Current state of the hitbox (0 = idle, 1 = crouch, 2 = long weapon equipped, 3 = short weapon equipped)
    /// </summary>
    private int state = 0; //0 = idle, 1 = crouch, 2 = long weapon, 3 = short weapon

    /// <summary>
    /// Polygon collider for the torso, modeled to fit idle standing.
    /// </summary>
    private PolygonCollider2D torsoColliderIdleStand;

    /// <summary>
    /// Polygon collider for the left arm, modeled to fit idle standing.
    /// </summary>
    private PolygonCollider2D leftArmColliderIdleStand;
    
    /// <summary>
    /// Polygon collider for the right arm, modeled to fit idle standing.
    /// </summary>
    private PolygonCollider2D rightArmColliderIdleStand;

    /// <summary>
    /// Polygon collider for the torso, modeled to fit idle crouching.
    /// </summary>
    private PolygonCollider2D torsoColliderIdleCrouch;

    /// <summary>
    /// Polygon collider for the left arm, modeled to fit idle crouching.
    /// </summary>
    private PolygonCollider2D leftArmColliderIdleCrouch;

    /// <summary>
    /// Polygon collider for the right arm, modeled to fit idle crouching.
    /// </summary>
    private PolygonCollider2D rightArmColliderIdleCrouch;

    /// <summary>
    /// Polygon collider for the torso, modeled to fit idle standing or crouching with a long weapon equipped.
    /// </summary>
    private PolygonCollider2D torsoColliderLongWeaponIdle;

    /// <summary>
    /// Polygon collider for the left arm, modeled to fit idle standing or crouching with a long weapon equipped.
    /// </summary>
    private PolygonCollider2D leftArmColliderLongWeaponIdle;

    /// <summary>
    /// Polygon collider for the right arm, modeled to fit idle standing or crouching with a long weapon equipped.
    /// </summary>
    private PolygonCollider2D rightArmColliderLongWeaponIdle;

    /// <summary>
    /// Polygon collider for the torso, modeled to fit idle standing or crouching with a short weapon equipped.
    /// </summary>
    private PolygonCollider2D torsoColliderShortWeaponIdle;

    /// <summary>
    /// Polygon collider for the left arm, modeled to fit idle standing or crouching with a short weapon equipped.
    /// </summary>
    private PolygonCollider2D leftArmColliderShortWeaponIdle;

    /// <summary>
    /// Polygon collider for the right arm, modeled to fit idle standing or crouching with a short weapon equipped.
    /// </summary>
    private PolygonCollider2D rightArmColliderShortWeaponIdle;

    /// <summary>
    /// Colider ofr the legs, does not change shape in different states.
    /// </summary>
    private CapsuleCollider2D legsCollider;

    /// <summary>
    /// Collider for the head, does not change shape in different states.
    /// </summary>
    private CapsuleCollider2D headCollider;

    /// <summary>
    /// Base probability for a hit to hit the head (if a bullet raycast passed through the head collider).
    /// </summary>
    private float headBaseProbability = 0.15f;
    /// <summary>
    /// Base probability for a hit to hit the torso (if a bullet raycast passed through the torso collider).
    /// </summary>
    private float torsoBaseProbability = 0.45f;

    /// <summary>
    /// Base probability for a hit to hit the legs (if a bullet raycast passed through the arms collider).
    /// </summary>
    private float legsBaseProbability = 0.4f;

    ///<summary>
    /// Base probability for a hit to hit to each individual arm (if a bullet raycast passed through the arm collider and the legs or head weren't determined as hits).
    private float armsBaseProbability = 0.2f;


    /// <summary>
    /// Get the necessary references on awake.
    /// </summary>
    private void Awake(){
        
        GameObject leftArm = this.transform.Find("LeftArm").gameObject;
        GameObject rightArm = this.transform.Find("RightArm").gameObject;

        PolygonCollider2D[] torsoColliders = GetComponents<PolygonCollider2D>();
        PolygonCollider2D[] leftArmColliders = leftArm.GetComponents<PolygonCollider2D>();
        PolygonCollider2D[] rightArmColliders = rightArm.GetComponents<PolygonCollider2D>();

        //Colliders have to be properly ordered in the inspector
        torsoColliderIdleStand = torsoColliders[0];
        leftArmColliderIdleStand = leftArmColliders[0];
        rightArmColliderIdleStand = rightArmColliders[0];

        torsoColliderIdleCrouch = torsoColliders[1];
        leftArmColliderIdleCrouch = leftArmColliders[1];
        rightArmColliderIdleCrouch = rightArmColliders[1];

        torsoColliderLongWeaponIdle = torsoColliders[2];
        leftArmColliderLongWeaponIdle = leftArmColliders[2];
        rightArmColliderLongWeaponIdle = rightArmColliders[2];


        torsoColliderShortWeaponIdle = torsoColliders[3];
        leftArmColliderShortWeaponIdle = leftArmColliders[3];
        rightArmColliderShortWeaponIdle = rightArmColliders[3];

        legsCollider = this.transform.Find("Legs").GetComponent<CapsuleCollider2D>();
        headCollider = this.transform.Find("HeadPivot").Find("Head").GetComponent<CapsuleCollider2D>();

    }

    /// <summary>
    /// Set the colliders to idle standing.
    /// </summary>
    public void SetIdleStandColliders(){

        if(state == 0){
            return;
        }

        torsoColliderIdleStand.enabled = true;
        leftArmColliderIdleStand.enabled = true;
        rightArmColliderIdleStand.enabled = true;

        torsoColliderIdleCrouch.enabled = false;
        leftArmColliderIdleCrouch.enabled = false;
        rightArmColliderIdleCrouch.enabled = false;

        torsoColliderLongWeaponIdle.enabled = false;
        leftArmColliderLongWeaponIdle.enabled = false;
        rightArmColliderLongWeaponIdle.enabled = false;

        torsoColliderShortWeaponIdle.enabled = false;
        leftArmColliderShortWeaponIdle.enabled = false;
        rightArmColliderShortWeaponIdle.enabled = false;

        legsCollider.offset = new Vector2(0.0f, -0.0f);
        headCollider.offset = new Vector2(0.0f, 0.0f);

        state = 0;
    }

    /// <summary>
    /// Set the colliders to idle crouching.
    /// </summary>
    public void SetIdleCrouchColliders(){

        if(state == 1){
            return;
        }

        torsoColliderIdleStand.enabled = false;
        leftArmColliderIdleStand.enabled = false;
        rightArmColliderIdleStand.enabled = false;

        torsoColliderIdleCrouch.enabled = true;
        leftArmColliderIdleCrouch.enabled = true;
        rightArmColliderIdleCrouch.enabled = true;

        torsoColliderLongWeaponIdle.enabled = false;
        leftArmColliderLongWeaponIdle.enabled = false;
        rightArmColliderLongWeaponIdle.enabled = false;

        torsoColliderShortWeaponIdle.enabled = false;
        leftArmColliderShortWeaponIdle.enabled = false;
        rightArmColliderShortWeaponIdle.enabled = false;

        legsCollider.offset = new Vector2(0.0f, -0.10f);
        headCollider.offset = new Vector2(0.0f, 0.0f);


        state = 1;
    }

    /// <summary>
    /// Set the colliders to idle standing or crouching with a long weapon equipped.
    /// </summary>
    public void SetLongWeaponIdleColliders(){

        if(state == 2){
            return;
        }

        torsoColliderIdleStand.enabled = false;
        leftArmColliderIdleStand.enabled = false;
        rightArmColliderIdleStand.enabled = false;

        torsoColliderIdleCrouch.enabled = false;
        leftArmColliderIdleCrouch.enabled = false;
        rightArmColliderIdleCrouch.enabled = false;

        torsoColliderLongWeaponIdle.enabled = true;
        leftArmColliderLongWeaponIdle.enabled = true;
        rightArmColliderLongWeaponIdle.enabled = true;

        torsoColliderShortWeaponIdle.enabled = false;
        leftArmColliderShortWeaponIdle.enabled = false;
        rightArmColliderShortWeaponIdle.enabled = false;

        legsCollider.offset = new Vector2(0.0f, -0.05f);
        headCollider.offset = new Vector2(0.05f, 0.0f);

        state = 2;
    }

    /// <summary>
    /// Set the colliders to idle standing or crouching with a short weapon equipped.
    /// </summary>
    public void SetShortWeaponIdleColliders(){

        if(state == 3){
            return;
        }

        torsoColliderIdleStand.enabled = false;
        leftArmColliderIdleStand.enabled = false;
        rightArmColliderIdleStand.enabled = false;

        torsoColliderIdleCrouch.enabled = false;
        leftArmColliderIdleCrouch.enabled = false;
        rightArmColliderIdleCrouch.enabled = false;

        torsoColliderLongWeaponIdle.enabled = false;
        leftArmColliderLongWeaponIdle.enabled = false;
        rightArmColliderLongWeaponIdle.enabled = false;

        torsoColliderShortWeaponIdle.enabled = true;
        leftArmColliderShortWeaponIdle.enabled = true;
        rightArmColliderShortWeaponIdle.enabled = true;

        legsCollider.offset = new Vector2(0.0f, 0.0f);
        headCollider.offset = new Vector2(0.0f, 0.0f);

        state = 3;
    }

    /// <summary>
    /// Gets the probabilities of main body parts (head, torso, legs) based on whether they were passed by a bullet raycast or not.
    /// </summary>
    /// <param name="headShot">Raycast passed through the head collider.</param>
    /// <param name="torsoShot">Raycast passed through the torso collider.</param>
    /// <param name="legsShot">Raycast passed through the legs collider.</param>
    /// <returns>Dictionary of main body parts (head, torso, legs) and the base probability of their hit (may not add up to 1)</returns>
    private Dictionary<string, float> GetMainBodyPartProbabilities(bool headShot, bool torsoShot, bool legsShot){
            
            Dictionary<string, float> mainBodyPartsProbabilities = new Dictionary<string, float>();

    
            if(headShot){
                mainBodyPartsProbabilities.Add("Head", headBaseProbability);
            }else{
                mainBodyPartsProbabilities.Add("Head", 0.0f);
            }
    
            if(torsoShot){
                mainBodyPartsProbabilities.Add("Torso", torsoBaseProbability);
            }else{
                mainBodyPartsProbabilities.Add("Torso", 0.0f);
            }
    
            if(legsShot){
                mainBodyPartsProbabilities.Add("Legs", legsBaseProbability);
            }else{
                mainBodyPartsProbabilities.Add("Legs", 0.0f);
            }
    
            return mainBodyPartsProbabilities;
    }

    /// <summary>
    /// Redistributes the probabilities of main body parts (head, torso, legs) so that they add up to 1.
    /// </summary>
    /// <param name="mainBodyPartsProbabilities">Dictionary of base probabilitis pf hits, if a bullet raycast passed through a main body part (head, torso, legs).</param>
    /// <param name="probabilitySum">Output parameter of what these probabilities added up to before being redistributed.</param>
    /// <returns>Dictionary of main body parts (head, torso, legs) and the redistributed probability of their hit (always adds up to 1)</returns>
    private Dictionary<string, float> RedistributeMainBodyPartProbabilities(Dictionary<string, float> mainBodyPartsProbabilities, out float probabilitySum){
        Dictionary<string, float> mainBodyPartsProbabilitiesRedistributed = new Dictionary<string, float>();
        probabilitySum = 0.0f;
        foreach(float probability in mainBodyPartsProbabilities.Values){
            probabilitySum += probability;
        }

        //redistribute probabilities
        foreach(string bodyPart in mainBodyPartsProbabilities.Keys){
            mainBodyPartsProbabilitiesRedistributed[bodyPart] = mainBodyPartsProbabilities[bodyPart] / probabilitySum;
        }

        return mainBodyPartsProbabilitiesRedistributed;
    }
    
    /// <summary>
    /// Randomly select a main body part hit (head, torso, legs) based on the redistributed probabilities. This means that these body parts will never be hit by the same bullet.
    /// If legs were shot, then the left or right leg will be randomly selected.
    /// </summary>
    /// <param name="mainBodyPartsProbabilitiesRedistributed">Dictionary of redistributed probabilities of hit main body parts (head, torso, legs)</param>
    /// <returns>Dictionary of body parts and booleans which decides which body parts were or were not shot.</returns>
    private Dictionary<string, bool> RandomMainBodyPartHits(Dictionary<string, float> mainBodyPartsProbabilitiesRedistributed){
        Dictionary<string, bool> bodyPartHits = new Dictionary<string, bool>(){
            {"Head", false},
            {"Torso", false},
            {"LeftLeg", false},
            {"RightLeg", false},
            {"LeftArm", false},
            {"RightArm", false}  
        };

        float randomValue = Random.Range(0.0f, 1.0f);
        float currentProbability = 0.0f;

        foreach(string bodyPart in mainBodyPartsProbabilitiesRedistributed.Keys){
            currentProbability += mainBodyPartsProbabilitiesRedistributed[bodyPart];

            if(randomValue <= currentProbability){
                if(bodyPart == "Legs"){
                    float randomLegValue = Random.Range(0.0f, 1.0f);
                    if(randomLegValue <= 0.5f){
                        bodyPartHits["LeftLeg"] = true;
                    }else{
                        bodyPartHits["RightLeg"] = true;
                    }
                }else{
                    bodyPartHits[bodyPart] = true;
                }
                break;
            }
        }


        return bodyPartHits;
    }

    /// <summary>
    /// Randomly decides whether an arm was hit. There are multiple criteria, however the most important one is whether the raycast passed through the arm but was not evaluated as legs or head shot.
    /// </summary>
    /// <param name="bodyPartHits">Dictionary containing the information about which main body part was hit. It is edited in this function to include arms.</param>
    /// <param name="leftArmShot">Whether a bullet raycast passed through the collider of the left arm.</param>
    /// <param name="rightArmShot"Whether a bullet raycast passed through the collider of the right arm.></param>
    /// <param name="mainBodyPartWasHit">Whether a main body part was evaluated as hit. This is false if the bullet just passed through a hand.</param>
    /// <returns>Altered bodyPartHits dictionary including potential arm hits.</returns>
    private Dictionary<string, bool> RandomArmHits(Dictionary<string, bool> bodyPartHits, bool leftArmShot, bool rightArmShot, bool mainBodyPartWasHit){
        
        bool torsoWasHit = bodyPartHits["Torso"];

        if(mainBodyPartWasHit && !torsoWasHit){//If one of the main body parts was hit but it was not torso, then arms couldn't be hit
            bodyPartHits["LeftArm"] = false;
            bodyPartHits["RightArm"] = false;
        }else if(mainBodyPartWasHit && torsoWasHit){//If a main body part was hit and it was torso, then arms could be hit, with a 50% probability each (if raycast passed through it)
            float armProbability = 0.5f;
            if(leftArmShot){
                float randomArmValue = Random.Range(0.0f, 1.0f);
                if(randomArmValue <= armProbability){
                    bodyPartHits["LeftArm"] = true;
                }
            }
            if(rightArmShot){
                float randomArmValue = Random.Range(0.0f, 1.0f);
                if(randomArmValue <= armProbability){
                    bodyPartHits["RightArm"] = true;
                }
            }
        }else if(!mainBodyPartWasHit){//If no main body part was hit, then arms has to be hit, if raycast passed through both, they have 50 % chance each, if only through one, that was the hit arm
            if(leftArmShot && rightArmShot){
                float armProbability = 0.5f;
                float randomArmValue = Random.Range(0.0f, 1.0f);
                if(randomArmValue <= 0.5f){
                    bodyPartHits["LeftArm"] = true;
                }
                randomArmValue = Random.Range(0.0f, 1.0f);
                if(randomArmValue <= 0.5f){
                    bodyPartHits["RightArm"] = true;
                }
            }else if(leftArmShot){
                bodyPartHits["LeftArm"] = true;
            }else if(rightArmShot){
                bodyPartHits["RightArm"] = true;
            }
        }

        //Note: This is a weird system. It has to account for the raycast missing the character completely in case of cover and such, which is evaluated in the firearm script.
        //Then it has to apply the probability for individual parts and therefore if the firearm script was evaluated as a hit then something should be hit 100%.
        //This causes a problem with arms especially. There is quite a low probability that you would hit someones extended arm, such as when they are holding a short firearm.
        //However in this system, if the raycast passes through the extended arm and the firearm determines it as a hit, then this script has to determine it as a guaranteed hit.

        return bodyPartHits;
    }

    /// <summary>
    /// Determines which body parts were truly shot when a bullet raycast passes through the hitbox. Called by the firearm script after it has evaluated shot as a hit.
    /// </summary>
    /// <param name="damage">Damage the firearm which fired the bullet deals.</param>
    /// <param name="headShot">Whether the raycast passed through the head collider.</param>
    /// <param name="torsoShot">Whether the raycast passed through the torso collider.</param>
    /// <param name="legsShot">Whether the raycast passed through the legs collider.</param>
    /// <param name="leftArmShot">Whether the raycast passed through the left arm collider.</param>
    /// <param name="rightArmShot">Whether the raycast passed through the right arm collider.</param>
    public void BulletHit(float damage, bool headShot, bool torsoShot, bool legsShot, bool leftArmShot, bool rightArmShot){

        Dictionary<string, float> mainBodyPartsProbabilities = new Dictionary<string, float>();
        Dictionary<string, float> mainBodyPartsProbabilitiesRedistributed = new Dictionary<string, float>();
        Dictionary<string, bool> bodyPartHits = new Dictionary<string, bool>();

        float probabilitySum = 0.0f;
        mainBodyPartsProbabilities = GetMainBodyPartProbabilities(headShot, torsoShot, legsShot);
        mainBodyPartsProbabilitiesRedistributed = RedistributeMainBodyPartProbabilities(mainBodyPartsProbabilities, out probabilitySum);
        bodyPartHits = RandomMainBodyPartHits(mainBodyPartsProbabilitiesRedistributed);

        bool mainBodyPartWasHit = false;
        if(probabilitySum > 0.0f){
            mainBodyPartWasHit = true;
        }

        bodyPartHits = RandomArmHits(bodyPartHits, leftArmShot, rightArmShot, mainBodyPartWasHit);

        if(transform.parent.CompareTag("Player")){
            PlayerStatus playerStatus = transform.parent.GetComponent<PlayerStatus>();
            foreach(string bodyPart in bodyPartHits.Keys){
                if(bodyPartHits[bodyPart]){
                    playerStatus.GotShot(damage, bodyPart);
                }
            }
        }else if(transform.parent.CompareTag("NPC")){
            NPCStatus npcStatus = transform.parent.GetComponent<NPCStatus>();
            foreach(string bodyPart in bodyPartHits.Keys){
                if(bodyPartHits[bodyPart]){
                    npcStatus.GotShot(damage, bodyPart);
                }
            }
        }

        /*
        Debug.Log("Redistributed probabilities: Head" + mainBodyPartsProbabilitiesRedistributed["Head"] + " Torso: " + mainBodyPartsProbabilitiesRedistributed["Torso"] + " Legs: " + mainBodyPartsProbabilitiesRedistributed["Legs"]); 
        Debug.Log("Hit body parts: Head: " + bodyPartHits["Head"] + " Torso: " + bodyPartHits["Torso"] + " Legs: " + bodyPartHits["Legs"] + " LeftArm: " + bodyPartHits["LeftArm"] + " RightArm: " + bodyPartHits["RightArm"]);
        */
    

    }

}
