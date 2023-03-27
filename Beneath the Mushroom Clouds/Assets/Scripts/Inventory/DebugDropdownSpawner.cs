using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Implements the behaviour of the dropdown menu used to dpawn items into the player's inventory in debug.
/// </summary>
public class DebugDropdownSpawner : MonoBehaviour
{

    /// <summary>
    /// Reference to the dropdown menu component.
    /// </summary>
    private TMP_Dropdown dropdown;
    /// <summary>
    /// Reference to the Inventory Screen, which contains the Inventory Controller script.
    /// </summary>
    [SerializeField] private GameObject inventoryScreen;

    /// <summary>
    /// Reference to the Inventory Controller script.
    /// </summary>
    private InventoryController inventoryController;
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        inventoryController = inventoryScreen.GetComponent<InventoryController>();
        dropdown.onValueChanged.AddListener(delegate {
            OnValueChanged();
        });
    }

    //NOTE: Since item is spawned on changing a value, the item will not spawn when clicking the value that is already selected
    /// <summary>
    /// Calls a method of the Inventory Controller script to spawn an item into the player's inventory when the vaue of the dropdown menu is changed.
    /// </summary>
    void OnValueChanged(){
        inventoryController.DropdownSpawnItem(dropdown.value);
    }
}
