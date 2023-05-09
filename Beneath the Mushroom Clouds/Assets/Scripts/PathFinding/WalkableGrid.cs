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
            LayerMask objectLayer = go.layer;
            LayerMask obstacleLayer = LayerMask.GetMask("FullObstacle", "HalfObstacle", "LowObstacle");
            if(obstacleLayer == (obstacleLayer | (1 << objectLayer))){
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

            //Local positions as sprite renderer bounds
            SpriteRenderer spriteRenderer = obstacle.GetComponent<SpriteRenderer>();
            localCorners[0] = new Vector3(-spriteRenderer.bounds.extents.x, -spriteRenderer.bounds.extents.y, 0);
            localCorners[1] = new Vector3(spriteRenderer.bounds.extents.x, -spriteRenderer.bounds.extents.y, 0);
            localCorners[2] = new Vector3(spriteRenderer.bounds.extents.x, spriteRenderer.bounds.extents.y, 0);
            localCorners[3] = new Vector3(-spriteRenderer.bounds.extents.x, spriteRenderer.bounds.extents.y, 0);

            // Check if the obstacle is rotated 90 degrees
            Quaternion obstacleRotation = obstacle.transform.rotation;
            float zRotation = obstacleRotation.eulerAngles.z;
            if (zRotation == 90 || zRotation == 270) {
                // Swap x and y coordinates of the local corners
                for (int i = 0; i < 4; i++) {
                    float tempX = localCorners[i].x;
                    localCorners[i].x = localCorners[i].y;
                    localCorners[i].y = -tempX;
                }
            }

            for (int i = 0; i < 4; i++) {
                obstacleCorners[i] = obstacle.transform.position + obstacle.transform.rotation * localCorners[i];
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
