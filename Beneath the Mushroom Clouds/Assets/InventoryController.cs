using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class InventoryController : MonoBehaviour
{
    //TODO: Debug only, remove later
    [SerializeField] private GameObject debugSpawner;
    private TMP_Dropdown debugSpawnerDropdown;

    //EquipmentType zero is reserved for non equippable items and therefore is never initilized in this field
    [SerializeField] private GameObject[] itemSlots;

    //GameObject attached to player from which the bullets are fired
    [SerializeField] private GameObject playerFirearm;
    private FirearmScript firearmScript;

    private List<GameObject> openWindows = new List<GameObject>();

    [SerializeField] private GameObject hudCanvas;
    private HUDController hudController;


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

    [SerializeField] private GameObject inventoryContainerWindowPrefab;
    [SerializeField] private GameObject inventoryInfoWindowPrefab;
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

    //TODO: Debug only, remove later
    public int spawnedItem = 0;

    //0 = no weapon
    //1 = primary weapon
    //2 = secondary weapon
    public int selectedWeaponSlot = 1;

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

    private void SetUpDebugDropdown(){
        List<string> itemNames = new List<string>();

        foreach(ItemData item in items){
            itemNames.Add(item.itemName);
        }

        debugSpawnerDropdown.AddOptions(itemNames);
    }


    private void Awake() {
        highlighter = GetComponent<InventoryHighlight>();
        firearmScript = playerFirearm.GetComponent<FirearmScript>();
        hudController = hudCanvas.GetComponent<HUDController>();
        debugSpawnerDropdown = debugSpawner.GetComponent<TMP_Dropdown>();
        SetUpDebugDropdown();
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
    public void QuickSpawnItem(){
        if(selectedItem != null){
            Destroy(selectedItem.gameObject);
        }

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        InventoryItem item = SpawnItem(items[selectedItemID]);

        if(item == null){
            return;
        }

        if(item.itemData.stackable){
            item.SetStack(UnityEngine.Random.Range(1, item.itemData.maxStack));
        }

    }

    public void DropdownSpawnItem(int itemID){
        if(selectedItem != null){
            Destroy(selectedItem.gameObject);
        }

        InventoryItem item = SpawnItem(items[itemID]);

        if(item == null){
            return;
        }

        if(item.itemData.stackable){
            item.SetStack(UnityEngine.Random.Range(1, item.itemData.maxStack));
        }

    }

    public InventoryItem SpawnItem(ItemData itemData){
        InventoryItem item = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = item;
        rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);

        item.Set(itemData);

        return item;

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

        if(selectedItem != null){
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
                RemoveEquippedItemFromDict(selectedItem);
                if(selectedItem.itemData.weapon){
                    WeaponSelectUpdate();
                }
            }
        }
        HighlightSlot(true);
    }

    private void PlaceItemToSlot(){
        if(selectedSlot.PlaceItem(selectedItem)){
            ToggleContainerGrid(selectedItem, true);
            AddOutlineSprite(selectedItem);
            selectedItem.isEquipped = true;
            AddEquippedItemToDict(selectedItem);
            if(selectedItem.itemData.weapon){
                WeaponSelectUpdate();
            }
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
                    CloseContainerItemWindow(selectedItem);
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


    }

    private void HighlightItem(){

        if(selectedGrid == null){
            return;
        }


        Vector2Int positionOnGrid = GetTilePosition();
        bool highlightGreen = false;

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
                InventoryItem overlappingItem = null;
                if(selectedGrid.OverlapCheck(positionOnGrid.x, positionOnGrid.y, selectedItem, out stackableFlag, out overlappingItem)){
                    highlightGreen = true;
                }else{
                    if(stackableFlag){
                        highlightGreen = true;
                    }else if(selectedItem.itemData.ammo){
                        if(overlappingItem != null && overlappingItem.itemData.magazine && overlappingItem.itemData.weaponType == selectedItem.itemData.weaponType){
                            highlightGreen = true;
                        }
                    }
                }
            }

            if(highlightGreen){
                highlighter.SetColor(Color.green);
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
        if(item.itemData.equipmentType == 0){
            return;
        }
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
        //Equipment item is being unequipped
        }else{
            //This is needed, so that the higlighter does not get destroyed
            highlighter.SetHighlighterParent(tempGrid);

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
            CloseContainerItemWindow(clickedItem);
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

    private bool AttemptTransferToContainer(InventoryItem clickedItem, ItemGrid itemGrid, ItemSlot itemSlot, bool grabItem = true){
        if(clickedItem.isOpened){
            CloseContainerItemWindow(clickedItem);
        }
        int posX;
        int posY;
        if(containerGrid.FindSpaceForItem(clickedItem, out posX, out posY)){
            InventoryItem transferedItem = null;
            if(grabItem){
                if(itemGrid != null){
                    transferedItem = itemGrid.GrabItem(clickedItem.gridPositionX, clickedItem.gridPositionY);
                }
                if(itemSlot != null){
                    transferedItem = itemSlot.GrabItem();
                }
            }else{
                transferedItem = clickedItem;
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


    private bool AttempTransferToEquippedItem(GameObject container, InventoryItem clickedItem, ItemGrid itemGrid, ItemSlot itemSlot, bool grabItem = true){
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
                if(grabItem){
                    if(itemGrid != null){
                        transferedItem = itemGrid.GrabItem(clickedItem.gridPositionX, clickedItem.gridPositionY);
                    }
                    if(itemSlot != null){
                        transferedItem = itemSlot.GrabItem();
                    }
                }else{
                    transferedItem = clickedItem;
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
            CloseContainerItemWindow(clickedItem);
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

    public void OpenContainerItemWindow(InventoryItem clickedItem){
        if(clickedItem == null){
            return;
        }

        if(clickedItem.itemData.containerPrefab == null){
            return;
        }

        GameObject inventoryWindow = Instantiate(inventoryContainerWindowPrefab);
        inventoryWindow.transform.SetParent(canvasTransform);
        inventoryWindow.transform.position = Input.mousePosition;
        InventoryWindow windowScript = inventoryWindow.GetComponent<InventoryWindow>();
        windowScript.Init(clickedItem, this, containerWindow: true);
        windowScript.LoadContainerGrid();
        openWindows.Add(inventoryWindow);
        clickedItem.isOpened = true;

    }

    public void CloseContainerItemWindow(InventoryItem item){
        GameObject saveWindow = null;
        foreach(GameObject window in openWindows){
            InventoryWindow windowScript = window.GetComponent<InventoryWindow>();
            if(windowScript.item == item){
                if(!windowScript.containerWindow){
                    continue;
                }
                saveWindow = window;
                GameObject container = window.transform.Find("Background").GetChild(0).gameObject;
                SaveContainerItems(container);
                //This is needed, so that the higlighter does not get destroyed
                highlighter.SetHighlighterParent(tempGrid);
                Destroy(window);
                break;
            }
        }
        item.isOpened = false;
        openWindows.Remove(saveWindow);
    }

    public void OpenItemInfoWindow(InventoryItem clickedItem){
        if(clickedItem == null){
            return;
        }

        GameObject inventoryWindow = Instantiate(inventoryInfoWindowPrefab);
        inventoryWindow.transform.SetParent(canvasTransform);
        inventoryWindow.transform.position = Input.mousePosition;
        InventoryWindow windowScript = inventoryWindow.GetComponent<InventoryWindow>();
        windowScript.Init(clickedItem, this, containerWindow: false);
        windowScript.LoadItemInfo();
        openWindows.Add(inventoryWindow);
        clickedItem.infoOpened = true;
    }

    public void CloseItemInfoWindow(InventoryItem item){
        GameObject saveWindow = null;
        foreach(GameObject window in openWindows){
            InventoryWindow windowScript = window.GetComponent<InventoryWindow>();
            if(windowScript.containerWindow){
                continue;
            }
            saveWindow = window;
            if(windowScript.item == item){
                Destroy(window);
                break;
            }
        }
        item.infoOpened = false;
        openWindows.Remove(saveWindow);
    }

    

    public void LoadAmmoIntoMagazine(InventoryItem magazine){
        if(magazine.ammoCount == magazine.itemData.magazineSize){
            return;
        }

        while(magazine.ammoCount < magazine.itemData.magazineSize){
            InventoryItem ammo = null;
            ammo = FindAmmo(magazine.itemData.weaponType);
            if(ammo == null){
                return;
            }

            int neededAmount = magazine.itemData.magazineSize - magazine.ammoCount;
            int availableAmount = ammo.currentStack;
            int addedAmount = 0;
            if(neededAmount > availableAmount){
                addedAmount = availableAmount;
            }else{
                addedAmount = neededAmount;
            }

            magazine.AddToMagazine(addedAmount);
            ammo.RemoveFromStack(addedAmount);

        }    
    }

    public void UnloadAmmoFromMagazine(InventoryItem magazine){

        if(magazine.ammoCount == 0){
            return;
        }

        int removedAmount = magazine.RemoveAllFromMagazine();

        InventoryItem ammo = SpawnItem(magazine.itemData.ammoItemData);
        ammo.SetStack(removedAmount);
    }

    public void UnloadAmmoFromFirearm(InventoryItem firearm){
        if(firearm.ammoCount == 0){
            return;
        }

        int removedAmount = firearm.UnloadAmmoFromInternalMagazine();

        InventoryItem ammo = SpawnItem(firearm.itemData.ammoItemData);
        ammo.SetStack(removedAmount);

    }

    public void LoadAmmoIntoFirearm(InventoryItem firearm){
        if(firearm.ammoCount == firearm.itemData.internalMagSize){
            return;
        }

        while(firearm.ammoCount < firearm.itemData.internalMagSize){
            InventoryItem ammo = null;
            ammo = FindAmmo(firearm.itemData.weaponType);
            if(ammo == null){
                return;
            }

            int neededAmount = firearm.itemData.internalMagSize - firearm.ammoCount;
            int availableAmount = ammo.currentStack;
            int addedAmount = 0;
            if(neededAmount > availableAmount){
                addedAmount = availableAmount;
            }else{
                addedAmount = neededAmount;
            }

            firearm.LoadAmmoIntoInternalMagazine(addedAmount);
            ammo.RemoveFromStack(addedAmount);

        }
    }

    public InventoryItem FindMagazine(string weaponType, bool quickReload){
        InventoryItem bestMagazine = null;
        //Minus one so that a magazine with 0 ammo is selected if no other is found
        //This could be done with ">=" when checking for bestAmmoCount but that would cause the last found magazine to be used
        int bestAmmoCount = -1;
        ItemGrid currentGrid = null;
        InventoryItem currentMagazine = null;
        //Loop through equipped items
        foreach(KeyValuePair<string, InventoryItem> item in equippedItems){
            //Skip if item is null
            if(item.Value == null){
                continue;
            }
            //Skip if item is not a container
            if(!item.Value.itemData.container){
                continue;
            }
            //Skip backpack if this is called during quick reload (pressing reload button during gameplay)
            if(quickReload){
                if(item.Value.itemData.equipmentType == 6){
                    continue;
                }
            }
            //Get container
            GameObject container = inventoryContent.transform.Find(item.Value.itemData.containerPrefab.name).gameObject;
            //Get grid
            GameObject grids = container.transform.Find("Grid").gameObject;
            //Get child count
            int childCount = grids.transform.childCount;
            //Loop through children
            for(int i = 0; i < childCount; i++){
                //Get child
                Transform child = grids.transform.GetChild(i);
                //Get grid
                currentGrid = child.GetComponent<ItemGrid>();
                currentMagazine = currentGrid.FindBestMagazine(weaponType);
                if(currentMagazine != null){
                    if(currentMagazine.ammoCount > bestAmmoCount){
                        bestMagazine = currentMagazine;
                        bestAmmoCount = currentMagazine.ammoCount;
                    }
                }
            }
        }

        //Skip container/ground as well if this is called during quick reload
        if(quickReload){
            return bestMagazine;
        }

        //Check container/ground as well
        currentGrid = containerGrid.GetComponent<ItemGrid>();
        currentMagazine = currentGrid.FindBestMagazine(weaponType);
        if(currentMagazine != null){
            if(currentMagazine.ammoCount > bestAmmoCount){
                bestAmmoCount = currentMagazine.ammoCount;
                bestMagazine = currentMagazine;
            }
        }
        return bestMagazine;
    }

    

    public InventoryItem FindAmmo(string weaponType, bool quickReload = false){

        ItemGrid currentGrid = null;
        InventoryItem ammo = null;


        foreach(KeyValuePair<string, InventoryItem> item in equippedItems){
            if(item.Value == null){
                continue;
            }
            if(!item.Value.itemData.container){
                continue;
            }
            if(quickReload){
                if(item.Value.itemData.equipmentType == 6){
                    continue;
                }
            }
            GameObject container = inventoryContent.transform.Find(item.Value.itemData.containerPrefab.name).gameObject;
            GameObject grids = container.transform.Find("Grid").gameObject;
            int childCount = grids.transform.childCount;
            for(int i = 0; i < childCount; i++){
                Transform child = grids.transform.GetChild(i);
                currentGrid = child.GetComponent<ItemGrid>();
                ammo = currentGrid.FindAmmo(weaponType);
                if(ammo != null){
                    return ammo;
                }
            }
        }

        //Skip container/ground as well if this is called during quick reload
        if(quickReload){
            return null;
        }

        //Check container/ground as well
        currentGrid = containerGrid.GetComponent<ItemGrid>();
        ammo = currentGrid.FindAmmo(weaponType);
        if(ammo != null){
            return ammo;
        }

        return null;
    
    }


    private bool AttemptMagPlaceToInventory(InventoryItem magazine){
        if(equippedItems["ChestRig"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["ChestRig"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, magazine, null, null, false))
                return true;
        }
        if(equippedItems["TorsoTopLayer"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["TorsoTopLayer"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, magazine, null, null, false))
                return true;
        }
        if(equippedItems["LegsTopLayer"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["LegsTopLayer"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, magazine, null, null, false))
                return true;
        }

        return false;
    }


    public void ReloadRemoveMagazine(InventoryItem weapon){
        int ammoCount = weapon.RemoveMagazine();
        InventoryItem magazine = SpawnItem(weapon.itemData.magazineItemData);
        //In this case the removed magazine should not be the selected item but should be automatically transfered to pockets
        selectedItem = null;
        magazine.AddToMagazine(ammoCount);

        if(AttemptMagPlaceToInventory(magazine)){
            return;
        }
        AttemptTransferToContainer(magazine, null, null, false);
    }

    public void AttachMagazine(InventoryItem weapon, bool quickReload){
        Debug.Log("AttachMagazine");
        InventoryItem magazine = FindMagazine(weapon.itemData.weaponType, quickReload);
        if(magazine == null){
            Debug.Log("AttachMagazine1");
            return;
        }

        Debug.Log("AttachMagazine2");

        weapon.AttachMagazine(magazine);
        Destroy(magazine.gameObject);
        
    }

    

    public void RemoveMagazine(InventoryItem weapon){
        int ammoCount = weapon.RemoveMagazine();
        InventoryItem magazine = SpawnItem(weapon.itemData.magazineItemData);
        magazine.AddToMagazine(ammoCount);

    }

    public bool LoadRound(InventoryItem weapon){
        InventoryItem ammo = FindAmmo(weapon.itemData.weaponType);
        if(ammo == null){
            return false;
        }

        if(weapon.LoadAmmoIntoInternalMagazine(1) > 0){
            ammo.RemoveFromStack(1);
            return true;
        }else{
            return false;
        }
    }

    public void ChamberRound(InventoryItem weapon){
        InventoryItem ammo = FindAmmo(weapon.itemData.weaponType);
        if(ammo == null){
            return;
        }

        if(weapon.ChamberRound()){
            ammo.RemoveFromStack(1);
        }
    }

    public void ClearChamber(InventoryItem weapon){
        weapon.ClearChamber();
        InventoryItem ammo = SpawnItem(weapon.itemData.ammoItemData);
        ammo.SetStack(1);
    }

    public void RackFirearm(InventoryItem weapon){
        if(weapon == null){
            return;
        }
        weapon.RackFirearm();
    }

    public InventoryItem GetSelectedWeapon(){
        if(selectedWeaponSlot == 0){
            return null;
        }else if(selectedWeaponSlot == 1){
            return equippedItems["PrimaryWeapon"];
        }else if(selectedWeaponSlot == 2){
            return equippedItems["SecondaryWeapon"];
        }

        return null;
    }

    public ItemSlot GetSelectedWeaponSlot(){
        if(selectedWeaponSlot == 0){
            return null;
        }else if(selectedWeaponSlot == 1){
            return itemSlots[11].GetComponent<ItemSlot>();
        }else if(selectedWeaponSlot == 2){
            return itemSlots[12].GetComponent<ItemSlot>();
        }

        return null;
    }

    public void WeaponSelectUpdate(){
        InventoryItem primaryWeapon = equippedItems["PrimaryWeapon"];
        InventoryItem secondaryWeapon = equippedItems["SecondaryWeapon"];
        if(selectedWeaponSlot == 0){
            if(primaryWeapon != null){
                primaryWeapon.isSelectedWeapon = false;
            }
            if(secondaryWeapon != null){
                secondaryWeapon.isSelectedWeapon = false;
            }
        }else if(selectedWeaponSlot == 1){
            if(primaryWeapon != null){
                primaryWeapon.isSelectedWeapon = true;
            }
            if(secondaryWeapon != null){
                secondaryWeapon.isSelectedWeapon = false;
            }
        }else if(selectedWeaponSlot == 2){
            if(primaryWeapon != null){
                primaryWeapon.isSelectedWeapon = false;
            }
            if(secondaryWeapon != null){
                secondaryWeapon.isSelectedWeapon = true;
            }
        }

        firearmScript.ChangeSelectedFirearm(GetSelectedWeapon());
        hudController.UpdateWeaponHUD(GetSelectedWeapon());

    }

    public void CycleWeapon(InputValue value){
        float input = value.Get<float>();
        if(selectedWeaponSlot == 0){
            if(input > 0){
                selectedWeaponSlot = 1;
            }else if(input < 0){
                selectedWeaponSlot = 2;
            }
        }else if(selectedWeaponSlot == 1){
            if(input > 0){
                selectedWeaponSlot = 2;
            }else if(input < 0){
                selectedWeaponSlot = 0;
            }
        }else if(selectedWeaponSlot == 2){
            if(input > 0){
                selectedWeaponSlot = 0;
            }else if(input < 0){
                selectedWeaponSlot = 1;
            }
        }

        WeaponSelectUpdate();

        
    }

    public void SelectWeapon(int value){
        if(selectedWeaponSlot == 0){
            selectedWeaponSlot = value;
        }else if(selectedWeaponSlot == 1){
            if(value == 1){
                selectedWeaponSlot = 0;
            }else if(value == 2){
                selectedWeaponSlot = 2;
            }
        }else if(selectedWeaponSlot == 2){
            if(value == 1){
                selectedWeaponSlot = 1;
            }else if(value == 2){
                selectedWeaponSlot = 0;
            }
        }

        WeaponSelectUpdate();
        
    }
}
