using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractText : MonoBehaviour
{

    [SerializeField] Camera mainCamera;

    public bool isActive = false;

    public void MoveTextAbovePlayer(Vector3 playerPosition)
    {
        Vector2 textPosition = mainCamera.WorldToScreenPoint(playerPosition);
        
        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.position = new Vector2(textPosition.x, textPosition.y + 50f);

    }

    public void SetText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = "[E] " + text;
    }

}
