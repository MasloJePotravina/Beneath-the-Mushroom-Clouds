using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    public PlayerStatus status;

    //Status Bars
    private GameObject healthBar, staminaBar, hungerBar, thirstBar, tirednessBar, bodyTempBar;
    private GameObject healthBarFill, staminaBarFill, hungerBarFill, thirstBarFill, tirednessBarFill;
    private RectTransform healthBarFillRect, staminaBarFillRect, hungerBarFillRect, thirstBarFillRect, tirednessBarFillRect;
    private Image healthBarFillImage, staminaBarFillImage, hungerBarFillImage, thirstBarFillImage, tirednessBarFillImage;
    private GameObject[] healthArrows, staminaArrows, hungerArrows, thirstArrows, tirednessArrows, bodyTempArrows;
    private GameObject bodyTempCursor;
    private GameObject stance;
    private Image stanceImage;

    //Watch
    private GameObject watch;
    private GameObject timeText, dateText, dayText, tempText;

    //Geiger Counter
    private GameObject geigerCounter;
    private GameObject geigerCounterText;
    private GameObject geigerCounterSign;

    //Weapon
    private GameObject weapon;
    private GameObject weaponName, weaponImage, fireModeText;
    private GameObject magazineBar, magazineBarFull, magazineBarEmpty, chamberFull, chamberEmpty, ammoCountText;


    // Start is called before the first frame update
    void Start()
    {
        GetStatusBarObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetStatusBarObjects(){
        healthBar = transform.Find("UpperStatusBars").Find("HealthBar").gameObject;
        staminaBar = transform.Find("UpperStatusBars").Find("StaminaBar").gameObject;
        hungerBar = transform.Find("LowerStatusBars").Find("HungerBar").gameObject;
        thirstBar = transform.Find("LowerStatusBars").Find("ThirstBar").gameObject;
        tirednessBar = transform.Find("LowerStatusBars").Find("TirednessBar").gameObject;
        bodyTempBar = transform.Find("UpperStatusBars").Find("BodyTempBar").gameObject;

        healthBarFill = healthBar.transform.Find("HealthBarFill").gameObject;
        staminaBarFill = staminaBar.transform.Find("StaminaBarFill").gameObject;
        hungerBarFill = hungerBar.transform.Find("HungerBarFill").gameObject;
        thirstBarFill = thirstBar.transform.Find("ThirstBarFill").gameObject;
        tirednessBarFill = tirednessBar.transform.Find("TirednessBarFill").gameObject;

        

        healthBarFillRect = healthBarFill.GetComponent<RectTransform>();
        staminaBarFillRect = staminaBarFill.GetComponent<RectTransform>();
        hungerBarFillRect = hungerBarFill.GetComponent<RectTransform>();
        thirstBarFillRect = thirstBarFill.GetComponent<RectTransform>();
        tirednessBarFillRect = tirednessBarFill.GetComponent<RectTransform>();

        healthBarFillImage = healthBarFill.GetComponent<Image>();
        staminaBarFillImage = staminaBarFill.GetComponent<Image>();
        hungerBarFillImage = hungerBarFill.GetComponent<Image>();
        thirstBarFillImage = thirstBarFill.GetComponent<Image>();
        tirednessBarFillImage = tirednessBarFill.GetComponent<Image>();

        healthArrows = new GameObject[6];
        staminaArrows = new GameObject[6];
        hungerArrows = new GameObject[6];
        thirstArrows = new GameObject[6];
        tirednessArrows = new GameObject[6];
        bodyTempArrows = new GameObject[6];


        for(int i = 0; i < 6; i++){
            //healthArrows[i] = healthBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            staminaArrows[i] = staminaBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            //hungerArrows[i] = hungerBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            //thirstArrows[i] = thirstBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            //tirednessArrows[i] = tirednessBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            //bodyTempArrows[i] = bodyTempBar.transform.Find("Arrow" + (i+1)).gameObject;
        }

        bodyTempCursor = bodyTempBar.transform.Find("BodyTempCursor").gameObject;

        stance = this.transform.Find("LowerStatusBars").Find("Stance").gameObject;
        stanceImage = stance.GetComponent<Image>();

    }

    public void ArrowEdges(string bar){
        if(bar == "stamina"){
            if(status.playerStamina <= 6){
                staminaArrows[2].SetActive(false);
                staminaArrows[5].SetActive(false);
            }
            if(status.playerStamina <= 4){
                staminaArrows[1].SetActive(false);
                staminaArrows[4].SetActive(false);
            }
            if(status.playerStamina <= 2){
                staminaArrows[0].SetActive(false);
                staminaArrows[3].SetActive(false);
            }
        }
    }

    public void UpdateStamina(float drain){
        staminaBarFillRect.offsetMax = new Vector2(-2-(400-4*status.playerStamina), 7.5f);
        if(status.playerStamina <= 20)
            staminaBarFillImage.color = new Color32(255, 0, 0, 255);
        else
            staminaBarFillImage.color = new Color32(137, 255, 112, 255);

        if(drain > 0){
            StaminaArrowsDrain(drain);
        }else{
            StaminaArrowsRegen(drain);
        }

        ArrowEdges("stamina");
    }

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
}
