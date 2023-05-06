using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implements the interaction with objects for the player.
/// </summary>
public class PlayerInteract : MonoBehaviour
{

    /// <summary>
    /// Reference to the HUD controller.
    /// </summary>
    [SerializeField] private HUDController hudController;

    /// <summary>
    /// List of interactable objects in range.
    /// </summary>
    private List<GameObject> interactableObjects = new List<GameObject>();

    /// <summary>
    /// Cirrently selected interactable object.
    /// </summary>
    private GameObject interactableObject;

    /// <summary>
    /// Each frame determines the object that should be interacted with.
    /// </summary>
    void Update()
    {
        if(interactableObjects.Count <= 0)
        {
            interactableObject = null;
            hudController.DeactivateInteractText();
            return;
        }

        interactableObject = GetClosestObject();
        InteractableBehaviour interactable = interactableObject.GetComponent<InteractableBehaviour>();
        ContainerObject container = interactableObject.GetComponent<ContainerObject>();
        string objectName;
        if(container != null){
            objectName = container.containerType;
        }else{
            objectName = interactable.interactableTag;
        }
        hudController.SetInteractText(objectName, interactable.GetInteractText());

        hudController.ActivateInteractText();
    }

    /// <summary>
    /// Interacts with the currently selected object.
    /// </summary>
    public void Interact()
    {

        if(interactableObject == null)
        {
            return;
        }

        InteractableBehaviour interactable = interactableObject.GetComponent<InteractableBehaviour>();

        interactable.Interact();
    }

    /// <summary>
    /// When an interactable object enters the range, it is added to the list of interactable objects.
    /// </summary>
    /// <param name="other">Collider which entered the range.</param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            interactableObjects.Add(other.gameObject);
            
        }
    }

    /// <summary>
    /// When an interactable object exits the range, it is removed from the list of interactable objects.
    /// </summary>
    /// <param name="other">Collider which exited the range.</param>
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            interactableObjects.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// Gets the closest object from the list of interactable objects.
    /// </summary>
    /// <returns>Closest object from the list of interactable objects.</returns>
    public GameObject GetClosestObject()
    {
        float closestDistance = 1000f;
        GameObject closestObject = null;
        GameObject door = null;
        bool doorInRange = false;
        foreach(GameObject obj in interactableObjects)
        {   

            //Override so that if a door is in range, it is selected
            //Prevents a problem where if an enemy dies next to a door, the door can't be opened as the body is selected as closest
            //This is a temoporary fix until selectable interaction is implemented
            if(obj.GetComponent<InteractableBehaviour>().interactableTag == "Door")
            {
                doorInRange = true;
                door = obj;
            }
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        if(closestObject.GetComponent<InteractableBehaviour>().interactableTag == "Dead Body" && doorInRange)
        {
            return door;
        }
        return closestObject;
    }
}
