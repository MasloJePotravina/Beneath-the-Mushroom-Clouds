using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implements the behaviour of loose items on the ground.
/// </summary>
public class LooseItem : MonoBehaviour
{
    /// <summary>
    /// Prefab for a small item represenation of items.
    /// </summary>
    [SerializeField] private Sprite smallItem;

    /// <summary>
    /// Reference to the item this loose item represents.
    /// </summary>
    public InventoryItem item;

    /// <summary>
    /// Initialize the lose item object. Sets the look, scale and item reference.
    /// </summary>
    /// <param name="item">Inventory item this loose item will represent.</param>
    public void Init(InventoryItem item){
        this.item = item;
        if(item.itemData.visibleOnDrop){
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.GetComponent<Image>().sprite;
            transform.localScale = Vector3.one * item.itemData.dropSizeMultiplier;
        }else{
            Animator animator = GetComponent<Animator>();
            animator.enabled = true;
            transform.localScale = new Vector3(20f, 20f, 1f);
        }
    }
}
