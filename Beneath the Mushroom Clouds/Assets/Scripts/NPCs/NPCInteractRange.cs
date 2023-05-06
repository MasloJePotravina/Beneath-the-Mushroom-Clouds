using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the interact range of the NPCs used to interact with doors.
/// </summary>
public class NPCInteractRange : MonoBehaviour
{
    
    /// <summary>
    /// When a closed door enters the interact range of the NPC, the door is opened.
    /// </summary>
    /// <param name="other">Collider of the object that entered the interact range of the NPC.</param>
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name == "Door"){
            DoorBehaviour door = other.gameObject.transform.parent.parent.GetComponent<DoorBehaviour>();
            if(door != null && !door.isOpen){
                door.Open();
            }
        }
    }
}
