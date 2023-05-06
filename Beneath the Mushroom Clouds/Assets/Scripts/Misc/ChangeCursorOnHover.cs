using System.Collections;
using UnityEngine;

/// <summary>
/// Implements changing of cursor to pointer when hovering over obhects like buttons.
/// </summary>
public class ChangeCursorOnHover : MonoBehaviour
{

    /// <summary>
    /// Texture for a hover cursor.
    /// </summary>
    public Texture2D hoverCursor;

    /// <summary>
    /// The cursor mode.
    /// </summary>
    private CursorMode cursorMode = CursorMode.Auto;

    /// <summary>
    /// Hot spot of the hover cursor (where the click truly happens - index finger of the hand)
    /// </summary>
    private Vector2 hotSpot = new Vector2(10, 0);

    /// <summary>
    /// Activates the hover cursor.
    /// </summary>
    public void ActivateHoverCursor()
    {
        Cursor.SetCursor(hoverCursor, hotSpot, cursorMode);
    }

    /// <summary>
    /// Deactivates the hover cursor.
    /// </summary>
    public void DeactivateHoverCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
