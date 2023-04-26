using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Implements the behaviour of a context menu that appears after right clicking an item in the inventory.
/// </summary>
public class ContextMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Reference to the item that the was created for
    /// </summary>
    public InventoryItem item;

    /// <summary>
    /// Referecne to the potential grid that the item is in.
    /// </summary>
    public ItemGrid grid;
    /// <summary>
    /// Reference to the potential slot that the item is in
    /// </summary>
    public ItemSlot slot;

    /// <summary>
    /// Reference to the inventory controller
    /// </summary>
    public InventoryController inventoryController;

    /// <summary>
    /// Dictionary of menu options and whether they are active or not, by default only the destroy option is active since it's always available
    /// </summary>
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
        {"OpenBolt", false},
        {"CloseBolt", false},
        {"UnloadAmmo", false},
        {"LoadAmmo", false},
        {"SplitStack", false},
        {"Destroy", true}
    };

    /// <summary>
    /// Dictionary of names of the buttons and references to them
    /// </summary>
    private Dictionary<string,GameObject> buttons = new Dictionary<string, GameObject>();

    /// <summary>
    /// Initializes the context menu
    /// </summary>
    /// <param name="item">Reference to the item that the menu was created for</param>
    /// <param name="grid">Reference to the grid the item potentially belongs to </param>
    /// <param name="slot">Reference to the equipment slot slot the item potentially belongs to</param>
    /// <param name="inventoryController">Reference to the inventory controller</param>
    public void Init(InventoryItem item, ItemGrid grid, ItemSlot slot, InventoryController inventoryController){
        this.item = item;
        this.grid = grid;
        this.slot = slot;
        this.inventoryController = inventoryController;
        GetButtons();
        MenuSetup();
    }


    /// <summary>
    /// Notifies the inventory controller when the mouse enters the context menu
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.mouseOverContextMenu = true;
    }

    /// <summary>
    /// Notifies the inventory controller when the mouse leaves the context menu
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.mouseOverContextMenu = false;
    }

    /// <summary>
    /// Gets references to the context menu buttons using the menuOptions dictionary
    /// </summary>
    private void GetButtons(){
        //for each key in menu options find child
        foreach(KeyValuePair<string, bool> option in menuOptions){
            buttons[option.Key] = transform.Find(option.Key + "Button").gameObject;
        }
    }

    /// <summary>
    /// Sets up the menu options based on itemData od the item and on the current state of the item.
    /// </summary>
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

        if(item.itemData.consumable){
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

            if(item.boltOpen){
                menuOptions["CloseBolt"] = true;
                if(item.isChambered){
                    menuOptions["ClearChamber"] = true;
                } else {
                    menuOptions["ChamberRound"] = true;
                }

            }else{
                menuOptions["OpenBolt"] = true;
                if(item.ammoCount > 0){
                    menuOptions["RackFirearm"] = true;
                }
            }
        }


        SetupMenuOptions();
        StartCoroutine(AdjustPosition());
    }

    /// <summary>
    /// Activates the buttons based on the menuOptions dictionary
    /// </summary>
    private void SetupMenuOptions(){
        foreach (KeyValuePair<string, bool> option in menuOptions)
        {
            buttons[option.Key].SetActive(option.Value);
        }


    }

    /// <summary>
    /// Coroutine that adjusts the position of the context menu if it goes off screen. 
    /// It needs to be a coroutine because the height and width of the menu are not set until the next frame (layout group).
    /// </summary>
    /// <returns>A reference to the running coroutine.</returns>
    private IEnumerator AdjustPosition(){
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

    /// <summary>
    /// Method called by the Info button. Calls a method of the Inventory Controller to open the item info window.
    /// </summary>
    public void InfoButton()
    {
        inventoryController.OpenItemInfoWindow(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Equip button. Calls a method of the Inventory Controller to equip the item.
    /// </summary>
    public void EquipButton()
    {
        inventoryController.QuickEquip(item, grid);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Unequip button. Calls a method of the Inventory Controller to unequip the item.
    /// </summary>
    public void UnequipButton()
    {
        inventoryController.QuickTransfer(item, grid, slot);
        inventoryController.CloseContextMenu();
    }
    
    /// <summary>
    /// Method called by the Open button. Calls a method of the Inventory Controller to open a container window for the item.
    /// </summary>
    public void OpenButton()
    {
        inventoryController.OpenContainerItemWindow(item);
        inventoryController.CloseContextMenu();
    }


    /// <summary>
    /// Method called by the Close button. Calls a method of the Inventory Controller to close a container window for the item.
    /// </summary>
    public void CloseButton()
    {
        inventoryController.CloseContainerItemWindow(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the SplitStack button. Currently not implemented.
    /// </summary>
    public void UseButton()
    {
        inventoryController.UseItem(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Attach Magazine button. Calls a method of the Inventory Controller to attach a magazine to the item (firearm).
    /// </summary>
    public void AttachMagazineButton()
    {
        inventoryController.AttachMagazine(item, false);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Remove Magazine button. Calls a method of the Inventory Controller to remove a magazine from the item (firearm).
    /// </summary>
    public void RemoveMagaizneButton()
    {
        inventoryController.RemoveMagazine(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Chamber Round button. Calls a method of the Inventory Controller to chamber a round into the item (firearm).
    /// </summary>
    public void ChamberRoundButton()
    {
        inventoryController.ChamberRound(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Clear Chamber button. Calls a method of the Inventory Controller to clear the chamber of the item (firearm).
    /// </summary>
    public void ClearChamberButton()
    {
        inventoryController.ClearChamber(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Rack Firearm button. Calls a method of the Inventory Controller to rack the item (firearm).
    /// </summary>
    public void RackFirearmButton()
    {
        inventoryController.RackFirearm(item);
        inventoryController.CloseContextMenu();
    }

    public void OpenBoltButton()
    {
        inventoryController.OpenBolt(item);
        inventoryController.CloseContextMenu();
    }

    public void CloseBoltButton()
    {
        inventoryController.CloseBolt(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Load Ammo button. Calls a method of the Inventory Controller to load ammo into the item (firearm or magazine).
    /// </summary>
    public void LoadAmmoButton()
    {
        if(item.itemData.firearm){
            inventoryController.LoadAmmoIntoFirearm(item);
        }else{
            inventoryController.LoadAmmoIntoMagazine(item);
        }
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Unload Ammo button. Calls a method of the Inventory Controller to unload ammo from the item (firearm or magazine).
    /// </summary>
    public void UnloadAmmoButton()
    {
        if(item.itemData.firearm){
            inventoryController.UnloadAmmoFromFirearm(item);
        }else{
            inventoryController.UnloadAmmoFromMagazine(item);
        }
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Split Stack button. Calls a method of the Inventory Controller to split the stack of the item.
    /// </summary>
    public void SplitStackButton()
    {
        inventoryController.SplitStack(item);
        inventoryController.CloseContextMenu();
    }

    /// <summary>
    /// Method called by the Destroy button. Calls a method of the Inventory Controller to destroy the item.
    /// </summary>
    public void DestroyButton()
    {
       inventoryController.DestroyItem(item);
    }

}
