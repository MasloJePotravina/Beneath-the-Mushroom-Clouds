using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class InventoryWindow : MonoBehaviour
{
    public bool containerWindow;
    public InventoryController inventoryController;
    public InventoryItem item;
    [SerializeField] private GameObject windowTop;

    [SerializeField] private GameObject statPrefab;
    [SerializeField] private GameObject leftSide;
    [SerializeField] private GameObject rightSide;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;

    private Dictionary <string, string> stats = new Dictionary<string, string>{};
    
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

    public void Init(InventoryItem item, InventoryController inventoryController, bool containerWindow){
        this.item = item;
        this.inventoryController = inventoryController;
        windowTop.GetComponent<InventoryWindowTop>().inventoryController = inventoryController;
        this.containerWindow = containerWindow;
    }

    public void LoadContainerGrid(){
        //instantiate prefab
        GameObject containerGrid = Instantiate(item.itemData.containerPrefab, transform.Find("Background"));
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

        GameObject containerName = containerGrid.transform.GetChild(0).gameObject;
        containerName.SetActive(false);
        //Change size
        RectTransform rectTransform = containerGrid.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - containerName.GetComponent<RectTransform>().sizeDelta.y);

        itemName.text = item.itemData.itemName;
    }

    public void LoadItemInfo(){
        itemImage.sprite = item.itemData.inventorySprite;
        itemName.text = item.itemData.itemName;
        itemDescription.text = item.itemData.description;
        CreateSlots();
        foreach(KeyValuePair<string, string> stat in stats){
            GameObject statObject;
            if(leftSide.transform.childCount <= rightSide.transform.childCount){
                statObject = Instantiate(statPrefab, leftSide.transform);
            }else{
                statObject = Instantiate(statPrefab, rightSide.transform);
            }

            statObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stat.Key + ":";
            string splitOnCapital = System.Text.RegularExpressions.Regex.Replace(stat.Value, "(\\B[A-Z])", " $1");
            statObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = splitOnCapital;
        }
    }

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

    public void CloseWindow(){
        if(containerWindow){
            inventoryController.CloseContainerItemWindow(item);
        }else{
            inventoryController.CloseItemInfoWindow(item);
        }
    }
}
