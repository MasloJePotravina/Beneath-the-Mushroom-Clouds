using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public InventoryItem[][,] itemGrids;

    public int gridPositionX;
    public int gridPositionY;

    public int Height{
        get{
            return rotated ? itemData.width : itemData.height;
        }
    }

    public int Width{
        get{
            return rotated ? itemData.height : itemData.width;
        }
    }

    public bool rotated = false;

    public void Set(ItemData itemData){
        this.itemData = itemData;

        if(this.itemData.container){
            itemGrids = new InventoryItem[this.itemData.gridAmount][,];

            for(int i = 0; i < this.itemData.gridAmount; i++){
                itemGrids[i] = new InventoryItem[this.itemData.gridData[i].width, this.itemData.gridData[i].height];
            }
        }



        GetComponent<Image>().sprite = itemData.inventorySprite;

        float tileDimension = ItemGrid.tileDimension;
        if(Screen.width != 1920 || Screen.height != 1080){
            tileDimension = ItemGrid.tileDimension * (Screen.width/1920f);
        }

        Vector2 size = new Vector2(itemData.width * tileDimension, itemData.height * tileDimension);
        GetComponent<RectTransform>().sizeDelta = size;

    }

    public InventoryItem[,] LoadGrid(int gridID){
        


        return itemGrids[gridID];

        
    }

    public void SaveGrid(int gridID, InventoryItem[,] items){


        itemGrids[gridID] = items;

        foreach(InventoryItem item in itemGrids[gridID]){
            if(item != null){
                item.transform.SetParent(transform);
                item.gameObject.SetActive(false);
            }
        }
        
    }

    public void Rotate(){
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.rotation = Quaternion.Euler(0, 0, rotated ? 90 : 0);
    }


}
