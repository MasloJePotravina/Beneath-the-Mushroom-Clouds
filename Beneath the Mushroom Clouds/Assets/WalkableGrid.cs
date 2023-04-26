using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableGrid : MonoBehaviour
{
    public bool[,] grid = new bool[200, 200];
    private List<GameObject> obstacles = new List<GameObject>();
    
    public void Awake(){
        InitializeWalkableGrid();
        FindAllObstacles();
        ConstructWalkableGrid();
        PathFinding pathFinding = new PathFinding(200, 200, grid);
    }


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

    private Vector2Int WorldToGridPosition(Vector3 worldPosition){
        Vector2Int positionOnGrid = new Vector2Int((int)((worldPosition.x + 1000f)/10f), (int)((worldPosition.y + 1000f)/10f));
        return positionOnGrid;
    }

    private void InitializeWalkableGrid(){
        for(int i = 0; i < 200; i++){
            for(int j = 0; j < 200; j++){
                grid[i, j] = true;
            }
        }
    }
}
