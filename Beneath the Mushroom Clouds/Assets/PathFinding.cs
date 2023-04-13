//Taken from: https://www.youtube.com/watch?v=alU04hvz6L4
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    public PathFindingGrid grid;

    public static PathFinding Instance { get; private set;}

    private List<PathNode> openList;
    //Using hashset as we only need to see if a node is in the closed list and no other information
    private HashSet<PathNode> closedList;
    // Start is called before the first frame update

    private string[] blockingLayers = new string[]{"PathFindPadding"};
    public PathFinding(int width, int height, bool[,] walkableGrid){
        
        this.grid = new PathFindingGrid(width, height);

        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                grid.GetNode(new Vector2Int(x, y)).isWalkable = walkableGrid[x, y];
            }
        }

        Instance = this;
    }

    public List<Vector3> FindIdealPath(Vector3 startWorldPosition, Vector3 endWorldPosition){
        List<Vector3> vectorPath = FindPath(startWorldPosition, endWorldPosition);
        if(vectorPath == null){
            return null;
        }
        else{
            InterpolatePath(vectorPath);
            vectorPath[vectorPath.Count - 1] += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            return vectorPath;
        }
        
    }

    private void InterpolatePath(List<Vector3> vectorPath){

        int endIndex = vectorPath.Count - 1;
        int startIndex = 0;

        while (startIndex < endIndex - 1)
        {
            Vector3 start = vectorPath[startIndex];
            Vector3 end = vectorPath[endIndex];
            RaycastHit2D hit = Physics2D.Linecast(start, end, LayerMask.GetMask(blockingLayers));
            bool hasObstacle = hit.collider != null;

            if (!hasObstacle)
            {
                // Remove all the positions in between start and end
                vectorPath.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
                // Update endIndex
                endIndex = startIndex + 1;
                
            }
            else
            {
                // Move to the next startIndex
                startIndex++;
            }
        }
        
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition){
        Vector2Int startNodePos = grid.WorldToGridPosition(startWorldPosition);
        Vector2Int endNodePos = grid.WorldToGridPosition(endWorldPosition);
        List<PathNode> path = FindPath(startNodePos, endNodePos);
        if(path == null){
            return null;
        }
        else{
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(PathNode node in path){
                vectorPath.Add(grid.GridToWorldPosition(new Vector2Int(node.x, node.y)));
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(Vector2Int startNodePos, Vector2Int endNodePos){
        PathNode startNode = grid.GetNode(startNodePos);
        PathNode endNode = grid.GetNode(endNodePos);

        //The player can theoretically land on a non walkable node in which case the alogrithm will find a the closes walkable
        //node and move to it, if there are no neighbouring walkable nodes then the algorithm will return null
        if(!endNode.isWalkable || endNode.isOccupied){
            endNode = FindAvailableNeighbour(endNode);
            if(endNode == null){
                return null;
            }
        }

        

        openList = new List<PathNode>(){startNode};
        closedList = new HashSet<PathNode>();
        openList.Add(startNode);


        for(int x = 0; x < grid.width; x++){
            for(int y = 0; y < grid.height; y++){
                PathNode node = grid.GetNode(new Vector2Int(x, y));
                node.gCost = int.MaxValue;
                node.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, endNode);

        while(openList.Count > 0){
            PathNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode){
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode)){
                if(closedList.Contains(neighbourNode)) continue;
                if(!neighbourNode.isWalkable){
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + GetDistance(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost){
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = GetDistance(neighbourNode, endNode);

                    if(!openList.Contains(neighbourNode)){
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    private List<PathNode> CalculatePath(PathNode endNode){
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null){
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode){
        List<PathNode> neighbourList = new List<PathNode>();

        if(currentNode.x - 1 >= 0){
            //Left
            neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x - 1, currentNode.y)));
            //Left Down
            if(currentNode.y - 1 >= 0){
                neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x - 1, currentNode.y - 1)));
            }
            //Left Up
            if(currentNode.y + 1 < grid.height){
                neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x - 1, currentNode.y + 1)));
            }
        }

        if(currentNode.x + 1 < grid.width){
            //Right
            neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x + 1, currentNode.y)));
            //Right Down
            if(currentNode.y - 1 >= 0){
                neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x + 1, currentNode.y - 1)));
            }
            //Right Up
            if(currentNode.y + 1 < grid.height){
                neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x + 1, currentNode.y + 1)));
            }
        }

        //Down
        if(currentNode.y - 1 >= 0){
            neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x, currentNode.y - 1)));
        }

        //Up
        if(currentNode.y + 1 < grid.height){
            neighbourList.Add(grid.GetNode(new Vector2Int(currentNode.x, currentNode.y + 1)));
        }

        return neighbourList;
    }

    private int GetDistance(PathNode a, PathNode b){
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return Mathf.Min(xDistance, yDistance) * 14 + remaining * 10;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList){
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++){
            if(pathNodeList[i].fCost < lowestFCostNode.fCost){
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private PathNode FindAvailableNeighbour(PathNode node){
        List<PathNode> neighbourList = GetNeighbourList(node);
        foreach(PathNode neighbour in neighbourList){
            if(neighbour.isWalkable && !neighbour.isOccupied){
                return neighbour;
            }
        }
        return null;
    }

 
}
