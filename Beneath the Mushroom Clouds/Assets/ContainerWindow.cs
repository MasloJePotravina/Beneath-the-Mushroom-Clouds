using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ContainerWindow : MonoBehaviour
{
    public InventoryController inventoryController;
    public InventoryItem item;
    [SerializeField] GameObject windowTop;

    public void Init(InventoryItem item, InventoryController inventoryController){
        this.item = item;
        this.inventoryController = inventoryController;
        windowTop.GetComponent<InventoryWindowTop>().inventoryController = inventoryController;
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

        this.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemData.itemName;
    }

    


}
