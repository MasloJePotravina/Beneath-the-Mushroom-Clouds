using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{



    //EquipmentType zero is reserved for non equippable items and therefore is never initilized in this field
    [SerializeField] private GameObject[] itemSlots;

    private List<GameObject> openedWindows = new List<GameObject>();


    Dictionary<string, InventoryItem> equippedItems = new Dictionary<string, InventoryItem>{
        {"ChestRig", null},
        {"TorsoTopLayer", null},
        {"LegsTopLayer", null},
        {"Backpack", null},
        {"PrimaryWeapon", null},
        {"SecondaryWeapon", null},
        {"Head", null},
        {"TorsoBaseLayer", null},
        {"Gloves", null},
        {"LegsBaseLayer", null},
        {"Socks", null},
        {"Footwear", null},
        {"Watch", null},
        {"GeigerCounter", null}

    };



    Dictionary<int, string> equipmentTypes = new Dictionary<int, string>{
        {1, "Head"},
        {2, "ChestRig"},
        {3, "TorsoBaseLayer"},
        {4, "TorsoTopLayer"},
        {5, "Gloves"},
        {6, "Backpack"},
        {7, "LegsBaseLayer"},
        {8, "LegsTopLayer"},
        {9, "Socks"},
        {10, "Footwear"},
        {11, "PrimaryWeapon"},
        {12, "SecondaryWeapon"},
        {13, "Watch"},
        {14, "GeigerCounter"}

    };

    [SerializeField] private GameObject inventoryContent;

    //Highlighter is set to be the child of this testGrid to avoid destruction when removing clothing items
    [SerializeField] private ItemGrid tempGrid;
    [SerializeField] private ItemGrid containerGrid;

    [SerializeField] private GameObject inventoryWindowPrefab;
    [SerializeField] private GameObject equipmentOutline;
    [SerializeField] private GameObject contextMenuPrefab;
    private GameObject contextMenu;
    public bool mouseOverContextMenu = false;

    private bool contextMenuOpen = false;

    public bool inventoryOpen = false;

    public bool splitStackButtonPressed = false;
    public bool quickTransferButtonPressed = false;
    public bool quickEquipButtonPressed = false;

    public InventoryWindowTop hoveredWindowTop;
    public InventoryWindowTop selectedWindowTop;

    private ItemGrid selectedGrid;

    public ItemGrid SelectedGrid{
        get{
            return selectedGrid;
        }
        set{
            selectedGrid = value;
            highlighter.SetHighlighterParent(value);
        }
    }

    private ItemSlot selectedSlot;

    public ItemSlot SelectedSlot{
        get{
            return selectedSlot;
        }
        set{
            selectedSlot = value;
        }
    }

    

    private InventoryItem selectedItem;
    private InventoryItem highlightedItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight highlighter;


    private void Awake() {
        highlighter = GetComponent<InventoryHighlight>();
    }

    
    private void Update(){

        if(!inventoryOpen){
            return;
        }

        if(selectedGrid == null){
            highlighter.Show(false);
        }

        if(selectedItem != null){
            rectTransform.position = Input.mousePosition;

            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();

            
        }

        if(selectedWindowTop != null){
            selectedWindowTop.DragWindow();
            return;
        }

        

        HighlightItem();
        
    }

    //TODO: Debug only, remove later
    public void SpawnItem(){
        if(selectedItem != null){
            return;
        }
        InventoryItem item = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = item;
        rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        item.Set(items[selectedItemID]);

        if(item.itemData.stackable){
            item.SetStack(UnityEngine.Random.Range(1, item.itemData.maxStack));
        }

    }

    public void InventoryLeftClick(InputValue value){

        if(hoveredWindowTop != null){
            if(value.isPressed){
                selectedWindowTop = hoveredWindowTop;
            }else{
                selectedWindowTop.prevMousePos = new Vector2(-1, -1);
                selectedWindowTop = null;
            }
        }

        if(!value.isPressed){
            if(selectedWindowTop != null){
                selectedWindowTop.prevMousePos = new Vector2(-1, -1);
                selectedWindowTop = null;
            }
        }

        

        if(contextMenuOpen){
            if(!mouseOverContextMenu){
                CloseContextMenu();
            }
            return;
        }
        if(selectedGrid == null && selectedSlot == null){
            return;
        }

        if(selectedGrid != null){
            Vector2Int tilePosition = GetTilePosition();


            if(selectedItem == null){
                if(quickTransferButtonPressed){
                    if(value.isPressed){
                        QuickTransfer(selectedGrid.GetItem(tilePosition.x, tilePosition.y), selectedGrid, selectedSlot);
                    }
                    return;
                }
                if(quickEquipButtonPressed){
                    if(value.isPressed){
                        QuickEquip(selectedGrid.GetItem(tilePosition.x, tilePosition.y), selectedGrid);
                    }
                    return;
                }
                GrabItemFromGrid(value, tilePosition.x, tilePosition.y);
            }else{
                PlaceItemToGrid(value, tilePosition.x, tilePosition.y);  
            }
        }else if(selectedSlot != null){
            if(selectedItem == null){
                if(quickTransferButtonPressed || quickEquipButtonPressed){
                    if(value.isPressed){
                        QuickTransfer(selectedSlot.GetItem(), selectedGrid, selectedSlot);
                    }
                    return;
                }
                GrabItemFromSlot(value);
            }else{
                PlaceItemToSlot();
            }
        }

    }

    public void SplitStackHotkey(InputValue value){
        splitStackButtonPressed = value.isPressed;
    }

    public void QuickTransferHotkey(InputValue value){
        quickTransferButtonPressed = value.isPressed;
    }

    public void QuickEquipHotkey(InputValue value){
        quickEquipButtonPressed = value.isPressed;
    }

    public void RightClick(){
        if(contextMenuOpen){
            CloseContextMenu();
        }
        if(selectedGrid == null && selectedSlot == null){
            return;
        }

        if(selectedGrid != null){
            Vector2Int tilePosition = GetTilePosition();
            InventoryItem clickedItem = selectedGrid.GetItem(tilePosition.x, tilePosition.y);
            if(clickedItem != null){
                OpenContextMenu(clickedItem);
            }
        }

        if(selectedSlot != null){
            InventoryItem clickedItem = selectedSlot.GetItem();
            if(clickedItem != null){
                OpenContextMenu(clickedItem);
            }
        }

    }

    private void GrabItemFromSlot(InputValue value){
        if(value.isPressed){
            selectedItem = selectedSlot.GrabItem();
            if(selectedItem != null){
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
                ToggleContainerGrid(selectedItem, false);
                selectedItem.isEquipped = false;
                RemoveOutlineSprite(selectedItem);
                equippedItems[equipmentTypes[selectedItem.itemData.equipmentType]] = null;
            }
        }
        HighlightSlot(true);
    }

    private void PlaceItemToSlot(){
        if(selectedSlot.PlaceItem(selectedItem)){
            ToggleContainerGrid(selectedItem, true);
            AddOutlineSprite(selectedItem);
            selectedItem.isEquipped = true;
            selectedItem = null;
            HighlightSlot(false);

        }
        
    }

    private void GrabItemFromGrid(InputValue value, int posX, int posY){
        if(value.isPressed){
            InventoryItem clickedItem = selectedGrid.GetItem(posX, posY);
            if(clickedItem != null){
                if(SplittingStack(clickedItem, posX, posY)){
                    SplitStack(clickedItem);
                }else{
                    selectedItem = selectedGrid.GrabItem(posX, posY);
                }
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
                if(selectedItem.isOpened){
                    CloseInventoryContainerWindow(selectedItem);
                }
            }
        }
        HighlightSlot(true);
    }

    private void PlaceItemToGrid(InputValue value, int posX, int posY){
        if(selectedGrid.PlaceItem(selectedItem, posX, posY)){
            selectedItem = null;
            HighlightSlot(false);
        }
        
    }

    private void HighlightSlot(bool highlight){
        if(!highlight){
            for(int i = 1; i < itemSlots.Length; i++){
                itemSlots[i].GetComponent<Image>().color = Color.white;
            }
            return;
        
        }
        if(selectedItem == null){

            return;
        }

        if(!selectedItem.itemData.equipment){
            return;
        }

        itemSlots[selectedItem.itemData.equipmentType].GetComponent<Image>().color = Color.green;

        //Secondary weapon can be used as primary
        if(selectedItem.itemData.equipmentType == 12){
            itemSlots[11].GetComponent<Image>().color = Color.green;
        }


    }

    private void HighlightItem(){

        if(selectedGrid == null){
            return;
        }


        Vector2Int positionOnGrid = GetTilePosition();

        if(selectedItem == null)
        {
            highlightedItem = selectedGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if(highlightedItem != null){
                highlighter.SetSize(highlightedItem);
                highlighter.SetPosition(selectedGrid, highlightedItem);
                highlighter.SetColor(Color.white);
                highlighter.Show(true);
            }else{
                highlighter.Show(false);
            }
        }else{
            highlighter.SetSize(selectedItem);
            highlighter.SetPosition(selectedGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);


            if(selectedGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.Width, selectedItem.Height)){
                bool stackableFlag = false;
                if(selectedGrid.OverlapCheck(positionOnGrid.x, positionOnGrid.y, selectedItem, ref stackableFlag)){
                    highlighter.SetColor(Color.green);
                }else{
                    if(stackableFlag){
                        highlighter.SetColor(Color.green);
                    }else{
                        highlighter.SetColor(Color.red);
                    }
                }
            }else{
                highlighter.SetColor(Color.red);
            }
            

            highlighter.Show(selectedGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.Width, selectedItem.Height));
        }
    }

    private Vector2Int GetTilePosition(){
        //Mouse offset when dragging item
        Vector2 position = Input.mousePosition;
        if(selectedItem != null){
            float tileDimension = ItemGrid.tileDimension;
            if(Screen.width != 1920 || Screen.height != 1080){
                tileDimension = (Screen.width / 1920f) * tileDimension;
            }
            position.x -= (selectedItem.Width - 1) * (tileDimension/2);
            position.y += (selectedItem.Height - 1) * (tileDimension/2);
        }
        return selectedGrid.GetTilePosition(position);
    }

    public void RotateItem(){
        if(selectedItem == null){
            return;
        }

        selectedItem.Rotate();

    }

    public void AddEquippedItemToDict(InventoryItem item){
        if(equippedItems.ContainsKey(equipmentTypes[item.itemData.equipmentType])){
            equippedItems[equipmentTypes[item.itemData.equipmentType]] = item;
        }
    }

    public void RemoveEquippedItemFromDict(InventoryItem item){
        if(equippedItems.ContainsKey(equipmentTypes[item.itemData.equipmentType])){
            equippedItems[equipmentTypes[item.itemData.equipmentType]] = null;
        }
    }

    public void ToggleContainerGrid(InventoryItem item, bool create){
        if(!item.itemData.container){
            return;
        }
        //Equipment item is being equipped
        if(create){
            //instantiate prefab
            GameObject containerGrid = Instantiate(item.itemData.containerPrefab, inventoryContent.transform);
            //Set the name of the container to the name of the prefab
            containerGrid.name = item.itemData.containerPrefab.name;

            GameObject grids = containerGrid.transform.Find("Grid").gameObject;

            //Bind grids to parent object
            //Get child count
            int childCount = grids.transform.childCount;
            //Loop through children
            for(int i = 0; i < childCount; i++){
                //Get child
                Transform child = grids.transform.GetChild(i);
                child.GetComponent<ItemGrid>().parentItem = item;
                
            }


            //Ordering container grids
            //If chest rig 
            if(item.itemData.equipmentType == 2){
                containerGrid.transform.SetAsFirstSibling();
            //If torso top layer
            }else if(item.itemData.equipmentType == 4){
                if(equippedItems["ChestRig"] == item){
                    containerGrid.transform.SetSiblingIndex(1);
                }else{
                    containerGrid.transform.SetAsFirstSibling();
                }
            //If legs top layer
            }else if(item.itemData.equipmentType == 8){
                if(equippedItems["ChestRig"] != null && equippedItems["TorsoTopLayer"] != null){
                    containerGrid.transform.SetSiblingIndex(2);
                }else if(equippedItems["ChestRig"] != null || equippedItems["TorsoTopLayer"] != null){
                    containerGrid.transform.SetSiblingIndex(1);
                }else{
                    containerGrid.transform.SetAsFirstSibling();
                }
            //If backpack
            }else{
                containerGrid.transform.SetAsLastSibling();
            }
            AddEquippedItemToDict(item);
        //Equipment item is being unequipped
        }else{
            //This is needed, so that the higlighter does not get destroyed
            highlighter.SetHighlighterParent(tempGrid);

            RemoveEquippedItemFromDict(item);

            //Destroyed prefab
            GameObject prefab = inventoryContent.transform.Find(item.itemData.containerPrefab.name).gameObject;


            //Saving items of the container
            SaveContainerItems(prefab);

            //Destroying container grid
            Destroy(prefab);
        }
    }

    public void AddOutlineSprite(InventoryItem item){
        if(item.itemData.clothing){
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().sprite = item.itemData.outlineSprite;
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().enabled = true;
        }
    }

    public void RemoveOutlineSprite(InventoryItem item){
        if(item.itemData.clothing){
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().enabled = false;
        }
    }

    private void SaveContainerItems(GameObject container){

        if(container == null){
            return;
        }

        //Get Grid child
        GameObject grids = container.transform.Find("Grid").gameObject;
        //Get child count
        int childCount = grids.transform.childCount;
        //Loop through children
        for(int i = 0; i < childCount; i++){
            //Get child
            Transform child = grids.transform.GetChild(i);
            //Get grid
            ItemGrid grid = child.GetComponent<ItemGrid>();
            
            grid.SaveItems();
        }
    }

    private bool SplittingStack(InventoryItem clickedItem, int posX, int posY){
        if(clickedItem.itemData.stackable){
            if(splitStackButtonPressed){
                if(clickedItem.currentStack > 1){
                    return true;
                }
            }
        }
    return false;

    }

    private void OpenContextMenu(InventoryItem clickedItem){
        contextMenu = Instantiate(contextMenuPrefab, canvasTransform);
        contextMenu.transform.position = Input.mousePosition;
        ContextMenu contextMenuComponent = contextMenu.GetComponent<ContextMenu>();
        contextMenuComponent.item = clickedItem;
        contextMenuComponent.selectedGrid = selectedGrid;
        contextMenuComponent.selectedSlot = selectedSlot;
        contextMenuComponent.MenuSetup();
        contextMenuOpen = true;
    }

    public void CloseContextMenu(){
        Destroy(contextMenu);
        contextMenuOpen = false;
    }

    public void SplitStack(InventoryItem clickedItem){
        InventoryItem item = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        item.Set(clickedItem.itemData);
        item.SetStack(clickedItem.currentStack/2);
        clickedItem.RemoveFromStack(item.currentStack);
        selectedItem = item;
        rectTransform = selectedItem.GetComponent<RectTransform>();
        rectTransform.SetAsLastSibling();
    }

    //Grid or slot of selected item, selectedGrid and selectedItem cannot be used here since this function is also
    //called from the context menu
    public void QuickTransfer(InventoryItem clickedItem, ItemGrid itemGrid, ItemSlot itemSlot){
        if(selectedItem != null){
            return;
        }

        if(clickedItem == null){
            return;
        }

        if(clickedItem.isOpened){
            CloseInventoryContainerWindow(clickedItem);
        }

        if(itemGrid != null){
            //Equipment grid => Container/Ground grid transfer
            if(itemGrid.parentItem != null){
                Debug.Log("Equipment grid => Container/Ground grid transfer");
                if(AttemptTransferToContainer(clickedItem, itemGrid, itemSlot))
                    return;
                

            //Container/Ground grid => Equipment grid transfer
            }else{
                if(AttemptTransferToInventory(clickedItem, itemGrid, itemSlot))
                    return;
            }
        }

        if(itemSlot != null){
            //Equipment slot => Inventory transfer , if not Container/Ground grid transfer
            //Works due to the fact that expression is evaluated from left to right
            if(AttemptTransferToInventory(clickedItem, itemGrid, itemSlot) || AttemptTransferToContainer(clickedItem, itemGrid, itemSlot)){
                ToggleContainerGrid(clickedItem, false);
                RemoveOutlineSprite(clickedItem);
                RemoveEquippedItemFromDict(clickedItem);
                clickedItem.isEquipped = false;
                return;
            }
        }
    }

    private bool AttemptTransferToContainer(InventoryItem clickedItem, ItemGrid itemGrid, ItemSlot itemSlot){
        if(clickedItem.isOpened){
            CloseInventoryContainerWindow(clickedItem);
        }
        int posX;
        int posY;
        if(containerGrid.FindSpaceForItem(clickedItem, out posX, out posY)){
            InventoryItem transferedItem = null;
            if(itemGrid != null){
                transferedItem = itemGrid.GrabItem(clickedItem.gridPositionX, clickedItem.gridPositionY);
            }
            if(itemSlot != null){
                transferedItem = itemSlot.GrabItem();
            }
            containerGrid.PlaceItem(transferedItem, posX, posY);
            return true;
        }
        return false;
    }

    private bool AttemptTransferToInventory(InventoryItem clickedItem, ItemGrid itemGrid, ItemSlot itemSlot){
        if(equippedItems["ChestRig"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["ChestRig"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, clickedItem, itemGrid, itemSlot))
                return true;
        }
        if(equippedItems["TorsoTopLayer"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["TorsoTopLayer"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, clickedItem, itemGrid, itemSlot))
                return true;
        }
        if(equippedItems["LegsTopLayer"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["LegsTopLayer"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, clickedItem, itemGrid, itemSlot))
                return true;
        }
        if(equippedItems["Backpack"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["Backpack"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, clickedItem, itemGrid, itemSlot))
                return true;
        }

        return false;
    }


    private bool AttempTransferToEquippedItem(GameObject container, InventoryItem clickedItem, ItemGrid itemGrid, ItemSlot itemSlot){
        int posX;
        int posY;
        GameObject grids = container.transform.Find("Grid").gameObject;
        //Get child count
        int childCount = grids.transform.childCount;
        //Loop through children
        for(int i = 0; i < childCount; i++){
            //Get child
            Transform child = grids.transform.GetChild(i);
            //Get grid
            ItemGrid grid = child.GetComponent<ItemGrid>();
            if(grid.parentItem == clickedItem){
                continue;
            }
            if(grid.FindSpaceForItem(clickedItem, out posX, out posY)){
                InventoryItem transferedItem = null;
                if(itemGrid != null){
                    transferedItem = itemGrid.GrabItem(clickedItem.gridPositionX, clickedItem.gridPositionY);
                }
                if(itemSlot != null){
                    transferedItem = itemSlot.GrabItem();
                }
                grid.PlaceItem(transferedItem, posX, posY);
                return true;
            }
        }
        return false;
    }

    public void QuickEquip(InventoryItem clickedItem, ItemGrid itemGrid){
        if(selectedItem != null){
            return;
        }

        if(clickedItem == null){
            return;
        }

        if(itemGrid == null){
            return;
        }

        if(clickedItem.isOpened){
            CloseInventoryContainerWindow(clickedItem);
        }

        AttemptToEquipItem(clickedItem, itemGrid);

    }

    private bool AttemptToEquipItem(InventoryItem clickedItem, ItemGrid itemGrid){
        if(clickedItem.itemData.equipment){
            if(equippedItems[equipmentTypes[clickedItem.itemData.equipmentType]] != null){
                return false;
            }
            InventoryItem equippedItem = itemGrid.GrabItem(clickedItem.gridPositionX, clickedItem.gridPositionY);
            itemSlots[equippedItem.itemData.equipmentType].GetComponent<ItemSlot>().PlaceItem(equippedItem);
            equippedItems[equipmentTypes[equippedItem.itemData.equipmentType]] = equippedItem;
            ToggleContainerGrid(equippedItem, true);
            AddOutlineSprite(equippedItem);
            AddEquippedItemToDict(equippedItem);
            equippedItem.isEquipped = true;

            return true;
        }
        return false;
    }

    public void OpenInventoryContainerWindow(InventoryItem clickedItem){
        if(clickedItem == null){
            return;
        }

        if(clickedItem.itemData.containerPrefab == null){
            return;
        }

        GameObject inventoryWindow = Instantiate(inventoryWindowPrefab);
        inventoryWindow.transform.SetParent(canvasTransform);
        inventoryWindow.transform.position = Input.mousePosition;
        ContainerWindow windowScript = inventoryWindow.GetComponent<ContainerWindow>();
        windowScript.Init(clickedItem, this);
        windowScript.LoadContainerGrid();
        openedWindows.Add(inventoryWindow);
        clickedItem.isOpened = true;

    }

    public void CloseInventoryContainerWindow(InventoryItem item){
        GameObject saveWindow = null;
        foreach(GameObject window in openedWindows){
            ContainerWindow windowScript = window.GetComponent<ContainerWindow>();
            if(windowScript.item == item){
                saveWindow = window;
                GameObject container = window.transform.Find("Background").GetChild(0).gameObject;
                SaveContainerItems(container);
                item.isOpened = false;
                //This is needed, so that the higlighter does not get destroyed
                highlighter.SetHighlighterParent(tempGrid);
                Destroy(window);
            }
        }

        openedWindows.Remove(saveWindow);
    }
}
