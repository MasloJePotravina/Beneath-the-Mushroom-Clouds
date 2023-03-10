using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugDropdownSpawner : MonoBehaviour
{

    private TMP_Dropdown dropdown;
    [SerializeField] private GameObject mainCamera;
    private InventoryController inventoryController;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        inventoryController = mainCamera.GetComponent<InventoryController>();
        dropdown.onValueChanged.AddListener(delegate {
            OnValueChanged();
        });
    }

    void OnValueChanged(){
        inventoryController.DropdownSpawnItem(dropdown.value);
    }
}
