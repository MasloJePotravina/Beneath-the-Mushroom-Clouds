using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// This class implements the HUD controller. If any elements of the game need to access the HUD, they should do so through this script.
/// </summary>
public class HUDController : MonoBehaviour
{

    public GameObject player;

    /// <summary>
    /// Reference to the player status script.
    /// </summary>
    public PlayerStatus status;
    
    ///@{ 
    /// <summary>
    /// Status bar game object reference.
    /// </summary>
    private GameObject healthBar, staminaBar, hungerBar, thirstBar, tirednessBar, bodyTempBar;
    ///@}

    ///@{
    /// <summary>
    /// Status bar fill game object reference.
    /// </summary>
    private GameObject healthBarFill, staminaBarFill, hungerBarFill, thirstBarFill, tirednessBarFill;
    ///@} 

    ///@{
    /// <summary>
    /// Status bar fill rect transform reference.
    /// </summary>
    private RectTransform healthBarFillRect, staminaBarFillRect, hungerBarFillRect, thirstBarFillRect, tirednessBarFillRect;
    ///@}    

    ///@{
    /// <summary>
    /// Status bar fill image component reference.
    /// </summary>
    private Image healthBarFillImage, staminaBarFillImage, hungerBarFillImage, thirstBarFillImage, tirednessBarFillImage;
    ///@}

    ///@{
    /// <summary>
    /// Array of references to the status bar arrow objects.
    /// </summary>
    private GameObject[] healthArrows, staminaArrows, bodyTempArrows;
    ///@}  

    /// <summary>
    /// Reference to the body temperature cursor object.
    /// </summary>
    private GameObject bodyTempCursor;

    /// <summary>
    /// Reference to the stance object.
    /// </summary>
    private GameObject stance;

    /// <summary> 
    /// Reference to the stance object image component.
    /// </summary>
    private RawImage stanceImage;

    /// <summary>
    /// Texture for standing stance
    /// </summary>
    [SerializeField] private Texture stanceStandingTexture;

    /// <summary>
    /// Texture for crouching stance
    /// </summary>
    [SerializeField] private Texture stanceCrouchingTexture;

    /// <summary>
    /// Reference to the Watch object.
    /// </summary>
    private GameObject watch;

    ///@{
    /// <summary>
    /// Reference to one of the watch text objects.
    /// <summary>
    private GameObject timeText, dateText, dayText, tempText;
    ///@}  

    /// <summary>
    /// Reference to the geiger counter object.
    /// </summary>
    private GameObject geigerCounter;

    /// <summary>
    ///  Reference to the geiger counter text object.
    /// </summary>
    private GameObject geigerCounterText;

    /// <summary>
    /// Reference to the geiger counter sign object.
    /// </summary>
    private GameObject geigerCounterSign;

    /// <summary>
    /// Reference to the weapon section of the HUD.
    /// </summary>
    private GameObject weapon;

    /// <summary>
    /// Reference to weapon name gameObject in the weapon section.
    /// </summary>
    private GameObject weaponName;
    /// <summary>
    /// Reference the weapon image outline in the weapon section.
    /// </summary>
    private GameObject weaponImage;
    /// <summary>
    /// Reference to the fire mode text in the weapon section.
    /// </summary>
    private GameObject fireModeText;
    /// <summary>
    /// Reference to the magazine bar in the weapon section.
    /// </summary>
    private GameObject magazineBar;
    /// <summary>
    /// Reference to the full magazine bar in the weapon section.
    /// </summary>
    private GameObject magazineBarFull;
    /// <summary>
    /// Reference to the empty magazine bar in the weapon section.
    /// </summary>
    private GameObject magazineBarEmpty;
    /// <summary>
    /// Reference to the full chamber image in the weapon section.
    /// </summary>
    private GameObject chamberFull;
    /// <summary>
    /// Reference to the empty chamber image in the weapon section.
    /// </summary>
    private GameObject chamberEmpty;
    /// <summary>
    /// Reference to the ammo count text in the weapon section.
    /// </summary>
    private GameObject ammoCountText;

    [SerializeField] GameObject interactText;

    private InteractText interactTextScript;


    /// <summary>
    /// Get all objects references on start.
    /// </summary>
    void Start()
    {
        interactTextScript = interactText.GetComponent<InteractText>();
        GetStatusBarObjects();
        GetWeaponObjects();
    }

    void Update(){
        if(interactTextScript.isActive){
            interactTextScript.MoveTextAbovePlayer(player.transform.position);
        }
    }

    /// <summary>
    /// Gets all of the status objects such ass all of the status bars, their components and the stance image reference.
    /// This method id not very effective but it is only called once at start.
    /// </summary>
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
        //bodyTempArrows = new GameObject[6];


        for(int i = 0; i < 6; i++){
            healthArrows[i] = healthBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            staminaArrows[i] = staminaBarFill.transform.Find("Arrow" + (i+1)).gameObject;
            //bodyTempArrows[i] = bodyTempBar.transform.Find("Arrow" + (i+1)).gameObject;
        }

        bodyTempCursor = bodyTempBar.transform.Find("BodyTempCursor").gameObject;

        stance = transform.Find("LowerStatusBars").transform.Find("Stance").gameObject;
        stanceImage = stance.GetComponent<RawImage>();

    }

    /// <summary>
    /// Gets all of the references tied to the weapon section of the HUD.
    /// This method is not very effective but it is only called once at start.
    /// </summary>
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


    /// <summary>
    /// Graduaaly disables drain and regeneration arrows in the upper status bars, when the value approaches 0.
    /// </summary>
    /// <param name="bar">String that determines which bar to disable arrows for.</param>
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

        if(bar == "health"){
            if(status.playerStamina <= 6){
                healthArrows[2].SetActive(false);
                healthArrows[5].SetActive(false);
            }
            if(status.playerStamina <= 4){
                healthArrows[1].SetActive(false);
                healthArrows[4].SetActive(false);
            }
            if(status.playerStamina <= 2){
                healthArrows[0].SetActive(false);
                healthArrows[3].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Updates the stamina bar fill, regenration and drain arrows, and the color of the bar fill.
    /// </summary>
    /// <param name="drain"></param>
    public void UpdateStamina(float drain){
        staminaBarFillRect.offsetMax = UpdateStaminaRectSize();
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




    //(regular drain/regeneration -> one arrow, double drain/regeneration -> two arrows, triple or more drain/regeneration -> three arrows)
    //TODO: Is not yet implemented for the body temperature bar, as that mechanic is not yet in the game


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
    private Vector2 UpdateStaminaRectSize(){
        return new Vector2(-2-(400-4*status.playerStamina), 7.5f);
    }


    /// <summary>
    /// Activates the weapon section of the HUD
    /// </summary>
    public void ActivateWeaponSection(){
        weapon.SetActive(true);
    }

    /// <summary>
    /// Deactivates the weapon section of the HUD
    /// </summary>
    public void DeactivateWeaponSection(){
        weapon.SetActive(false);
    }

    /// <summary>
    /// Updates the weapon name in the HUD
    /// </summary>
    /// <param name="name">Name of the weapon</param>
    public void UpdateWeaponName(string name){
        weaponName.GetComponent<TextMeshProUGUI>().text = name;
    }

    /// <summary>
    /// Updates the weapon image (outline) in the HUD
    /// </summary>
    /// <param name="image">Sprite of the weapon's outline</param>
    public void UpdateWeaponImage(Sprite image){
        weaponImage.GetComponent<Image>().sprite = image;
    }

    /// <summary>
    /// Updates the fire mode text in the HUD
    /// </summary>
    /// <param name="mode">Weapon's fire mode</param>
    public void UpdateFireModeText(string mode){
        fireModeText.GetComponent<TextMeshProUGUI>().text = mode;
    }
    /// <summary>
    /// Updates the magazine bar in the HUD, swaps images for different weapons and sets appropriate height and fill amount
    /// </summary>
    /// <param name="ammoBarFull">Sprite of the full magazine bar</param>
    /// <param name="ammoBarEmpty">Sprite of the empty magazine bar</param>
    /// <param name="weaponType">Type of the weapon</param>
    public void UpdateMagazineBarImage(Sprite ammoBarFull, Sprite ammoBarEmpty, string weaponType){

        magazineBarFull.GetComponent<Image>().sprite = ammoBarFull;
        magazineBarEmpty.GetComponent<Image>().sprite = ammoBarEmpty;

        Image chaberFullImage = chamberFull.GetComponent<Image>();
        Image chamberEmptyImage = chamberEmpty.GetComponent<Image>();

        chaberFullImage.sprite = ammoBarFull;
        chamberEmptyImage.sprite = ammoBarEmpty;

        float maxAmmo = 0;
        float barHeight = 0;
        
        //The maximum ammo and ammo bar height depends on the weapon type
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

    /// <summary>
    /// Updates the ammunition status in the hud.
    /// </summary>
    /// <param name="ammoCount">Current ammunition count of the weapon (chambered round excluded).
    /// <param name="maxAmmo">Maximum capacity of the magazine in the weapon.</param>
    /// <param name="isChambered">Whether the weapon is chambered or not.</param>
    public void UpdateAmmoStatus(int ammoCount, int maxAmmo, bool isChambered){
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

    /// <summary>
    /// Master method which calls all of the methods needed to update the weapon section of the HUD
    /// </summary>
    /// <param name="item">Currently selected weapon</param>
    public void UpdateWeaponHUD(InventoryItem item){
        if(item == null){
            DeactivateWeaponSection();
            return;
        }
        ActivateWeaponSection();
        //TODO: Since melee weapons are not yet implemented, only works for firearms
        if(item.itemData.firearm){
            UpdateWeaponName(item.itemData.itemName.ToUpper());
            UpdateWeaponImage(item.itemData.weaponHUDOutlineSprite);
            UpdateFireModeText(item.GetFiremode().ToUpper());
            UpdateMagazineBarImage( item.itemData.ammoBarFullSprite, item.itemData.ammoBarEmptySprite, 
                                    item.itemData.weaponType);
            UpdateAmmoStatus(item.ammoCount, item.currentMagazineSize, item.isChambered);
        }
    }

    /// <summary>
    /// Switches the stance image to the standing stance
    /// </summary>
    public void StanceStand(){
        stanceImage.texture = stanceStandingTexture;
    }

    /// <summary>
    /// Switches the stance image to the crouching stance
    /// </summary>
    public void StanceCrouch(){
        stanceImage.texture = stanceCrouchingTexture;
    }

    public void SetInteractText(string text){
        interactTextScript.SetText(text);
    }

    public void ActivateInteractText(){
        interactText.SetActive(true);
    }

    public void DeactivateInteractText(){
        interactText.SetActive(false);
    }
    
        
}