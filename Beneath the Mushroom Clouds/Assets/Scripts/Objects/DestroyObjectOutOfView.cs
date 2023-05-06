using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hadles destroying of an object when it is time to destruct it and it leaves the view of the player.
/// </summary>
public class DestroyObjectOutOfView : MonoBehaviour
{
    /// <summary>
    /// Reference to the player.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Reference to the world status.
    /// </summary>
    private WorldStatus worldStatus;

    /// <summary>
    /// Time to destruction in in-game seconds.
    /// </summary>
    public float timeToDestruction = 300f;

    /// <summary>
    /// Gets the reference to the player and the world status on awake.
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        worldStatus = GameObject.FindObjectOfType<WorldStatus>();
    }

    /// <summary>
    /// Each frame checks if the item should be destroyed.
    /// </summary>
    void Update()
    {
        if(timeToDestruction <= 0){
            float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
            if(distanceFromPlayer > 300){
                Destroy(gameObject);
            }
        }else{
            timeToDestruction -= Time.deltaTime * worldStatus.timeMultiplier;
        }
    }
}
