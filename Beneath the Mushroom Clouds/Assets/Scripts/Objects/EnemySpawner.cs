using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of the enemy spawner.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    
    /// <summary>
    /// Reference to the spawned enemy.
    /// </summary>
    private GameObject enemy;

    /// <summary>
    /// Reference to the player.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Hostile NPC prefab.
    /// </summary>
    [SerializeField] private GameObject enemyPrefab;

    /// <summary>
    /// List of patrol locations the enemy spawned form this spawner will patrol in local space relative to the building prefab this spawner belongs to.
    /// </summary>
    /// <typeparam name="Vector3">Patrol locations.</typeparam>
    [SerializeField] private List<Vector3> patrolLocations;

    /// <summary>
    /// List of patrol locations the enemy spawned form this spawner will patrol in world space.
    /// </summary>
    /// <typeparam name="Vector3">Patrol locations.</typeparam>
    private List<Vector3> patrolLocationsWorldSpace = new List<Vector3>();

    /// <summary>
    /// Reference to the parent building prefab.
    /// </summary>
    [SerializeField] private GameObject parentBuilding;

    /// <summary>
    /// Gets the reference to the player on awake and calculates the patrol locations in world space.
    /// </summary>
    void Awake(){
        player = GameObject.Find("Player");
        if(parentBuilding != null){
            foreach(Vector3 location in patrolLocations){
                patrolLocationsWorldSpace.Add(parentBuilding.transform.TransformPoint(location));
            }
        }
        
    }


    /// <summary>
    /// Spawns an enemy if there is none spawned already and the player is far enough away.
    /// </summary>
    /// <param name="initialSpawn">Whether this is the initial spawning at the start of the scene.</param>
    public void SpawnEnemy(bool initialSpawn = false){
        if(enemy != null){
            return;
        }

        float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);

        //Dont' spawn enemies too close to the player, unless its the intial spawn on scene load
        if(!initialSpawn && distanceFromPlayer < 500){
            return;
        }

        enemy = Instantiate(enemyPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        enemy.GetComponent<NPCBehaviourHostile>().patrolLocations = patrolLocationsWorldSpace;
    }
}
