using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryWindowTop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2 prevMousePos = new Vector2(-1,-1);
    public InventoryController inventoryController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.hoveredWindowTop = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.hoveredWindowTop = null;
    }

    public void DragWindow()
    {
        Vector2 mousePos = Input.mousePosition;
        if(prevMousePos.x == -1 && prevMousePos.y == -1){
            prevMousePos = mousePos;
            return;
        }
        Vector2 delta = mousePos - prevMousePos;
        prevMousePos = mousePos;
        transform.parent.transform.position += new Vector3(delta.x, delta.y,0);
    }

    public void CloseButton(){
        GameObject window = this.gameObject.transform.parent.gameObject;
        window.GetComponent<InventoryWindow>().CloseWindow();
    }
}
