using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Implements the behavior for the UI control scheme. ontains methods to which the messages from the InputSystem are sent and redirects those messages to other relevant scripts.
/// </summary>
public class UIControls : MonoBehaviour
{

    /// <summary>
    /// Reference to the PlayerInput component of the player.
    /// </summary>
    private PlayerInput playerInput;

    /// <summary>
    /// Reference to the Inventory Screen GameObject, which contains the Inventory Controller.
    /// </summary>
    [SerializeField] GameObject inventoryScreen;

    /// <summary>
    /// Reference to the Inventory Controller.
    /// </summary>
    private InventoryController inventoryController;

    /// <summary>
    /// Reference to the Player Animation Controller.
    /// </summary>
    private PlayerAnimationController playerAnimationController;

    /// <summary>
    /// Reference to the Firearm Script.
    /// </summary>
    private FirearmScript firearmScript;



    /// <summary>
    /// On Awake, initializes all needed references.
    /// </summary>
    void Awake(){
        playerInput = GetComponent<PlayerInput>();
        inventoryController = inventoryScreen.GetComponent<InventoryController>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        firearmScript = transform.Find("PlayerFirearm").GetComponent<FirearmScript>();
    }

    /// <summary>
    /// Method called by the InputSystem when the player attempts to open the inventory.
    /// </summary>
    public void OnOpenInventory()
    {
        playerInput.SwitchCurrentActionMap("UI");
        inventoryController.OpenInventory();
        firearmScript.InventoryOpened();
        playerAnimationController.DisableMovementBools();
             
    }

    /// <summary>
    /// Method called by the InputSystem when the player attempts to close the inventory.
    /// </summary>
    void OnCloseInventory()
    {
        playerInput.SwitchCurrentActionMap("Player");
        inventoryController.CloseInventory();
        playerAnimationController.ResetQuickWeaponChangeTriggers();
    }

    /// <summary>
    /// Method called by the InputSystem when the player left clicks within the inventory.
    /// </summary>
    /// <param name="value">Input Value sent by the Input System</param>
    void OnLeftClick(InputValue value)
    {
        inventoryController.InventoryLeftClick(value);
        
    }

    /// <summary>
    /// Method called by the InputSystem when the player presses the quick spawn item button in debug.
    /// </summary>
    void OnDebugSpawnItem()
    {
        inventoryController.QuickSpawnItem();
    }

    /// <summary>
    /// Method called by the InputSystem when the player presses rotate item hotkey.
    /// </summary>
    void OnRotateItem(){
        inventoryController.RotateItem();
    }

    /// <summary>
    /// Method called by the InputSystem when the player presses the split stack hotkey.
    /// </summary>
    /// <param name="value">Input Value sent by the Input System</param>
    void OnSplitStack(InputValue value){
        inventoryController.SplitStackHotkey(value);
    }

    /// <summary>
    /// Method called by the InputSystem when the player presses right mouse button within the inventory.
    /// </summary>
    void OnRightClick(){
        inventoryController.RightClick();
    }

    /// <summary>
    /// Method called by the InputSystem when the player presses the quick transfer hotkey.
    /// </summary>
    /// <param name="value">Input Value sent by the Input System</param>
    void OnQuickTransfer(InputValue value){
        inventoryController.QuickTransferHotkey(value);
    }

    /// <summary>
    /// Method called by the InputSystem when the player presses the quick equip hotkey.
    /// </summary>
    /// <param name="value">Input Value sent by the Input System</param>
    void OnQuickEquip(InputValue value){
        inventoryController.QuickEquipHotkey(value);
    }

}
