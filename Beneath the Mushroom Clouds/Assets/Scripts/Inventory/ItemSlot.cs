using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
/// <summary>
/// Implements the behaviour of equipment slots.
/// </summary>
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    /// <summary>
    /// Reference to the item currently stored in the slot.
    /// </summary>
    private InventoryItem item;

    /// <summary>
    /// Reference to the Inventory Screen, which contains the Inventory Controller script.
    /// </summary>
    private GameObject inventoryScreen;

    /// <summary>
    /// Reference to the Inventory Controller.
    /// </summary>
    private InventoryController inventoryController;

    /// <summary>
    /// Reference to the RectTransform component of the slot.
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// Reference to the Image component of the slot.
    /// </summary>
    private Image image;

    /// <summary>
    /// Width of the slot.
    /// </summary>
    float slotWidth;

    /// <summary>
    /// Height of the slot.
    /// </summary>
    float slotHeight;

    /// <summary>
    /// Ratio of the slot's width to its height.
    /// </summary>
    float slotRatio;

    /// <summary>
    /// Type of the equipment that can be stored in the slot.
    /// </summary>
    public int equipmentType;

    /// <summary>
    /// Detects when the mouse enters a slot and sets the slot as the selected slot in the Inventory Controller.
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedSlot = this;
    }

    /// <summary>
    /// Detects when the mouse leaves a slot and sets the selected slot in the Inventory Controller to null.
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedSlot = null;
    }

    /// <summary>
    /// On awake initializes the slot's width, height, ratio and references to the RectTransform and Image components.
    /// </summary>
    private void Awake() {
        inventoryScreen = GameObject.Find("InventoryScreen");
        inventoryController = inventoryScreen.GetComponent<InventoryController>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        slotWidth = rectTransform.sizeDelta.x - 0.20f * rectTransform.sizeDelta.x;
        slotHeight = rectTransform.sizeDelta.y - 0.20f * rectTransform.sizeDelta.y;
        slotRatio = slotWidth / slotHeight;
    }

    /// <summary>
    /// Places an item into the slot if possible. The item is resized to fit the slot.
    /// </summary>
    /// <param name="placedItem"></param>
    /// <returns></returns>
    public bool PlaceItem(InventoryItem placedItem){
        //If there is already an item in the slot, check for special cases.
        if(item != null){
            //Inserting magazine into a firearm
            if(item.itemData.firearm){
                if(placedItem.itemData.magazine){
                    if(item.itemData.weaponType == placedItem.itemData.weaponType){
                        if(item.AttachMagazine(placedItem)){
                            Destroy(placedItem.gameObject);
                            return true;
                        }else{
                            return false;
                        }
                    }
                }
                //Inserting ammo into a firearm
                if(placedItem.itemData.ammo){
                    if(item.itemData.weaponType == placedItem.itemData.weaponType){
                        if(!item.itemData.usesMagazines){
                            if(item.ChamberRound()){
                                if(placedItem.RemoveFromStack(1) == 0){
                                    return true; 
                                }
                            }
                            int amountTransfered = item.LoadAmmoIntoInternalMagazine(placedItem.currentStack);
                            if(placedItem.RemoveFromStack(amountTransfered) == 0){
                                return true;
                            }else{
                                return false;
                            }
                        }else{
                            if(item.ChamberRound()){
                                placedItem.RemoveFromStack(1);
                                return false;
                            }
                        }
                        
                    }
                }
            }
            //If no special cases are met, return false.
            return false;
        }

        //If the slot is empty, but the equipment type of the item does not match the slot's equipment type, return false.
        if(placedItem.itemData.equipmentType != equipmentType){
            return false;
        }
        
        //Place item into the slot
        item = placedItem;
        RectTransform itemRectTransform = placedItem.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);
        itemRectTransform.SetAsFirstSibling();
        itemRectTransform.localPosition = Vector2.zero;

        PlaceResizeItem(placedItem);
        

        return true;
    }

    /// <summary>
    /// Grabs an item from the slot. The item is resized back to its original size.
    /// </summary>
    /// <returns>Reference to the item that was grabbed from the slot.</returns>
    public InventoryItem GrabItem(){
        InventoryItem item = this.item;
        this.item = null;
        GrabResizeItem(item);
        return item;
    }

    /// <summary>
    /// Gets the current item in the slot.
    /// </summary>
    /// <returns>Returns the current item in the slot.</returns>
    public InventoryItem GetItem(){
        return item;
    }

    /// <summary>
    /// Deletes the item in the slot.
    /// </summary>
    public void DeleteItem(){
        Destroy(item.gameObject);
        item = null;
    }

    /// <summary>
    /// Resizes the item to fit the slot when placing it.
    /// </summary>
    /// <param name="item">Placed item.</param>
    private void PlaceResizeItem(InventoryItem item){
        //If the item is rotated, rotate it back to its original orientation.
        if (item.rotated){
            item.Rotate();
        }

        //Get the ration of the item's width to its height.
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        float ratio = (float)item.Width / item.Height;

        //If the ratio of the item is greater than the ratio of the slot, resize the item to fit the width of the slot.
        //Otherwise, resize the item to fit the height of the slot.
        if(ratio > slotRatio){
            itemRectTransform.sizeDelta = new Vector2(slotWidth, slotWidth / ratio);
        } else {
            itemRectTransform.sizeDelta = new Vector2(slotHeight * ratio, slotHeight);
        }

    }

    /// <summary>
    /// Resizes the item to its original size when grabbing it from the slot.
    /// </summary>
    /// <param name="item">Grabbed item.</param>
    private void GrabResizeItem(InventoryItem item){
        if(item == null){
            return;
        }
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.sizeDelta = new Vector2(item.Width * ItemGrid.tileDimension, item.Height * ItemGrid.tileDimension);
    }

}
