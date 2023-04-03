using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedBehaviour : MonoBehaviour
{
    private HUDController hudController;

    public void Awake(){
        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
    }


    public void Sleep(){
        hudController.ActivateRestMenu();
    }
}
