using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        GetWeaponObjects();
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

    private void GetWeaponObjects(){
        weapon = transform.Find("Weapon").gameObject;

        weaponName = weapon.transform.Find("WeaponName").gameObject;
        weaponImage = weapon.transform.Find("WeaponImage").gameObject;
        fireModeText = weapon.transform.Find("FireModeText").gameObject;
        magazineBar = weapon.transform.Find("MagazineBar").gameObject;

        magazineBarFull = magazineBar.transform.Find("MagazineBarFull").gameObject;
        magazineBarEmpty = magazineBar.transform.Find("MagazineBarEmpty").gameObject;

        chamberFull = magazineBarFull.transform.Find("ChamberFull").gameObject;
        chamberEmpty = magazineBarEmpty.transform.Find("ChamberEmpty").gameObject;

        ammoCountText = chamberEmpty.transform.Find("AmmoCountText").gameObject;
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

    public void ActivateWeaponSection(){
        weapon.SetActive(true);
    }

    public void DeactivateWeaponSection(){
        weapon.SetActive(false);
    }

    public void UpdateWeaponName(string name){
        weaponName.GetComponent<TextMeshProUGUI>().text = name;
    }

    public void UpdateWeaponImage(Sprite image){
        weaponImage.GetComponent<Image>().sprite = image;
    }

    public void UpdateFireModeText(string mode){
        fireModeText.GetComponent<TextMeshProUGUI>().text = mode;
    }

    public void UpdateMagazineBarImage(Sprite ammoBarFull, Sprite ammoBarEmpty, string weaponType){

        magazineBarFull.GetComponent<Image>().sprite = ammoBarFull;
        magazineBarEmpty.GetComponent<Image>().sprite = ammoBarEmpty;

        Image chaberFullImage = chamberFull.GetComponent<Image>();
        Image chamberEmptyImage = chamberEmpty.GetComponent<Image>();

        chaberFullImage.sprite = ammoBarFull;
        chamberEmptyImage.sprite = ammoBarEmpty;

        float maxAmmo = 0;
        float barHeight = 0;

        switch(weaponType){
            case "AssaultRifle":
                maxAmmo = 30.0f;
                barHeight = 150f;
                break;
            case "Pistol":
                maxAmmo = 12.0f;
                barHeight = 130f;
                break;
            case "Shotgun":
                maxAmmo = 6.0f;
                barHeight = 55f;
                break;
            case "HuntingRifle":
                maxAmmo = 4.0f;
                barHeight = 20f;
                break;
            default:
                maxAmmo = 1.0f;
                barHeight = 0f;
                break;
        }

        magazineBarFull.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, barHeight);
        magazineBarEmpty.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, barHeight);

        chaberFullImage.fillAmount = 1/maxAmmo;
        chamberEmptyImage.fillAmount = 1/maxAmmo;
    }

    public void UpdateAmmoStatus(int ammoCount, int maxAmmo, bool isChambered, bool manuallyChambered){
        float ammoPercent = (float)ammoCount / (float)maxAmmo;
        magazineBarFull.GetComponent<Image>().fillAmount = ammoPercent;
        if(isChambered){
            chamberFull.SetActive(true);
        }else{
            chamberFull.SetActive(false);
        }

        if(isChambered){
            ammoCount += 1;
        }
        this.ammoCountText.GetComponent<TextMeshProUGUI>().text = ammoCount.ToString() + "/" + maxAmmo.ToString();

    }

    public void UpdateWeaponHUD(InventoryItem item){
        if(item == null){
            DeactivateWeaponSection();
            return;
        }
        ActivateWeaponSection();
        if(item.itemData.firearm){
            UpdateWeaponName(item.itemData.itemName.ToUpper());
            UpdateWeaponImage(item.itemData.weaponHUDOutlineSprite);
            //TODO: Change when firemode selection is added
            UpdateFireModeText("AUTOMATIC");
            UpdateMagazineBarImage( item.itemData.ammoBarFullSprite, item.itemData.ammoBarEmptySprite, 
                                    item.itemData.weaponType);
            UpdateAmmoStatus(item.ammoCount, item.currentMagazineSize, item.isChambered, item.itemData.manuallyChambered);
        }
    }
    
        
}
