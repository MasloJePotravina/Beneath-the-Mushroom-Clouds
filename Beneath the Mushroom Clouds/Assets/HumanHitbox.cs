using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHitbox : MonoBehaviour
{

    private int state = 0; //0 = idle, 1 = crouch, 2 = long weapon, 3 = short weapon

    private PolygonCollider2D torsoColliderIdleStand;
    private PolygonCollider2D leftArmColliderIdleStand;
    private PolygonCollider2D rightArmColliderIdleStand;

    private PolygonCollider2D torsoColliderIdleCrouch;
    private PolygonCollider2D leftArmColliderIdleCrouch;
    private PolygonCollider2D rightArmColliderIdleCrouch;

    private PolygonCollider2D torsoColliderLongWeaponIdle;
    private PolygonCollider2D leftArmColliderLongWeaponIdle;
    private PolygonCollider2D rightArmColliderLongWeaponIdle;

    private PolygonCollider2D torsoColliderShortWeaponIdle;
    private PolygonCollider2D leftArmColliderShortWeaponIdle;
    private PolygonCollider2D rightArmColliderShortWeaponIdle;

    private CapsuleCollider2D legsCollider;
    private CapsuleCollider2D headCollider;

    private float headBaseProbability = 0.15f;
    private float torsoBaseProbability = 0.45f;
    private float legsBaseProbability = 0.4f;
    private float armsBaseProbability = 0.2f;


    private void Awake(){
        
        GameObject leftArm = this.transform.Find("LeftArm").gameObject;
        GameObject rightArm = this.transform.Find("RightArm").gameObject;

        PolygonCollider2D[] torsoColliders = GetComponents<PolygonCollider2D>();
        PolygonCollider2D[] leftArmColliders = leftArm.GetComponents<PolygonCollider2D>();
        PolygonCollider2D[] rightArmColliders = rightArm.GetComponents<PolygonCollider2D>();

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

        return bodyPartHits;
    }

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
