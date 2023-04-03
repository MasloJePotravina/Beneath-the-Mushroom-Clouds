using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D crosshairCursor;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 crosshairHotSpot = new Vector2(16, 16);

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }


    public void SwitchToDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void SwitchToCrosshairCursor()
    {
        Cursor.SetCursor(crosshairCursor, crosshairHotSpot, cursorMode);
    }



}
