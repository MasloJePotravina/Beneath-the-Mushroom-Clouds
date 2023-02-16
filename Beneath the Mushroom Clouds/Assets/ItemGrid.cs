using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{

    public const float tileDimension = 64;

    InventoryItem[,] inventorySlot;


    RectTransform rectTransform;

    [SerializeField] int gridWidth;
    [SerializeField] int gridHeight;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridWidth, gridHeight);

   
    }

    private void Init(int width, int height)
    {
        inventorySlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileDimension, height * tileDimension);
        rectTransform.sizeDelta = size;
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

            tileOnGrid.x = (int)(positionOnGrid.x / tileDimension);
            tileOnGrid.y = (int)(positionOnGrid.y / tileDimension);

        }else{
            //Normalize mouse position
            mousePosition.x = mousePosition.x/Screen.width;
            mousePosition.y = mousePosition.y/Screen.height;

            //Normalize width and height of the grid to reference resolution of 1920x1080
            float normalizedGridWidth = rectTransform.rect.width/1920f;
            float normalizedGridHeight = rectTransform.rect.height/1080f;
            
            //Normalize position of the grid
            Vector2 normalizedGridPosition = new Vector2(rectTransform.position.x/Screen.width, rectTransform.position.y/Screen.height);


            //Upper left corner and lower right corner of the grid normalized
            Vector2 upperLeftCorner = new Vector2(normalizedGridPosition.x, normalizedGridPosition.y);
            Vector2 lowerRightCorner = new Vector2(normalizedGridPosition.x + normalizedGridWidth, normalizedGridPosition.y - normalizedGridHeight);


            positionOnGrid.x = (mousePosition.x - upperLeftCorner.x)/(lowerRightCorner.x - upperLeftCorner.x);
            positionOnGrid.y = (upperLeftCorner.y - mousePosition.y)/(upperLeftCorner.y - lowerRightCorner.y);

            tileOnGrid.x = (int)(positionOnGrid.x / (tileDimension/rectTransform.rect.width));
            tileOnGrid.y = (int)(positionOnGrid.y / (tileDimension/rectTransform.rect.height));
        }

        return tileOnGrid;
    }

    public InventoryItem GetItem(int posX, int posY){
        return inventorySlot[posX, posY];
    }

    public InventoryItem GrabItem(int posX, int posY){
        InventoryItem item = inventorySlot[posX, posY];

        if(item == null){
            return null;
        }
        
        for(int x = 0; x < item.itemData.width; x++){
            for(int y = 0; y < item.itemData.height; y++){
                inventorySlot[item.gridPositionX + x, item.gridPositionY + y] = null;
            }
        }

        return item;
    }

    public bool PlaceItem(InventoryItem item, int posX, int posY){

        if(!BoundaryCheck(posX, posY, item.itemData.width, item.itemData.height)){
            return false;
        }

        if(!OverlapCheck(posX, posY, item.itemData.width, item.itemData.height)){
            return false;
        }

        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);

        for(int x = 0; x < item.itemData.width; x++){
            for(int y = 0; y < item.itemData.height; y++){
                inventorySlot[posX + x, posY + y] = item;
            }
        }

        item.gridPositionX = posX;
        item.gridPositionY = posY;

        Vector2 position = CalculateGridPosition(item, posX, posY);
        

        itemRectTransform.localPosition = position;

        //Items have to be resized if the resolution is different from 1920x1080
        if(Screen.width != 1920 || Screen.height != 1080){
            float newTileDimension = tileDimension * (Screen.width/1920f);
            itemRectTransform.sizeDelta = new Vector2(item.itemData.width * newTileDimension, item.itemData.height * newTileDimension);
        }

        return true;

        
    }

    public Vector2 CalculateGridPosition(InventoryItem item, int posX, int posY){
        Vector2 position = new Vector2();
        position.x = posX * tileDimension + (tileDimension * item.itemData.width) / 2;
        position.y = -(posY * tileDimension + (tileDimension * item.itemData.height) / 2);
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

    bool OverlapCheck(int posX, int posY, int width, int height){
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                if(inventorySlot[posX + x, posY + y] != null){
                    return false;
                }
            }
        }
        return true;
    }


}
