using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpperStatusHUD : MonoBehaviour
{
    private Image healthBarFillImage;
    private Image staminaBarFillImage;
    private Image bodyTempFillImage;

    private RectTransform healthBarFillRect;
    private RectTransform staminaBarFillRect;

    private GameObject[] healthArrows;
    private GameObject[] staminaArrows;
    private GameObject[] bodyTempArrows;

    private TextMeshProUGUI healthBarText;
    private GameObject bodyTempCursor;

    private PlayerStatus status;


    void Awake()
    {
        GameObject healthBarFill = GameObject.Find("HealthBarFill");
        GameObject staminaBarFill = GameObject.Find("StaminaBarFill");
        GameObject bodyTempFill = GameObject.Find("BodyTempFill");

        healthBarFillImage = healthBarFill.GetComponent<Image>();
        staminaBarFillImage = staminaBarFill.GetComponent<Image>();

        healthBarFillRect = healthBarFill.GetComponent<RectTransform>();
        staminaBarFillRect = staminaBarFill.GetComponent<RectTransform>();

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

        status = GameObject.Find("Player").GetComponent<PlayerStatus>();
    }

    public void Update()
    {
        
        UpdateStamina(status.playerStamina, status.staminaDrain);
        UpdateHealth(status.playerHealth, status.healthDrain);
        //UpdateBodyTemp(bodyTempDrain);

    }

    /// <summary>
    /// Graduaaly disables drain and regeneration arrows in the upper status bars, when the value approaches 0.
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
        if(stamina <= 20)
            staminaBarFillImage.color = new Color32(255, 0, 0, 255);
        else
            staminaBarFillImage.color = new Color32(137, 255, 112, 255);

        if(drain > 0){
            StaminaArrowsDrain(drain);
        }else{
            StaminaArrowsRegen(drain);
        }

        ArrowEdges("stamina", stamina);
    }

    public void UpdateHealth(float health, float drain){
        healthBarFillRect.offsetMax = UpdateUpperBarFillRectSize(health);
        if(health <= 20)
            healthBarFillImage.color = new Color32(255, 0, 0, 255);
        else
            healthBarFillImage.color = new Color32(255, 54, 54, 255);

        if(drain > 0){
            HealthArrowsDrain(drain);
        }else{
            HealthArrowsRegen(drain);
        }

        ArrowEdges("health", health);
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
            if(drain > i*status.baseStaminaDrain){
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
            if(drain > i*status.baseHealthDrain){
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
            if(drain < -((i-3)*status.baseStaminaRegen)){
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
        for(int i = 3; i < 6; i++){
            if(drain < -((i-3)*status.baseHealthRegen)){
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
