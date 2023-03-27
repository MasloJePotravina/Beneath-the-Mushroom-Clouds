using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Implements the behaviour of item subtexts.
/// </summary>
public class ItemSubtext : MonoBehaviour
{

    /// <summary>
    /// Reference to the item to which the subtext belongs.
    /// </summary>
    public InventoryItem item;

    /// <summary>
    /// Updates the position of the subtext to always be in the bottom right corner of the item.
    /// </summary>
    public void UpdatePosition()
    {

        RectTransform rectTransform = GetComponent<RectTransform>();
        if(item.rotated){
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.anchoredPosition = new Vector2(-2,-2);
        }else{
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.anchoredPosition = new Vector2(-2,2);
        }

        rectTransform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Updates the text of the subtext.
    /// </summary>
    /// <param name="text">New text.</param>
    public void UpdateText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
}
