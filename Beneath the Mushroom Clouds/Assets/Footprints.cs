using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprints : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private float bloodySoleDistance = 0f;
    private Vector3 oldPosition;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.startColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        oldPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        particleSystem.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 distanceMoved = transform.position - oldPosition;

        bloodySoleDistance -= distanceMoved.magnitude;
        if(bloodySoleDistance < 0f)
        {
            bloodySoleDistance = 0f;
        }
        oldPosition = transform.position;
        UpdateFootstepColor();
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "BloodPuddle")
        {
            bloodySoleDistance = 100f;
            particleSystem.startColor = new Color(0.4f, 0, 0, 0.5f);
        }
    }

    void UpdateFootstepColor(){
        float bloodMultiplier = 1 - (bloodySoleDistance / 100f);
        particleSystem.startColor = new Color(0.4f - bloodMultiplier * 0.2f, bloodMultiplier * 0.2f , bloodMultiplier * 0.2f, 0.5f - bloodMultiplier * 0.3f);
    }
}
