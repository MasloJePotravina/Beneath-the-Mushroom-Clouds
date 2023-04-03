using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehaviour : MonoBehaviour
{
    public string interactableTag = "Door";

    public bool interactEnabled = true;

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

    public void DisableInteract(){
        interactEnabled = false;
    }

    public void EnableInteract(){
        interactEnabled = true;
    }
}
