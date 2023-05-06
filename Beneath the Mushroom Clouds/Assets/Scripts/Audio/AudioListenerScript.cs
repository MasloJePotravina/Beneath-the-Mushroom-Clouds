using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the rotation of an audio listener object attached to the player.
/// </summary>
public class AudioListenerScript : MonoBehaviour
{
    /// <summary>
    /// On LateUpdate sets the rotation of the audio listener to the rotation of the main camera, effectively cancelling the rotation of the player.
    /// This helps to achieve a directional sound effect.
    /// </summary>
    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

}
