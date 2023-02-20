using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    ItemSlot itemSlot;

    private void Awake()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        itemSlot = GetComponent<ItemSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedSlot = itemSlot;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedSlot = null;
    }
}
