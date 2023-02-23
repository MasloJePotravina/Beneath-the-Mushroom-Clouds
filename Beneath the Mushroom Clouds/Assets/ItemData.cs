using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class ItemData : ScriptableObject
{

    public long id;
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
    public int equipmentType;

    public GameObject containerPrefab;

    public Sprite outlineSprite;


    public Sprite inventorySprite;
}
