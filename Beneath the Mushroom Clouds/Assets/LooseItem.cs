using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooseItem : MonoBehaviour
{
    [SerializeField] private Sprite smallItem;

    public InventoryItem item;


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
