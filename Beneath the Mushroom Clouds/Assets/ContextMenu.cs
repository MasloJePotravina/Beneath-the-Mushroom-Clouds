using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public InventoryItem item;
    public ItemGrid selectedGrid;
    public ItemSlot selectedSlot;
    public InventoryController inventoryController;
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

    private Dictionary<string,GameObject> buttons = new Dictionary<string, GameObject>();

   


    public void Init(InventoryItem item, ItemGrid selectedGrid, ItemSlot selectedSlot, InventoryController inventoryController){
        this.item = item;
        this.selectedGrid = selectedGrid;
        this.selectedSlot = selectedSlot;
        this.inventoryController = inventoryController;
        GetButtons();
        MenuSetup();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.mouseOverContextMenu = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.mouseOverContextMenu = false;
    }

    private void GetButtons(){
        //for each key in menu options find child
        foreach(KeyValuePair<string, bool> option in menuOptions){
            buttons[option.Key] = transform.Find(option.Key + "Button").gameObject;
        }
    }


    public void MenuSetup(){
        
        if(!item.infoOpened){
            menuOptions["Info"] = true;
        } else {
            menuOptions["Info"] = false;
        }

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

        StartCoroutine(AdjustPosition());
    }

    private void SetupMenuOptions(){
        foreach (KeyValuePair<string, bool> option in menuOptions)
        {
            buttons[option.Key].SetActive(option.Value);
        }


    }

    //Coroutine adjust the position of the menu if it goes off screen
    //It has to be a coroutine because the height and width of the menu are not set until the next frame (layout group)
    IEnumerator AdjustPosition(){
        yield return new WaitForSeconds(0);
        RectTransform rectTransform = GetComponent<RectTransform>();
        float newXPos = rectTransform.position.x;
        float newYPos = rectTransform.position.y;
        if(rectTransform.position.x + rectTransform.rect.width > Screen.width){
            newXPos = Screen.width - rectTransform.rect.width;
        }
        if(rectTransform.position.y - rectTransform.rect.height < 0){
            
            newYPos = rectTransform.rect.height;
        }

        rectTransform.position = new Vector3(newXPos, newYPos, rectTransform.position.z);

    }

    public void InfoButton()
    {
        inventoryController.OpenItemInfoWindow(item);
        inventoryController.CloseContextMenu();
    }

    public void EquipButton()
    {
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
        inventoryController.OpenContainerItemWindow(item);
        inventoryController.CloseContextMenu();
    }

    public void CloseButton()
    {
        inventoryController.CloseContainerItemWindow(item);
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
       inventoryController.DestroyItem(item);
    }

}
