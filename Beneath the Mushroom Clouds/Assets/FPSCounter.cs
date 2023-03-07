using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour
{
     private TextMeshProUGUI fpsText;
     public float deltaTime;

     void Start(){
        fpsText = GetComponent<TextMeshProUGUI>();
     }
 
     void Update () {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil (fps).ToString ();
     }
}
