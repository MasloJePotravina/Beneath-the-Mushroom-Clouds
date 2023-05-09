using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjusts the scale of the object to fit the sprite renderer of the parent object.
/// </summary>
public class FitParentSpriteRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = transform.parent.gameObject;

        //Change scale to parents sprite renderer size
        Vector3 parentSize = parent.GetComponent<SpriteRenderer>().bounds.size;

        //Flip values if parent is rotated
        if(parent.transform.rotation.eulerAngles.z == 90 || parent.transform.rotation.eulerAngles.z == 270){
            parentSize = new Vector3(parentSize.y, parentSize.x, 1);
        }
        transform.localScale = new Vector3(parentSize.x, parentSize.y, 1);
    }
}
