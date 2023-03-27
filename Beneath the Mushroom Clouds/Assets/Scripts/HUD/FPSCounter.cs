using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Implements the behaviour of a simple FPS counter.
/// </summary>
public class FPSCounter : MonoBehaviour
{
   /// <summary>
   /// Reference to the TextMeshProUGUI component of the FPS counter.
   /// </summary>
   private TextMeshProUGUI fpsText;

   /// <summary>
   /// Time between frames.
   /// </summary>
   public float deltaTime;

   /// <summary>
   /// Gets the TextMeshProUGUI component of the FPS counter.
   /// </summary>
   void Start(){
      fpsText = GetComponent<TextMeshProUGUI>();
   }
   
   /// <summary>
   /// Each frame calculate the current FPS based on the time between frames and display it.
   /// </summary>
   void Update () {
      deltaTime += (Time.deltaTime - deltaTime);
      float fps = 1.0f / deltaTime;
      fpsText.text = Mathf.Ceil (fps).ToString ();
   }
}
