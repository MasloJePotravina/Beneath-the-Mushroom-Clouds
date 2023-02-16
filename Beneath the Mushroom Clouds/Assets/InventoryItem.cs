using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int gridPositionX;
    public int gridPositionY;


    public void Set(ItemData itemData){
        this.itemData = itemData;
        GetComponent<Image>().sprite = itemData.sprite;

        float tileDimension = ItemGrid.tileDimension;
        if(Screen.width != 1920 || Screen.height != 1080){
            tileDimension = ItemGrid.tileDimension * (Screen.width/1920f);
        }

        Vector2 size = new Vector2(itemData.width * tileDimension, itemData.height * tileDimension);
        GetComponent<RectTransform>().sizeDelta = size;

    }


}
