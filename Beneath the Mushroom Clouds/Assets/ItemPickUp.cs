using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private List<GameObject> itemObjects = new List<GameObject>();

    private int groundGridWidth = 9;
    private int groundGridHeight = 14;


    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("LooseItem")){
            itemObjects.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("LooseItem")){
            itemObjects.Remove(other.gameObject);
        }
    }

    public InventoryItem[,] CreateGroundGrid(){
        InventoryItem[,] groundGrid = new InventoryItem[groundGridWidth,groundGridHeight];
        Vector2Int lastPosition = new Vector2Int(0,0);

        foreach(GameObject itemObject in itemObjects){
            if(lastPosition.x >= groundGridWidth - 1){
                lastPosition = new Vector2Int(0, lastPosition.y + 1);
            }
            InventoryItem item = itemObject.GetComponent<LooseItem>().item;
            if(item.rotated){
                item.Rotate();
            }
            lastPosition = FindSpaceForItem(item, lastPosition, groundGrid);
        }

        return groundGrid;
    }

    private Vector2Int FindSpaceForItem(InventoryItem item, Vector2Int lastPosition, InventoryItem[,] groundGrid){
        for(int i = lastPosition.y; i < groundGridHeight; i++){
            for(int j = lastPosition.x; j < groundGridWidth; j++){
                if(BoundaryCheck(j + item.itemData.width - 1, i + item.itemData.height - 1)
                    && OverlapCheck(j, i, item.itemData.width, groundGrid))
                    {
                    item.gridPositionX = j;
                    item.gridPositionY = i;
                    for(int k = 0; k < item.itemData.width; k++){
                        for(int l = 0; l < item.itemData.height; l++){
                            groundGrid[j + k, i + l] = item;
                        }
                    }
                    lastPosition = new Vector2Int(j, i);
                    return lastPosition;
                }
            }
        }
        return lastPosition;
    }



    private bool BoundaryCheck(int x, int y){
        if(x >= groundGridWidth || y >= groundGridHeight){
            return false;
        }
        return true;
    }

    //We only need width here since item are placed row by row
    private bool OverlapCheck(int x, int y, int width, InventoryItem[,] itemObjects){
        for(int i = 0; i < width; i++){
            if(itemObjects[x + i, y] != null){
                return false;
            }
        }
        return true;
    }

    public void DestroyItemObject(InventoryItem item){
        foreach(GameObject itemObject in itemObjects){
            if(itemObject.GetComponent<LooseItem>().item == item){
                itemObjects.Remove(itemObject);
                Destroy(itemObject);
                return;
            }
        }
    }
}
