using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseReceiver : MonoBehaviour
{
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
