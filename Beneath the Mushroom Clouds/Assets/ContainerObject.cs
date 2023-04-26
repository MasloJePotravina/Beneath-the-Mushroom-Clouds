using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerObject : MonoBehaviour
{
    public bool wasOpened = false;
    public bool isOpen = false;
    public string containerType;
    [SerializeField] private ItemData[] possibleItems;
    [SerializeField] private bool animated;
    public int gridWidth;
    public int gridHeight;
    private InventoryItem[,] items;

    [SerializeField] private GameObject itemPrefab;

    private InventoryController inventoryController;


    private PlayerControls playerControls;

    private Animator animator;

    [SerializeField] private AnimatorOverrideController animatorOverrideController;

    void Awake(){
        inventoryController = GameObject.FindObjectOfType<InventoryController>(true);
        playerControls = GameObject.FindObjectOfType<PlayerControls>();
        animator = GetComponent<Animator>();
        if(animated)
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

    //We only need width here since item are placed row by row
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
        RectTransform itemTransform = item.GetComponent<RectTransform>();
        itemTransform.localScale = Vector2.zero;
        itemTransform.rotation = Quaternion.identity;
        item.Set(itemData);
        for(int i = 0; i < itemData.width; i++){
            for(int j = 0; j < itemData.height; j++){
                items[x + i, y + j] = item;
            }
        }

        item.gridPositionX = x;
        item.gridPositionY = y;

        if(itemData.stackable){
            item.SetStack(Random.Range(1, itemData.maxStack));
        }
        
    }

    public void Open(){
        isOpen = true;
        if(!wasOpened){
            wasOpened = true;
            GenerateRandomItems();
        }

        playerControls.OnOpenInventory();
        inventoryController.OpenContainer(this);
        if(animated){
            animator.ResetTrigger("close");
            animator.SetTrigger("open");
        }
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
        if(animated){
            animator.ResetTrigger("open");
            animator.SetTrigger("close");
        }
    }



}
