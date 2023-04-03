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


    [SerializeField] GameObject interactText;

    private InteractText interactTextScript;

    private CursorController cursorController;

    public bool watchEquipped = true;
    public bool geigerCounterEquipped = false;


    [SerializeField] private LowerStatusHUD lowerStatusHUD;
    [SerializeField] private WeaponHUD weaponHUD;
    [SerializeField] private RestMenu restMenu;

    [SerializeField] private RestScreen restScreen;

    [SerializeField] private WorldStatus worldStatus;
    

    /// <summary>
    /// Get all objects references on start.
    /// </summary>
    void Awake()
    {
        interactTextScript = interactText.GetComponent<InteractText>();
        cursorController = GameObject.Find("GameManager").GetComponent<CursorController>();
        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
    }

    void Update(){
        if(interactTextScript.isActive){
            interactTextScript.MoveTextAbovePlayer(player.transform.position);
        }
        
    }

    


    /// <summary>
    /// Master method which calls all of the methods needed to update the weapon section of the HUD
    /// </summary>
    /// <param name="item">Currently selected weapon</param>
    public void UpdateWeaponHUD(InventoryItem item){
        if(item == null){
            weaponHUD.DeactivateWeaponSection();
            return;
        }
        weaponHUD.ActivateWeaponSection();
        //TODO: Since melee weapons are not yet implemented, only works for firearms
        if(item.itemData.firearm){
            weaponHUD.UpdateWeaponName(item.itemData.itemName.ToUpper());
            weaponHUD.UpdateWeaponImage(item.itemData.weaponHUDOutlineSprite);
            weaponHUD.UpdateFireModeText(item.GetFiremode().ToUpper());
            weaponHUD.UpdateMagazineBarImage( item.itemData.ammoBarFullSprite, item.itemData.ammoBarEmptySprite, 
                                    item.itemData.weaponType);
            weaponHUD.UpdateAmmoStatus(item.ammoCount, item.currentMagazineSize, item.isChambered);
        }
    }

    /// <summary>
    /// Switches the stance image to the standing stance
    /// </summary>
    public void StanceStand(){
        lowerStatusHUD.StanceStand();
    }

    /// <summary>
    /// Switches the stance image to the crouching stance
    /// </summary>
    public void StanceCrouch(){
        lowerStatusHUD.StanceCrouch();
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

    public void ActivateRestMenu(){
        restMenu.gameObject.SetActive(true);
        cursorController.SwitchToDefaultCursor();
    }

    public void DeactivateRestMenu(){
        restMenu.gameObject.SetActive(false);
        cursorController.SwitchToCrosshairCursor();
    }

    public void Rest(float hours){
        StartCoroutine(RestTransition(hours));
    }

    public IEnumerator ActivateRestScreen(float hours){
        restScreen.SetAlpha(0f);
        restScreen.gameObject.SetActive(true);
        restScreen.Init(hours);
        StartCoroutine(RestScreenFade(true));
        yield return new WaitForSecondsRealtime(3f);
        restScreen.Activate();
        
    }

    public IEnumerator DeactivateRestScreen(){
        restScreen.Deactivate();
        StartCoroutine(RestScreenFade(false));
        yield return new WaitForSecondsRealtime(3f);
        restScreen.gameObject.SetActive(false);
    }

    private IEnumerator RestScreenFade(bool fadeIn){

        for(float alpha = 0; alpha <= 1; alpha += Time.unscaledDeltaTime/3f){
            if(fadeIn){
                restScreen.SetAlpha(alpha);
            } else {
                restScreen.SetAlpha(1f - alpha);
            }
            yield return null;
        }
    }

    private IEnumerator RestTransition(float hours){
        DeactivateRestMenu();
        worldStatus.StopTime();
        StartCoroutine(ActivateRestScreen(hours));
        yield return new WaitForSecondsRealtime(5.5f);
        worldStatus.TimeSkip(hours);
        restMenu.ApplySleepEffects();
        yield return new WaitForSecondsRealtime(2.5f);
        StartCoroutine(DeactivateRestScreen());
        yield return new WaitForSecondsRealtime(3f);
        worldStatus.StartTime();
        
    }
}
