using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ItemSubtext : MonoBehaviour
{

    public InventoryItem item;

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

    public void UpdateText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
}
