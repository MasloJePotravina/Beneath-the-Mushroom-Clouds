using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractRange : MonoBehaviour
{
    

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name == "Door"){
            Debug.Log("Door");
            DoorBehaviour door = other.gameObject.transform.parent.parent.GetComponent<DoorBehaviour>();
            if(door != null && !door.isOpen){
                door.Open();
            }
        }
    }
}
