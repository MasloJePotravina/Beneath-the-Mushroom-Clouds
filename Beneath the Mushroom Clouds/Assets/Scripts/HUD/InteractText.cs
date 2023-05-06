using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the behaciour of the interact text displayed above the player when an object is within the interact range.
/// </summary>
public class InteractText : MonoBehaviour
{
    /// <summary>
    /// Reference to the main camera.
    /// </summary>
    [SerializeField] Camera mainCamera;

    /// <summary>
    /// Whether the text is active or not.
    /// </summary>
    public bool isActive = false;

    /// <summary>
    /// Moves the text above the player. Calculated using the player's position and camera.
    /// </summary>
    /// <param name="playerPosition">Position of the player.</param>
    public void MoveTextAbovePlayer(Vector3 playerPosition)
    {
        //Get the position of the text on the screen based on the player's position
        Vector2 textPosition = mainCamera.WorldToScreenPoint(playerPosition);

        //Set the position of the text to the player's position with an offset to appear above.
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = new Vector2(textPosition.x, textPosition.y + 50f);

    }

    /// <summary>
    /// Set the interact text in the following format: [Tag/Name] Action
    /// </summary>
    /// <param name="interactableTag">Tag or a name of the interactable object.</param>
    /// <param name="text">Action which will be performed by interaction.</param>
    public void SetText(string interactableTag, string text)
    {
        GetComponent<TextMeshProUGUI>().text = "[" + interactableTag +"] " + text;
    }

}
