using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
/// <summary>
/// Implements the behavior for the top of the inventory window.
/// </summary>
public class InventoryWindowTop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Previous mouse position of the mouse. Used to calculate the placement of the window.
    /// </summary>
    public Vector2 prevMousePos = new Vector2(-1,-1);

    /// <summary>
    /// Reference to the Inventory Controller.
    /// </summary>
    public InventoryController inventoryController;

    /// <summary>
    /// Detects when the mouse enters a the window top and sets the window top as the hovered window top in the Inventory Controller.
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.hoveredWindowTop = this;
    }

    /// <summary>
    /// Detects when the mouse leaves a the window top and sets the hovered window top in the Inventory Controller to null.
    /// </summary>
    /// <param name="eventData">Event data associated with input. Not used in this function but required for compilation.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.hoveredWindowTop = null;
    }

    //Function that drags the window
    //NOTE: This is done instead of just setting the window to the mouse position to avoid a problem where
    //when the windows is clicked its position is is automatically shifted in a way in which the mouse is 
    // in the top irght corner of the window (just clicking the window will move it to the mouse position)
    /// <summary>
    /// Deags the window by calculating the difference of the current position and the previous position of the mouse.
    /// </summary>
    public void DragWindow()
    {
        //Get the mouse position
        Vector2 mousePos = Input.mousePosition;

        //If this is the first frame that the mouse is being dragged, set the previous mouse position to the current mouse position and return
        if(prevMousePos.x == -1 && prevMousePos.y == -1){
            prevMousePos = mousePos;
            return;
        }
        //Calculate the delta between the previous mouse position and the current mouse position
        Vector2 delta = mousePos - prevMousePos;
        //Set the previous mouse position to the current mouse position
        prevMousePos = mousePos;
        //Move the window by the delta
        transform.parent.transform.position += new Vector3(delta.x, delta.y,0);
    }

    /// <summary>
    /// Method called by the close button located in the top right of the window. Calls a close window function in the Inventory Window script.
    /// </summary>
    public void CloseButton(){
        GameObject window = this.gameObject.transform.parent.gameObject;
        window.GetComponent<InventoryWindow>().CloseWindow();
    }
}
