using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements random generation of items for containers.
/// </summary>
public class RandomizedItemGenerator : MonoBehaviour
{

    /// <summary>
    /// Struct that holds the item data, probability and whether the item is should be unique within that container.
    /// </summary>
    [System.Serializable]
    public struct ItemProbabilities
    {
        /// <summary>
        /// Item data of the item.
        /// </summary>
        public ItemData itemData;

        /// <summary>
        /// Probability the item will be spawned in a roll.
        /// </summary>
        public float spawnProbabilityPerRoll;

        /// <summary>
        /// Whether the item should be unique within the container.
        /// </summary>
        public bool unique;
    }

    /// <summary>
    /// List of possible items, their probabilities and their uniqeness.
    /// </summary>
    public List<ItemProbabilities> possibleItems = new List<ItemProbabilities>();

    /// <summary>
    /// List of items that are forced to spawn in the container.
    /// </summary>
    /// <typeparam name="ItemData">ItemData of the forced item.</typeparam>
    public List<ItemData> forcedItems = new List<ItemData>();

    /// <summary>
    /// How many rolls should be performed that have possibility to spawn an item.
    /// </summary>
    public int rollCount = 5;

    /// <summary>
    /// Generates a random set of items.
    /// </summary>
    /// <returns>Dictionary saying which items were spawned and how many of those items were spawned (not stack count).</returns>
    public Dictionary<ItemData, int> GenerateRandomItems()
    {
        Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();
        for(int i = 0; i < rollCount; i++)
        {
            
            foreach (ItemProbabilities possibleItem in possibleItems)
            {
                float randomValue = Random.Range(0f, 1f);
                if (randomValue <= possibleItem.spawnProbabilityPerRoll)
                {
                    if(items.ContainsKey(possibleItem.itemData)){
                        if(possibleItem.unique){
                            continue;
                        }
                        items[possibleItem.itemData]++;
                    }else{
                        items.Add(possibleItem.itemData, 1);
                    }
                }
            }
        }
        return items;
    }

    /// <summary>
    /// Return the list of pre-set forced items that should be spawned in the container.
    /// </summary>
    /// <returns></returns>
    public List<ItemData> GetForcedItems()
    {
        return forcedItems;
    }
}
