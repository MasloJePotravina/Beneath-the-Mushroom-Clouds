using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// This class implements the HUD controller. If any elements of the game need to access the HUD, they should do so through this script.
/// </summary>
public class HUDController : MonoBehaviour
{
    /// <summary>
    /// Reference to the player object.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Reference to the player playerStatus script.
    /// </summary>
    private PlayerStatus playerStatus;


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
    /// Reference to the interact text which is desplayed when the player is near an interactable object.
    /// </summary>
    [SerializeField] GameObject interactText;

    /// <summary>
    /// Reference to the script which controls the interact text.
    /// </summary>
    private InteractText interactTextScript;

    /// <summary>
    /// Reference to the cursor controller.
    /// </summary>
    private CursorController cursorController;

    /// <summary>
    /// Bool which indicates whether the watch is equipped or not.
    /// </summary>
    public bool watchEquipped = true;

    /// <summary>
    /// Bool which indicates whether the geiger counter is equipped or not.
    /// </summary>
    public bool geigerCounterEquipped = false;

    /// <summary>
    /// Reference to the lower status HUD (Hunger, thirst, tiredness and stance).
    /// </summary>
    [SerializeField] private LowerStatusHUD lowerStatusHUD;

    /// <summary>
    /// Reference to the weapon HUD.
    /// </summary>
    [SerializeField] private WeaponHUD weaponHUD;

    /// <summary>
    /// Reference to the watch HUD.
    /// </summary>
    [SerializeField] private GameObject watchHUD;

    /// <summary>
    /// Reference to rest menu displayed when player interacts with a bed.
    /// </summary>
    [SerializeField] private RestMenu restMenu;

    /// <summary>
    /// Reference to the rest screen displayed when the player is resting.
    /// </summary>
    [SerializeField] private RestScreen restScreen;

    /// <summary>
    /// Reference to the world status.
    /// </summary>
    [SerializeField] private WorldStatus worldStatus;
    
    

    /// <summary>
    /// Get all objects references on start.
    /// </summary>
    void Awake()
    {
        player = GameObject.Find("Player");
        interactTextScript = interactText.GetComponent<InteractText>();
        cursorController = GameObject.Find("GameManager").GetComponent<CursorController>();
        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
        playerStatus = player.GetComponent<PlayerStatus>();
    }

    /// <summary>
    /// If the interact text is active, move it above the player each frame.
    /// </summary>
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

    /// <summary>
    /// Sets the interact text above the player.
    /// </summary>
    /// <param name="interactableTag">Tag or name of the interactable object.</param>
    /// <param name="text">text describing the action to perform on interaction.</param>
    public void SetInteractText(string interactableTag, string text){
        interactTextScript.SetText(interactableTag, text);
    }

    /// <summary>
    /// Activates the interact text.
    /// </summary>
    public void ActivateInteractText(){
        interactText.SetActive(true);
        
    }

    /// <summary>
    /// Deactivates the interact text.
    /// </summary>
    public void DeactivateInteractText(){
        interactText.SetActive(false);
        
    }

    /// <summary>
    /// Activates the rest menu when interacting with a bed.
    /// </summary>
    public void ActivateRestMenu(){
        restMenu.gameObject.SetActive(true);
        cursorController.SwitchToDefaultCursor();
        PlayerInput playerInput = GameObject.FindObjectOfType<PlayerInput>(true);
        playerInput.SwitchCurrentActionMap("UI");
    }

    /// <summary>
    /// Deactivates the menu when the player closes it or chooses to rest.
    /// </summary>
    public void DeactivateRestMenu(){
        restMenu.gameObject.SetActive(false);
        cursorController.SwitchToCrosshairCursor();
        PlayerInput playerInput = GameObject.FindObjectOfType<PlayerInput>(true);
        playerInput.SwitchCurrentActionMap("Player");
    }

    
    /// <summary>
    /// Coroutine which slowly activates the rest screen.
    /// </summary>
    /// <returns>Reference to the running coroutine.</returns>
    public IEnumerator ActivateRestScreen(){
        restScreen.SetAlpha(0f);
        restScreen.gameObject.SetActive(true);
        StartCoroutine(RestScreenFade(true));
        yield return new WaitForSecondsRealtime(3f);
        
    }

    /// <summary>
    /// Coroutine which deactivates the rest screen.
    /// </summary>
    /// <returns>Reference to the running coroutine.</returns>
    public IEnumerator DeactivateRestScreen(){
        StartCoroutine(RestScreenFade(false));
        yield return new WaitForSecondsRealtime(3f);
        restScreen.gameObject.SetActive(false);
    }

    /// <summary>
    /// The fade-in or fade-out of the rest screen.
    /// </summary>
    /// <param name="fadeIn">Whether the screen should fade in (true) or fade out (false).</param>
    /// <returns></returns>
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


    /// <summary>
    /// Enables the watch HUD.
    /// </summary>
    public void EnableWatch(){
        watchHUD.SetActive(true);
    }

    /// <summary>
    /// Disables the watch HUD.
    /// </summary>
    public void DisableWatch(){
        watchHUD.SetActive(false);
    }
}
