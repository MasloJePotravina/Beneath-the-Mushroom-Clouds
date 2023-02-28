using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public InventoryItem[][,] itemGrids;

    public int gridPositionX;
    public int gridPositionY;

    public int currentStack;
    private GameObject currentStackText;

    public bool isEquipped = false;
    public bool isOpened = false;

    //For magazines
    public int ammoCount = 0;
    private GameObject currentAmmoText;
    

    public int Height{
        get{
            return rotated ? itemData.width : itemData.height;
        }
    }

    public int Width{
        get{
            return rotated ? itemData.height : itemData.width;
        }
    }

    public bool rotated = false;

    public void Set(ItemData itemData){
        this.itemData = itemData;

        if(this.itemData.container){
            itemGrids = new InventoryItem[this.itemData.gridAmount][,];

            for(int i = 0; i < this.itemData.gridAmount; i++){
                itemGrids[i] = new InventoryItem[this.itemData.gridData[i].width, this.itemData.gridData[i].height];
            }
        }

        if(itemData.stackable){
            currentStackText = Instantiate(this.itemData.currentStackTextPrefab, transform, false);
        }

        if(itemData.magazine){
            currentAmmoText = Instantiate(this.itemData.currentStackTextPrefab, transform, false);
            UpdateAmmoText();
        }



        GetComponent<Image>().sprite = itemData.inventorySprite;

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(Width * ItemGrid.tileDimension, Height * ItemGrid.tileDimension);
        rectTransform.localScale = Vector2.one;

    }

    public InventoryItem[,] LoadGrid(int gridID){
        


        return itemGrids[gridID];

        
    }

    public void SaveGrid(int gridID, InventoryItem[,] items){


        itemGrids[gridID] = items;

        foreach(InventoryItem item in itemGrids[gridID]){
            if(item != null){
                item.transform.SetParent(transform);
                item.gameObject.SetActive(false);
            }
        }
        
    }

    public void Rotate(){
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.rotation = Quaternion.Euler(0, 0, rotated ? -90 : 0);


        if(currentStackText != null){
            currentStackText.transform.rotation = Quaternion.Euler(0, 0, 0);
            RectTransform textRect = currentStackText.GetComponent<RectTransform>();

        }

        if(currentAmmoText != null){
            currentAmmoText.transform.rotation = Quaternion.Euler(0, 0, 0);
            
        }
    }

    public void SetStack(int count){
        currentStack = count;
        UpdateStackText();
    }

    public int AddToStack(int count){
        if(currentStack + count > itemData.maxStack){
            int prevStack = currentStack;
            currentStack = itemData.maxStack;
            UpdateStackText();
            return (itemData.maxStack - prevStack);
        }else{
            currentStack += count;
            UpdateStackText();
            return count;
        }


    }

    public int RemoveFromStack(int count){
        if(currentStack - count < 0){
            currentStack = 0;
        }else{
            currentStack -= count;
        }

        UpdateStackText();
        return currentStack;

    }

    private void UpdateStackText(){
        if(currentStackText != null){
            currentStackText.GetComponent<TextMeshProUGUI>().text = currentStack.ToString();
        }
    }

    
    public int AddToMagaize(int count){
        if(ammoCount + count > itemData.magazineSize){
            int prevCount = ammoCount;
            ammoCount = itemData.magazineSize;
            UpdateAmmoText();
            return (itemData.magazineSize - prevCount);

        }else{
            ammoCount += count;
            UpdateAmmoText();
            return count;
        }
    }

    public int RemoveAllFromMagazine(){
        int count = ammoCount;
        ammoCount = 0;
        UpdateAmmoText();
        return count;
    }

    private void UpdateAmmoText(){
        if(currentAmmoText != null){
            currentAmmoText.GetComponent<TextMeshProUGUI>().text = ammoCount.ToString() + "/" + itemData.magazineSize.ToString();
        }
    }


}
