using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerObject : MonoBehaviour
{
    public bool wasOpened = false;
    public bool isOpen = false;
    [SerializeField] private string containerType;
    [SerializeField] private ItemData[] possibleItems;
    public int gridWidth;
    public int gridHeight;
    private InventoryItem[,] items;

    [SerializeField] private GameObject itemPrefab;

    private GameObject inventoryScreen;

    private InventoryController inventoryController;

    private GameObject player;

    private UIControls uiControls;

    private Animator animator;

    [SerializeField] private AnimatorOverrideController animatorOverrideController;

    void Awake(){
        inventoryScreen = GameObject.Find("InventoryScreen");
        inventoryController = inventoryScreen.GetComponent<InventoryController>();

        player = GameObject.Find("Player");
        uiControls = player.GetComponent<UIControls>();

        animator = GetComponent<Animator>();

        animator.runtimeAnimatorController = animatorOverrideController;


    }



    void GenerateRandomItems(){
        items = new InventoryItem[gridWidth, gridHeight];
        int itemsToSpawn = Random.Range(1, gridWidth * gridHeight);
        int itemsSpawned = 0;
        for(int i = 0; i < gridHeight; i++){
            for(int j = 0; j < gridWidth; j++){
                int randomIndex = Random.Range(0, possibleItems.Length);
                ItemData itemData = possibleItems[randomIndex];
                if(BoundaryCheck(j + itemData.width - 1, i + itemData.height - 1) && OverlapCheck(j, i, itemData.width)){
                    SpawnItem(itemData, j, i);
                    itemsSpawned++;
                    if(itemsSpawned >= itemsToSpawn){
                        return;
                    }
                }
            }
        }
    }

    private bool BoundaryCheck(int x, int y){
        if(x >= gridWidth || y >= gridHeight){
            return false;
        }
        return true;
    }

    private bool OverlapCheck(int x, int y, int width){
        for(int i = 0; i < width; i++){
            if(items[x + i, y] != null){
                return false;
            }
        }
        return true;
    }

    private void SpawnItem(ItemData itemData, int x, int y){
        InventoryItem item = Instantiate(itemPrefab, transform).GetComponent<InventoryItem>();
        item.Set(itemData);
        for(int i = 0; i < itemData.width; i++){
            for(int j = 0; j < itemData.height; j++){
                items[x + i, y - j] = item;
            }
        }

        item.gridPositionX = x;
        item.gridPositionY = y;

        if(itemData.stackable){
            item.SetStack(Random.Range(1, itemData.maxStack));
        }
        RectTransform itemTransform = item.GetComponent<RectTransform>();
        itemTransform.localScale = Vector2.zero;
    }

    public void Open(){
        isOpen = true;
        if(!wasOpened){
            wasOpened = true;
            GenerateRandomItems();
        }

        uiControls.OnOpenInventory();
        inventoryController.OpenContainer(this);
        animator.ResetTrigger("close");
        animator.SetTrigger("open");


    }

    public InventoryItem[,] LoadGrid(){
        return items;
    }

    public void SaveGrid(InventoryItem[,] items){
        this.items = items;
        foreach(InventoryItem item in items){
            if(item != null){
                item.transform.SetParent(transform);
                RectTransform itemTransform = item.GetComponent<RectTransform>();
                itemTransform.localScale = Vector2.zero;
                item.gameObject.SetActive(false);
            }
        }
    }

    public void Close(){
        isOpen = false;
        animator.ResetTrigger("open");
        animator.SetTrigger("close");
    }

}
