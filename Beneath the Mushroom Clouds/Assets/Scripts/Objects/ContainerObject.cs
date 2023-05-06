using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of container objects.
/// </summary>
public class ContainerObject : MonoBehaviour
{   
    /// <summary>
    /// Whether the container has been opened already. This is reset when items are respawned in the container.
    /// </summary>
    public bool wasOpened = false;

    /// <summary>
    /// Whether this container should periodically respawn loot.
    /// </summary>
    public bool respawnsLoot = true;

    /// <summary>
    /// Whether the container is currently open.
    /// </summary>
    public bool isOpen = false;

    /// <summary>
    /// Container type/name.
    /// </summary>
    public string containerType;

    /// <summary>
    /// Whether the container is animated.
    /// </summary>
    [SerializeField] private bool animated;

    /// <summary>
    /// Width of the grid of the container object.
    /// </summary>
    public int gridWidth;

    /// <summary>
    /// Height of the grid of the container object.
    /// </summary>
    public int gridHeight;

    /// <summary>
    /// 2 dimenstional array of references to items in the container (the item grid).
    /// </summary>
    private InventoryItem[,] items;

    /// <summary>
    /// Prefab for inventory item.
    /// </summary>
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// Reference to the inventory controller.
    /// </summary>
    private InventoryController inventoryController;

    /// <summary>
    /// Reference to the player controls.
    /// </summary>
    private PlayerControls playerControls;

    /// <summary>
    /// Reference to the animator.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Animator override asset which switches the animation of the container object based on what container it is.
    /// </summary>
    [SerializeField] private AnimatorOverrideController animatorOverrideController;

    /// <summary>
    /// Reference to the randomized item generator.
    /// </summary>
    private RandomizedItemGenerator randomizedItemGenerator;

    /// <summary>
    /// Timer for container resetting.
    /// </summary>
    private float containerLootResetTimer = 0f;

    /// <summary>
    /// Time for container resetting.
    /// </summary>
    private float containerLootResetTime = 18000f;

    /// <summary>
    /// Reference to the world status.
    /// </summary>
    private WorldStatus worldStatus;

    /// <summary>
    /// Get all of the necessary references.
    /// </summary>
    void Awake(){
        inventoryController = GameObject.FindObjectOfType<InventoryController>(true);
        playerControls = GameObject.FindObjectOfType<PlayerControls>();
        animator = GetComponent<Animator>();
        if(animated)
            animator.runtimeAnimatorController = animatorOverrideController;

        randomizedItemGenerator = GetComponent<RandomizedItemGenerator>();
        worldStatus = GameObject.FindObjectOfType<WorldStatus>();
    }

    /// <summary>
    /// Each frame determines whether the loot should be respawned.
    /// </summary>
    void Update(){
        if(!respawnsLoot || !wasOpened){
            return;
        }
        //Only start updating the timer after the container has been opened
        if(containerLootResetTimer < containerLootResetTime){
            containerLootResetTimer += Time.deltaTime * worldStatus.timeMultiplier;
        }else{
            ResetContainerLoot();
            containerLootResetTimer = 0f;
        }
    }


    /// <summary>
    /// Spawn all of the forced and randomly generated items.
    /// </summary>
    void SpawnItems(){
        items = new InventoryItem[gridWidth, gridHeight];
        List<ItemData> forcedItems = randomizedItemGenerator.GetForcedItems();
        Dictionary<ItemData, int> generatedItems = randomizedItemGenerator.GenerateRandomItems();

        //First spawn all forced items (if possible)
        foreach(ItemData itemData in forcedItems){
            Vector2Int position = FindSpaceForItem(itemData.width, itemData.height);
            if(position.x == -1){
                Debug.Log("Could not find a position for item " + itemData.itemName);
                break;
            }
            SpawnItem(itemData, position.x, position.y);
        }

        //Then spawn all randomly generated items
        foreach(KeyValuePair<ItemData, int> item in generatedItems){
            for(int i = 0; i < item.Value; i++){
                Vector2Int position = FindSpaceForItem(item.Key.width, item.Key.height);
                if(position.x == -1){
                    Debug.Log("Could not find a position for item " + item.Key.itemName);
                    break;
                }
                SpawnItem(item.Key, position.x, position.y);
            }
        }
    }

    /// <summary>
    /// Boundary check used when spawning items into the grid of the ocntainer object.
    /// </summary>
    /// <param name="x">Whether this X value protrudes from the grid.</param>
    /// <param name="y">Whether this Y value protrudes from the grid.</param>
    /// <returns>True if the item does not protrude from the grid, false otherwise.</returns>
    private bool BoundaryCheck(int x, int y){
        if(x >= gridWidth || y >= gridHeight){
            return false;
        }
        return true;
    }

    //We only need width here since item are placed row by row
    /// <summary>
    /// Overlap check used when spawning items into the grid of the container object.
    /// </summary>
    /// <param name="x">X position of the item on the grid.</param>
    /// <param name="y">Y position of the item on the grid.</param>
    /// <param name="width">Width of the item.</param>
    /// <returns>True if the item does not overlap with an item, false otherwise.</returns>
    private bool OverlapCheck(int x, int y, int width){
        for(int i = 0; i < width; i++){
            if(items[x + i, y] != null){
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Spawn an item into the container grid.
    /// </summary>
    /// <param name="itemData">Item Data of the spawned item.</param>
    /// <param name="x">X position into which the item should be spawned.</param>
    /// <param name="y">Y position into which the item should be spawned.</param>
    private void SpawnItem(ItemData itemData, int x, int y){
        InventoryItem item = Instantiate(itemPrefab, transform).GetComponent<InventoryItem>();
        RectTransform itemTransform = item.GetComponent<RectTransform>();
        itemTransform.localScale = Vector2.zero;
        itemTransform.rotation = Quaternion.identity;

        //Flll the grid tiles taken up by this item to reference the item
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

    /// <summary>
    /// Opens the container item, spawns the items into it if it is opened for the first time in this respawn cycle.
    /// </summary>
    public void Open(){
        isOpen = true;
        if(!wasOpened){
            wasOpened = true;
            SpawnItems();
        }
        //Container object is set before opening the inventory to prevent loading items on the ground around the object
        inventoryController.containerObject = this;
        playerControls.OnOpenInventory();
        inventoryController.OpenContainer(this);
        if(animated){
            animator.ResetTrigger("close");
            animator.SetTrigger("open");
        }
    }

    /// <summary>
    /// Loads the items from the container grid.
    /// </summary>
    /// <returns>2D array which holds the references to the items stored in the grid.</returns>
    public InventoryItem[,] LoadGrid(){
        return items;
    }

    /// <summary>
    /// Saves the items into the container's grid.
    /// </summary>
    /// <param name="items">2D array of the items to be saved.</param>
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

    /// <summary>
    /// Closes the container.
    /// </summary>
    public void Close(){
        isOpen = false;
        if(animated){
            animator.ResetTrigger("open");
            animator.SetTrigger("close");
        }
    }

    /// <summary>
    /// Finds a space for an item basedo n its width and height.
    /// </summary>
    /// <param name="itemWidth">Width of the item that is being placed.</param>
    /// <param name="itemHeight">Height of the item that is being placed.</param>
    /// <returns></returns>
    private Vector2Int FindSpaceForItem(int itemWidth, int itemHeight){
        for(int i = 0; i < gridHeight; i++){
            for(int j = 0; j < gridWidth; j++){
                if(BoundaryCheck(j + itemWidth - 1, i + itemHeight - 1) && OverlapCheck(j, i, itemWidth)){
                    return new Vector2Int(j, i);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Resets the container's loot.
    /// </summary>
    public void ResetContainerLoot(){
        if(isOpen){
            return;
        }

        foreach(InventoryItem item in items){
            if(item != null){
                Destroy(item.gameObject);
            }
        }
        items = null;
        wasOpened = false;
    }

}
