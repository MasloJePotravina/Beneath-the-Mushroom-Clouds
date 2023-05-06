using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls what cursor is displayed.
/// </summary>
public class CursorController : MonoBehaviour
{   
    /// <summary>
    /// Texture for the crosshair cursor.
    /// </summary>
    [SerializeField] private Texture2D crosshairCursor;

    /// <summary>
    /// Cursor mode.
    /// </summary>
    private CursorMode cursorMode = CursorMode.Auto;

    /// <summary>
    /// Where the point of click actually is on the crosshair cursor (the middle).
    /// </summary>
    private Vector2 crosshairHotSpot = new Vector2(16, 16);

    /// <summary>
    /// Set the cursor state to condifned on awake.
    /// </summary>
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    /// <summary>
    /// Switches to the default cursor.
    /// </summary>
    public void SwitchToDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    /// <summary>
    /// Switches to the crosshair cursor.
    /// </summary>
    public void SwitchToCrosshairCursor()
    {
        Cursor.SetCursor(crosshairCursor, crosshairHotSpot, cursorMode);
    }



}
