using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Implements the behaviour of the Item Highlighter.
/// </summary>
public class InventoryHighlight : MonoBehaviour
{   
    /// <summary>
    /// Reference to the RectTransform component of the highlighter.
    /// </summary>
    /// <param name="targetItem"></param>
    private RectTransform highlighter;

    /// <summary>
    /// Initializes the RectTransform component of the highlighter.
    /// </summary>
    private void Awake() {
        highlighter = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Sets the size of the heighlighter to fit the size of the item it is currently highlighting or the size of the selected item.
    /// </summary>
    /// <param name="targetItem">Reference to the item from which the size of the highlighter is determined.</param>
    public void SetSize(InventoryItem targetItem){
        highlighter.sizeDelta = new Vector2(targetItem.Width * ItemGrid.tileDimension, targetItem.Height * ItemGrid.tileDimension);
    }

    /// <summary>
    /// Enables or disables the highlighter.
    /// </summary>
    /// <param name="show">True to enable the highlighter, false to disable it.</param>
    public void Show(bool show){
        highlighter.gameObject.SetActive(show);
    }

    /// <summary>
    /// Sets the parent of the highlighter to the target grid.
    /// </summary>
    /// <param name="targetGrid">The grid to which the highlighter belongs.</param>
    public void SetHighlighterParent(ItemGrid targetGrid){
        if(targetGrid == null){
            return;
        }
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
        highlighter.SetAsLastSibling();
    }

    /// <summary>
    /// Sets the position of the highlighter on the grid based on the item it is currently highlighting.
    /// This function is one of two overloaded functions that set the position of the highlighter.
    /// </summary>
    /// <param name="targetGrid">Grid on which the highlighter is placed.</param>
    /// <param name="targetItem">Item that the highlighter is highlighting.</param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem){

        Vector2 pos = targetGrid.CalculateGridPosition(targetItem, targetItem.gridPositionX, targetItem.gridPositionY);

        highlighter.localPosition = pos;
    }

    /// <summary>
    /// Sets the position of the highlighter on the grid based on a selected item to show where the item will be placed if dropped.
    /// This function is one of two overloaded functions that set the position of the highlighter.
    /// </summary>
    /// <param name="targetGrid">Grid on which the highlighter is placed.</param>
    /// <param name="targetItem">Selected item.</param>
    /// <param name="posX">X position on which the highlighter should be placed on the grid.</param>
    /// <param name="posY">Y position on which the highlighter should be placed on the grid.</param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY){

        Vector2 pos = targetGrid.CalculateGridPosition(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }

    /// <summary>
    /// Sets the color of the highlighter.
    /// </summary>
    /// <param name="color">Color to which the highlighter should be set.</param>
    public void SetColor(Color color){
        highlighter.GetComponent<UnityEngine.UI.Image>().color = color;
    }
}
