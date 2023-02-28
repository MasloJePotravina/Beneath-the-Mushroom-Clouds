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
        {"UnloadAmmo", false},
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
        if(item.isEquipped){
            menuOptions["Unequip"] = true;
        } else {
            menuOptions["Equip"] = true;
        }

        if(item.itemData.container && !item.isEquipped && !item.isOpened){
            menuOptions["Open"] = true;
        }

        if(item.itemData.container && !item.isEquipped && item.isOpened){
            menuOptions["Close"] = true;
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
        Debug.Log("Use Button");
        inventoryController.CloseContextMenu();
    }

    public void RemoveMagaizneButton()
    {
        Debug.Log("Use Button");
        inventoryController.CloseContextMenu();
    }

    public void ChamberRoundButton()
    {
        Debug.Log("Use Button");
        inventoryController.CloseContextMenu();
    }

    public void ClearChamberButton()
    {
        Debug.Log("Use Button");
        inventoryController.CloseContextMenu();
    }

    public void UnloadAmmoButton()
    {
        Debug.Log("Use Button");
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
            selectedSlot.DeleteItem();

        }
        Destroy(item.gameObject);
        if(item.isOpened){
            inventoryController.CloseInventoryContainerWindow(item);
        }
        inventoryController.CloseContextMenu();
    }

}
