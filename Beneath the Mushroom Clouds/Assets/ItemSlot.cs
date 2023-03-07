using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{

    InventoryItem item;
    RectTransform rectTransform;
    Image image;

    float slotWidth;
    float slotHeight;
    float slotRatio;

    public int equipmentType;


    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        slotWidth = rectTransform.sizeDelta.x - 0.20f * rectTransform.sizeDelta.x;
        slotHeight = rectTransform.sizeDelta.y - 0.20f * rectTransform.sizeDelta.y;
        slotRatio = slotWidth / slotHeight;
    }


    public bool PlaceItem(InventoryItem placedItem){
        if(item != null){
            if(item.itemData.firearm){
                if(placedItem.itemData.magazine){
                    if(item.itemData.weaponType == placedItem.itemData.weaponType){
                        item.AttachMagazine(placedItem);
                        Destroy(placedItem.gameObject);
                        return true;
                    }
                }
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
            return false;
        }

        if(placedItem.itemData.equipmentType != equipmentType){
            return false;
        }
        
        item = placedItem;
        RectTransform itemRectTransform = placedItem.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);
        itemRectTransform.SetAsFirstSibling();
        itemRectTransform.localPosition = Vector2.zero;

        PlaceResizeItem(placedItem);
        

        return true;
    }

    public InventoryItem GrabItem(){
        InventoryItem item = this.item;
        this.item = null;

        GrabResizeItem(item);

        return item;
    }

    public InventoryItem GetItem(){
        return item;
    }

    public void DeleteItem(){
        Destroy(item.gameObject);
        item = null;
    }

    private void PlaceResizeItem(InventoryItem item){
        if (item.rotated){
            item.Rotate();
        }
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        float ratio = (float)item.Width / item.Height;

        if(ratio > slotRatio){
            itemRectTransform.sizeDelta = new Vector2(slotWidth, slotWidth / ratio);
        } else {
            itemRectTransform.sizeDelta = new Vector2(slotHeight * ratio, slotHeight);
        }

        //Scale to 1
        itemRectTransform.localScale = Vector3.one;

    }

    private void GrabResizeItem(InventoryItem item){
        if(item == null){
            return;
        }
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.sizeDelta = new Vector2(item.Width * ItemGrid.tileDimension, item.Height * ItemGrid.tileDimension);
    }

}
