using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : MonoBehaviour
{

    [SerializeField] private GameObject player;
    private PlayerControls playerControls;
    public bool inventoryOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        playerControls = player.GetComponent<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
