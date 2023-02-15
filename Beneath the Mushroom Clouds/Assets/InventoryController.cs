using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{

    [SerializeField] ItemGrid selectedItemGrid;
    
    private void Update(){
        if(selectedItemGrid == null)
            return;

        Vector2 x = selectedItemGrid.GetTilePosition(Input.mousePosition);
        Debug.Log(x);
    }
}
