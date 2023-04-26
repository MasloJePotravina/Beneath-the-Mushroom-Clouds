using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    [SerializeField] private HUDController hudController;
    private List<GameObject> interactableObjects = new List<GameObject>();
    private GameObject interactableObject;

    void Update()
    {
        if(interactableObjects.Count <= 0)
        {
            interactableObject = null;
            hudController.DeactivateInteractText();
            return;
        }

        interactableObject = GetClosestObject();

        hudController.SetInteractText(interactableObject.GetComponent<InteractableBehaviour>().GetInteractText());

        hudController.ActivateInteractText();
    }

    
    public void Interact()
    {

        if(interactableObject == null)
        {
            return;
        }

        InteractableBehaviour interactable = interactableObject.GetComponent<InteractableBehaviour>();

        interactable.Interact();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            interactableObjects.Add(other.gameObject);
            
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            interactableObjects.Remove(other.gameObject);
        }
    }

    public GameObject GetClosestObject()
    {
        float closestDistance = 1000f;
        GameObject closestObject = null;
        foreach(GameObject obj in interactableObjects)
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }
}
