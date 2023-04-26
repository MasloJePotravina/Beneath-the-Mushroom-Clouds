using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStatusDebug : MonoBehaviour
{
    PlayerStatus playerStatus;

    private Dictionary <string, bool> headStatusEffects = new Dictionary<string, bool>();
    private Dictionary <string, bool> torsoStatusEffects = new Dictionary<string, bool>();
    private Dictionary <string, bool> leftArmStatusEffects = new Dictionary<string, bool>();
    private Dictionary <string, bool> rightArmStatusEffects = new Dictionary<string, bool>();
    private Dictionary <string, bool> leftLegStatusEffects = new Dictionary<string, bool>();
    private Dictionary <string, bool> rightLegStatusEffects = new Dictionary<string, bool>();

    private Dictionary<string, Dictionary<string, bool>> bodyPartStatusEffects;

    void Awake(){
        playerStatus = GameObject.FindObjectOfType<PlayerStatus>();

        bodyPartStatusEffects = new Dictionary<string, Dictionary<string, bool>>(){
            {"Head", headStatusEffects},
            {"Torso", torsoStatusEffects},
            {"LeftArm", leftArmStatusEffects},
            {"RightArm", rightArmStatusEffects},
            {"LeftLeg", leftLegStatusEffects},
            {"RightLeg", rightLegStatusEffects}
        };

        foreach (string bodyPart in bodyPartStatusEffects.Keys){
            bodyPartStatusEffects[bodyPart].Add("Bleeding", false);
            bodyPartStatusEffects[bodyPart].Add("BandageClean", false);
            bodyPartStatusEffects[bodyPart].Add("BandageDirty", false);
            bodyPartStatusEffects[bodyPart].Add("Stitched", false);
            bodyPartStatusEffects[bodyPart].Add("Infection", false);
            bodyPartStatusEffects[bodyPart].Add("Disinfected", false);
        }
    }

    public void Bleeding(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Bleeding"] = !bodyPartStatusEffects[bodyPart]["Bleeding"];
    }

    public void BandageClean(string bodyPart){
        bodyPartStatusEffects[bodyPart]["BandageClean"] = !bodyPartStatusEffects[bodyPart]["BandageClean"];
    }

    public void BandageDirty(string bodyPart){
        bodyPartStatusEffects[bodyPart]["BandageDirty"] = !bodyPartStatusEffects[bodyPart]["BandageDirty"];
    }

    public void Stitched(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Stitched"] = !bodyPartStatusEffects[bodyPart]["Stitched"];
    }

    public void Infection(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Infection"] = !bodyPartStatusEffects[bodyPart]["Infection"];
    }

    public void Disinfected(string bodyPart){
        bodyPartStatusEffects[bodyPart]["Disinfected"] = !bodyPartStatusEffects[bodyPart]["Disinfected"];
    }

    public void ApplyEffects(){
        foreach (string bodyPartString in bodyPartStatusEffects.Keys){
            Dictionary<string, bool> bodyPart = bodyPartStatusEffects[bodyPartString];
            if(bodyPart["Bleeding"])
                playerStatus.ApplyBleed(bodyPartString);
            if(bodyPart["Stitched"])
                playerStatus.StitchWound(bodyPartString);
            if(bodyPart["Infection"])
                playerStatus.GetInfection(bodyPartString);
            else
                playerStatus.RemoveInfection(bodyPartString);
            if(bodyPart["Disinfected"])
                playerStatus.Disinfect(bodyPartString);
            /*if(bodyPart["BandageClean"])
                playerStatus.ApplyBandage(bodyPartString, false);
            if(bodyPart["BandageDirty"])
                playerStatus.ApplyBandage(bodyPartString, true);
            if(!bodyPart["BandageDirty"] && !bodyPart["BandageClean"])
                playerStatus.RemoveBandage(bodyPartString);*/

        }
    }
}
