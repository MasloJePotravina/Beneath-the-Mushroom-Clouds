//Inventory system based on: https://www.youtube.com/watch?v=2ajD1GDbEzA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class implements the behavior of the inventory controller. Any communication between the different parts of the inventory system should be done through the methods of this class.
/// Any communication between the inventory system and other parts of the game should be also done through the methods of this class.
/// </summary>
public class InventoryController : MonoBehaviour
{
    /// <summary>
    /// Dropdown item spawner TMP_Dropdown component.
    /// </summary>
    private TMP_Dropdown debugSpawnerDropdown;

    /// <summary>
    /// Player gameObject reference.
    /// </summary>
    [SerializeField] private GameObject player;

    private PlayerStatus playerStatus;

    /// <summary>
    /// Array of equipment slots. The index of the array corresponds to the integer representation of the equipment type. Starts at 1 as 0 is reserved for non equippable items.
    /// </summary>
    [SerializeField] private GameObject[] itemSlots;

    /// <summary>
    /// Reference to the player firearm GameObject.
    /// </summary>
    [SerializeField] private GameObject playerFirearm;

    /// <summary>
    /// Firearm script attached to the player firearm GameObject.
    /// </summary>
    private FirearmScript firearmScript;

    /// <summary>
    /// List of all open inventory windows.
    /// </summary>
    private List<GameObject> openWindows = new List<GameObject>();

    /// <summary>
    /// Reference to the HUD canvas GameObject.
    /// </summary>
    [SerializeField] private GameObject hudCanvas;
    /// <summary>
    /// HudController component attached to the HUD canvas GameObject.
    /// </summary>
    private HUDController hudController;

    /// <summary>
    /// Player animation controller component attached to the player GameObject.
    /// </summary>
    private HumanAnimationController playerAnimationController;

    /// <summary>
    /// Dictionary of references to equipped items based on the equipment slot.
    /// </summary>
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

    /// <summary>
    /// Dictionary mapping the integer representation of equipment types to their string representation.
    /// </summary>
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

    /// <summary>
    /// Reference to the gameObject in the middle section of the inventory screen, where the itemGrids of all equipped container items are displayed.
    /// </summary>
    [SerializeField] private GameObject inventoryContent;

    /// <summary>
    /// A temporary grid to which the highlighter is assigned when the grid it is currently assigned to is destroyed.
    /// </summary>
    [SerializeField] private ItemGrid tempGrid;

    //TODO: This is only temporary until world containers (such as chests) and ground dropping are implemented
    /// <summary>
    /// Reference to the grid on the right side of the inventory screen.
    /// </summary>
    [SerializeField] private ItemGrid containerGrid;

    [SerializeField] private TextMeshProUGUI containerName;

    /// <summary>
    /// Prefab for inventory container windows.
    /// </summary>
    [SerializeField] private GameObject inventoryContainerWindowPrefab;

    /// <summary>
    /// Prefab for inventory item info windows.
    /// </summary>
    [SerializeField] private GameObject inventoryInfoWindowPrefab;

    /// <summary>
    /// Prefab for context menus.
    /// </summary>
    [SerializeField] private GameObject contextMenuPrefab;


    /// <summary>
    /// Equipment outline gameObject reference located in the left section of the inventory screen.
    /// </summary>
    [SerializeField] private GameObject equipmentOutline;
    
    /// <summary>
    /// Reference to the currently open context menu.
    /// </summary>
    private GameObject contextMenu;
    
    /// <summary>
    /// Flag indicating whether the mouse is hovering over the context menu.
    /// </summary>
    public bool mouseOverContextMenu = false;
    
    /// <summary>
    /// Flag indicating whether the context menu is currently open.
    /// </summary>
    private bool contextMenuOpen = false;

    /// <summary>
    /// Flag indicating whether the inventory is currently open.
    /// </summary>
    public bool inventoryOpen = false;

    /// <summary>
    /// Flag indicating whether the quick split stack hotkey is pressed.
    /// </summary>
    public bool splitStackButtonPressed = false;

    /// <summary>
    /// Flag indicating whether the quick transfer hotkey is pressed.
    /// </summary>
    public bool quickTransferButtonPressed = false;

    /// <summary>
    /// Flag indicating whether the quick equip hotkey is pressed.
    /// </summary>
    public bool quickEquipButtonPressed = false;

    /// <summary>
    /// Reference to a window top of an inventory window that is currently being hovered over.
    /// </summary>
    public InventoryWindowTop hoveredWindowTop;

    /// <summary>
    /// Reference to a window top of an inventory window that is currently selected.
    /// </summary>
    public InventoryWindowTop selectedWindowTop;

    /// <summary>
    /// Reference to the currently selected weapon slot. (0 = no weapon, 1 = primary weapon, 2 = secondary weapon)
    /// </summary>
    public int selectedWeaponSlot = 1;

    /// <summary>
    /// Currently selected (hovered over) grid.
    /// </summary>
    private ItemGrid selectedGrid;

    /// <summary>
    /// Encapsulation of the selectedGrid variable. Used, so that when the selectedGrid is set, the highlighter is set to be the child of the selected grid.
    /// </summary>
    public ItemGrid SelectedGrid{
        get{
            return selectedGrid;
        }
        set{
            selectedGrid = value;
            highlighter.SetHighlighterParent(value);
        }
    }

    /// <summary>
    /// Currently selected (hovered over) slot.
    /// </summary>
    private ItemSlot selectedSlot;

    /// <summary>
    /// Encapsulation of the selectedSlot variable. Only implemented due to consistency with the selectedGrid.
    /// </summary>
    /// <value></value>
    public ItemSlot SelectedSlot{
        get{
            return selectedSlot;
        }
        set{
            selectedSlot = value;
        }
    }

    /// <summary>
    /// Currently selected (clicked and dragged) item.
    /// </summary>
    private InventoryItem selectedItem;

    /// <summary>
    /// RectTransform of the currently selected item.
    /// </summary>
    private RectTransform selectedItemRectTransform;

    /// <summary>
    /// Currently highlighted item.
    /// </summary>
    private InventoryItem highlightedItem;

    /// <summary>
    /// List of all items in the game. Used to set up the debug spawn dropdown menu.
    /// </summary>
    [SerializeField] private List<ItemData> items;

    /// <summary>
    /// Prefab for inventory items.
    /// </summary>
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// Canvas transform reference of the inventory canvas.
    /// </summary>
    private RectTransform canvasTransform;

    /// <summary>
    /// Highlighter script reference.
    /// </summary>
    private InventoryHighlight highlighter;

    private ContainerObject containerObject = null;

    private ItemGrid previousGrid = null;
    private ItemSlot previousSlot = null;
    private Vector2Int previousGridPosition = new Vector2Int(0, 0);

    [SerializeField] private GameObject looseItemPrefab;

    private GameObject interactRange;

    private ItemPickUp itemPickUp;

    private CursorController cursorController;

    private AudioManager audioManager;

    private HealthStatusUI playerStatusUI;

    private TextMeshProUGUI carryWeightText;

    

    /// <summary>
    /// Sets up all the references to other objects and scripts and sets up the debug dropdown menu.
    /// </summary>
    private void Awake() {
        highlighter = GameObject.FindObjectOfType<InventoryHighlight>();
        firearmScript = playerFirearm.GetComponent<FirearmScript>();
        hudController = hudCanvas.GetComponent<HUDController>();
        playerAnimationController = player.GetComponent<HumanAnimationController>();
        debugSpawnerDropdown = GameObject.FindObjectOfType<DebugDropdownSpawner>().gameObject.GetComponent<TMP_Dropdown>();
        interactRange = GameObject.FindObjectOfType<PlayerInteract>().gameObject;
        itemPickUp = interactRange.GetComponent<ItemPickUp>();
        cursorController = GameObject.FindObjectOfType<CursorController>();
        canvasTransform = GetComponent<RectTransform>();
        playerStatus = player.GetComponent<PlayerStatus>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        playerStatusUI = GameObject.FindObjectOfType<HealthStatusUI>(true);
        carryWeightText = transform.Find("Tab/InventorySection/WeightInfo/WeightText").GetComponent<TextMeshProUGUI>();
        SetUpDebugDropdown();



        //Since awake is not called on disabled objects but the inventory screen has to be disabled at the start,
        // the inventory screen is enabled, variables are set up, and then the inventory screen is disabled again.
        //This is done due to the fact that other scripts may use functions of InventoryController before the inventory
        // screen is opened for the first time.
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Each frame handles the dragging of items, the highlighting of items, and the dragging of windows.
    /// </summary>
    private void Update(){



        if(!inventoryOpen){
            return;
        }

        //If no grid is selected, disable highlighter
        if(selectedGrid == null){
            highlighter.Show(false);
        }
        //If an item is selected, move it with the mouse
        if(selectedItem != null){
            selectedItemRectTransform.position = Input.mousePosition;
            selectedItemRectTransform.SetParent(canvasTransform);
            selectedItemRectTransform.SetAsLastSibling();

            
        }

       

        //If a window top is selected (clicked and dragged), drag the window
        if(selectedWindowTop != null){
            selectedWindowTop.DragWindow();
            return;
        }

        //Highlight the item that the mouse is over
        HighlightItem();

        UpdateCarryWeight();
        
    }

    /// <summary>
    /// Opens the inventory.
    /// </summary>
    public void OpenInventory(){
        inventoryOpen = true;
        cursorController.SwitchToDefaultCursor();
        this.gameObject.SetActive(true);
        containerGrid.Init(9, 14, true);
        containerName.text = "Ground";
        containerGrid.LoadItemsFromGround(itemPickUp);

        splitStackButtonPressed = false;
        quickTransferButtonPressed = false;
        quickEquipButtonPressed = false;
    }

    public void CloseInventory(){
        inventoryOpen = false;
        cursorController.SwitchToCrosshairCursor();
        this.gameObject.SetActive(false);
        if(selectedItem != null){
            ReturnItem(selectedItem);
            selectedItem = null;
            HighlightSlot(false);
        }
        if(containerObject != null){
            containerGrid.SaveItemsToContainerObject(containerObject);
            containerGrid.Init(9, 14, true);
            containerObject.Close();
            containerObject = null;
        }else{
            containerGrid.DisableItemsInGrid();
        }
        
        //Close all open windows
        int openWindowCnt = openWindows.Count;
        for(int i = openWindowCnt - 1; i >= 0; i--){
            openWindows[i].GetComponent<InventoryWindow>().CloseWindow();
        }

        if(contextMenuOpen){
            CloseContextMenu();
        }
    }

    public void OpenContainer(ContainerObject container){
        containerObject = container;
        containerGrid.Init(container.gridWidth, container.gridHeight);
        containerGrid.LoadItemsFromContainerObject(container);
        containerName.text = container.containerType;
    }

    /// <summary>
    /// Sets up the debug spawn dropdown menu
    /// </summary>
    private void SetUpDebugDropdown(){
        List<string> itemNames = new List<string>();

        foreach(ItemData item in items){
            itemNames.Add(item.itemName);
        }

        debugSpawnerDropdown.AddOptions(itemNames);
    }

    //TODO: Debug only, remove later
    /// <summary>
    /// Spawns a random item.
    /// </summary>
    public void QuickSpawnItem(){
        if(selectedItem != null){
            Destroy(selectedItem.gameObject);
        }

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        InventoryItem item = SpawnItem(items[selectedItemID]);
        

        if(item == null){
            return;
        }

        //If the item is stackable, set the stack to a random number between 1 and the max stack
        if(item.itemData.stackable){
            item.SetStack(UnityEngine.Random.Range(1, item.itemData.maxStack));
        }

        selectedItem = item;

    }

    /// <summary>
    /// Spawns an item from the debug dropdown menu.
    /// </summary>
    /// <param name="itemID">ID of the item to spawn</param>
    public void DropdownSpawnItem(int itemID){
        if(selectedItem != null){
            Destroy(selectedItem.gameObject);
        }

        InventoryItem item = SpawnItem(items[itemID]);


        if(item == null){
            return;
        }

        //If the item is stackable, set the stack to a random number between 1 and the max stack
        if(item.itemData.stackable){
            item.SetStack(item.itemData.maxStack);
        }

        selectedItem = item;

    }

    /// <summary>
    /// Spawns an item and sets it as the new selected item.
    /// </summary>
    /// <param name="itemData">The item data of the item to spawn</param>
    /// <returns>Reference to the spawned item</returns>
    public InventoryItem SpawnItem(ItemData itemData){
        InventoryItem item = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItemRectTransform = item.GetComponent<RectTransform>();
        selectedItemRectTransform.SetParent(canvasTransform);

        item.Set(itemData);

        return item;
    }

    /// <summary>
    /// Handles left clicks in the inventory. This method is called both when the left click is pressed and released
    /// </summary>
    /// <param name="value">The value of the left click input</param>
    public void InventoryLeftClick(InputValue value){

        // If the mouse is over a window top and is pressed, select that window top
        // if a window top is hovered but the mouse is not pressed, deselect the window top
        //NOTE: It may seem like a better idea to just store the hovered over window and if the left click is pressed,
        // call the drag function. However due to the fact that the mouse can be moved too quickly and the window top
        // is narrow, the mouse can easily leave the window top between frames and the window is no longere hovered over
        // (in short, without this, the player cannot drag windows around quickly)
        if(hoveredWindowTop != null){
            if(value.isPressed){
                selectedWindowTop = hoveredWindowTop;
            }else{
                selectedWindowTop.prevMousePos = new Vector2(-1, -1);
                selectedWindowTop = null;
            }
        }

        //If the left click currently pressed (this function was called on the release of left click),
        // stop dragging the selected window
        if(!value.isPressed){
            if(selectedWindowTop != null){
                selectedWindowTop.prevMousePos = new Vector2(-1, -1);
                selectedWindowTop = null;
            }
        }

        
        //If a context menu is open but the mouse is not over it when clicking, close the context menu
        if(contextMenuOpen){
            if(!mouseOverContextMenu){
                CloseContextMenu();
                
            }
            return;
        }

        //If the mouse is not over a grid or slot, there is nothing more to be done
        if(selectedGrid == null && selectedSlot == null){
            return;
        }

        //If the mouse is over a grid
        if(selectedGrid != null){
            //Get the tile over which the mouse is positioned
            Vector2Int tilePosition = GetTilePosition();

            //If no item is selected
            if(selectedItem == null){
                //If the player is attempting a quick transfer
                if(quickTransferButtonPressed){
                    //If the mouse button is pressed down, transfer the item
                    if(value.isPressed){
                        QuickTransfer(selectedGrid.GetItem(tilePosition.x, tilePosition.y), selectedGrid, selectedSlot);
                    }
                    SaveInventoryGrids();
                    return;
                }
                //If the player is attempting a quick equip
                if(quickEquipButtonPressed){
                    //If the mouse button is pressed down, equip the item
                    if(value.isPressed){
                        QuickEquip(selectedGrid.GetItem(tilePosition.x, tilePosition.y), selectedGrid);
                    }
                    SaveInventoryGrids();
                    return;
                }
                //If the player is not attempting any quick actions, grab the item from the grid
                GrabItemFromGrid(value, tilePosition.x, tilePosition.y);

            }else{
                //If an item was selected and mouse was either pressed or released, place the item in the grid
                PlaceItemToGrid(tilePosition.x, tilePosition.y);  
            }
        
        //If an equipment slot is hovered over
        }else if(selectedSlot != null){
            //If no item is selected
            if(selectedItem == null){
                //if player is attempting a quick transfer or quick equip (unequip in this case), transfer item
                if(quickTransferButtonPressed || quickEquipButtonPressed){
                    if(value.isPressed){
                        QuickTransfer(selectedSlot.GetItem(), selectedGrid, selectedSlot);
                    }
                    SaveInventoryGrids();
                    return;
                }
                //If not, grab the item from the slot
                GrabItemFromSlot(value);
            }else{
                //If an item was selected and mouse was either pressed or released, place the item in the slot
                PlaceItemToSlot();
            }
        }
        SaveInventoryGrids();

        

    }

    /// <summary>
    /// Handles quick split stack hotkey
    /// </summary>
    /// <param name="value">Input value of the hotkey</param>
    public void SplitStackHotkey(InputValue value){
        splitStackButtonPressed = value.isPressed;
    }

    /// <summary>
    /// Handles quick transfer hotkey
    /// </summary>
    /// <param name="value">Input value of the hotkey</param>
    public void QuickTransferHotkey(InputValue value){
        quickTransferButtonPressed = value.isPressed;
    }

    /// <summary>
    /// Handles quick equip hotkey
    /// </summary>
    /// <param name="value">Input value of the hotkey</param>
    public void QuickEquipHotkey(InputValue value){
        quickEquipButtonPressed = value.isPressed;
    }


    /// <summary>
    /// Handles right clicks in the inventory.
    /// </summary>
    public void RightClick(){

        //If a context menu is open, close it, do execute button presses on right click
        if(contextMenuOpen){
            CloseContextMenu();
        }

        //If the mouse is not over a grid or slot, there is nothing more to be done
        if(selectedGrid == null && selectedSlot == null){
            return;
        }

        //If an item is selected, right click should do nothing
        if(selectedItem != null){
            return;
        }

        //If the mouse is over a grid
        if(selectedGrid != null){
            //Get the item on the position which was right clicked, if there was an item open context menu
            Vector2Int tilePosition = GetTilePosition();
            InventoryItem clickedItem = selectedGrid.GetItem(tilePosition.x, tilePosition.y);
            if(clickedItem != null){
                OpenContextMenu(clickedItem);
            }
        }

        //If the mouse is over a slot, and it has an item in it, open the context menu
        if(selectedSlot != null){
            InventoryItem clickedItem = selectedSlot.GetItem();
            if(clickedItem != null){
                OpenContextMenu(clickedItem);
            }
        }

    }

    /// <summary>
    /// Handles grabbing items from an equipment slot
    /// </summary>
    /// <param name="value">Input value of the left click</param>
    private void GrabItemFromSlot(InputValue value){
        if(value.isPressed){
            selectedItem = selectedSlot.GrabItem();
            //If an item was removed from the slot, remove potential container grids,
            // remove the potential image from the outline, unset the item as equipped and remove the
            // item from the equipped items dictionary
            //If it was a weapon, also update weapon selection
            if(selectedItem != null){
                selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
                selectedItemRectTransform.SetAsLastSibling();
                ToggleContainerGrid(selectedItem, false);
                selectedItem.isEquipped = false;
                RemoveOutlineSprite(selectedItem);
                RemoveEquippedItemFromDict(selectedItem);
                if(selectedItem.itemData.weapon){
                    WeaponSelectUpdate();
                }

                if(selectedItem.itemData.healthItem){
                    HealthItemRemove();
                }
            }
        }
        //The slot is highlighted 
        HighlightSlot(true);
    }

    /// <summary>
    /// Handles placing items to an equipment slot
    /// </summary>
    private void PlaceItemToSlot(){
        //If the slot was successfully filled, add the potential container grid, add the potential image to the outline,
        // set the item as equipped and add the item to the equipped items dictionary
        //If it was a weapon, also update weapon selection
        if(selectedSlot.PlaceItem(selectedItem)){
            ToggleContainerGrid(selectedItem, true);
            AddOutlineSprite(selectedItem);
            if(selectedItem.itemData.equipment){
                selectedItem.isEquipped = true;
                AddEquippedItemToDict(selectedItem);
            }
            
            if(selectedItem.itemData.weapon){
                WeaponSelectUpdate();
            }
            
            if(selectedSlot.isHealthStatusSlot){
                ApplyHealthItemToSlot();
            }

            if(selectedItem.itemData.magazine){
                DestroyItem(selectedItem);
            }

            //The item is no longer selected and the slot is no longer highlighted
            if(!selectedItem.itemData.stackable){
                selectedItem = null;
            }
            HighlightSlot(false);

        }
        
        //Update the weapon HUD regardless of whether an item was placed or not
        //This is done to update the hud when a magazine or ammo is placed in the equipped weapon
        hudController.UpdateWeaponHUD(GetSelectedWeapon());
          
    }

    /// <summary>
    /// Handles grabbing items from a grid.
    /// </summary>
    /// <param name="value">Input value of the mouse button</param>
    /// <param name="posX">X position of the clicked tile from which an item is grabbed.</param>
    /// <param name="posY">Y position of the clicked tile from which an item is grabbed.</param>
    private void GrabItemFromGrid(InputValue value, int posX, int posY){
        if(value.isPressed){
            InventoryItem clickedItem = selectedGrid.GetItem(posX, posY);
            //If an item was clicked
            if(clickedItem != null){
                //If the player attempted to split the stack, split the stack, otherwise just grab the item
                if(SplittingStack(clickedItem)){
                    SplitStack(clickedItem);
                    SaveInventoryGrids();
                }else{
                    selectedItem = selectedGrid.GrabItem(posX, posY);
                }
                selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
                selectedItemRectTransform.SetAsLastSibling();
                //If the item had a container window openned, close it (prevents the player from storing the item in itself)
                if(selectedItem.isOpened){
                    CloseContainerItemWindow(selectedItem);
                }

                previousGrid = selectedGrid;
                previousGridPosition = new Vector2Int(selectedItem.gridPositionX, selectedItem.gridPositionY);

                
            }
        }
        //Potentially highlight an equipment slot
        HighlightSlot(true);
    }

    /// <summary>
    /// Handles placing items to a grid
    /// </summary>
    /// <param name="posX">X position of the tile onto which the item is being placed (upper left corner tile of the item is used as reference point).</param>
    /// <param name="posY">Y position of the tile onto which the item is being placed (upper left corner tile of the item is used as reference point).</param>
    private void PlaceItemToGrid(int posX, int posY){
        if(selectedGrid.PlaceItem(selectedItem, posX, posY)){
            PlayInventoryAudio(selectedItem.itemData.name + "PlaceSound");
            selectedItem = null;
            HighlightSlot(false);
            selectedGrid.SaveItems();
            
        }
        
    }

    /// <summary>
    /// Highlights an equipment slot if a selected item is an equipment
    /// </summary>
    /// <param name="highlight">Whether the slot should be highlighted or not</param>
    private void HighlightSlot(bool highlight){
        if(!highlight){
            for(int i = 1; i < itemSlots.Length; i++){
                itemSlots[i].GetComponent<Image>().color = Color.white;
            }

            foreach(string bodyPart in playerStatusUI.bodyPartHighlightObjects.Keys){
                playerStatusUI.BodyPartHighlight(bodyPart, false);
            }
            return;
        
        }
        if(selectedItem == null){
            return;
        }

        if(!selectedItem.itemData.equipment && !selectedItem.itemData.healthItem){
            return;
        }
    

        if(selectedItem.itemData.healthItem){
            List<string> highlightedBodyParts = playerStatus.GetRelevantBodyParts(selectedItem.itemData.itemName);
            foreach(string bodyPart in highlightedBodyParts){
                playerStatusUI.BodyPartHighlight(bodyPart, true);
            }
        }else{
            itemSlots[selectedItem.itemData.equipmentType].GetComponent<Image>().color = Color.green;
        }
        


    }

    /// <summary>
    /// Handles highlighting of items. THe highlighter is white, greem or red depending on certain conditions.
    /// </summary>
    private void HighlightItem(){
        
        //If no grid is selected, there is nothing to highlight
        if(selectedGrid == null){
            return;
        }

        Vector2Int positionOnGrid = GetTilePosition();
        //Variable that determines whether the item should be highlighted in green or red
        bool highlightGreen = false;
        
        //If no item is selected, and the mouse is over an item, highlight the item in white
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
        //If an item is selected, set the highlighter to the size of the selected item
        }else{
            highlighter.SetSize(selectedItem);
            highlighter.SetPosition(selectedGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);

            //If the item passes the boundary check
            if(selectedGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.Width, selectedItem.Height)){
                bool stackableFlag = false;
                InventoryItem overlappingItem = null;
                //If the item passes the overlap check or some other instance where the highlighter should allow overlap
                //set the highlighter to green
                //TODO: This will be expanded and moved to a function as there are quite a few overlap exceptions
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

            //Set the color of the highlighter
            if(highlightGreen){
                highlighter.SetColor(Color.green);
            }else{
                highlighter.SetColor(Color.red);
            }
            
            //Show the highlighter
            highlighter.Show(selectedGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.Width, selectedItem.Height));
        }
    }

    /// <summary>
    /// Get the tile position of a grid based on the mouse position
    /// </summary>
    /// <returns>Integer vector of the tile position</returns>
    private Vector2Int GetTilePosition(){
        //Mouse offset when dragging item to the top left tile of the item
        Vector2 position = Input.mousePosition;
        if(selectedItem != null){
            float tileDimension = ItemGrid.tileDimension;
            //Since tile dimensions are tied to the resolution, we need to scale their size if the resolution is not 1920x1080
            if(Screen.width != 1920 || Screen.height != 1080){
                tileDimension = (Screen.width / 1920f) * tileDimension;
            }
            position.x -= (selectedItem.Width - 1) * (tileDimension/2);
            position.y += (selectedItem.Height - 1) * (tileDimension/2);
        }
        return selectedGrid.GetTilePosition(position);
    }


    /// <summary>
    /// Handles the rotation of an item.
    /// </summary>
    public void RotateItem(){
        if(selectedItem == null){
            return;
        }

        selectedItem.Rotate();

    }

    /// <summary>
    /// Adds an item to the dictionary of equipped items.
    /// </summary>
    /// <param name="item">Reference to the item</param>
    public void AddEquippedItemToDict(InventoryItem item){
        //0 means that the item is not an equipment
        if(item.itemData.equipmentType == 0){
            return;
        }
        //If the dictionary contains the key for the equipment type, add the item to the dictionary
        //NOTE: This is a bit redundant and in retrospect, the equipment types shoudl've been a string type from the beginning
        if(equippedItems.ContainsKey(equipmentTypes[item.itemData.equipmentType])){
            equippedItems[equipmentTypes[item.itemData.equipmentType]] = item;
        }
    }

    /// <summary>
    /// Removes an item from the dictionary of equipped items
    /// </summary>
    /// <param name="item">Reference to the item</param>
    public void RemoveEquippedItemFromDict(InventoryItem item){
        if(item.itemData.equipmentType == 0){
            return;
        }
        if(equippedItems.ContainsKey(equipmentTypes[item.itemData.equipmentType])){
            equippedItems[equipmentTypes[item.itemData.equipmentType]] = null;
        }
    }

    /// <summary>
    /// Turns the grid of a container equipment item (in the middle of the inventory screen) on or off.
    /// </summary>
    /// <param name="item">Reference to the item</param>
    /// <param name="create">Whether the grid should be created or destroyed</param>
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

            //Grid in this case is the collection of all the individual grids of the container
            //(for example a chest rig is composed of six 1x2 grids)
            GameObject grids = containerGrid.transform.Find("Grid").gameObject;

            //Bind grids to parent object
            int childCount = grids.transform.childCount;
            for(int i = 0; i < childCount; i++){
                Transform child = grids.transform.GetChild(i);
                ItemGrid itemGrid = child.GetComponent<ItemGrid>();
                itemGrid.parentItem = item;
                itemGrid.LoadItemsFromContainerItem();
                
            }

            //Ordering container grids based on equipment type
            //This is done so that the container grids are always ordered: Chest Rig, Torso Top Layer, Legs Top Layer and Backpack
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
            //This is needed, so that the higlighter does not get destroyed with the grid
            highlighter.SetHighlighterParent(tempGrid);

            //Destroyed prefab
            GameObject prefab = inventoryContent.transform.Find(item.itemData.containerPrefab.name).gameObject;


            //Saving items of the container
            SaveContainerItems(prefab);

            //Destroying container grid
            Destroy(prefab);
        }
    }

    /// <summary>
    /// Adds the outline sprite to the equipment outline when a clothing item is equipped.
    /// </summary>
    /// <param name="item">Reference to the equipped item</param>
    public void AddOutlineSprite(InventoryItem item){
        if(item.itemData.clothing){
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().sprite = item.itemData.outlineSprite;
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().enabled = true;
        }
    }

    /// <summary>
    /// Removes the outline sprite from the equipment outline when a clothing item is unequipped
    /// </summary>
    /// <param name="item">Reference to the unequipped item</param>
    public void RemoveOutlineSprite(InventoryItem item){
        if(item.itemData.clothing){
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().enabled = false;
        }
    }

    /// <summary>
    /// Saves the items of a container item when it is unequipped.
    /// </summary>
    /// <param name="container">Reference to the container gameobject located in the center of the inventory screen.</param>
    private void SaveContainerItems(GameObject container){

        if(container == null){
            return;
        }

        //Get the gamobject that contains all the grids of the container
        GameObject grids = container.transform.Find("Grid").gameObject;
        
        //Loop through children and individually save the items
        int childCount = grids.transform.childCount;
        for(int i = 0; i < childCount; i++){
            Transform child = grids.transform.GetChild(i);
            ItemGrid grid = child.GetComponent<ItemGrid>(); 
            grid.SaveItems();
            grid.HideItems();
        }
    }

    //
    /// <summary>
    /// Check if the player is attempting to split a stack.
    /// </summary>
    /// <param name="item">Reference to the item which is about to be split.</param>
    /// <returns>True if the player is attempting to split a stack, false otherwise.</returns>
    private bool SplittingStack(InventoryItem item){
        if(item.itemData.stackable){
            if(splitStackButtonPressed){
                if(item.currentStack > 1){
                    return true;
                }
            }
        }
    return false;
    }

    /// <summary>
    /// Opens context menu after right clicking an item.
    /// </summary>
    /// <param name="item">Reference to the item to which the context menu belongs.</param>
    private void OpenContextMenu(InventoryItem item){
        contextMenu = Instantiate(contextMenuPrefab, canvasTransform);
        contextMenu.transform.position = Input.mousePosition;
        ContextMenu contextMenuComponent = contextMenu.GetComponent<ContextMenu>();
        contextMenuComponent.Init(item, selectedGrid, selectedSlot, this);
        contextMenuOpen = true;
    }

    /// <summary>
    /// Closes context menu. Resets the cursor to the default cursor.
    /// </summary>
    public void CloseContextMenu(){
        Destroy(contextMenu);
        contextMenuOpen = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        SaveInventoryGrids();
    }


    /// <summary>
    /// Splits the stack of an item in half, sets the new half of the stack as the selected item.
    /// </summary>
    /// <param name="item">Reference to the item being split</param>
    public void SplitStack(InventoryItem item){
        InventoryItem newItem = SpawnItem(item.itemData);
        newItem.SetStack(item.currentStack/2);
        item.RemoveFromStack(newItem.currentStack);
        selectedItem = newItem;
    }

    //
    //itemGrid OR itemSlot will be set as the "parent" of the item
    //Unlike usual, selectedGrid and selectedSlot cannot be used because this function is called from the context menu as well,
    // and therefore no grid or slot is hovered over
    /// <summary>
    /// Quickly transfers items between grids.
    /// </summary>
    /// <param name="item">Reference to the item being transferred</param>
    /// <param name="itemGrid">Reference to the grid the item is being transferred from</param>
    /// <param name="itemSlot">Reference to the slot the item is being transferred from</param>
    public void QuickTransfer(InventoryItem item, ItemGrid itemGrid, ItemSlot itemSlot){

        //If an item is selected (dragged around) quick transfer is disabled
        if(selectedItem != null){
            return;
        }

        //If no item was clicked nothing happens
        if(item == null){
            return;
        }

        //If the item had a container window open, close it
        if(item.isOpened){
            CloseContainerItemWindow(item);
        }


        if(itemGrid != null){
            //Equipment grid -> Container/Ground grid transfer
            if(itemGrid.parentItem != null){
                if(AttemptTransferToContainer(item, itemGrid, itemSlot))
                    return;
                

            //Container/Ground grid -> Equipment grid transfer
            }else{
                if(AttemptTransferToInventory(item, itemGrid, itemSlot))
                    return;
            }
        }

        if(itemSlot != null){
            //Equipment slot -> Inventory or Container/Ground grid transfer
            //Works due to the fact that expression is evaluated from left to right
            if(AttemptTransferToInventory(item, itemGrid, itemSlot) || AttemptTransferToContainer(item, itemGrid, itemSlot)){
                ToggleContainerGrid(item, false);
                RemoveOutlineSprite(item);
                RemoveEquippedItemFromDict(item);
                item.isEquipped = false;
                //If the transfered item was a weapon from the weapon slot, update weapon select
                if(item.itemData.equipmentType == 11 || item.itemData.equipmentType == 12){
                    WeaponSelectUpdate();
                }
                return;
            }
        }
    }

    /// <summary>
    /// Attempt transfer to a container or ground (right side of the inventory screen).
    /// </summary>
    /// <param name="item">Reference to the item to be transfered</param>
    /// <param name="itemGrid">Reference to the grid the item is currently in</param>
    /// <param name="itemSlot">Reference to the slot the item is currently in</param>
    /// <param name="grabItem">Whether the item should be removed from the parent grid/slot (if false, the item does not have a parent grid/slot)</param>
    /// <returns>True if the item was transfered, false otherwise</returns>
    private bool AttemptTransferToContainer(InventoryItem item, ItemGrid itemGrid, ItemSlot itemSlot, bool grabItem = true){
        //If the item was opened, close it (may be redundant, won't risk it)
        if(item.isOpened){
            CloseContainerItemWindow(item);
        }
        int posX;
        int posY;
        //If a space was found for the item
        if(containerGrid.FindSpaceForItem(item, out posX, out posY)){
            InventoryItem transferedItem = null;
            //If the item is supposed to be grabbed, grab it, if not just assign it
            //While transfered item is the clicked item in euther case, calling the grab
            //functions is necessary for removing item references and such
            if(grabItem){
                if(itemGrid != null){
                    transferedItem = itemGrid.GrabItem(item.gridPositionX, item.gridPositionY);
                }
                if(itemSlot != null){
                    transferedItem = itemSlot.GrabItem();
                }
            }else{
                transferedItem = item;
            }
            //Place the item into the found position
            containerGrid.PlaceItem(transferedItem, posX, posY);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Attempts to transfer an intem into the player's inventory.
    /// </summary>
    /// <param name="item">Reference to the item to be transfered</param>
    /// <param name="itemGrid">Reference to the grid the item is currently in</param>
    /// <param name="itemSlot">Reference to the slot the item is currently in</param>
    /// <returns>True if the item was transfered, false otherwise</returns>
    private bool AttemptTransferToInventory(InventoryItem item, ItemGrid itemGrid, ItemSlot itemSlot){
        if(equippedItems["ChestRig"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["ChestRig"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, item, itemGrid, itemSlot))
                return true;
        }
        if(equippedItems["TorsoTopLayer"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["TorsoTopLayer"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, item, itemGrid, itemSlot))
                return true;
        }
        if(equippedItems["LegsTopLayer"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["LegsTopLayer"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, item, itemGrid, itemSlot))
                return true;
        }
        if(equippedItems["Backpack"] != null){
            GameObject container = inventoryContent.transform.Find(equippedItems["Backpack"].itemData.containerPrefab.name).gameObject;
            if(AttempTransferToEquippedItem(container, item, itemGrid, itemSlot))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to transfer an item into a specific equipped container item.
    /// </summary>
    /// <param name="container">Reference to the container GameObject</param>
    /// <param name="item">Reference to the item to be transfered</param>
    /// <param name="itemGrid">Reference to grid the item is currently in</param>
    /// <param name="itemSlot">Reference to slot the item is currently in</param>
    /// <param name="grabItem">Whether the item should be removed from the parent grid/slot (if false, the item does not have a parent grid/slot)</param>
    /// <returns>True if the item was transfered, false otherwise</returns>
    private bool AttempTransferToEquippedItem(GameObject container, InventoryItem item, ItemGrid itemGrid, ItemSlot itemSlot, bool grabItem = true){
        int posX;
        int posY;
        GameObject grids = container.transform.Find("Grid").gameObject;
        int childCount = grids.transform.childCount;
        for(int i = 0; i < childCount; i++){
            Transform child = grids.transform.GetChild(i);
            ItemGrid grid = child.GetComponent<ItemGrid>();
            //If the item that is to be transfered is one of the equipped container items, skip it
            if(grid.parentItem == item){
                continue;
            }
            //If a space was found for the item, transfer it
            if(grid.FindSpaceForItem(item, out posX, out posY)){
                InventoryItem transferedItem = null;
                if(grabItem){
                    if(itemGrid != null){
                        transferedItem = itemGrid.GrabItem(item.gridPositionX, item.gridPositionY);
                    }
                    if(itemSlot != null){
                        transferedItem = itemSlot.GrabItem();
                    }
                }else{
                    transferedItem = item;
                }
                
                grid.PlaceItem(transferedItem, posX, posY);
                grid.SaveItems();
                PlayInventoryAudio(transferedItem.itemData.name + "PlaceSound");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Quickly equips an item into an equipment slot if possible.
    /// </summary>
    /// <param name="item">Reference to the item.</param>
    /// <param name="itemGrid">Reference to the item's parent grid.</param>
    public void QuickEquip(InventoryItem item, ItemGrid itemGrid){
        //Quick equip does not work if the player has a selected item
        if(selectedItem != null){
            return;
        }

        if(item == null){
            return;
        }

        if(itemGrid == null){
            return;
        }

        //If the item was opened, close it
        if(item.isOpened){
            CloseContainerItemWindow(item);
        }

        AttemptToEquipItem(item, itemGrid);

    }

    /// <summary>
    /// Attempts to equip an item into an equipment slot
    /// </summary>
    /// <param name="item">Reference to the item.</param>
    /// <param name="itemGrid">Reference to the item's parent grid.</param>
    /// <returns></returns>
    private bool AttemptToEquipItem(InventoryItem item, ItemGrid itemGrid){
        if(item.itemData.equipment){
            //If the equipment slot for this item is already occupied, don't do anything
            if(equippedItems[equipmentTypes[item.itemData.equipmentType]] != null){
                return false;
            }
            InventoryItem equippedItem = itemGrid.GrabItem(item.gridPositionX, item.gridPositionY);
            itemSlots[equippedItem.itemData.equipmentType].GetComponent<ItemSlot>().PlaceItem(equippedItem);
            equippedItems[equipmentTypes[equippedItem.itemData.equipmentType]] = equippedItem;
            ToggleContainerGrid(equippedItem, true);
            AddOutlineSprite(equippedItem);
            AddEquippedItemToDict(equippedItem);
            equippedItem.isEquipped = true;
            //If a weapon was equipped, update weapon select
            if(equippedItem.itemData.equipmentType == 11 || equippedItem.itemData.equipmentType == 12){
                WeaponSelectUpdate();
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// Opens the inventory container window for container item.
    /// </summary>
    /// <param name="item">Reference to the item.</param>
    public void OpenContainerItemWindow(InventoryItem item){
        if(item == null){
            return;
        }

        if(item.itemData.containerPrefab == null){
            return;
        }
        //Instantiate the window, initialize it, add it into the list of opened windows and set the item as opened
        GameObject inventoryWindow = Instantiate(inventoryContainerWindowPrefab);
        inventoryWindow.transform.SetParent(canvasTransform);
        inventoryWindow.transform.position = Input.mousePosition;
        InventoryWindow windowScript = inventoryWindow.GetComponent<InventoryWindow>();
        windowScript.Init(item, this, containerWindow: true);
        windowScript.LoadContainerGrid();
        openWindows.Add(inventoryWindow);
        item.isOpened = true;

    }
    /// <summary>
    /// Closes the container window associated with an item.
    /// </summary>
    /// <param name="item">Reference to the item.</param>
    public void CloseContainerItemWindow(InventoryItem item){
        GameObject saveWindow = null;
        //Find the window corresponding to the item
        foreach(GameObject window in openWindows){
            InventoryWindow windowScript = window.GetComponent<InventoryWindow>();
            //If the found window is not the container window (it is the info window) skip it
            if(!windowScript.containerWindow){
                continue;
            }
            
            if(windowScript.item == item){
                saveWindow = window;
                //Find the container part of the window and save its items
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

    /// <summary>
    /// Opens an item info window for an item.
    /// </summary>
    /// <param name="item">Reference to the item.</param>
    public void OpenItemInfoWindow(InventoryItem item){
        if(item == null){
            return;
        }
        //Instantiate the window, initialize it, add it into the list of opened windows and set the item's infoOpened variable to true
        GameObject inventoryWindow = Instantiate(inventoryInfoWindowPrefab);
        inventoryWindow.transform.SetParent(canvasTransform);
        inventoryWindow.transform.position = Input.mousePosition;
        InventoryWindow windowScript = inventoryWindow.GetComponent<InventoryWindow>();
        windowScript.Init(item, this, containerWindow: false);
        windowScript.LoadItemInfo();
        openWindows.Add(inventoryWindow);
        item.infoOpened = true;
    }


    /// <summary>
    /// Closes an item info window associated with an item.
    /// </summary>
    /// <param name="item">Reference to the item.</param>
    public void CloseItemInfoWindow(InventoryItem item){
        GameObject saveWindow = null;
        foreach(GameObject window in openWindows){
            InventoryWindow windowScript = window.GetComponent<InventoryWindow>();
            //If the window is a container window, skip it
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

    
    /// <summary>
    /// Finds ammunition for a magazine and fills it as much as possible.
    /// </summary>
    /// <param name="magazine">Reference to the magazine.</param>
    public void LoadAmmoIntoMagazine(InventoryItem magazine){
        //If the magazine is already full, don't do anything
        if(magazine.ammoCount == magazine.itemData.magazineSize){
            return;
        }

        //While the magazine is not full, find ammo and add it to the magazine
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
            if(ammo.currentStack == 0){
                if(containerObject == null)
                    itemPickUp.DestroyItemObject(ammo);
            }   

        }    
    }

    /// <summary>
    /// Unloads all ammunition from a magazine. Sets the extracted ammunition as the selected item.
    /// </summary>
    /// <param name="magazine">Reference to the magazine</param>
    public void UnloadAmmoFromMagazine(InventoryItem magazine){
        //If the magazine is empty, don't do anything
        if(magazine.ammoCount == 0){
            return;
        }

        //Spawn the ammo and set its stack to the removed amount
        int removedAmount = magazine.RemoveAllFromMagazine();
        InventoryItem ammo = SpawnItem(magazine.itemData.ammoItemData);
        ammo.SetStack(removedAmount);
        selectedItem = ammo;
    }

    /// <summary>
    /// Unloads all ammunition from a firearm that uses an internal magazine, does not remove the chambered round. Sets the extracted ammunition as the selected item.
    /// </summary>
    /// <param name="firearm">Reference to the firearm</param>
    public void UnloadAmmoFromFirearm(InventoryItem firearm){
        //If the firearm is empty, don't do anything
        if(firearm.ammoCount == 0){
            return;
        }

        //Spawn the ammo and set its stack to the removed amount
        int removedAmount = firearm.UnloadAmmoFromInternalMagazine();
        InventoryItem ammo = SpawnItem(firearm.itemData.ammoItemData);
        ammo.SetStack(removedAmount);
        selectedItem = ammo;

        //If the firearm is equipped and selected, update the weapon HUD
        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }

    }

    /// <summary>
    /// Finds ammunition for a firearm that uses an internal magazine and fills it as much as possible.
    /// </summary>
    /// <param name="firearm">Reference to the firearm</param>
    public void LoadAmmoIntoFirearm(InventoryItem firearm){
        //If the firearm is already full, don't do anything
        if(firearm.ammoCount == firearm.itemData.internalMagSize){
            return;
        }

        //While the firearm is not full, find ammo and add it to the firearm
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
            if(ammo.currentStack == 0){
                if(containerObject == null)
                    itemPickUp.DestroyItemObject(ammo);
            }  
        }

        //If the firearm is equipped and selected, update the weapon HUD
        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }
    }

    /// <summary>
    /// Finds a magazine for a firearm.
    /// </summary>
    /// <param name="weaponType">Weapon type of the firearm used to identify the corresponding magazine.</param>
    /// <param name="quickReload">If true, only the pockets and chest rig are checked.</param>
    /// <returns>Reference to the found magazine.</returns>
    public InventoryItem FindMagazine(string weaponType, bool quickReload){
        InventoryItem bestMagazine = null;
        //Minus one so that a magazine with 0 ammo is selected if no other is found
        //This could be done with ">=" when checking for bestAmmoCount but that would cause the last found magazine to be used instead of the first one
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
            //Skip backpack if this is called during quick reload (pressing reload button while not in inventory)
            if(quickReload){
                if(item.Value.itemData.equipmentType == 6){
                    continue;
                }
            }
            //Get container
            GameObject container = inventoryContent.transform.Find(item.Value.itemData.containerPrefab.name).gameObject;
            //Get object with all grids
            GameObject grids = container.transform.Find("Grid").gameObject;
            int childCount = grids.transform.childCount;
            //Loop through individual grids and attempt to fins the best (fullest) magazine
            for(int i = 0; i < childCount; i++){
                Transform child = grids.transform.GetChild(i);
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

    
    /// <summary>
    /// Finds ammunition for a magazine or a firearm that uses an internal magazine and fills it as much as possible
    /// </summary>
    /// <param name="weaponType">Weapon type used to identify the correct ammunition</param>
    /// <param name="quickReload">If true, only the pockets and chest rig are checked</param>
    /// <returns>Reference to the found ammunition item</returns>
    public InventoryItem FindAmmo(string weaponType, bool quickReload = false){

        ItemGrid currentGrid = null;
        InventoryItem ammo = null;

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
            //Skip backpack if this is called during quick reload (pressing reload button while not in inventory)
            if(quickReload){
                if(item.Value.itemData.equipmentType == 6){
                    continue;
                }
            }
            //Loop through all the grids of the container and find the first fitting ammo
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

    /// <summary>
    /// Attempts to place magazine into the inventory. The backpack is excluded due to the fact that this method is called when reloading in-game.
    /// </summary>
    /// <param name="magazine">Reference to the magazine to be placed</param>
    /// <returns>True if the magazine was placed, false if not</returns>
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

    /// <summary>
    /// Removes a magazine from the weapon during in-game reload.
    /// </summary>
    /// <param name="firearm">Reference to the firearm.</param>
    public void ReloadRemoveMagazine(InventoryItem firearm){
        int ammoCount = firearm.RemoveMagazine();
        InventoryItem magazine = SpawnItem(firearm.itemData.magazineItemData);
       
        magazine.AddToMagazine(ammoCount);

        //In this case the removed magazine should not be the selected item but should be automatically transfered to pockets
        if(!AttemptMagPlaceToInventory(magazine)){
            AttemptTransferToContainer(magazine, null, null, false);
        }

        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }
        
    }

    /// <summary>
    /// Finds the best magazine for a firearm and attaches it during a reload.
    /// </summary>
    /// <param name="firearm">Reference to the firearm.</param>
    /// <param name="quickReload">If true, only pockets and chest rig are checked for magazines.</param>
    public void AttachMagazine(InventoryItem firearm, bool quickReload){
        InventoryItem magazine = FindMagazine(firearm.itemData.weaponType, quickReload);
        if(magazine == null){
            return;
        }

        firearm.AttachMagazine(magazine);

        if(containerObject == null){
            itemPickUp.DestroyItemObject(magazine);
        }

        Destroy(magazine.gameObject);

        PlayInventoryAudio(firearm.itemData.weaponType + "LoadMagazine");

        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }
        
    }


    /// <summary>
    /// Removes magazine from a firearm and sets it as the selected item.
    /// </summary>
    /// <param name="firearm">Reference to the firearm.</param>
    public void RemoveMagazine(InventoryItem firearm){
        int ammoCount = firearm.RemoveMagazine();
        InventoryItem magazine = SpawnItem(firearm.itemData.magazineItemData);
        selectedItem = magazine;
        magazine.AddToMagazine(ammoCount);

        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }

        PlayInventoryAudio(firearm.itemData.weaponType + "UnloadMagazine");

    }

    /// <summary>
    /// Loads a single round into a firearm. Used during the reload of weapons with internal magazines.
    /// </summary>
    /// <param name="firearm">Reference to the firearm.</param>
    /// <returns>True if the round was loaded, false if there was no ammo in the inventory.</returns>
    public bool LoadRound(InventoryItem firearm){
        InventoryItem ammo = FindAmmo(firearm.itemData.weaponType);
        if(ammo == null){
            return false;
        }

        if(firearm.LoadAmmoIntoInternalMagazine(1) > 0){
            ammo.RemoveFromStack(1);
            if(firearm.isEquipped && firearm.isSelectedWeapon){
                hudController.UpdateWeaponHUD(firearm);
            }
            if(ammo.currentStack == 0){
                if(containerObject == null)
                    itemPickUp.DestroyItemObject(ammo);
            }  
            return true;
        }else{
            return false;
        }

        
    }

    public void OpenBolt(InventoryItem firearm){
        if(firearm == null)
            return;
        if(firearm.OpenBolt()){
            InventoryItem ammo = SpawnItem(firearm.itemData.ammoItemData);
            ammo.SetStack(1);
            selectedItem = ammo;
        }
        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }

        PlayInventoryAudio(firearm.itemData.weaponType + "BoltOpen");
    }

    public void CloseBolt(InventoryItem firearm){
        if(firearm == null)
            return;
        firearm.CloseBolt();
        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }

        PlayInventoryAudio(firearm.itemData.weaponType + "BoltClose");
    }

    /// <summary>
    /// Loads a single round, out of the inventory or container, into the chanber of the firearm.
    /// </summary>
    /// <param name="firearm">Reference to the firearm.</param>
    public void ChamberRound(InventoryItem firearm){
        InventoryItem ammo = FindAmmo(firearm.itemData.weaponType);
        if(ammo == null){
            return;
        }

        if(firearm.ChamberRound()){
            ammo.RemoveFromStack(1);
        }

        //Since stacks destroy themselves on count 0, we need to destroy the item object if the stack is 0 in case
        // the player is chambering the last round from the ground
        if(ammo.currentStack == 0){
            if(containerObject == null){
                itemPickUp.DestroyItemObject(ammo);
            }
        }

        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }
    }

    /// <summary>
    /// Clears the chamber of the firearm and the round is set as the selected item.
    /// </summary>
    /// <param name="firearm">Reference to the firearm.</param>
    public void ClearChamber(InventoryItem firearm){
        firearm.ClearChamber();
        InventoryItem ammo = SpawnItem(firearm.itemData.ammoItemData);
        ammo.SetStack(1);
        selectedItem = ammo;
        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }
    }

    /// <summary>
    /// Loads a round into the chamber from the magazine of a firearm.
    /// </summary>
    /// <param name="firearm">Reference to the firearm.</param>
    public void RackFirearm(InventoryItem firearm){
        if(firearm == null){
            return;
        }
        if(firearm.isChambered){
            firearm.ClearChamber();
            InventoryItem ammo = SpawnItem(firearm.itemData.ammoItemData);
            ammo.SetStack(1);
            selectedItem = ammo;
        }
        firearm.ChamberFromMagazine();


        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }

        PlayInventoryAudio(firearm.itemData.weaponType + "Rack");
    }

    public void UseItem(InventoryItem item){
        if(item == null){
            return;
        }

        if(item.itemData.consumable){
            playerStatus.IncreaseHunger(item.itemData.hunger);
            playerStatus.IncreaseThirst(item.itemData.thirst);
            playerStatus.IncreaseTiredness(item.itemData.tiredness);

            if(item.itemData.healthItem){
                playerStatus.UseHealthItem(item.itemData);
            }

            if(item.itemData.stackable){
                if(item.RemoveFromStack(1) == 0){
                    DestroyItem(item);
                }
            }else{
                DestroyItem(item);
            }
        }
    }

    /// <summary>
    /// Get the currently selected weapon.
    /// </summary>
    /// <returns>Reference to the currently selected weapon.</returns>
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

    /// <summary>
    /// Gets the selected weapon slot.
    /// </summary>
    /// <returns>Reference to the selected weapon slot.</returns>
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

    /// <summary>
    /// Updates which weapon is currently selected. The method is used to notify other scripts of the change.
    /// </summary>
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

        //Send message to firearm script, HUD controller and if the inventory is open when this change happens, play the quick weapon change animation
        InventoryItem selectedWeapon = GetSelectedWeapon();
        firearmScript.ChangeSelectedFirearm(selectedWeapon);
        hudController.UpdateWeaponHUD(selectedWeapon);
        if(inventoryOpen){
            playerAnimationController.InventoryQuickWeaponChangeAnimation(selectedWeapon);
        }
        

    }

    /// <summary>
    /// Cycle through weapons using a mouse wheel.
    /// </summary>
    /// <param name="value">Mouse wheel input (120 for scroll up, -120 for scroll down)</param>
    /// <returns>Reference to the selected weapon.</returns>
    public InventoryItem CycleWeapon(InputValue value){
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

        return GetSelectedWeapon();

        
    }

    /// <summary>
    /// Selectes or deselects a weapon.
    /// </summary>
    /// <param name="value">Which weapon slot was selected/deselected.</param>
    /// <returns>Reference to the selected weapon.</returns>
    public InventoryItem SelectWeapon(int value){
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

        return GetSelectedWeapon();
        
    }

    /// <summary>
    /// Fires a round from the currently selected weapon
    /// </summary>
    /// <returns>True if the round was fired, false otherwise.</returns>
    public bool FireRound(){
        InventoryItem selectedWeapon = GetSelectedWeapon();
        if(selectedWeapon == null){
            return false;
        }

        if(selectedWeapon.FireRound()){
            if(selectedWeapon.isEquipped && selectedWeapon.isSelectedWeapon){
                hudController.UpdateWeaponHUD(selectedWeapon);
            }
            return true;
        }else{
            return false;
        }
    }
    /// <summary>
    /// Destroys an item.
    /// </summary>
    /// <param name="item">Item to be destroyed.</param>
    public void DestroyItem(InventoryItem item){
        if(item.isEquipped){
            ToggleContainerGrid(item, false);
            RemoveOutlineSprite(item);
            RemoveEquippedItemFromDict(item);
            if(item.itemData.weapon && item.isSelectedWeapon){
                WeaponSelectUpdate();
            }

        }
        //Can be safely called without check for containerGrid, if the item was not an item on the ground, nothing happens
        //This check just saves time looping through items on the ground if a container is opened
        // (as in that case there is surely nothing being destroyed on the ground)
        if(containerObject == null)
            itemPickUp.DestroyItemObject(item);
        Destroy(item.gameObject);
        if(item.isOpened){
            CloseContainerItemWindow(item);
        }
        CloseItemInfoWindow(item);
        CloseContextMenu();
    }

    /// <summary>
    /// Switches the firemode of a firearm.
    /// </summary>
    /// <param name="firearm">Firearm that has its fire mode changed.</param>
    /// <returns>Currently selected fire mode.</returns>
    public string SwitchFiremode(InventoryItem firearm){
        if(firearm == null){
            return null;
        }

        string firemode = firearm.SwitchFiremode();
        if(firearm.isEquipped && firearm.isSelectedWeapon){
            hudController.UpdateWeaponHUD(firearm);
        }
        return firemode;
    }


    private void ReturnItem(InventoryItem item){
        if(previousGrid != null){
            previousGrid.PlaceItem(item, previousGridPosition.x, previousGridPosition.y);
            previousGrid.SaveItems();
            previousGrid = null;
            previousGridPosition = Vector2Int.zero;
            return;
        }
        if(previousSlot != null){
            previousSlot.PlaceItem(item);
            previousSlot = null;
            return;
        }

        DestroyItem(item);

    }

    public void DropItem(InventoryItem item){
        //Instantiate a loose item gameobject with random rotation and slight position variation from the player
        //Slight position variation also helps due to the fact that the player can fill the ground grid with items
        //but when the grid is loaded on inventory open, the items have their rotations and grid positions reset. Therefore
        //not all items may fit. This way the player can move around a bit have some items appear in the ground grid an some not.
        GameObject dropedItem = Instantiate(looseItemPrefab, new Vector3(player.transform.position.x + Random.Range(-5f, 5f),
                                            player.transform.position.y + Random.Range(-5f, 5f), player.transform.position.z),
                                            Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        dropedItem.GetComponent<LooseItem>().Init(item);

    }

    public void PickUpItem(InventoryItem item){
        itemPickUp.DestroyItemObject(item);
    }

    private void PlayInventoryAudio(string name){
        if(inventoryOpen){
            audioManager.Play(name, this.gameObject);
        }
    }

    private void HealthItemRemove(){
        if(selectedItem.itemData.itemName == "Clean Bandage" || selectedItem.itemData.itemName == "Dirty Bandage"){
            playerStatus.RemoveBandage(selectedSlot.bodyPart);
        }
    }

    private void ApplyHealthItemToSlot(){
        if(selectedItem.itemData.itemName == "Clean Bandage"){
            playerStatus.ApplyBandage(selectedSlot.bodyPart, selectedItem);
        }

        if(selectedItem.itemData.itemName == "Dirty Bandage"){
            playerStatus.ApplyBandage(selectedSlot.bodyPart, selectedItem);
        }

        if(selectedItem.itemData.itemName == "Suture Needle"){
            playerStatus.StitchWound(selectedSlot.bodyPart);
        }

        if(selectedItem.itemData.itemName == "Antiseptic"){
            playerStatus.Disinfect(selectedSlot.bodyPart);  
        }
    }

    public void SwapForDirtyBandage(string bodyPart, ItemData dirtyBandageItemData){
        GameObject healthItemSlotObject = playerStatusUI.gameObject.transform.Find("Outline/OutlineSlots/" + bodyPart + "Slot").gameObject;
        ItemSlot healthItemSlot = healthItemSlotObject.GetComponent<ItemSlot>();

        //just in case
        if(healthItemSlot.GetItem().itemData.itemName == "Clean Bandage"){
            healthItemSlot.DeleteItem();
            playerStatus.RemoveBandage(bodyPart);
            InventoryItem dirtyBandage = SpawnItem(dirtyBandageItemData);
            healthItemSlot.PlaceItem(dirtyBandage);
            playerStatus.ApplyBandage(bodyPart, dirtyBandage);
        }
    }

    private void UpdateCarryWeight(){
        float carryWeight = 0;
        foreach(InventoryItem item in equippedItems.Values){
            if(item != null){
                carryWeight += item.currentWeight;
            }
        }

        playerStatus.currentCarryWeight = carryWeight;
        float carryWeightRounded = Mathf.Round(carryWeight * 1000f) / 1000f;
        float maxCarryWeightRounded = Mathf.Round(playerStatus.maxCarryWeight * 1000f) / 1000f;
        carryWeightText.text = carryWeightRounded + "/" + maxCarryWeightRounded;
    }

    private void SaveInventoryGrids(){
        foreach(Transform equippedItem in inventoryContent.transform){
            GameObject grids = equippedItem.transform.GetChild(1).gameObject;
            foreach(Transform grid in grids.transform){
                grid.GetComponent<ItemGrid>().SaveItems();
            }
        }
    }
}
