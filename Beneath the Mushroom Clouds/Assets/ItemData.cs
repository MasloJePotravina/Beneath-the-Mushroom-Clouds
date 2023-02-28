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
    public int id;
    public string itemName;

    public int width;
    public int height;

    public bool equipment;
    public bool clothing;
    public bool weapon;

    public bool container;
    public int gridAmount;
    [System.Serializable]
    public struct GridData{
        public int gridID;
        public int width;
        public int height;
    }

    public GridData[] gridData;

    public bool stackable;
    public int maxStack;
    public GameObject currentStackTextPrefab;

    public bool magazine;
    public int magazineSize;
    public string magazineWeaponType;

    public bool usable;

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
    //11 - primary weapon (only)
    //12 - secondary weapon (can be also used as primary)
    //13 - watch
    //14 - geiger counter
    public int equipmentType;

    public GameObject containerPrefab;

    public Sprite outlineSprite;


    public Sprite inventorySprite;
}
