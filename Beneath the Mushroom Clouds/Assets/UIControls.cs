using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControls : MonoBehaviour
{

    private PlayerInput playerInput;
    [SerializeField] GameObject inventoryScreen;
    private InventoryController inventoryController;
    [SerializeField] GameObject mainCamera;

    void Awake(){
        playerInput = GetComponent<PlayerInput>();
        inventoryController = mainCamera.GetComponent<InventoryController>();
    }
    void OnOpenInventory()
    {
        
        
        playerInput.SwitchCurrentActionMap("UI");
        inventoryScreen.SetActive(true);
        mainCamera.GetComponent<InventoryController>().inventoryOpen = true;

        
    }

    void OnCloseInventory()
    {
        playerInput.SwitchCurrentActionMap("Player");
        inventoryScreen.SetActive(false);
        mainCamera.GetComponent<InventoryController>().inventoryOpen = false;
    }

    //UI action map funtion
    void OnLeftClick(InputValue value)
    {
        inventoryController.InventoryLeftClick(value);
        
    }

    //TODO: Debug only, remove later
    void OnDebugSpawnItem()
    {
        inventoryController.QuickSpawnItem();
    }

    void OnRotateItem(){
        inventoryController.RotateItem();
    }

    void OnSplitStack(InputValue value){
        inventoryController.SplitStackHotkey(value);
    }

    void OnRightClick(){
        inventoryController.RightClick();
    }

    void OnQuickTransfer(InputValue value){
        inventoryController.QuickTransferHotkey(value);
    }

    void OnQuickEquip(InputValue value){
        inventoryController.QuickEquipHotkey(value);
    }
}
