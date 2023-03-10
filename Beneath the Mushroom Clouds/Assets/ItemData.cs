using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class ItemData : ScriptableObject
{

    //Item IDs are used to identify each item in the game, same items will have the same ID (does NOT differentiate individual items)
    //First 2 numbers determine the type of item (100 possible types)
    //Last 4 numbers determine the item itself (10000 possible items per type)
    //A list of item IDs can be found in the ItemIDs.txt file in the Items folder
    //Item Types:
    //00 - Junk/Valuables
    //01 - Consumables
    //02 - Clothing
    //03 - Weapons
    //04 - Magazines
    //05 - Ammo
    //06 - Health items
    //07 - Quest items
    [Header("Required")]
    public int id;
    public string itemName;
    public int width;
    public int height;
    public float weight;
    public float value;
    public string description;

    [Header("Equipment")]
    public bool equipment;
    //0 - reserved for non equppable items
    //1 - head
    //2 - chest rig
    //3 - torso base layer
    //4 - torso top layer
    //5 - gloves
    //6 - backpack
    //7 - legs base layer
    //8 - legs top layer
    //9 - socks
    //10 - boots
    //11 - primary weapon 
    //12 - secondary weapon 
    //13 - watch
    //14 - geiger counter
    public int equipmentType;
    public bool clothing;
    public Sprite outlineSprite;



    [Header("Container")]
    public bool container;
    public int gridAmount;
    [System.Serializable]
    public struct GridData{
        public int gridID;
        public int width;
        public int height;
    }
    public GameObject containerPrefab;
    public GridData[] gridData;
    

    [Header("Weapon")]
    public bool weapon;
    public bool firearm;
    public bool manuallyChambered;
    public bool usesMagazines;
    public int internalMagSize;
    //Used for ammo, magazihnes and weapons for compatibility checks
    public string weaponType;
    public Sprite weaponHUDOutlineSprite;
    public Sprite ammoBarFullSprite;
    public Sprite ammoBarEmptySprite;

    public ItemData ammoItemData;
    //Item data for the magazine is needed to spawn the appropriate magazine type when the magazine is removed from weapon
    public ItemData magazineItemData;

    public bool ammo;
    public bool magazine;
    public int magazineSize;

    [Header("Usable")]
    public bool usable;


    [Header("Misc")]
    public bool stackable;
    public int maxStack;
    public GameObject itemSubtextPrefab;
    public Sprite inventorySprite;
    //Used for example for weapons with and without magazines
    public Sprite inventorySpriteSecondary;



    //Item data for ammuniti0n is needed to spawn the appropriate ammo type when the magazine is emptied
    

    

    

    

    


    
}
