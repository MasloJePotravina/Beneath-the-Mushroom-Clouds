using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Implements the upper status HUD behavour (Health, stamina and body temperature (body temp is not implemented yet))
/// </summary>
public class UpperStatusHUD : MonoBehaviour
{
    /// <summary>
    /// Image of the health bar fill.
    /// </summary>
    private Image healthBarFillImage;

    /// <summary>
    /// Image of the stamina bar fill.
    /// </summary>
    private Image staminaBarFillImage;

    /// <summary>
    /// RectTransform of the health bar fill.
    /// </summary>
    private RectTransform healthBarFillRect;
    /// <summary>
    /// RectTransform of the stamina bar fill.
    /// </summary>
    private RectTransform staminaBarFillRect;

    /// <summary>
    /// RectTransform of the health bar negative fill (the part of health which is not accessible due to injuries).
    /// </summary>
    private RectTransform healthBarFillDisabledRect;

    /// <summary>
    /// RectTransform of the stamina bar negative fill (the part of stamina which is not accessible due to infection).
    /// </summary>
    private RectTransform staminaBarFillDisabledRect;

    /// <summary>
    /// Reference to the 6 arrows that indicate the health drain/regen.
    /// </summary>
    private GameObject[] healthArrows;

    /// <summary>
    /// Reference to the 6 arrows that indicate the stamina drain/regen.
    /// </summary>
    private GameObject[] staminaArrows;

    /// <summary>
    /// Reference to the 6 arrows that indicate the body temperature drain/regen.
    /// </summary>
    private GameObject[] bodyTempArrows;

    /// <summary>
    /// Reference to the text (current value) of the health bar.
    /// </summary>
    private TextMeshProUGUI healthBarText;

    /// <summary>
    /// Reference to the body temperature cursor.
    /// </summary>
    private GameObject bodyTempCursor;

    /// <summary>
    /// Reference to the player status.
    /// </summary>
    private PlayerStatus playerStatus;

    /// <summary>
    /// Dictionary that maps the status name to the color of the status.
    /// </summary>
    /// <typeparam name="string">Status value name.</typeparam>
    /// <typeparam name="Color32">Color of the status bar.</typeparam>
    /// <returns></returns>
    Dictionary<string, Color32> statusColors = new Dictionary<string, Color32>(){
        {"health", new Color32(255, 54, 54, 255)},
        {"stamina", new Color32(137, 255, 112, 255)}
    };

    /// <summary>
    /// Get all the references at the beginning.
    /// </summary>
    void Awake()
    {
        GameObject healthBarFill = GameObject.Find("HealthBarFill");
        GameObject staminaBarFill = GameObject.Find("StaminaBarFill");
        GameObject healthBarFillDisabled = GameObject.Find("HealthBarFillDisabled");
        GameObject staminaBarFillDisabled = GameObject.Find("StaminaBarFillDisabled");
        GameObject bodyTempFill = GameObject.Find("BodyTempFill");

        healthBarFillImage = healthBarFill.GetComponent<Image>();
        staminaBarFillImage = staminaBarFill.GetComponent<Image>();

        healthBarFillRect = healthBarFill.GetComponent<RectTransform>();
        staminaBarFillRect = staminaBarFill.GetComponent<RectTransform>();
        healthBarFillDisabledRect = healthBarFillDisabled.GetComponent<RectTransform>();
        staminaBarFillDisabledRect = staminaBarFillDisabled.GetComponent<RectTransform>();


        healthBarText = GameObject.Find("HealthBarText").GetComponent<TextMeshProUGUI>();
        bodyTempCursor = GameObject.Find("BodyTempCursor");

        healthArrows = new GameObject[6];
        staminaArrows = new GameObject[6];
        bodyTempArrows = new GameObject[6];

        for(int i = 0; i < 6; i++){
            healthArrows[i] = healthBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            staminaArrows[i] = staminaBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            bodyTempArrows[i] = bodyTempFill.transform.Find("Arrow" + (i+1)).gameObject;
        }

        playerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
    }

    /// <summary>
    /// Each frame updates the status bars (body temperature is not implemented yet).
    /// </summary>
    public void Update()
    {
        
        UpdateStamina(playerStatus.playerStamina, playerStatus.staminaDrain);
        UpdateHealth(playerStatus.playerHealth, playerStatus.healthDrain);
        //UpdateBodyTemp(bodyTempDrain);

    }

    /// <summary>
    /// Gradualy disables drain and regeneration arrows in the upper status bars, when the value approaches 0.
    /// </summary>
    /// <param name="bar">String that determines which bar to disable arrows for.</param>
    public void ArrowEdges(string bar, float value){
        if(bar == "stamina"){
            if(value <= 6){
                staminaArrows[2].SetActive(false);
                staminaArrows[5].SetActive(false);
            }
            if(value <= 4){
                staminaArrows[1].SetActive(false);
                staminaArrows[4].SetActive(false);
            }
            if(value <= 2){
                staminaArrows[0].SetActive(false);
                staminaArrows[3].SetActive(false);
            }
        }else if(bar == "health"){
            if(value <= 6){
                healthArrows[2].SetActive(false);
                healthArrows[5].SetActive(false);
            }
            if(value <= 4){
                healthArrows[1].SetActive(false);
                healthArrows[4].SetActive(false);
            }
            if(value <= 2){
                healthArrows[0].SetActive(false);
                healthArrows[3].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Updates the stamina bar fill, regenration and drain arrows, and the color of the bar fill.
    /// </summary>
    /// <param name="drain"></param>
    public void UpdateStamina(float stamina, float drain){
        staminaBarFillRect.offsetMax = UpdateUpperBarFillRectSize(stamina);
        staminaBarFillDisabledRect.offsetMin = -UpdateUpperBarFillRectSize(100-playerStatus.maxPlayerStamina);
        if(stamina <= 20)
            staminaBarFillImage.color = Color.red;
        else
            staminaBarFillImage.color = statusColors["stamina"];

        if(drain > 0){
            StaminaArrowsDrain(drain);
        }else{
            StaminaArrowsRegen(drain);
        }

        ArrowEdges("stamina", stamina);
    }

    /// <summary>
    /// Updates the health bar fill, regenration and drain arrows, and the color of the bar fill.
    /// </summary>
    public void UpdateHealth(float health, float drain){
        healthBarFillRect.offsetMax = UpdateUpperBarFillRectSize(health);
        healthBarFillDisabledRect.offsetMin = -UpdateUpperBarFillRectSize(100-playerStatus.maxPlayerHealth);
        if(health <= 20)
            healthBarFillImage.color = Color.red;
        else
            healthBarFillImage.color = statusColors["health"];

        if(drain > 0){
            HealthArrowsDrain(drain);
        }else{
            HealthArrowsRegen(drain);
        }

        ArrowEdges("health", health);

        healthBarText.text = health.ToString("F0");
    }


    /// <summary>
    /// Determines how many arrows to show for drain of the stamina bar (left facing arrows).
    /// </summary>
    /// <param name="drain">Current stamina drain.</param>
    private void StaminaArrowsDrain(float drain){
        staminaArrows[3].SetActive(false);
        staminaArrows[4].SetActive(false);
        staminaArrows[5].SetActive(false);
        for(int i = 0; i < 3; i++){
            if(drain > i*playerStatus.baseStaminaDrain){
                staminaArrows[i].SetActive(true);
            }else{
                staminaArrows[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Determines how many arrows to show for drain of the health bar (left facing arrows).
    /// </summary>
    /// <param name="drain">Current health drain.</param>
    private void HealthArrowsDrain(float drain){
        healthArrows[3].SetActive(false);
        healthArrows[4].SetActive(false);
        healthArrows[5].SetActive(false);
        for(int i = 0; i < 3; i++){
            if(drain > i*playerStatus.baseHealthDrain){
                healthArrows[i].SetActive(true);
            }else{
                healthArrows[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Determines how many arrows to show for regeneration of the stamina bar (right facing arrows).
    /// </summary>
    /// <param name="drain">Current stamina drain (negative value as it is regeneration in this case).</param>
    private void StaminaArrowsRegen(float drain){
        staminaArrows[0].SetActive(false);
        staminaArrows[1].SetActive(false);
        staminaArrows[2].SetActive(false);
        for(int i = 3; i < 6; i++){
            if(drain < -((i-3)*playerStatus.baseStaminaRegen)){
                staminaArrows[i].SetActive(true);
            }else{
                staminaArrows[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Determines how many arrows to show for regeneration of the health bar (right facing arrows).
    /// </summary>
    /// <param name="drain">Current health drain (negative value as it is regeneration in this case).</param>
    private void HealthArrowsRegen(float drain){
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
            if(drain < -((i-3)*playerStatus.baseHealthRegen)){
                healthArrows[i].SetActive(true);
            }else{
                healthArrows[i].SetActive(false);
            }
        }
    }

    //NOTE: This is done instead of fill amount because the stamina arrows need to move together with the end of the bar
    //If fill amount was used, like in the lower status bars, the arrows would need a calculation like this anyway
    //This way, the arrows are anchored to the end of the bar and move with it

    /// <summary>
    /// Updates the size of the stamina bar fill.
    /// </summary>
    /// <returns></returns>
    private Vector2 UpdateUpperBarFillRectSize(float value){
        return new Vector2(-2-(400-4*value), 7.5f);
    }


}
