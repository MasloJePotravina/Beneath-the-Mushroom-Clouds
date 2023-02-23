using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{



    //EquipmentType zero is reserved for non equippable items and therefore is never initilized in this field
    [SerializeField] private GameObject[] itemSlots;


    Dictionary<string, bool> equipmentOn = new Dictionary<string, bool>{
        {"ChestRig", false},
        {"TorsoTopLayer", false},
        {"LegsTopLayer", false},
        {"Backpack", false}
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
        {12, "SecondaryWeapon"}
    };

    [SerializeField] private GameObject inventoryContent;

    //Highlighter is set to be the child of this testGrid to avoid destruction when removing clothing items
    [SerializeField] private ItemGrid tempGrid;

    [SerializeField] private GameObject equipmentOutline;

    public bool inventoryOpen = false;

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

    }

    public void InventoryLeftClick(InputValue value){
        if(selectedGrid == null && selectedSlot == null){
            return;
        }

        if(selectedGrid != null){
            Vector2Int tilePosition = GetTilePosition();

            if(selectedItem == null){
                GrabItemFromGrid(value, tilePosition.x, tilePosition.y);
            }else{
                PlaceItemToGrid(value, tilePosition.x, tilePosition.y);  
            }
        }else if(selectedSlot != null){
            if(selectedItem == null){
                GrabItemFromSlot(value);
            }else{
                PlaceItemToSlot(value);
            }
        }

    }

    private void GrabItemFromSlot(InputValue value){
        if(value.isPressed){
            selectedItem = selectedSlot.GrabItem();
            if(selectedItem != null){
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
                if(selectedItem.itemData.container){
                    ToggleContainerGrid(selectedItem, false);
                }
                RemoveOutlineSprite(selectedItem);
            }
        }
        HighlightSlot(true);
    }

    private void PlaceItemToSlot(InputValue value){
        if(selectedSlot.PlaceItem(selectedItem)){
            if(selectedItem.itemData.container){
                ToggleContainerGrid(selectedItem, true);
            }
            AddOutlineSprite(selectedItem);
            selectedItem = null;
            HighlightSlot(false);

        }
        
    }

    private void GrabItemFromGrid(InputValue value, int posX, int posY){
        if(value.isPressed){
            selectedItem = selectedGrid.GrabItem(posX, posY);
            if(selectedItem != null){
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
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
                if(selectedGrid.OverlapCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.Width, selectedItem.Height)){
                    highlighter.SetColor(Color.green);
                }else{
                    highlighter.SetColor(Color.red);
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
            position.x -= (selectedItem.Width - 1) * (ItemGrid.tileDimension/2);
            position.y += (selectedItem.Height - 1) * (ItemGrid.tileDimension/2);
        }

        return selectedGrid.GetTilePosition(position);
    }

    public void RotateItem(){
        if(selectedItem == null){
            return;
        }

        selectedItem.Rotate();

    }

    public void ToggleContainerGrid(InventoryItem item, bool create){
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
                equipmentOn["ChestRig"] = true;
            //If torso top layer
            }else if(item.itemData.equipmentType == 4){
                if(equipmentOn["ChestRig"] == true){
                    containerGrid.transform.SetSiblingIndex(1);
                }else{
                    containerGrid.transform.SetAsFirstSibling();
                }
                equipmentOn["TorsoTopLayer"] = true;
            //If legs top layer
            }else if(item.itemData.equipmentType == 8){
                if(equipmentOn["ChestRig"] == true && equipmentOn["TorsoTopLayer"] == true){
                    containerGrid.transform.SetSiblingIndex(2);
                }else if(equipmentOn["ChestRig"] == true || equipmentOn["TorsoTopLayer"] == true){
                    containerGrid.transform.SetSiblingIndex(1);
                }else{
                    containerGrid.transform.SetAsFirstSibling();
                }
                equipmentOn["LegsTopLayer"] = true;
            //If backpack
            }else{
                containerGrid.transform.SetAsLastSibling();
                equipmentOn["Backpack"] = true;
            }
        //Equipment item is being unequipped
        }else{
            //This is needed, so that the higlighter does not get destroyed
            highlighter.SetHighlighterParent(tempGrid);

            //Resetting bools
            if(item.itemData.equipmentType == 2){
                equipmentOn["ChestRig"] = false;
            }else if(item.itemData.equipmentType == 4){
                equipmentOn["TorsoTopLayer"] = false;
            }else if(item.itemData.equipmentType == 8){
                equipmentOn["LegsTopLayer"] = false;
            }else{
                equipmentOn["Backpack"] = false;
            }

            //Destroyed prefab
            GameObject prefab = inventoryContent.transform.Find(item.itemData.containerPrefab.name).gameObject;


            //Saving items of the container
            SaveContainerItems(prefab);

            //Destroying container grid
            Destroy(prefab);
        }
    }

    private void AddOutlineSprite(InventoryItem item){
        if(item.itemData.clothing){
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().sprite = item.itemData.outlineSprite;
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().enabled = true;
        }
    }

    private void RemoveOutlineSprite(InventoryItem item){
        if(item.itemData.clothing){
            equipmentOutline.transform.Find(equipmentTypes[item.itemData.equipmentType]).GetComponent<Image>().enabled = false;
        }
    }

    private void SaveContainerItems(GameObject container){

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
    
}
