using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{

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
            }
        }
    }

    private void PlaceItemToSlot(InputValue value){
        if(selectedSlot.PlaceItem(selectedItem)){
            selectedItem = null;
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
    }

    private void PlaceItemToGrid(InputValue value, int posX, int posY){
        if(selectedGrid.PlaceItem(selectedItem, posX, posY)){
            selectedItem = null;
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

    
}
