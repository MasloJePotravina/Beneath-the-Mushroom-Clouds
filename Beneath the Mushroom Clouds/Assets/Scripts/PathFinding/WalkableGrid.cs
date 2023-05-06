using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the set up of unwalkable tiles in the pathfinding grid.
/// </summary>
public class WalkableGrid : MonoBehaviour
{   
    /// <summary>
    /// 2D array of booleans which indicate which path nodes are walkable.
    /// </summary>
    public bool[,] grid = new bool[200, 200];

    /// <summary>
    /// List of all of the obstacles in the scene.
    /// </summary>
    private List<GameObject> obstacles = new List<GameObject>();
    

    /// <summary>
    /// Finds all of the obstacles and marks which nodes are not walkable based on their position. Then contructs the pathfinding instance.
    /// </summary>
    public void Awake(){
        InitializeWalkableGrid();
        FindAllObstacles();
        ConstructWalkableGrid();
        PathFinding pathFinding = new PathFinding(200, 200, grid);
    }

    /// <summary>
    /// Finds all of the obstacles in the scene.
    /// </summary>
    private void FindAllObstacles(){
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject go in allGameObjects){
            if(go.activeInHierarchy == false) continue;
            if(go.layer == LayerMask.NameToLayer("FullObstacle") || go.layer == LayerMask.NameToLayer("HalfObstacle")){
                if(go.name != "Door")
                    obstacles.Add(go);
            }
        }
    }

    /// <summary>
    /// Constructs the walkable grid based on the obstacles in the scene.
    /// </summary>
    private void ConstructWalkableGrid(){
        foreach(GameObject obstacle in obstacles){

            Vector2[] obstacleCorners = new Vector2[4];
            Vector2Int[] obstacleCornersOnGrid = new Vector2Int[4];

            Vector3[] localCorners = new Vector3[4];

            //Local positions
            Vector2 halfSize = obstacle.transform.localScale / 2f;
            localCorners[0] = new Vector3(-halfSize.x, -halfSize.y, 0); // Bottom-left
            localCorners[1] = new Vector3(halfSize.x, -halfSize.y, 0); // Bottom-right
            localCorners[2] = new Vector3(halfSize.x, halfSize.y, 0); // Top-right
            localCorners[3] = new Vector3(-halfSize.x, halfSize.y, 0); // Top-left

            //Rake rotation into account
            Quaternion rotation = obstacle.transform.rotation;
            for (int i = 0; i < 4; i++) {
                Vector3 worldPosition = obstacle.transform.position + rotation * localCorners[i];
                obstacleCorners[i] = new Vector2(worldPosition.x, worldPosition.y);
            }

            System.Array.Sort(obstacleCorners, (a, b) => {
                int compare = a.x.CompareTo(b.x);
                if (compare == 0) {
                    compare = b.y.CompareTo(a.y);
                }
                return compare;
            });

            for(int i = 0; i < 4; i++){
                obstacleCornersOnGrid[i] = WorldToGridPosition(new Vector3(obstacleCorners[i].x, obstacleCorners[i].y, 0));
                Debug.DrawLine(obstacle.transform.position, obstacleCorners[i], Color.red, 100f);
            }

            //Note: This does not work on diagonal obstacles and therefore, for now, there are no diagonal obstacles allowed when creating a map!
            Vector2Int minCorner = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int maxCorner = new Vector2Int(int.MinValue, int.MinValue);

            foreach (Vector2Int cornerOnGrid in obstacleCornersOnGrid) {
                minCorner = Vector2Int.Min(minCorner, cornerOnGrid);
                maxCorner = Vector2Int.Max(maxCorner, cornerOnGrid);
            }

            for (int x = minCorner.x; x <= maxCorner.x; x++) {
                for (int y = minCorner.y; y <= maxCorner.y; y++) {
                    grid[x, y] = false;
                }
            }
            
        }
    }

    /// <summary>
    /// Converts a world position to a grid position.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    private Vector2Int WorldToGridPosition(Vector3 worldPosition){
        Vector2Int positionOnGrid = new Vector2Int((int)((worldPosition.x + 1000f)/10f), (int)((worldPosition.y + 1000f)/10f));
        return positionOnGrid;
    }

    /// <summary>
    /// Initializes the walkable grid to be all walkable at the start.
    /// </summary>
    private void InitializeWalkableGrid(){
        for(int i = 0; i < 200; i++){
            for(int j = 0; j < 200; j++){
                grid[i, j] = true;
            }
        }
    }
}
