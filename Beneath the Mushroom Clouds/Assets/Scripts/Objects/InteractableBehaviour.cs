using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of interactable objects.
/// </summary>
public class InteractableBehaviour : MonoBehaviour
{
    /// <summary>
    /// Tag of the interactable object.
    /// </summary>
    public string interactableTag = "Door";

    /// <summary>
    /// Whether the object can be interacted with or not.
    /// </summary>
    public bool interactEnabled = true;

    /// <summary>
    /// Interacts with the object based on its tag.
    /// </summary>
    public void Interact(){
        if(!interactEnabled)
        {
            return;
        }


        if(interactableTag == "Door")
        {
            DoorBehaviour doorBehaviour = this.GetComponent<DoorBehaviour>();
            if(doorBehaviour.isOpen)
            {
                doorBehaviour.Close();
            }
            else
            {
                doorBehaviour.Open();
            }
        }

        if(interactableTag == "Container")
        {
            ContainerObject containerObject = this.GetComponent<ContainerObject>();
            containerObject.Open();
        }

        if(interactableTag == "Bed")
        {
            BedBehaviour bedBehaviour = this.GetComponent<BedBehaviour>();
            bedBehaviour.Sleep();
        }
    }

    /// <summary>
    /// Gets the interact text based on the tag of the object.
    /// </summary>
    /// <returns>The action the player will perform by interacting with the object.</returns>
    public string GetInteractText(){
        if(interactableTag == "Door")
        {
            DoorBehaviour doorBehaviour = this.GetComponent<DoorBehaviour>();
            if(doorBehaviour.isOpen)
            {
                return "Close";
            }
            else
            {
                return "Open";
            }
        }
        else if(interactableTag == "Container")
        {
            return "Open";
        }
        else if(interactableTag == "Bed")
        {
            return "Rest";
        }
        else
        {
            return "Interact";
        }
    }

    /// <summary>
    /// Diasbles the interaction with the object.
    /// </summary>
    public void DisableInteract(){
        interactEnabled = false;
    }

    /// <summary>
    /// Enables the interaction with the object.
    /// </summary>
    public void EnableInteract(){
        interactEnabled = true;
    }
}
