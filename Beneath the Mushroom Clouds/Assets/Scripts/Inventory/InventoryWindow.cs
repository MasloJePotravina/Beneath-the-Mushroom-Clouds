using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Implements the behaviour of inventory windows (item info windows and container windows).
/// </summary>
public class InventoryWindow : MonoBehaviour
{
    /// <summary>
    /// Whether the window is a container window. If not, it is an item info window.
    /// </summary>
    public bool containerWindow;

    /// <summary>
    /// Reference to the Inventory Controller.
    /// </summary>
    public InventoryController inventoryController;

    /// <summary>
    /// Reference to the item to which the window belongs.
    /// </summary>
    public InventoryItem item;

    /// <summary>
    /// Top of the window.
    /// </summary>
    [SerializeField] private GameObject windowTop;

    /// <summary>
    /// Prefab which is used to used in Info Windows for the display of item stats.
    /// </summary>
    [SerializeField] private GameObject statPrefab;

    /// <summary>
    /// Reference to the left side of item stats in the Info Window.
    /// </summary>
    [SerializeField] private GameObject leftSide;

    /// <summary>
    /// Reference to the right side of item stats in the Info Window.
    /// </summary>
    [SerializeField] private GameObject rightSide;

    /// <summary>
    /// Image of the item in the Info Window.
    /// </summary>
    [SerializeField] private Image itemImage;

    /// <summary>
    /// Text component which will display the name of the item the window belongs to. It is located at the top of the window.
    /// </summary>
    [SerializeField] private TextMeshProUGUI itemName;

    /// <summary>
    /// Text component which will display the description of the item the window belongs to (used for Info Windows)
    /// </summary>
    [SerializeField] private TextMeshProUGUI itemDescription;

    /// <summary>
    /// List of stats of the item the window belongs to (such as value, weight and so on).
    /// </summary>
    private Dictionary <string, string> stats = new Dictionary<string, string>{};
    
    /// <summary>
    /// Dictionary mapping equipment types to their names.
    /// </summary>
    /// <value></value>
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
    /// Initializes the window.
    /// </summary>
    /// <param name="item">Item to which the window belongs.</param>
    /// <param name="inventoryController">Reference to the Inventory Controller.</param>
    /// <param name="containerWindow">Whether the window is a container window.</param>
    public void Init(InventoryItem item, InventoryController inventoryController, bool containerWindow){
        this.item = item;
        this.inventoryController = inventoryController;
        windowTop.GetComponent<InventoryWindowTop>().inventoryController = inventoryController;
        this.containerWindow = containerWindow;
    }

    /// <summary>
    /// Loads the items into the window grid if the window is a container window.
    /// </summary>
    public void LoadContainerGrid(){
        //Instantiate the container grid and set its parent as the background of the window
        GameObject containerGrid = Instantiate(item.itemData.containerPrefab, transform.Find("Background"));
        //Set the name of the container to the name of the prefab
        containerGrid.name = item.itemData.containerPrefab.name;

        //Get the object containing all of the grids
        GameObject grids = containerGrid.transform.Find("Grid").gameObject;
        //Loop through child grids and set their parent item to the item the window belongs to
        int childCount = grids.transform.childCount;
        for(int i = 0; i < childCount; i++){
            Transform child = grids.transform.GetChild(i);
            ItemGrid itemGrid = child.GetComponent<ItemGrid>();
            itemGrid.parentItem = item;
            itemGrid.LoadItemsFromContainerItem();
        }

        //Find the irst child of the container grid, which is the name of the item and disable it as it is not displayed in the container window
        GameObject containerName = containerGrid.transform.GetChild(0).gameObject;
        containerName.SetActive(false);
        //Resize the container grid prefab to exclude the disabled name of the item
        RectTransform rectTransform = containerGrid.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - containerName.GetComponent<RectTransform>().sizeDelta.y);

        //Display the name of the item in the top of the window instead
        itemName.text = item.itemData.itemName;
    }

    /// <summary>
    /// Loads the information about the item needed for the Info Window.
    /// </summary>
    public void LoadItemInfo(){
        itemImage.sprite = item.itemData.inventorySprites[0];
        itemName.text = item.itemData.itemName;
        itemDescription.text = item.itemData.description;
        CreateSlots();
        //For each stat in the stats dictionary, create a stat prefab and set its text to the stat name and value
        foreach(KeyValuePair<string, string> stat in stats){
            GameObject statObject;
            if(leftSide.transform.childCount <= rightSide.transform.childCount){
                statObject = Instantiate(statPrefab, leftSide.transform);
            }else{
                statObject = Instantiate(statPrefab, rightSide.transform);
            }

            //Format the strings properly
            statObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stat.Key + ":";
            string splitOnCapital = System.Text.RegularExpressions.Regex.Replace(stat.Value, "(\\B[A-Z])", " $1");
            statObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = splitOnCapital;
        }
    }

    /// <summary>
    /// Creates combination of stat names and values.
    /// </summary>
    private void CreateSlots(){
        if(item.itemData.equipment){
            stats.Add("Equipment Type", equipmentTypes[item.itemData.equipmentType]);
        }
        if(item.itemData.weapon){
            stats.Add("Weapon Type", item.itemData.weaponType);
        }
        stats.Add("Weight", item.itemData.weight.ToString() + "kg");
        stats.Add("Value", item.itemData.value.ToString() + "R");
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    public void CloseWindow(){
        if(containerWindow){
            inventoryController.CloseContainerItemWindow(item);
        }else{
            inventoryController.CloseItemInfoWindow(item);
        }
    }
}
