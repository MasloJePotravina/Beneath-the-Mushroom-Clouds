using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{


    InventoryItem item;
    RectTransform rectTransform;

    float slotWidth;
    float slotHeight;
    float slotRatio;


    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        slotWidth = rectTransform.sizeDelta.x - 0.20f * rectTransform.sizeDelta.x;
        slotHeight = rectTransform.sizeDelta.y - 0.20f * rectTransform.sizeDelta.y;
        slotRatio = slotWidth / slotHeight;
    }


    public bool PlaceItem(InventoryItem item){
        if(this.item != null){
            return false;
        }
        this.item = item;
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);
        itemRectTransform.SetAsFirstSibling();
        itemRectTransform.localPosition = Vector2.zero;

        PlaceResizeItem(item);

        return true;
    }

    public InventoryItem GrabItem(){
        InventoryItem item = this.item;
        this.item = null;

        GrabResizeItem(item);

        return item;
    }

    private void PlaceResizeItem(InventoryItem item){
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
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.sizeDelta = new Vector2(item.Width * ItemGrid.tileDimension, item.Height * ItemGrid.tileDimension);
    }
}
