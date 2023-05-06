using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Implements the behaviour of the weapon section of the HUD.
/// </summary>
public class WeaponHUD : MonoBehaviour
{
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
    
    /// <summary>
    /// Gets all of the references tied to the weapon section of the HUD.
    /// This method is not very effective but it is only called once at start.
    /// </summary>
    private void Awake(){


        weaponName = transform.Find("WeaponName").gameObject;
        weaponImage = transform.Find("WeaponImage").gameObject;
        fireModeText = transform.Find("FireModeText").gameObject;
        magazineBar = transform.Find("MagazineBar").gameObject;

        magazineBarFull = magazineBar.transform.Find("MagazineBarFull").gameObject;
        magazineBarEmpty = magazineBar.transform.Find("MagazineBarEmpty").gameObject;

        chamberFull = magazineBarFull.transform.Find("ChamberFull").gameObject;
        chamberEmpty = magazineBarEmpty.transform.Find("ChamberEmpty").gameObject;

        ammoCountText = chamberEmpty.transform.Find("AmmoCountText").gameObject;
    }

    /// <summary>
    /// Activates the weapon section of the HUD
    /// </summary>
    public void ActivateWeaponSection(){
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates the weapon section of the HUD
    /// </summary>
    public void DeactivateWeaponSection(){
        gameObject.SetActive(false);
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
    


}
