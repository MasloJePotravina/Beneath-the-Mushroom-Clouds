using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthStatusUI : MonoBehaviour
{
    [SerializeField] private HealthStatusSpriteData headSpriteData;
    [SerializeField] private HealthStatusSpriteData torsoSpriteData;
    [SerializeField] private HealthStatusSpriteData leftArmSpriteData;
    [SerializeField] private HealthStatusSpriteData rightArmSpriteData;
    [SerializeField] private HealthStatusSpriteData leftLegSpriteData;
    [SerializeField] private HealthStatusSpriteData rightLegSpriteData;

    [SerializeField] private GameObject statusEffectPrefab;

    private Dictionary<string, HealthStatusSpriteData> bodyPartSpriteData = new Dictionary<string, HealthStatusSpriteData>();


    private Dictionary<string, GameObject> headSprites = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> torsoSprites = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> leftArmSprites = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> rightArmSprites = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> leftLegSprites = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> rightLegSprites = new Dictionary<string, GameObject>();

    private Dictionary<string, Dictionary<string, GameObject>> bodyPartSprites; 

    public Dictionary<string, GameObject> bodyPartHighlightObjects;

    [SerializeField] private GameObject healthInfo;

    private TextMeshProUGUI healthtext;

    private GameObject[] healthArrows = new GameObject[6];

   

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
        {"Over-encumbered", Color.red}
    };

    




    void Awake(){

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


    public void ApplyBleed(string bodyPart){
        bodyPartSprites[bodyPart]["Bleeding"].SetActive(true);
        bodyPartSprites[bodyPart]["Wound"].SetActive(false);
        bodyPartSprites[bodyPart]["Bandage"].SetActive(false);

    }

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

    public void RemoveBandage(string bodyPart, bool openWound){
        bodyPartSprites[bodyPart]["Bandage"].SetActive(false);
        if(openWound)
            bodyPartSprites[bodyPart]["Bleeding"].SetActive(true);


    }

    public void DirtyBandage(string bodyPart){
        bodyPartSprites[bodyPart]["Bandage"].GetComponent<RawImage>().texture = bodyPartSpriteData[bodyPart].bandageDirty;

    }

    public void StitchWound(string bodyPart){
        bodyPartSprites[bodyPart]["Wound"].GetComponent<RawImage>().texture = bodyPartSpriteData[bodyPart].stitchedWound;
        bodyPartSprites[bodyPart]["Bleeding"].SetActive(false);
        bodyPartSprites[bodyPart]["Wound"].SetActive(true);

    }

    public void GetInfection(string bodyPart){
        bodyPartSprites[bodyPart]["Infection"].SetActive(true);

    }

    public void RemoveInfection(string bodyPart){
        bodyPartSprites[bodyPart]["Infection"].SetActive(false);

    }


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

    private void RemoveLocalStatusEffect(string bodyPart, string statusEffect){
        GameObject statusEffectsObject = transform.Find(bodyPart + "Status").Find("StatusEffects").gameObject;
        if(statusEffectsObject.transform.Find(statusEffect) != null)
            Destroy(statusEffectsObject.transform.Find(statusEffect).gameObject);
    }

    public void BodyPartHighlight(string bodyPart, bool highlight){
        bodyPartHighlightObjects[bodyPart].SetActive(highlight);
    }

    

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

    public void RemoveGlobalStatusEffect(string statusEffect){
        GameObject statusEffectsObject = transform.Find("GlobalStatus").gameObject;
        if(statusEffectsObject.transform.Find(statusEffect) != null)
            Destroy(statusEffectsObject.transform.Find(statusEffect).gameObject);
    }

    public void UpdateHealthtext(float currentHealth, float maxHealth){
        healthtext.text = Mathf.FloorToInt(currentHealth) + "/" + Mathf.FloorToInt(maxHealth);
    }

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
