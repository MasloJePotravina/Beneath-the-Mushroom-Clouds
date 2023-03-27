using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
/// <summary>
/// A scriptable object which contains the data variables for all items. All items of the same type will share variables stored in this scriptable object.
/// </summary>
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
    /// <summary>
    /// ID of the item.
    /// </summary>
    public int id;
    /// <summary>
    /// Name of the item.
    /// </summary>
    public string itemName;
    /// <summary>
    /// Width of the item in tiles.
    /// </summary>
    public int width;
    /// <summary>
    /// Height of the item in tiles.
    /// </summary>
    public int height;
    /// <summary>
    /// Weight of the item in kg.
    /// </summary>
    public float weight;
    /// <summary>
    /// Value of the item in Roubles.
    /// </summary>
    public float value;
    /// <summary>
    /// Description of the item.
    /// </summary>
    [TextAreaAttribute] public string description;

    [Header("Equipment")]
    /// <summary>
    /// Whether the item is equippable.
    /// </summary>
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
    /// <summary>
    /// Equipment type of the item stored as an integer. 0 is reserved for non equippable items.
    /// </summary>
    public int equipmentType;
    /// <summary>
    /// Whether the item is clothing.
    /// </summary>
    public bool clothing;
    /// <summary>
    /// Outpline sprite of the item (the image which will appear on the equipment outline in the inventory screen).
    /// </summary>
    public Sprite outlineSprite;



    [Header("Container")]
    /// <summary>
    /// Whether the item is a container.
    /// </summary>
    public bool container;
    /// <summary>
    /// Amount of grids within the item (if the item is a container).
    /// </summary>
    public int gridAmount;
    [System.Serializable]
    /// <summary>
    /// Information about each grid within the item. Contains the ID of the grid, width and height of the grid in tiles.
    /// </summary>
    public struct GridData{
        public int gridID;
        public int width;
        public int height;
    }
    /// <summary>
    /// Container prefab of the item (the prefab which will be spawned when the item is equipped or opened).
    /// </summary>
    public GameObject containerPrefab;

    /// <summary>
    /// Array of GridData structs which contain information about each grid within the item.
    /// </summary>
    public GridData[] gridData;
    

    [Header("Weapon")]
    /// <summary>
    /// Whether the item is a weapon.
    /// </summary>
    public bool weapon;
    /// <summary>
    /// Whether the item is a firearm.
    /// </summary>
    public bool firearm;
    /// <summary>
    /// Whether the item is manually chambered.
    /// </summary>
    public bool manuallyChambered;
    /// <summary>
    /// Array of strings which contain the names of the fire modes of the weapon.
    /// </summary>
    public string[] fireModes;
    /// <summary>
    /// Whether the item uses magazines.
    /// </summary>
    public bool usesMagazines;
    /// <summary>
    /// Internal magazine size of the weapon.
    /// </summary>
    public int internalMagSize;
    /// <summary>
    /// Type of the weapon. It is used to group ammunition, magazines and weapons to the same category.
    /// </summary>
    public string weaponType;
    /// <summary>
    /// Length of the weapon. 0 - short, 1 - long.
    /// </summary>
    public int weaponLength;
    /// <summary>
    /// Sprite for the outline of the weapon shown in the bottom right corner of the HUD.
    /// </summary>
    public Sprite weaponHUDOutlineSprite;
    /// <summary>
    /// Sprite for the Full Ammo Bar sprite shown in the bottom right corner of the HUD.
    /// </summary>
    public Sprite ammoBarFullSprite;
    /// <summary>
    /// Sprite for the Empty Ammo Bar sprite shown in the bottom right corner of the HUD.
    /// </summary>
    public Sprite ammoBarEmptySprite;
    /// <summary>
    /// Animator override controller for the weapon. It is used to override the default weapon animations with the animations for the specific weapon.
    /// </summary>
    public AnimatorOverrideController weaponAnimationController;
    /// <summary>
    /// ItemData for the ammunition this item uses. It is used to spawn the appropriate ammunition when the weapon or magazine is emptied.
    /// </summary>
    public ItemData ammoItemData;
    /// <summary>
    /// ItemData for the magazine this item uses. It is used to spawn the appropriate magazine when the magazine is removed from the item.
    /// </summary>
    public ItemData magazineItemData;
    /// <summary>
    /// Whether the item is ammo.
    /// </summary>
    public bool ammo;
    /// <summary>
    /// Whether the item is a magazine.
    /// </summary>
    public bool magazine;
    /// <summary>
    /// Ammo capacity of the item (magazine).
    /// </summary>
    public int magazineSize;

    [Header("Usable")]
    /// <summary>
    /// Whether the item is usable (not yet implemented).
    /// </summary>
    public bool usable;


    [Header("Misc")]
    /// <summary>
    /// Whether the item is stackable.
    /// </summary>
    public bool stackable;
    /// <summary>
    /// Maximum stack size of the item.
    /// </summary>
    public int maxStack;
    /// <summary>
    /// Prefab for the item subtext.
    /// </summary>
    public GameObject itemSubtextPrefab;
    /// <summary>
    /// Primary sprite of the item in the inventory.
    /// </summary>
    public Sprite inventorySprite;
    /// <summary>
    /// Secondary sprite of the item in the inventory (i.e. the sprite of a weapon with and without magazine).
    /// </summary>
    public Sprite inventorySpriteSecondary;
}
