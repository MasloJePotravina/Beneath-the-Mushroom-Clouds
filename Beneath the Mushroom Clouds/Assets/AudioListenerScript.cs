using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerScript : MonoBehaviour
{
       // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

}
