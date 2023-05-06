using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implements the behaviour of the upper tab buttons in the inventory screen.
/// </summary>
public class InventoryTabButtons : MonoBehaviour
{
    /// <summary>
    /// Reference to the "Equipment" tab button (disguised toggle).
    /// </summary>
    private GameObject equipmentToggle;
    /// <summary>
    /// Reference to the "Health" tab button (disguised toggle).
    /// </summary>
    private GameObject healthToggle;
    /// <summary>
    /// Reference to the "Map" tab button (disguised toggle).
    /// </summary>
    private GameObject mapToggle;
    /// <summary>
    /// Reference to the "Journal" tab button (disguised toggle).
    /// </summary>
    private GameObject journalToggle;

    /// <summary>
    /// Reference to the "Equipped" section (left side of screen) in "Equipment" tab.
    /// </summary>
    private GameObject equippedSection;

    /// <summary>
    /// Reference to the "Health Status" section (left side of screen) in "Health" tab.
    /// </summary>
    private GameObject healthStatusSection;

    /// <summary>
    /// Reference to the "Inventory" section (middle of the screen) in "Equipment" and "Health" tabs.
    /// </summary>
    private GameObject inventorySection;

    /// <summary>
    /// Reference to the "Container" section (right side of screen) in "Equipment" and "Health" tabs.
    /// </summary>
    private GameObject containerSection;


    /// <summary>
    /// Get all necessary references on awake.
    /// </summary>
    void Awake(){
        equipmentToggle = transform.Find("EquipmentToggle").gameObject;
        healthToggle = transform.Find("HealthToggle").gameObject;
        mapToggle = transform.Find("MapToggle").gameObject;
        journalToggle = transform.Find("JournalToggle").gameObject;

        GameObject tab = transform.parent.Find("Tab").gameObject;

        equippedSection = tab.transform.Find("EquippedSection").gameObject;
        healthStatusSection = tab.transform.Find("HealthStatusSection").gameObject;
        inventorySection = tab.transform.Find("InventorySection").gameObject;
        containerSection = tab.transform.Find("ContainerSection").gameObject;

        equipmentToggle.GetComponent<Toggle>().Select();
    }

    /// <summary>
    /// Switches to the "Equipment" tab.
    /// </summary>
    public void EquipmentTab(){
        equippedSection.SetActive(true);
        healthStatusSection.SetActive(false);
    }

    /// <summary>
    /// Switches to the "Health" tab.
    /// </summary>
    public void HealthTab(){
        equippedSection.SetActive(false);
        healthStatusSection.SetActive(true);
    }

}
