using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Implements the behaviour of item grids.
/// </summary>
public class ItemGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Parent item of the grid. This is used to associate grids with container items.
    /// </summary>
    public InventoryItem parentItem = null;

    /// <summary>
    /// Reference to the Inventory Screen which contains the Inventory Controller script.
    /// </summary>
    private GameObject inventoryScreen;

    /// <summary>
    /// Reference to the Inventory Controller.
    /// </summary>
    private InventoryController inventoryController;

    /// <summary>
    /// The dimension in pixels of one tile.
    /// </summary>
    public const float tileDimension = 64;

    /// <summary>
    /// Two dimensional array of references to items. It is used to determine what item each tile of the grid contains.
    /// </summary>
    private InventoryItem[,] inventorySlots;

    /// <summary>
    /// Rect transform of the grid.
    /// </summary>
    public RectTransform rectTransform;

    /// <summary>
    /// Width of the grid in tiles.
    /// </summary>
    public int gridWidth;
    /// <summary>
    /// Height of the grid in tiles.
    /// </summary>
    public int gridHeight;

    /// <summary>
    /// Position on the grid in pixels.
    /// </summary>
    Vector2 positionOnGrid = new Vector2();

    /// <summary>
    /// Position on the grid in tiles.
    /// </summary>
    /// <returns></returns>
    Vector2Int tileOnGrid = new Vector2Int();


    bool isGround;

    /// <summary>
    /// Detects when the mouse enters a grid and sets the grid as the selected grid in the Inventory Controller.
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedGrid = this;
    }

    /// <summary>
    /// Detects when the mouse leaves a grid and sets the selected grid in the Inventory Controller to null.
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedGrid = null;
    }

    /// <summary>
    /// On start, initializes the grid and load items if the grid is a child of a container item.
    /// </summary>
    void Awake()
    {
        inventoryScreen = GameObject.Find("InventoryScreen");
        inventoryController = inventoryScreen.GetComponent<InventoryController>();
        rectTransform = GetComponent<RectTransform>();
        Init(gridWidth, gridHeight);
    }

    /// <summary>
    /// Initializes the grid.
    /// </summary>
    /// <param name="width">Width of the grid in tiles.</param>
    /// <param name="height">Height of the grid in tiles.</param>
    /// <param name="isGround">If true, the grid is a ground grid.</param>
    public void Init(int width, int height, bool isGround = false)
    {
        inventorySlots = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileDimension, height * tileDimension);
        rectTransform.sizeDelta = size;
        gridWidth = width;
        gridHeight = height;
        this.isGround = isGround;
    }

    public void LoadItemsFromContainerObject(ContainerObject container)
    {
        if(container == null)
            return;
        inventorySlots = container.LoadGrid();
        
        foreach(InventoryItem item in inventorySlots){
            if(item != null){
                item.transform.SetParent(transform);
                item.transform.localPosition = CalculateGridPosition(item, item.gridPositionX, item.gridPositionY);
                RectTransform itemRectTransform = item.GetComponent<RectTransform>();
                itemRectTransform.localScale = Vector2.one;
                item.gameObject.SetActive(true);
            }
        }
    }

    public void LoadItemsFromGround(ItemPickUp itemPickUp)
    {
        inventorySlots = itemPickUp.CreateGroundGrid();
        foreach(InventoryItem item in inventorySlots){
            if(item != null){
                item.transform.SetParent(transform);
                item.transform.localPosition = CalculateGridPosition(item, item.gridPositionX, item.gridPositionY);
                RectTransform itemRectTransform = item.GetComponent<RectTransform>();
                itemRectTransform.localScale = Vector2.one;
                item.gameObject.SetActive(true);
            }
        }
    }

    public void SaveItemsToContainerObject(ContainerObject container)
    {
        if(container == null)
            return;
        
        container.SaveGrid(inventorySlots);
    }

    /// <summary>
    /// Loads items stored in the parent item of the grid.
    /// </summary>
    public void LoadItemsFromContainerItem()
    {
        if(parentItem == null)
            return;
        inventorySlots = parentItem.LoadGrid(transform.GetSiblingIndex());
        foreach(InventoryItem item in inventorySlots){
            if(item != null){
                item.transform.SetParent(transform);
                item.transform.localPosition = CalculateGridPosition(item, item.gridPositionX, item.gridPositionY);
                item.gameObject.SetActive(true);
            }
        }
    }


    

    /// <summary>
    /// Saves items stored in the grid into the parent item of the grid.
    /// </summary>
    public void SaveItems()
    {
        if(parentItem == null)
            return;
        
        parentItem.SaveGrid(transform.GetSiblingIndex(), inventorySlots);
    }

    /// <summary>
    ///Hides the items from the grid.
    /// </summary>
    public void HideItems()
    {
        if(parentItem == null)
            return;

        parentItem.HideGridItems(transform.GetSiblingIndex());
    }

    
    /// <summary>
    /// Gets the tile position on the grid.
    /// </summary>
    /// <param name="mousePosition">Position of the mouse in pixels.</param>
    /// <returns>Coordinates of the tile which the mouse is pointing at.</returns>
    public Vector2Int GetTilePosition(Vector2 mousePosition)
    {

        //The game is designed to be played in 1920x1080 resolution
        //If the resolution is different, the calculations for tiles in the grid will not work properly due to different mouse and grid positions and have to be normalized
        //This is less efficient and therefore the 1920x1080 resolution is recommended

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

    /// <summary>
    /// Gets the item stored in a tile.
    /// </summary>
    /// <param name="posX">X position of the tile.</param>
    /// <param name="posY">Y position of the tile.</param>
    /// <returns>Reference to the item which occupies the tile.</returns>
    public InventoryItem GetItem(int posX, int posY){
        if(posX > gridWidth-1 || posY > gridHeight-1 || posX < 0 || posY < 0){
            return null;
        }
        return inventorySlots[posX, posY];
    }

    /// <summary>
    /// Grabs the item stored in a tile. Works just like GetItem, but also removes the item from the grid.
    /// </summary>
    /// <param name="posX">X position of the tile.</param>
    /// <param name="posY">Y position of the tile.</param>
    /// <returns>Reference to the item which was grabbed from the grid.</returns>
    public InventoryItem GrabItem(int posX, int posY){
        InventoryItem item = inventorySlots[posX, posY];

        if(item == null){
            
            return null;
        }
        
        for(int x = 0; x < item.Width; x++){
            for(int y = 0; y < item.Height; y++){
                inventorySlots[item.gridPositionX + x, item.gridPositionY + y] = null;
            }
        }

        if(isGround){
            inventoryController.PickUpItem(item);
        }
        SaveItems();
        return item;
    }

    /// <summary>
    /// Places item into the grid.
    /// </summary>
    /// <param name="item">Reference to the item which is being placed.</param>
    /// <param name="posX">X position of the tile where the item is being placed (top left tile).</param>
    /// <param name="posY">Y position of the tile where the item is being placed (top left tile).</param>
    /// <returns>True if the item was placed successfully, false otherwise.</returns>
    public bool PlaceItem(InventoryItem item, int posX, int posY){
        if(item == null){
            return false;
        }
        //Forbid placing an item within itself (a bit redundant as selecting an open item destroys the grid but just in case)
        if(parentItem != null){
            if(item == parentItem){
                return false;
            }
        }


        //If the item does protrudes outside of the grid return false.
        if(!BoundaryCheck(posX, posY, item.Width, item.Height)){
            return false;
        }

        //If the item overlaps with aqnother item
        bool stackableFlag = false;
        InventoryItem overlappingItem = null;
        if(!OverlapCheck(posX, posY, item, out stackableFlag, out overlappingItem)){
            //If this is overlap due to item stacking, stack the items, if the selected item fit into the item stack return true, otherwise return false
            if(stackableFlag){
                int amountTransfered = inventorySlots[posX, posY].AddToStack(item.currentStack);
                if(item.RemoveFromStack(amountTransfered) == 0){
                    SaveItems();
                    return true;
                }else{
                    return false;
                }
            }
            //If this overlap is due to loading ammunition into a magazine or a firearm, attemt to load the ammunition and return true if the item was loaded, false otherwise
            if(item.itemData.ammo){
                if(overlappingItem.itemData.magazine){
                    if(overlappingItem.itemData.weaponType == item.itemData.weaponType){
                        int amountTransfered = overlappingItem.AddToMagazine(item.currentStack);
                        if(item.RemoveFromStack(amountTransfered) == 0){
                            SaveItems();
                            return true;
                        }else{
                            return false;
                        }
                    }
                }else if(overlappingItem.itemData.firearm){
                    if(overlappingItem.itemData.weaponType == item.itemData.weaponType){
                        //If the item uses an internal magazine, first attempt to chamber the round and then attempt to fill the internal magazine
                        if(!overlappingItem.itemData.usesMagazines){
                            if(overlappingItem.ChamberRound()){
                                if(item.RemoveFromStack(1) == 0){
                                    SaveItems();
                                    return true; 
                                }
                            }
                            int amountTransfered = overlappingItem.LoadAmmoIntoInternalMagazine(item.currentStack);
                            if(item.RemoveFromStack(amountTransfered) == 0){
                                SaveItems();
                                return true;
                            }else{
                                return false;
                            }
                        //IF the item uses regular magazines, juust attempt to chamber the round
                        }else{
                            if(overlappingItem.ChamberRound()){
                                if(item.RemoveFromStack(1) == 0){
                                    SaveItems();
                                    return true;
                                }else{
                                    return false;
                                }
                            }
                        }
                        
                    }
                }
            }

            //If this overlap is due to loading a magazine into a firearm, attempt to load the magazine and return true if the magazine was loaded, false otherwise
            if(item.itemData.magazine){
                if(overlappingItem.itemData.firearm){
                    if(overlappingItem.itemData.weaponType == item.itemData.weaponType){
                        if(overlappingItem.AttachMagazine(item)){
                            Destroy(item.gameObject);
                            SaveItems();
                            return true;
                        }else{
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        //If no overlap occured, place the item into the grid
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);

        //Set all of the tiles occupied by the item to the item
        for(int x = 0; x < item.Width; x++){
            for(int y = 0; y < item.Height; y++){
                inventorySlots[posX + x, posY + y] = item;
            }
        }

        item.gridPositionX = posX;
        item.gridPositionY = posY;

        Vector2 position = CalculateGridPosition(item, posX, posY);
    
        itemRectTransform.localPosition = position;

        if(isGround){
            inventoryController.DropItem(item);
        }
        return true;
        
    }

    /// <summary>
    /// Calculates the position of the item in pixels based on grid position
    /// </summary>
    /// <param name="item">Item to calculate the position for</param>
    /// <param name="posX">X position of the tile where the item is being placed (top left tile).</param>
    /// <param name="posY">Y position of the tile where the item is being placed (top left tile).</param>
    /// <returns>Coordinates of the item in pixels relative to the parent grid</returns>
    public Vector2 CalculateGridPosition(InventoryItem item, int posX, int posY){
        Vector2 position = new Vector2();
        position.x = posX * tileDimension + (tileDimension * item.Width) / 2;
        position.y = -(posY * tileDimension + (tileDimension * item.Height) / 2);
        return position;
    }

    /// <summary>
    /// Checks if specified position is within the grid
    /// </summary>
    /// <param name="posX">X coordinate of the position</param>
    /// <param name="posY">Y coordinate of the position</param>
    /// <returns>Ture if the position is within the grid, false otherwise</returns>
    bool PositionCheck(int posX, int posY){
        //Placing outside of grid
        if(posX < 0 || posX >= gridWidth || posY < 0 || posY >= gridHeight){
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the item being placed is within the grid
    /// </summary>
    /// <param name="posX">X position of the tile where the item is being placed (top left tile).</param>
    /// <param name="posY">Y position of the tile where the item is being placed (top left tile).</param>
    /// <param name="width"> Width of the item</param>
    /// <param name="height">Height of the item</param>
    /// <returns>True if the item is within the grid, false otherwise</returns>
    public bool BoundaryCheck(int posX, int posY, int width, int height){
        //Check the top left and bottom right corners of the item
        if(PositionCheck(posX, posY) == false){
            return false;
        }

        if(PositionCheck(posX + width - 1, posY + height - 1) == false){
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the item being placed overlaps with another item
    /// </summary>
    /// <param name="posX">X position of the placed item (top left tile)</param>
    /// <param name="posY">Y position of the placed item (top left tile)</param>
    /// <param name="item">Item being placed</param>
    /// <param name="stackableFlag">Out parameter, true if the item can be stacked with the overlapped item, false otherwise</param>
    /// <param name="overlappingItem">Out parameter, the item that is being overlapped</param>
    /// <returns>True if the item is overlapping with another item (even if stackable), false otherwise</returns>
    public bool OverlapCheck(int posX, int posY, InventoryItem item, out bool stackableFlag, out InventoryItem overlappingItem){
        stackableFlag = false;
        overlappingItem = null;
        for(int x = 0; x < item.Width; x++){
            for(int y = 0; y < item.Height; y++){
                if(inventorySlots[posX + x, posY + y] != null){
                    if(CheckIfStackable(inventorySlots[posX + x, posY + y], item)){
                        stackableFlag = true;
                    }
                    overlappingItem = inventorySlots[posX + x, posY + y];
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if two items can be stacked
    /// </summary>
    /// <param name="item1">First item (the item being placed)</param>
    /// <param name="item2">Second item (the item being overlapped)</param>
    /// <returns>True if the items can be stacked, false otherwise</returns>
    private bool CheckIfStackable(InventoryItem item1, InventoryItem item2){
        //Stackable check
        if(item1.itemData.stackable == false || item2.itemData.stackable == false){
            return false;
        }
        //Same item type check
        if(item1.itemData.id != item2.itemData.id){
            return false;
        }
        //Overlapped item not at max stack check
        if(item1.itemData.maxStack == item1.currentStack){
            return false;
        }
        return true;
    }

    /// <summary>
    /// Finds space for an item within the grid
    /// </summary>
    /// <param name="item">Reference to the item</param>
    /// <param name="posX">Out parameter, X position of the tile where the item can be placed. Set to -1 if no space was found.</param>
    /// <param name="posY">Out parameter, Y position of the tile where the item can be placed. Set to -1 if no space was found.</param>
    /// <returns>True if space was found, false otherwise</returns>
    public bool FindSpaceForItem(InventoryItem item, out int posX, out int posY){
        if(item == null){
            posX = -1;
            posY = -1;
            return false;
        }
        //Check every tile in the grid for boundary or overlap
        //This can likely be optimized
        for(int y = 0; y < gridHeight; y++){
            for(int x = 0; x < gridWidth; x++){
                if(BoundaryCheck(x, y, item.Width, item.Height)){
                    bool boolStackableFlag = false;
                    InventoryItem overlappingItem = null;
                    if(OverlapCheck(x, y, item, out boolStackableFlag, out overlappingItem)){
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

    /// <summary>
    /// Finds the best (fullest) magazine for the specified weapon type.
    /// </summary>
    /// <param name="weaponType">Weapon type to find magazine for</param>
    /// <returns>Reference to the best magazine, null if no magazine was found</returns>
    public InventoryItem FindBestMagazine(string weaponType){
        int mostAmmo = -1;
        InventoryItem bestMagazine = null;
        foreach(InventoryItem item in inventorySlots){
            if(item == null){
                continue;
            }
            if(item.itemData.magazine){
                if(item.itemData.weaponType != weaponType){
                    continue;
                }
                if(item.ammoCount > mostAmmo){
                    mostAmmo = item.ammoCount;
                    bestMagazine = item;
                }
            }
        }
        return bestMagazine;
    }

    /// <summary>
    /// Finds ammo for the specified weapon type.
    /// </summary>
    /// <param name="weaponType">Weapon type to find ammo for</param>
    /// <returns>Reference to the ammo, null if no ammo was found</returns>
    public InventoryItem FindAmmo(string weaponType){
        foreach(InventoryItem item in inventorySlots){
            if(item == null){
                continue;
            }
            if(item.itemData.ammo){
                if(item.itemData.weaponType == weaponType){
                    //Interesting note:
                    //While all stackable items are destroyed on empty stack and
                    //this condition seems futile it is actually very important
                    //Since Destroy() is called at the end of the frame, but this function is called
                    //in a loop (which executes multiple times per frame), this function would return a reference 
                    //to an empty stack and cause a never ending loop and crash (2 hours of debugging btw) 
                    if(item.currentStack > 0)
                        return item;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Disables all items in the grid
    /// </summary>
    public void DisableItemsInGrid(){
        foreach(InventoryItem item in inventorySlots){
            if(item != null){
                item.gameObject.SetActive(false);
            }
        }
    }

}
