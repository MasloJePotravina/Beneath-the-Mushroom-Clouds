using System.Collections;
using UnityEngine;

public class ChangeCursorOnHover : MonoBehaviour
{

    public Texture2D hoverCursor;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = new Vector2(10, 0);
    public void ActivateHoverCursor()
    {
        Cursor.SetCursor(hoverCursor, hotSpot, cursorMode);
    }

    public void DeactivateHoverCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
