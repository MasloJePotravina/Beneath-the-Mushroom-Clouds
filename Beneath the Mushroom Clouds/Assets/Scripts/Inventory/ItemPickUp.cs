using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of picking up of loose items.
/// </summary>
public class ItemPickUp : MonoBehaviour
{
    /// <summary>
    /// List of the loose item objects in the player's interact range.
    /// </summary>
    /// <typeparam name="GameObject">References to the inedividual lose item objects.</typeparam>
    private List<GameObject> itemObjects = new List<GameObject>();

    /// <summary>
    /// Width of the ground grid.
    /// </summary>
    private int groundGridWidth = 9;

    /// <summary>
    /// Height of the ground grid.
    /// </summary>
    private int groundGridHeight = 14;

    /// <summary>
    /// When the player's interact range is triggered by a loose item, add it to the list of loose items.
    /// </summary>
    /// <param name="other">An object that triggered the interact range.</param>
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("LooseItem")){
            itemObjects.Add(other.gameObject);
        }
    }

    /// <summary>
    /// When the player's interact range is no longer triggered by a loose item, remove it from the list of loose items.
    /// </summary>
    /// <param name="other">Object that left the interact range.</param>
    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("LooseItem")){
            itemObjects.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// Creates the ground grid from all of the items in the interact range.
    /// </summary>
    /// <returns>2 dimensional array of item references (just like any other item grid holds within itself)</returns>
    public InventoryItem[,] CreateGroundGrid(){
        InventoryItem[,] groundGrid = new InventoryItem[groundGridWidth,groundGridHeight];
        Vector2Int lastPosition = new Vector2Int(0,0);

        foreach(GameObject itemObject in itemObjects){
            if(lastPosition.x >= groundGridWidth - 1){
                lastPosition = new Vector2Int(0, lastPosition.y + 1);
            }
            LooseItem looseItem = itemObject.GetComponent<LooseItem>();
            InventoryItem item = looseItem.item;

            if(item.rotated){
                item.Rotate();
            }
            lastPosition = FindSpaceForItem(item, lastPosition, groundGrid);
        }

        return groundGrid;
    }

    /// <summary>
    /// Finds space for item in the ground grid. If there is no space, the item is not placed.
    /// </summary>
    /// <param name="item">Item that needs to be placed.</param>
    /// <param name="lastPosition">The position on which the last item was placed. Prevents unncesseary serching.</param>
    /// <param name="groundGrid">The grid into which the items are placed.</param>
    /// <returns>The position onto which the item was placed.</returns>
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


    /// <summary>
    /// Checks whether an item woud protrude the grid in a position.
    /// </summary>
    /// <param name="x">X position of the item.</param>
    /// <param name="y">Y position of the item.</param>
    /// <returns></returns>
    private bool BoundaryCheck(int x, int y){
        if(x >= groundGridWidth || y >= groundGridHeight){
            return false;
        }
        return true;
    }

    //We only need width here since item are placed row by row
    /// <summary>
    /// Checks whether an item at this position would overlap with another already placed item. COnsidering that the item are spawned
    /// left to right, row by row, the height of the item does not need to be checked.
    /// </summary>
    /// <param name="x">X position of the item.</param>
    /// <param name="y">Y position of the item.</param>
    /// <param name="width">Width of the item.</param>
    /// <param name="groundGrid">The ground grid that is being created.</param>
    /// <returns></returns>
    private bool OverlapCheck(int x, int y, int width, InventoryItem[,] groundGrid){
        for(int i = 0; i < width; i++){
            if(groundGrid[x + i, y] != null){
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Destroys the item.
    /// </summary>
    /// <param name="item"></param>
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
