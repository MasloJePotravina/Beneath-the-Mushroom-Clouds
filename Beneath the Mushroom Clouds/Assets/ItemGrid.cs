using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{

    public InventoryItem parentItem = null;

    public const float tileDimension = 64;

    private InventoryItem[,] inventorySlot;


    public RectTransform rectTransform;

    public int gridWidth;
    public int gridHeight;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridWidth, gridHeight);

        LoadItems(parentItem);

   
    }

    private void Init(int width, int height)
    {
        inventorySlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileDimension, height * tileDimension);
        rectTransform.sizeDelta = size;
    }

    private void LoadItems(InventoryItem parentItem)
    {
        if(parentItem == null)
            return;
        inventorySlot = parentItem.LoadGrid(transform.GetSiblingIndex());
        foreach(InventoryItem item in inventorySlot){
            if(item != null){
                item.transform.SetParent(transform);
                item.transform.localPosition = CalculateGridPosition(item, item.gridPositionX, item.gridPositionY);
                item.gameObject.SetActive(true);
            }
        }
    }

    public void SaveItems()
    {
        if(parentItem == null)
            return;
        
        parentItem.SaveGrid(transform.GetSiblingIndex(), inventorySlot);
    }

    Vector2 positionOnGrid = new Vector2();
    Vector2Int tileOnGrid = new Vector2Int();

    public Vector2Int GetTilePosition(Vector2 mousePosition)
    {

        //The game is designed to be played in 1920x1080 resolution
        //If the resolution is different, the calculations for tiles in the grid will not work properly due to different mouse and grid positions and have to be normalized
        //This is MUCH less efficient and therefore the 1920x1080 resolution is HIGHLY recommended
        //1920x1080: ~290FPS, different resolution: ~250FPS => ~40FPS difference 



        if(Screen.width == 1920 && Screen.height == 1080){
            positionOnGrid.x = mousePosition.x - rectTransform.position.x;
            positionOnGrid.y = rectTransform.position.y - mousePosition.y;
        }else{
            //Normalize mouse position
            mousePosition.x = mousePosition.x/Screen.width;
            mousePosition.y = mousePosition.y/Screen.height;

            mousePosition.x *= 1920f;
            mousePosition.y *= 1080f;

            Vector2 transformedGridPosition = new Vector2();

            //Normalize grid position
            transformedGridPosition.x = rectTransform.position.x/Screen.width;
            transformedGridPosition.y = rectTransform.position.y/Screen.height;

            transformedGridPosition.x *= 1920f;
            transformedGridPosition.y *= 1080f;

            positionOnGrid.x = mousePosition.x - transformedGridPosition.x;
            positionOnGrid.y = transformedGridPosition.y - mousePosition.y;

        }

        tileOnGrid.x = (int)(positionOnGrid.x / tileDimension);
        tileOnGrid.y = (int)(positionOnGrid.y / tileDimension);

        return tileOnGrid;
    }

    public InventoryItem GetItem(int posX, int posY){
        if(posX > gridWidth-1 || posY > gridHeight-1 || posX < 0 || posY < 0){
            return null;
        }
        return inventorySlot[posX, posY];
    }

    public InventoryItem GrabItem(int posX, int posY){
        InventoryItem item = inventorySlot[posX, posY];

        if(item == null){
            return null;
        }
        
        for(int x = 0; x < item.Width; x++){
            for(int y = 0; y < item.Height; y++){
                inventorySlot[item.gridPositionX + x, item.gridPositionY + y] = null;
            }
        }

        return item;
    }

    public bool PlaceItem(InventoryItem item, int posX, int posY){

        if(parentItem != null){
            if(item == parentItem){
                return false;
            }
        }

        if(!BoundaryCheck(posX, posY, item.Width, item.Height)){
            return false;
        }

        bool stackableFlag = false;
        if(!OverlapCheck(posX, posY, item, ref stackableFlag)){
            if(stackableFlag){
                int amountTransfered = inventorySlot[posX, posY].AddToStack(item.currentStack);
                item.RemoveFromStack(amountTransfered);
                if(item.currentStack == 0){
                    Destroy(item.gameObject);
                    return true;
                }else{
                    return false;
                }
            }
            return false;
        }

        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);

        for(int x = 0; x < item.Width; x++){
            for(int y = 0; y < item.Height; y++){
                inventorySlot[posX + x, posY + y] = item;
            }
        }

        item.gridPositionX = posX;
        item.gridPositionY = posY;

        Vector2 position = CalculateGridPosition(item, posX, posY);
        

        itemRectTransform.localPosition = position;
        return true;
        
    }

    public Vector2 CalculateGridPosition(InventoryItem item, int posX, int posY){
        Vector2 position = new Vector2();
        position.x = posX * tileDimension + (tileDimension * item.Width) / 2;
        position.y = -(posY * tileDimension + (tileDimension * item.Height) / 2);
        return position;
    }

    //Object placed outside of the grid
    bool PositionCheck(int posX, int posY){
        //Placing outside of grid
        if(posX < 0 || posX >= gridWidth || posY < 0 || posY >= gridHeight){
            return false;
        }
        return true;
    }

    public bool BoundaryCheck(int posX, int posY, int width, int height){
        //Placing outside of grid
        if(PositionCheck(posX, posY) == false){
            return false;
        }

        if(PositionCheck(posX + width - 1, posY + height - 1) == false){
            return false;
        }

        return true;
    }

    public bool OverlapCheck(int posX, int posY, InventoryItem item, ref bool stackableFlag){
        for(int x = 0; x < item.Width; x++){
            for(int y = 0; y < item.Height; y++){
                if(inventorySlot[posX + x, posY + y] != null){
                    if(CheckIfStackable(inventorySlot[posX + x, posY + y], item)){
                        stackableFlag = true;
                    }
                    return false;
                }
            }
        }
        return true;
    }

    private bool CheckIfStackable(InventoryItem item1, InventoryItem item2){
        if(item1.itemData.stackable == false || item2.itemData.stackable == false){
            return false;
        }
        if(item1.itemData.id != item2.itemData.id){
            return false;
        }
        if(item1.itemData.maxStack == item1.currentStack){
            return false;
        }
        return true;
    }

    public bool FindSpaceForItem(InventoryItem item, out int posX, out int posY){
        if(item == null){
            posX = -1;
            posY = -1;
            return false;
        }
        for(int y = 0; y < gridHeight; y++){
            for(int x = 0; x < gridWidth; x++){
                if(BoundaryCheck(x, y, item.Width, item.Height)){
                    bool boolStackableFlag = false;
                    if(OverlapCheck(x, y, item, ref boolStackableFlag)){
                        posX = x;
                        posY = y;
                        return true;
                    }
                }   
            }
        }
        posX = -1;
        posY = -1;
        return false;
    }


}
