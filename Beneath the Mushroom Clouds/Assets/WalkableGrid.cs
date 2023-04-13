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
            Vector3[] obstacleCorners = new Vector3[4];
            Vector2Int[] obstacleCornersOnGrid = new Vector2Int[4];
            Bounds spriteBounds = obstacle.GetComponent<SpriteRenderer>().bounds;

            obstacleCorners[0] = new Vector3(spriteBounds.min.x, spriteBounds.min.y, 0);
            obstacleCorners[1] = new Vector3(spriteBounds.min.x, spriteBounds.max.y, 0);
            obstacleCorners[2] = new Vector3(spriteBounds.max.x, spriteBounds.max.y, 0);
            obstacleCorners[3] = new Vector3(spriteBounds.max.x, spriteBounds.min.y, 0);


            System.Array.Sort(obstacleCorners, (a, b) =>
            {
                if (a.x != b.x)
                    return a.x.CompareTo(b.x);
                else
                    return a.y.CompareTo(b.y);
            });

            for(int i = 0; i < 4; i++){
                obstacleCornersOnGrid[i] = WorldToGridPosition(obstacleCorners[i]);
                Debug.DrawLine(obstacle.transform.position, obstacleCorners[i], Color.red, 100f);
            }


            for(int i = obstacleCornersOnGrid[0].x; i <= obstacleCornersOnGrid[2].x; i++){
                for(int j = obstacleCornersOnGrid[0].y; j <= obstacleCornersOnGrid[1].y; j++){
                    grid[i, j] = false;
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
