using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{


    InventoryItem item;
    RectTransform rectTransform;
    Image image;

    float slotWidth;
    float slotHeight;
    float slotRatio;

    [SerializeField] private int equipmentType;


    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        slotWidth = rectTransform.sizeDelta.x - 0.20f * rectTransform.sizeDelta.x;
        slotHeight = rectTransform.sizeDelta.y - 0.20f * rectTransform.sizeDelta.y;
        slotRatio = slotWidth / slotHeight;
    }


    public bool PlaceItem(InventoryItem item){
        if(this.item != null){
            return false;
        }

        if(item.itemData.equipmentType != equipmentType){
            if(item.itemData.equipmentType != 12 || equipmentType != 11)
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
        if(item == null){
            return;
        }
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.sizeDelta = new Vector2(item.Width * ItemGrid.tileDimension, item.Height * ItemGrid.tileDimension);
    }
}
