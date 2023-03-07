using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{

    private GameObject HUDCanvas;
    private HUDController hudController;

    public ItemData itemData;

    public InventoryItem[][,] itemGrids;


    public int gridPositionX;
    public int gridPositionY;

    public int currentStack;
    public GameObject itemSubtext;

    public bool isEquipped = false;
    public bool isSelectedWeapon = false;
    public bool isOpened = false;

    //For magazines
    public int ammoCount = 0;

    //For weapons
    public int currentMagazineSize = 0;

    public bool isChambered = false;
    public bool hasMagazine = false;

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

        if(itemData.stackable || itemData.magazine || itemData.firearm){
            InstantiateSubtext();
        }

        GetComponent<Image>().sprite = itemData.inventorySprite;

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(Width * ItemGrid.tileDimension, Height * ItemGrid.tileDimension);
        rectTransform.localScale = Vector2.one;

        HUDCanvas = GameObject.Find("HUD");
        hudController = HUDCanvas.GetComponent<HUDController>();

        if(itemData.firearm){
            if(!itemData.usesMagazines){
                currentMagazineSize = itemData.internalMagSize;
            }
        }

        
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

        if(itemSubtext != null){
            itemSubtext.GetComponent<ItemSubtext>().UpdatePosition();
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

        if(currentStack == 0){
            Destroy(gameObject);
        }


        UpdateStackText();
        return currentStack;

    }

    private void UpdateStackText(){
        if(itemSubtext != null){
            itemSubtext.GetComponent<ItemSubtext>().UpdateText(currentStack.ToString());
        }
    }

    public int LoadAmmoIntoInternalMagazine(int count){
        int ammoLoaded = 0;
        if(ammoCount + count > itemData.internalMagSize){
            int prevCount = ammoCount;
            ammoCount = itemData.internalMagSize;
            UpdateFirearmText();
            ammoLoaded = (itemData.internalMagSize - prevCount);
        }else{
            ammoCount += count;
            UpdateFirearmText();
            ammoLoaded =  count;
        }

        if(isEquipped && isSelectedWeapon){
            hudController.UpdateWeaponHUD(this);
        }

        return ammoLoaded;

        
    }

    public int UnloadAmmoFromInternalMagazine(){
        int ammoUnloaded = ammoCount;
        ammoCount = 0;
        UpdateFirearmText();
        if(isEquipped && isSelectedWeapon){
            hudController.UpdateWeaponHUD(this);
        }
        return ammoUnloaded;
    }

    
    public int AddToMagazine(int count){
        if(ammoCount + count > itemData.magazineSize){
            int prevCount = ammoCount;
            ammoCount = itemData.magazineSize;
            UpdateMagazineText();
            return (itemData.magazineSize - prevCount);

        }else{
            ammoCount += count;
            UpdateMagazineText();
            return count;
        }
    }

    public int RemoveAllFromMagazine(){
        int count = ammoCount;
        ammoCount = 0;
        UpdateMagazineText();
        return count;
    }

    private void UpdateMagazineText(){
        if(itemSubtext != null){
            itemSubtext.GetComponent<ItemSubtext>().UpdateText(ammoCount.ToString() + "/" + itemData.magazineSize.ToString()); ;
        }
    }

    private void UpdateFirearmText(){
        if(itemSubtext != null){
            if(isChambered){
                itemSubtext.GetComponent<ItemSubtext>().UpdateText(ammoCount.ToString() + "+1"  + "/" + currentMagazineSize.ToString());
            }else{
                itemSubtext.GetComponent<ItemSubtext>().UpdateText(ammoCount.ToString() + "/" + currentMagazineSize.ToString());
            }
        }
    }

    private void InstantiateSubtext(){
        itemSubtext = Instantiate(this.itemData.itemSubtextPrefab, transform, false);
        if(itemData.stackable){
            UpdateStackText();
        }
        if(itemData.magazine){
            UpdateMagazineText();
        }

        if(itemData.firearm){
            UpdateFirearmText();
        }

        itemSubtext.GetComponent<ItemSubtext>().item = this;
        itemSubtext.GetComponent<ItemSubtext>().UpdatePosition();
    }

    public void AttachMagazine(InventoryItem magazine){
        hasMagazine = true;
        ammoCount = magazine.ammoCount;
        currentMagazineSize = magazine.itemData.magazineSize;
        UpdateFirearmText();
        SelectSprite(1);
        if(isEquipped && isSelectedWeapon)
            hudController.UpdateWeaponHUD(this);
    }

    public int RemoveMagazine(){
        hasMagazine = false;
        int count = ammoCount;
        ammoCount = 0;
        currentMagazineSize = 0;
        UpdateFirearmText();
        SelectSprite(0);
        if(isEquipped && isSelectedWeapon)
            hudController.UpdateWeaponHUD(this);
        return count;
    }

    public bool ChamberRound(){
        if(isChambered){
            return false;
        }
        isChambered = true;
        UpdateFirearmText();
        if(isEquipped && isSelectedWeapon)
            hudController.UpdateWeaponHUD(this);
        return true;
    }

    public bool ClearChamber(){
        if(!isChambered){
            return false;
        }
        isChambered = false;
        UpdateFirearmText();
        if(isEquipped && isSelectedWeapon)
            hudController.UpdateWeaponHUD(this);
        return true;
    }

    public bool RackFirearm(){
        if(isChambered || ammoCount == 0){
            return false;
        }
        isChambered = true;
        ammoCount--;
        UpdateFirearmText();
        if(isEquipped && isSelectedWeapon)
            hudController.UpdateWeaponHUD(this);
        return true;
    }

    public bool FireRound(){
        if(!isChambered){
            return false;
        }
        isChambered = false;
        UpdateFirearmText();
        if(isEquipped && isSelectedWeapon)
            hudController.UpdateWeaponHUD(this);
        return true;
    }

    public bool ChamberFromMagazine(){
        if(isChambered || ammoCount == 0){
            return false;
        }
        isChambered = true;
        ammoCount--;
        UpdateFirearmText();
        if(isEquipped && isSelectedWeapon)
            hudController.UpdateWeaponHUD(this);
        return true;
    }

    private void SelectSprite(int selector){
        if(selector == 0){
            GetComponent<Image>().sprite = itemData.inventorySprite;
        }else if(selector == 1){
            GetComponent<Image>().sprite = itemData.inventorySpriteSecondary;
        }
    }

    


}
