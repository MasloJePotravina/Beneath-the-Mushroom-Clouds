using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Implements the behaviour of all inventory items and stores variables that are specific to an instance of an item.
/// </summary>
public class InventoryItem : MonoBehaviour
{
    /// <summary>
    /// Reference to the Item Data scriptable object which holds the data about this type of item.
    /// </summary>
    public ItemData itemData;

    /// <summary>
    /// Array of two dimensional arrays of inventory items. Used to store items that are contained within this item (container items).
    /// </summary>
    public InventoryItem[][,] itemGrids;

    /// <summary>
    /// X position of the item in the inventory grid.
    /// </summary>
    public int gridPositionX;
    /// <summary>
    /// Y position of the item in the inventory grid.
    /// </summary>
    public int gridPositionY;

    /// <summary>
    /// Current stack of the item (if the item is stackable).
    /// </summary>
    public int currentStack;

    /// <summary>
    /// Reference to the text component of the item's subtext (stackable items, magazines and firearms).
    /// </summary>
    public GameObject itemSubtext;

    /// <summary>
    /// Whether the item is equipped or not (equippable items)
    /// </summary>
    public bool isEquipped = false;
    
    /// <summary>
    /// Item is the currently selected weapon (weapon items) 
    /// </summary>
    public bool isSelectedWeapon = false;

    /// <summary>
    /// Whether item's container window is opened (container items)
    /// </summary>
    public bool isOpened = false;

    /// <summary>
    /// Whether item's info window is opened.
    /// </summary>
    public bool infoOpened = false;

    /// <summary>
    /// Current ammo count (firearms and magazines)
    /// </summary>
    public int ammoCount = 0;

    /// <summary>
    /// Current maximum capacity of a firearm based on the inserted magazine (firearms)
    /// </summary>
    public int currentMagazineSize = 0;

    /// <summary>
    /// Whether the item is chambered (firearms)
    /// </summary>
    public bool isChambered = false;

    public bool shellInChamber = false;

    public bool boltOpen = false;

    /// <summary>
    /// Whether the item has a magazine inserted (firearms)
    /// </summary>
    public bool hasMagazine = false;

    /// <summary>
    /// Current index of the fire mode of the item (firearms)
    /// </summary>
    private int currentFireModeIndex;

    /// <summary>
    /// Height of the item in the inventory. If the item is rotated, the width is returned as the height of the item.
    /// </summary>
    public int Height{
        get{
            return rotated ? itemData.width : itemData.height;
        }
    }

    /// <summary>
    /// Width of the item in the inventory. If the item is rotated, the height is returned as the width of the item.
    /// </summary>
    public int Width{
        get{
            return rotated ? itemData.height : itemData.width;
        }
    }

    /// <summary>
    /// Whether the item is rotated or not.
    /// </summary>
    public bool rotated = false;

    /// <summary>
    /// Initializes the necessary values of the item upon spawning.
    /// </summary>
    /// <param name="itemData">Item Data scriptable object which holds the data about this type of item.</param>
    public void Set(ItemData itemData){
        this.itemData = itemData;

        //If the item is a containerm, initialize the itemGrids array.
        if(this.itemData.container){
            itemGrids = new InventoryItem[this.itemData.gridAmount][,];

            for(int i = 0; i < this.itemData.gridAmount; i++){
                itemGrids[i] = new InventoryItem[this.itemData.gridData[i].width, this.itemData.gridData[i].height];
            }
        }

        //If the item is stackable, magazine or firearm, instantiate the subtext.
        if(itemData.stackable || itemData.magazine || itemData.firearm){
            InstantiateSubtext();
        }

        GetComponent<Image>().sprite = itemData.inventorySprites[0];

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(Width * ItemGrid.tileDimension, Height * ItemGrid.tileDimension);
        rectTransform.localScale = Vector2.one;

        //If the item is a firearm, but does not use magazines, set the current magazine size to the internal magazine size of the firearm and select the first fire mode.
        if(itemData.firearm){
            if(!itemData.usesMagazines){
                currentMagazineSize = itemData.internalMagSize;
            }
            currentFireModeIndex = 0;
        }

        
    }

    /// <summary>
    /// Loads the item's grid data from the itemGrids array.
    /// </summary>
    /// <param name="gridID">ID of the grid to load.</param>
    /// <returns>Two dimensional array of inventory item references.</returns>
    public InventoryItem[,] LoadGrid(int gridID){
        return itemGrids[gridID];
    }

    /// <summary>
    /// Saves the item's grid data to the itemGrids array.
    /// </summary>
    /// <param name="gridID">ID of the grid to save.</param>
    /// <param name="items">Two dimensional array of inventory item references to items that are currently stored in the grid.</param>
    public void SaveGrid(int gridID, InventoryItem[,] items){


        itemGrids[gridID] = items;

        foreach(InventoryItem item in itemGrids[gridID]){
            if(item != null){
                item.transform.SetParent(transform);
                item.gameObject.SetActive(false);
            }
        }
        
    }


    /// <summary>
    /// Rotates the item 90 degrees.
    /// </summary>
    public void Rotate(){
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.rotation = Quaternion.Euler(0, 0, rotated ? -90 : 0);

        if(itemSubtext != null){
            itemSubtext.GetComponent<ItemSubtext>().UpdatePosition();
        }


        
    }

    /// <summary>
    /// Sets the item's stack count.
    /// </summary>
    /// <param name="count">New stack count.</param>
    public void SetStack(int count){
        currentStack = count;
        UpdateStackText();
    }

    /// <summary>
    /// Adds the specified amount of items to the stack.
    /// </summary>
    /// <param name="count">Amount of items to add to the stack.</param>
    /// <returns>Amount of items that were really added to the stack.</returns>
    public int AddToStack(int count){
        int amountAdded = 0;
        if(currentStack + count > itemData.maxStack){
            int prevStack = currentStack;
            currentStack = itemData.maxStack;
            amountAdded = itemData.maxStack - prevStack;
        }else{
            currentStack += count;
            amountAdded = count;
        }
        UpdateStackText();
        return amountAdded;
    }

    /// <summary>
    /// Removes the specified amount of items from the stack.
    /// </summary>
    /// <param name="count">Amount of items to remove from the stack.</param>
    /// <returns>Amount of item left in the stack.</returns>
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

    /// <summary>
    /// Updates the stack text of the item's subtext.
    /// </summary>
    private void UpdateStackText(){
        if(itemSubtext != null){
            itemSubtext.GetComponent<ItemSubtext>().UpdateText(currentStack.ToString());
        }
    }

    /// <summary>
    /// Loads the specified amount of ammo into the item's (firearm's) internal magazine.
    /// </summary>
    /// <param name="count">Amount of ammo to load into the internal magazine.</param>
    /// <returns>Amount of ammo that was really loaded into the internal magazine.</returns>
    public int LoadAmmoIntoInternalMagazine(int count){
        int ammoLoaded = 0;
        if(ammoCount + count > itemData.internalMagSize){
            int prevCount = ammoCount;
            ammoCount = itemData.internalMagSize;;
            ammoLoaded = (itemData.internalMagSize - prevCount);
        }else{
            ammoCount += count;
            ammoLoaded =  count;
        }
        UpdateFirearmText();
        return ammoLoaded;    
    }

    /// <summary>
    /// Unloads all ammo from the item's (firearm's) internal magazine.
    /// </summary>
    /// <returns>Amount of ammo that was unloaded from the internal magazine.</returns>
    public int UnloadAmmoFromInternalMagazine(){
        int ammoUnloaded = ammoCount;
        ammoCount = 0;
        UpdateFirearmText();
        return ammoUnloaded;
    }

    /// <summary>
    /// Adds the specified amount of ammo to the item (magazine).
    /// </summary>
    /// <param name="count">Amount of ammo to add to the magazine.</param>
    /// <returns>Amount of ammo that was really added to the magazine.</returns>
    public int AddToMagazine(int count){
        int amountAdded = 0;
        if(ammoCount + count > itemData.magazineSize){
            int prevCount = ammoCount;
            ammoCount = itemData.magazineSize;
            amountAdded = itemData.magazineSize - prevCount;

        }else{
            ammoCount += count;
            amountAdded = count;
        }
        UpdateMagazineText();
        return amountAdded;
    }

    /// <summary>
    /// Removes the all ammo from the item (magazine).
    /// </summary>
    /// <returns>Amount of ammo that was removed from the magazine.</returns>
    public int RemoveAllFromMagazine(){
        int count = ammoCount;
        ammoCount = 0;
        UpdateMagazineText();
        return count;
    }

    /// <summary>
    /// Updates the ammo text of the item's (magazine's) subtext.
    /// </summary>
    private void UpdateMagazineText(){
        if(itemSubtext != null){
            itemSubtext.GetComponent<ItemSubtext>().UpdateText(ammoCount.ToString() + "/" + itemData.magazineSize.ToString()); ;
        }
    }

    /// <summary>
    /// Updates the ammo text of the item's (firearm's) subtext.
    /// </summary>
    private void UpdateFirearmText(){
        if(itemSubtext != null){
            if(isChambered){
                itemSubtext.GetComponent<ItemSubtext>().UpdateText(ammoCount.ToString() + "+1"  + "/" + currentMagazineSize.ToString());
            }else{
                itemSubtext.GetComponent<ItemSubtext>().UpdateText(ammoCount.ToString() + "/" + currentMagazineSize.ToString());
            }
        }
    }

    /// <summary>
    /// Instantiates the item's subtext based on the item type.
    /// </summary>
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
    
    /// <summary>
    /// Attaches a magazine to the item (firearm).
    /// </summary>
    /// <param name="magazine">Reference to the magazine.</param>
    /// <returns>True if the magazine was attached.</returns>
    public bool AttachMagazine(InventoryItem magazine){
        if(hasMagazine){
            return false;
        }
        hasMagazine = true;
        ammoCount = magazine.ammoCount;
        currentMagazineSize = magazine.itemData.magazineSize;
        UpdateFirearmText();
        if(boltOpen){
            FirearmSelectSprite("MagBoltOpen");
        }else{
            FirearmSelectSprite("MagBoltClosed");
        }
        return true;
    }

    /// <summary>
    /// Removes the magazine from the item (firearm).
    /// </summary>
    /// <returns>Amount of ammo that was in the removed magazine.</returns>
    public int RemoveMagazine(){
        hasMagazine = false;
        int count = ammoCount;
        ammoCount = 0;
        currentMagazineSize = 0;
        UpdateFirearmText();
        if(boltOpen){
            FirearmSelectSprite("NoMagBoltOpen");
        }else{
            FirearmSelectSprite("NoMagBoltClosed");
        }
        return count;
    }

    /// <summary>
    /// Chamvers a round into the item (firearm) if it is not already chambered.
    /// </summary>
    /// <returns>True if the round was chambered, false if the item (firearm) was already chambered.</returns>
    public bool ChamberRound(){
        if(isChambered){
            return false;
        }
        isChambered = true;
        UpdateFirearmText();
        return true;
    }

    /// <summary>
    /// Clears the chamber of the item (firearm) if it is chambered.
    /// </summary>
    /// <returns>True if the chamber was cleared, false if the item (firearm) was not chambered.</returns>
    public bool ClearChamber(){
        if(!isChambered){
            return false;
        }
        isChambered = false;
        UpdateFirearmText();
        return true;
    }

    /// <summary>
    /// Fires a round from the item (firearm) if it is chambered. Works identically to ClearChamber() but is called under different circumstances.
    /// </summary>
    /// <returns>True if the round was fired, false if the item (firearm) was not chambered.</returns>
    public bool FireRound(){
        if(!isChambered){
            return false;
        }
        isChambered = false;
        if(itemData.usesMagazines){
            if(ammoCount == 0){
                OpenBolt();
            }
        }
        UpdateFirearmText();
        return true;
    }

    /// <summary>
    /// Chamber a round from the item's (firearm's) internal magazine.
    /// </summary>
    /// <returns>True if a round was chambered, false if the item (firearm) was already chambered or the magazine was empty.</returns>
    public bool ChamberFromMagazine(){
        if(isChambered || ammoCount == 0){
            return false;
        }
        isChambered = true;
        ammoCount--;
        UpdateFirearmText();
        return true;
    }

    public bool OpenBolt(){
        if(!boltOpen){
            boltOpen = true;
            if(hasMagazine){
                FirearmSelectSprite("MagBoltOpen");
            }else{
                FirearmSelectSprite("NoMagBoltOpen");
            }
        }

        if(isChambered){
            isChambered = false;
            UpdateFirearmText();
            return true;
        }else{
            return false;
        }
    }

    public void CloseBolt(){
        if(boltOpen){
            boltOpen = false;
            if(hasMagazine){
                FirearmSelectSprite("MagBoltClosed");
            }else{
                FirearmSelectSprite("NoMagBoltClosed");
            }
        }

        if(ammoCount > 0 && !isChambered){
            ammoCount--;
            isChambered = true;
            UpdateFirearmText();
        }
    }

    /// <summary>
    /// Selects the sprite of the item. Each item may have a primary and secondary sprite. For example weapons have a sprite with and without a magazine.
    /// </summary>
    /// <param name="selector">0 for primary sprite, 1 for secondary sprite.</param>
    private void FirearmSelectSprite(string selector){
        if(selector == "NoMagBoltClosed"){
            GetComponent<Image>().sprite = itemData.inventorySprites[0];
        }else if(selector == "MagBoltClosed"){
            GetComponent<Image>().sprite = itemData.inventorySprites[1];
        }else if(selector == "NoMagBoltOpen"){
            GetComponent<Image>().sprite = itemData.inventorySprites[2];
        }else if(selector == "MagBoltOpen"){
            GetComponent<Image>().sprite = itemData.inventorySprites[3];
        }
    }

    

    /// <summary>
    /// Switches the fire mode of the item (firearm).
    /// </summary>
    /// <returns>The new fire mode of the item (firearm).</returns>
    public string SwitchFiremode(){
        if(itemData.fireModes.Length == 1){
            return itemData.fireModes[0];
        }
        //Choose next fire mode, if this is the last choice, cycle form beginning
        if(currentFireModeIndex == itemData.fireModes.Length - 1){
            currentFireModeIndex = 0;
        }else{
            currentFireModeIndex++;
        }
        return itemData.fireModes[currentFireModeIndex];
    }

    /// <summary>
    /// Gets the current fire mode of the item (firearm).
    /// </summary>
    /// <returns>Current fire mode of the item (firearm).</returns>
    public string GetFiremode(){
        return itemData.fireModes[currentFireModeIndex];
    }
    


}
