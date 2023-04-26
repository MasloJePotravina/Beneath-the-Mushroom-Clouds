using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTabButtons : MonoBehaviour
{
    private GameObject equipmentToggle;
    private GameObject healthToggle;
    private GameObject mapToggle;
    private GameObject journalToggle;

    private GameObject equippedSection;
    private GameObject healthStatusSection;
    private GameObject inventorySection;
    private GameObject containerSection;


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

    public void EquipmentTab(){
        equippedSection.SetActive(true);
        healthStatusSection.SetActive(false);
    }

    public void HealthTab(){
        equippedSection.SetActive(false);
        healthStatusSection.SetActive(true);
    }

}
