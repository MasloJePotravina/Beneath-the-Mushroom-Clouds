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

    private InventoryItem selectedItem;
    private InventoryItem highlightedItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight highlighter;
    Vector2Int higlighterOldPos;


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
        }

        HighlightItem();
        
    }

    //TODO: Debug only, remove later
    public void SpawnItem(){
        InventoryItem item = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = item;
        rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        item.Set(items[selectedItemID]);

    }

    public void InventoryLeftClick(InputValue value){
        if(selectedGrid == null){
            return;
        }

        Vector2Int tilePosition = GetTilePosition();

        if(selectedItem == null){
            GrabItem(value, tilePosition.x, tilePosition.y);
        }else{
            PlaceItem(value, tilePosition.x, tilePosition.y);  
        }

    }

    private void GrabItem(InputValue value, int posX, int posY){
        if(value.isPressed){
            selectedItem = selectedGrid.GrabItem(posX, posY);
            if(selectedItem != null){
                rectTransform = selectedItem.GetComponent<RectTransform>();
            }
        }
    }

    private void PlaceItem(InputValue value, int posX, int posY){
        if(selectedGrid.PlaceItem(selectedItem, posX, posY)){
            selectedItem = null;
        }
    }

    private void HighlightItem(){

        if(selectedGrid == null){
            return;
        }

        Vector2Int positionOnGrid = GetTilePosition();
        if(positionOnGrid == higlighterOldPos){
            return;
        }
        if(selectedItem == null)
        {
            highlightedItem = selectedGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if(highlightedItem != null){
                highlighter.SetSize(highlightedItem);
                highlighter.SetPosition(selectedGrid, highlightedItem);
                highlighter.Show(true);
            }else{
                highlighter.Show(false);
            }
        }else{
            highlighter.SetSize(selectedItem);
            highlighter.SetPosition(selectedGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
            highlighter.Show(selectedGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.itemData.width, selectedItem.itemData.height));
        }
    }

    private Vector2Int GetTilePosition(){
        //Mouse offset when dragging item
        Vector2 position = Input.mousePosition;
        if(selectedItem != null){
            position.x -= (selectedItem.itemData.width - 1) * (ItemGrid.tileDimension/2);
            position.y += (selectedItem.itemData.height - 1) * (ItemGrid.tileDimension/2);
        }

        return selectedGrid.GetTilePosition(position);
    }

    
}
