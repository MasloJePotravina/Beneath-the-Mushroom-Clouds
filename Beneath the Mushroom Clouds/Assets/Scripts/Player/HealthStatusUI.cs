using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Implements the behaviour of the health status UI (health screen).
/// </summary>
public class HealthStatusUI : MonoBehaviour
{
    /// <summary>
    /// Scriptable object holdning the relevant injury and treatment sprites for the head in the health screen.
    /// </summary>
    [SerializeField] private HealthStatusSpriteData headSpriteData;

    /// <summary>
    /// Scriptable object holdning the relevant injury and treatment sprites for the torso in the health screen.
    /// </summary>
    [SerializeField] private HealthStatusSpriteData torsoSpriteData;

    /// <summary>
    /// Scriptable object holdning the relevant injury and treatment sprites for the left arm in the health screen.
    /// </summary>
    [SerializeField] private HealthStatusSpriteData leftArmSpriteData;

    /// <summary>
    /// Scriptable object holdning the relevant injury and treatment sprites for the right arm in the health screen.
    /// </summary>
    [SerializeField] private HealthStatusSpriteData rightArmSpriteData;

    /// <summary>
    /// Scriptable object holdning the relevant injury and treatment sprites for the left leg in the health screen.
    /// </summary>
    [SerializeField] private HealthStatusSpriteData leftLegSpriteData;

    /// <summary>
    /// Scriptable object holdning the relevant injury and treatment sprites for the right leg in the health screen.
    /// </summary>
    [SerializeField] private HealthStatusSpriteData rightLegSpriteData;

    /// <summary>
    /// Prefab for a status effect text.
    /// </summary>
    [SerializeField] private GameObject statusEffectPrefab;

    /// <summary>
    /// Dictionary of body parts and different sprites for different injuries and treatments.
    /// </summary>
    private Dictionary<string, HealthStatusSpriteData> bodyPartSpriteData = new Dictionary<string, HealthStatusSpriteData>();


    /// <summary>
    /// GameObjects which serve as sprites for different injuries and treatments for the head in the health screen.
    /// </summary>
    private Dictionary<string, GameObject> headSprites = new Dictionary<string, GameObject>();

    /// <summary>
    /// GameObjects which serve as sprites for different injuries and treatments for the torso in the health screen.
    /// </summary>
    private Dictionary<string, GameObject> torsoSprites = new Dictionary<string, GameObject>();

    /// <summary>
    /// GameObjects which serve as sprites for different injuries and treatments for the left arm in the health screen.
    /// </summary>
    private Dictionary<string, GameObject> leftArmSprites = new Dictionary<string, GameObject>();

    /// <summary>
    /// GameObjects which serve as sprites for different injuries and treatments for the right arm in the health screen.
    /// </summary>
    private Dictionary<string, GameObject> rightArmSprites = new Dictionary<string, GameObject>();

    /// <summary>
    /// GameObjects which serve as sprites for different injuries and treatments for the left leg in the health screen.
    /// </summary>
    private Dictionary<string, GameObject> leftLegSprites = new Dictionary<string, GameObject>();

    /// <summary>
    /// GameObjects which serve as sprites for different injuries and treatments for the right leg in the health screen.
    /// </summary>
    private Dictionary<string, GameObject> rightLegSprites = new Dictionary<string, GameObject>();

    /// <summary>
    /// Dictionary of body parts and different sprites for different injuries and treatments.
    /// </summary>
    private Dictionary<string, Dictionary<string, GameObject>> bodyPartSprites; 

    /// <summary>
    /// Highlits for different body parts when holding a medical item.
    /// </summary>
    public Dictionary<string, GameObject> bodyPartHighlightObjects;

    /// <summary>
    /// Information about the current health (variable) of the player.
    /// </summary>
    [SerializeField] private GameObject healthInfo;

    /// <summary>
    /// Current health of the player represented as a text.
    /// </summary>
    private TextMeshProUGUI healthtext;

    /// <summary>
    /// Array of arrows representing the current health loss or regeneration of the player's health.
    /// </summary>
    private GameObject[] healthArrows = new GameObject[6];

   
    /// <summary>
    /// Dictionary of different status effect and their corresponding colors.
    /// </summary>
    private Dictionary<string, Color> statusEffectColors = new Dictionary<string, Color>(){
        {"Bleeding", Color.red},
        {"BandageClean", Color.green},
        {"BandageDirty", Color.yellow},
        {"StitchedWound", Color.green},
        {"OpenWound", Color.red},
        {"Infection", Color.yellow},
        {"Disinfected", Color.green},
        {"Pain", Color.yellow},
        {"Nourished", Color.green},
        {"Antibiotics", Color.white},
        {"Painkillers", Color.white},
        {"Hypothermia",  new Color(0, 255, 255)},
        {"Hyperthermia", new Color(255, 120, 0)},
        {"Dehydrated", Color.red},
        {"Starving", Color.red},
        {"Over-encumbered", Color.red},
        {"Tired", Color.red}
    };

    



    /// <summary>
    /// Gets the necessary references and initializes values on awake.
    /// </summary>
    void Awake(){

        //Assign sprite data to different sprites
        bodyPartSprites = new Dictionary<string, Dictionary<string, GameObject>>(){
            {"Head", headSprites},
            {"Torso", torsoSprites},
            {"LeftArm", leftArmSprites},
            {"RightArm", rightArmSprites},
            {"LeftLeg", leftLegSprites},
            {"RightLeg", rightLegSprites}
        };

        

        GameObject outline = transform.Find("Outline").gameObject;

        GameObject[] statusSpiteSections = new GameObject[6];

        for(int i = 0; i < statusSpiteSections.Length; i++){
            statusSpiteSections[i] = outline.transform.GetChild(i).gameObject;
        }

        int cnt = 0;
        foreach(Dictionary <string, GameObject> bodyPart in bodyPartSprites.Values){
            for(int j = 0; j < 4; j++){
                GameObject statusSprite = statusSpiteSections[cnt].transform.GetChild(j).gameObject;
                bodyPart.Add(statusSprite.name, statusSprite.gameObject);
                statusSprite.SetActive(false);
            }
            cnt++;
        }

        bodyPartSpriteData.Add("Head", headSpriteData);
        bodyPartSpriteData.Add("Torso", torsoSpriteData);
        bodyPartSpriteData.Add("LeftArm", leftArmSpriteData);
        bodyPartSpriteData.Add("RightArm", rightArmSpriteData);
        bodyPartSpriteData.Add("LeftLeg", leftLegSpriteData);
        bodyPartSpriteData.Add("RightLeg", rightLegSpriteData);

        GameObject outlineHighlight = outline.transform.Find("OutlineHighlight").gameObject;
        bodyPartHighlightObjects = new Dictionary<string, GameObject>(){
            {"Head", outlineHighlight.transform.GetChild(0).gameObject},
            {"Torso", outlineHighlight.transform.GetChild(1).gameObject},
            {"LeftArm", outlineHighlight.transform.GetChild(2).gameObject},
            {"RightArm", outlineHighlight.transform.GetChild(3).gameObject},
            {"LeftLeg", outlineHighlight.transform.GetChild(4).gameObject},
            {"RightLeg", outlineHighlight.transform.GetChild(5).gameObject}
        };


        healthtext = healthInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>();


        for(int i = 0; i < 6; i++){
            healthArrows[i] = healthInfo.transform.GetChild(1).GetChild(i).gameObject;
        }

        this.gameObject.SetActive(false);
        
    }

    /// <summary>
    /// Applies bleed to a body part.
    /// </summary>
    /// <param name="bodyPart">Body part to which the bleed should be applied to.</param>
    public void ApplyBleed(string bodyPart){
        bodyPartSprites[bodyPart]["Bleeding"].SetActive(true);
        bodyPartSprites[bodyPart]["Wound"].SetActive(false);
        bodyPartSprites[bodyPart]["Bandage"].SetActive(false);

    }

    /// <summary>
    /// Applies a badage to a body part.
    /// </summary>
    /// <param name="bodyPart">The body part the bandage is applied to.</param>
    /// <param name="isDirty">Whether the applied bandage is dirty.</param>
    public void ApplyBandage(string bodyPart, bool isDirty){
            
        bodyPartSprites[bodyPart]["Bleeding"].SetActive(false);
        if(isDirty){
            bodyPartSprites[bodyPart]["Bandage"].GetComponent<RawImage>().texture = bodyPartSpriteData[bodyPart].bandageDirty;
        }
        else{
            bodyPartSprites[bodyPart]["Bandage"].GetComponent<RawImage>().texture = bodyPartSpriteData[bodyPart].bandageClean;
        }
        bodyPartSprites[bodyPart]["Bandage"].SetActive(true);

    
    }

    /// <summary>
    /// Removes a bandage from a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part has its bandage removed.</param>
    /// <param name="openWound">Whether there is an open wound under that bandage.</param>
    public void RemoveBandage(string bodyPart, bool openWound){
        bodyPartSprites[bodyPart]["Bandage"].SetActive(false);
        if(openWound)
            bodyPartSprites[bodyPart]["Bleeding"].SetActive(true);


    }

    /// <summary>
    /// Switch the bandage texture to a dirty one.
    /// </summary>
    /// <param name="bodyPart">Which body part the bandage change applies to.</param>
    public void DirtyBandage(string bodyPart){
        bodyPartSprites[bodyPart]["Bandage"].GetComponent<RawImage>().texture = bodyPartSpriteData[bodyPart].bandageDirty;

    }

    /// <summary>
    /// Applies a wound stitch to a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part the stitch is applied to.</param>
    public void StitchWound(string bodyPart){
        bodyPartSprites[bodyPart]["Wound"].GetComponent<RawImage>().texture = bodyPartSpriteData[bodyPart].stitchedWound;
        bodyPartSprites[bodyPart]["Bleeding"].SetActive(false);
        bodyPartSprites[bodyPart]["Wound"].SetActive(true);

    }

    /// <summary>
    /// Removes a wound stitch from a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part the stitch is removed from.</param>
    public void RemoveStitchedWound(string bodyPart){
        bodyPartSprites[bodyPart]["Wound"].SetActive(false);
    }

    /// <summary>
    /// Applies an infection to a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part the infection is applied to.</param>
    public void GetInfection(string bodyPart){
        bodyPartSprites[bodyPart]["Infection"].SetActive(true);

    }

    /// <summary>
    /// Removes an infection from a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part the infection is removed from.</param>
    public void RemoveInfection(string bodyPart){
        bodyPartSprites[bodyPart]["Infection"].SetActive(false);

    }

    /// <summary>
    /// Updates what local status effects are active on a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part is being evaluated.</param>
    /// <param name="statusEffects">Dictionary of active status effects.</param>
    public void UpdateStatusEffects(string bodyPart, Dictionary<string, bool> statusEffects){
        foreach(string statusEffect in statusEffects.Keys){
            if(statusEffects[statusEffect]){
                AddLocalStatusEffect(bodyPart, statusEffect);
            }
            else{
                RemoveLocalStatusEffect(bodyPart, statusEffect);
            }
        }
    }

    /// <summary>
    /// Adds a text represenation of a status effect to a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part is affected by said local status effect.</param>
    /// <param name="statusEffect">What status effect is being applied.</param>
    private void AddLocalStatusEffect(string bodyPart, string statusEffect){
        GameObject statusEffectsObject = transform.Find(bodyPart + "Status").Find("StatusEffects").gameObject;
        if(statusEffectsObject.transform.Find(statusEffect) != null)
            return;
        GameObject newStatusEffect = Instantiate(statusEffectPrefab, statusEffectsObject.transform);
        TextMeshProUGUI statusEffectText = newStatusEffect.GetComponent<TextMeshProUGUI>();
        statusEffectText.color = statusEffectColors[statusEffect];
        newStatusEffect.name = statusEffect;

        if(statusEffect == "BandageClean" || statusEffect == "BandageDirty"){
            statusEffectText.text = "Bandaged";
        }else{
            statusEffectText.text = statusEffect;
        }

        if(statusEffect == "OpenWound"){
            statusEffectText.text = "Open Wound";
        }
        if(statusEffect == "StitchedWound"){
            statusEffectText.text = "Stitched Wound";
        }
    }

    /// <summary>
    /// Removes a text representation of a status effect from a body part.
    /// </summary>
    /// <param name="bodyPart">Which body part is no longer affected by said local status effect.</param>
    /// <param name="statusEffect">What status effect is being removed.</param>
    private void RemoveLocalStatusEffect(string bodyPart, string statusEffect){
        GameObject statusEffectsObject = transform.Find(bodyPart + "Status").Find("StatusEffects").gameObject;
        if(statusEffectsObject.transform.Find(statusEffect) != null)
            Destroy(statusEffectsObject.transform.Find(statusEffect).gameObject);
    }

    /// <summary>
    /// Highlights a body part to mark that it can be treated by held helath item.
    /// </summary>
    /// <param name="bodyPart">What body aprt is being evaluated.</param>
    /// <param name="highlight">Whether the highlight should be active.</param>
    public void BodyPartHighlight(string bodyPart, bool highlight){
        bodyPartHighlightObjects[bodyPart].SetActive(highlight);
    }

    
    /// <summary>
    /// Adds a text represenatation of a  global status effect to the player.
    /// </summary>
    /// <param name="statusEffect">Name of the added status effect.</param>
    public void AddGlobalStatusEffect(string statusEffect){
        GameObject statusEffectsObject = transform.Find("GlobalStatus").gameObject;
        if(statusEffectsObject.transform.Find(statusEffect) != null)
            return;
        GameObject newStatusEffect = Instantiate(statusEffectPrefab, statusEffectsObject.transform);
        TextMeshProUGUI statusEffectText = newStatusEffect.GetComponent<TextMeshProUGUI>();
        statusEffectText.color = statusEffectColors[statusEffect];
        statusEffectText.text = statusEffect;
        statusEffectText.fontSize = 18;
        newStatusEffect.name = statusEffect;
    }

    /// <summary>
    /// Removes a text representation of a global status effect from the player.
    /// </summary>
    /// <param name="statusEffect">Name of the removed status effect.</param>
    public void RemoveGlobalStatusEffect(string statusEffect){
        GameObject statusEffectsObject = transform.Find("GlobalStatus").gameObject;
        if(statusEffectsObject.transform.Find(statusEffect) != null)
            Destroy(statusEffectsObject.transform.Find(statusEffect).gameObject);
    }

    /// <summary>
    /// Updates the health text displayer in the health screen.
    /// </summary>
    /// <param name="currentHealth">Current health of the player.</param>
    /// <param name="maxHealth">Current maximum helth of the player.</param>
    public void UpdateHealthtext(float currentHealth, float maxHealth){
        healthtext.text = Mathf.FloorToInt(currentHealth) + "/" + Mathf.FloorToInt(maxHealth);
    }

    /// <summary>
    /// Updates health arrows in the health screen.
    /// </summary>
    /// <param name="drain">How much health is currently drained/regeberated.</param>
    /// <param name="baseHealthDrain">Base health drain.</param>
    /// <param name="baseHealthRegen">Base health regeneration.</param>
    public void UpdateHealthArrows(float drain, float baseHealthDrain, float baseHealthRegen){
        if(drain < 0){
            HealthArrowsRegen(drain, baseHealthRegen);
        }else{
            HealthArrowsDrain(drain, baseHealthDrain);
        }
    }

   
    /// <summary>
    /// Determines how many arrows to show for health drain in the health section of the Inventory Screen.
    /// </summary>
    /// <param name="drain">Current health drain.</param>
    private void HealthArrowsDrain(float drain, float baseHealthDrain){
        healthArrows[3].SetActive(false);
        healthArrows[4].SetActive(false);
        healthArrows[5].SetActive(false);
        for(int i = 0; i < 3; i++){
            if(drain > i*baseHealthDrain){
                healthArrows[i].SetActive(true);
            }else{
                healthArrows[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Determines how many arrows to show for health regeneration in the health section of the Inventory Screen.
    /// </summary>
    /// <param name="drain">Current health drain (negative value as it is regeneration in this case).</param>
    private void HealthArrowsRegen(float drain, float baseHealthRegen){
        healthArrows[0].SetActive(false);
        healthArrows[1].SetActive(false);
        healthArrows[2].SetActive(false);
        if(drain == 0f){
            healthArrows[3].SetActive(false);
            healthArrows[4].SetActive(false);
            healthArrows[5].SetActive(false);
            return;
        }
        for(int i = 3; i < 6; i++){
            if(drain < -((i-3)*baseHealthRegen)){
                healthArrows[i].SetActive(true);
            }else{
                healthArrows[i].SetActive(false);
            }
        }
    }


}
