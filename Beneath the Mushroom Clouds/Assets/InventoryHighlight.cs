using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;

    public void SetSize(InventoryItem targetItem){
        highlighter.sizeDelta = new Vector2(targetItem.itemData.width * ItemGrid.tileDimension, targetItem.itemData.height * ItemGrid.tileDimension);
    }

    public void Show(bool show){
        highlighter.gameObject.SetActive(show);
    }

    public void SetHighlighterParent(ItemGrid targetGrid){
        if(targetGrid == null){
            return;
        }
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
        highlighter.SetAsLastSibling();
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem){

        Vector2 pos = targetGrid.CalculateGridPosition(targetItem, targetItem.gridPositionX, targetItem.gridPositionY);

        highlighter.localPosition = pos;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY){

        Vector2 pos = targetGrid.CalculateGridPosition(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }
}
