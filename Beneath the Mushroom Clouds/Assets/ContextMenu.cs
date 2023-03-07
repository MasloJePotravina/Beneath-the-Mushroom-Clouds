using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public InventoryItem item;
    public ItemGrid selectedGrid;
    public ItemSlot selectedSlot;
    InventoryController inventoryController;
    private Dictionary<string, bool> menuOptions = new Dictionary<string, bool>{
        {"Info", false},
        {"Equip", false},
        {"Unequip", false},
        {"Open", false},
        {"Close", false},
        {"Use", false},
        {"AttachMagazine", false},
        {"RemoveMagazine", false},
        {"ChamberRound", false},
        {"ClearChamber", false},
        {"RackFirearm", false},
        {"UnloadAmmo", false},
        {"LoadAmmo", false},
        {"SplitStack", false},
        {"Destroy", true}
    };

    private void Awake()
    {
        inventoryController = FindObjectOfType<InventoryController>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.mouseOverContextMenu = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.mouseOverContextMenu = false;
    }

    public void MenuSetup(){
        if(item.itemData.equipment){
            if(item.isEquipped){
                menuOptions["Unequip"] = true;
            } else {
                menuOptions["Equip"] = true;
            }
        }

        if(item.itemData.container){
            if(!item.isEquipped){
                if(!item.isOpened){
                    menuOptions["Open"] = true;
                } else {
                    menuOptions["Close"] = true;
                }
            }
        }
        
        if(item.itemData.stackable){
            if(item.currentStack > 1)
                menuOptions["SplitStack"] = true;
        }

        if(item.itemData.usable){
            menuOptions["Use"] = true;
        }

        if(item.itemData.magazine){
            if(item.ammoCount > 0)
                menuOptions["UnloadAmmo"] = true;
            if(item.ammoCount < item.itemData.magazineSize)
                menuOptions["LoadAmmo"] = true;
        }

        if(item.itemData.firearm){
            if(item.itemData.usesMagazines){
                if(item.hasMagazine){
                    menuOptions["RemoveMagazine"] = true;
                } else {
                    menuOptions["AttachMagazine"] = true;
                }
            }else{
                if(item.ammoCount > 0)
                    menuOptions["UnloadAmmo"] = true;
                if(item.ammoCount < item.itemData.internalMagSize)
                    menuOptions["LoadAmmo"] = true;
            }

            if(item.isChambered){
                menuOptions["ClearChamber"] = true;
            } else {
                menuOptions["ChamberRound"] = true;
                if(item.ammoCount > 0){
                    menuOptions["RackFirearm"] = true;
                }
            }
        }

        SetupMenuOptions();
    }

    private void SetupMenuOptions(){
        foreach (KeyValuePair<string, bool> option in menuOptions)
        {
            GameObject.Find(option.Key + "Button").SetActive(option.Value);
        }
    }

    public void InfoButton()
    {
        Debug.Log("Info Button");
        inventoryController.CloseContextMenu();
    }

    public void EquipButton()
    {
        Debug.Log("Equip Button");
        inventoryController.QuickEquip(item, selectedGrid);
        inventoryController.CloseContextMenu();
    }

    public void UnequipButton()
    {
        inventoryController.QuickTransfer(item, selectedGrid, selectedSlot);
        inventoryController.CloseContextMenu();
    }
    
    public void OpenButton()
    {
        inventoryController.OpenInventoryContainerWindow(item);
        inventoryController.CloseContextMenu();
    }

    public void CloseButton()
    {
        inventoryController.CloseInventoryContainerWindow(item);
        inventoryController.CloseContextMenu();
    }

    public void UseButton()
    {
        Debug.Log("Use Button");
        inventoryController.CloseContextMenu();
    }

    public void AttachMagazineButton()
    {
        inventoryController.AttachMagazine(item, false);
        inventoryController.CloseContextMenu();
    }

    public void RemoveMagaizneButton()
    {
        inventoryController.RemoveMagazine(item);
        inventoryController.CloseContextMenu();
    }

    public void ChamberRoundButton()
    {
        inventoryController.ChamberRound(item);
        inventoryController.CloseContextMenu();
    }

    public void ClearChamberButton()
    {
        inventoryController.ClearChamber(item);
        inventoryController.CloseContextMenu();
    }

    public void RackFirearmButton()
    {
        inventoryController.RackFirearm(item);
        inventoryController.CloseContextMenu();
    }

    public void LoadAmmoButton()
    {
        if(item.itemData.firearm){
            inventoryController.LoadAmmoIntoFirearm(item);
        }else{
            inventoryController.LoadAmmoIntoMagazine(item);
        }
        inventoryController.CloseContextMenu();
    }

    public void UnloadAmmoButton()
    {
        if(item.itemData.firearm){
            inventoryController.UnloadAmmoFromFirearm(item);
        }else{
            inventoryController.UnloadAmmoFromMagazine(item);
        }
        inventoryController.CloseContextMenu();
    }

    public void SplitStackButton()
    {
        inventoryController.SplitStack(item);
        inventoryController.CloseContextMenu();
    }

    public void DestroyButton()
    {
        if(item.isEquipped){
            inventoryController.ToggleContainerGrid(item, false);
            inventoryController.RemoveOutlineSprite(item);
            bool wasWeapon = item.itemData.weapon;
            if(wasWeapon){
                inventoryController.WeaponSelectUpdate();
            }

        }
        Destroy(item.gameObject);
        if(item.isOpened){
            inventoryController.CloseInventoryContainerWindow(item);
        }
        inventoryController.CloseContextMenu();
    }

}
