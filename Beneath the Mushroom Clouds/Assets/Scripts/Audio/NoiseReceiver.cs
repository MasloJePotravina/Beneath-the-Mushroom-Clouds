using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Acts as a receiver for noises the NPCs can hear.
/// </summary>
public class NoiseReceiver : MonoBehaviour
{
    /// <summary>
    /// Used to notify a parent object about a noise that was heard. Called by Noise Origin when a noise is generated.
    /// </summary>
    /// <param name="noisePosition">Where the noise originated from.</param>
    public void HeardNoise(Vector3 noisePosition)
    {
        if(transform.CompareTag("NPC")){
            NPCBehaviourHostile npcBehaviourHostile = transform.parent.GetComponent<NPCBehaviourHostile>();
            if(npcBehaviourHostile != null){
                npcBehaviourHostile.HeardNoise(noisePosition);
            }
        }
    }
}
