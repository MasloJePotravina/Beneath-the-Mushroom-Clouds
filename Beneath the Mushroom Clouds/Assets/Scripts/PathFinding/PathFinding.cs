//Based on: https://www.youtube.com/watch?v=alU04hvz6L4
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements pathfinding algorithm using the A* algorithm.
/// </summary>
public class PathFinding
{
    /// <summary>
    /// Pathfinding grid.
    /// </summary>
    public PathFindingGrid grid;

    /// <summary>
    /// Instance of the pathfinding class.
    /// </summary>
    public static PathFinding Instance;

    /// <summary>
    /// List of open nodes.
    /// </summary>
    private List<PathNode> openList;
    /// <summary>
    /// Hashset of closed nodes.
    /// </summary>
    private HashSet<PathNode> closedList;
    // Start is called before the first frame update

    /// <summary>
    /// Array of Layers which block the shortening of the path.
    /// </summary>
    private string[] blockingLayers = new string[]{"PathFindPadding"};

    /// <summary>
    /// Constructor for the pathfinding class.
    /// </summary>
    /// <param name="width">Width of pathfinding grid.</param>
    /// <param name="height">Height of the pathfinsing grid.</param>
    /// <param name="walkableGrid">2D bool array the same size as pathfinding grid, marking which nodes are walkable and which are not.</param>
    public PathFinding(int width, int height, bool[,] walkableGrid){
        
        this.grid = new PathFindingGrid(width, height);

        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                grid.GetNode(new Vector2Int(x, y)).isWalkable = walkableGrid[x, y];
            }
        }

        Instance = this;
    }

    /// <summary>
    /// Finds the ideal path (removes unnecessary locations)
    /// </summary>
    /// <param name="startWorldPosition">Starting position of the path.</param>
    /// <param name="endWorldPosition">Ending position of the path.</param>
    /// <returns>List of Vector3 world positions of the ideal path.</returns>
    public List<Vector3> FindIdealPath(Vector3 startWorldPosition, Vector3 endWorldPosition){
        List<Vector3> vectorPath = FindPath(startWorldPosition, endWorldPosition);
        if(vectorPath == null){
            return null;
        }
        else{
            InterpolatePath(vectorPath);
            vectorPath[vectorPath.Count - 1] += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            //Remove the first position as that is the NPC's current position and therefore not needed
            vectorPath.RemoveAt(0);
            return vectorPath;
        }
        
    }

    /// <summary>
    /// Interpolates the points on the path that the A* algorithm found and makes the path more natural.
    /// </summary>
    /// <param name="vectorPath">List of Vector3 world positions of the path found by the A* algorithm.</param>
    private void InterpolatePath(List<Vector3> vectorPath){

        int endIndex = vectorPath.Count - 1;
        int startIndex = 0;

        while (startIndex < endIndex - 1)
        {
            Vector3 start = vectorPath[startIndex];
            Vector3 end = vectorPath[endIndex];
            RaycastHit2D hit = Physics2D.Linecast(start, end, LayerMask.GetMask(blockingLayers));

            if (hit.collider == null)
            {
                // Remove all the positions in between start and end
                vectorPath.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
                // Update endIndex, reset startIndex
                endIndex = startIndex;
                startIndex = 0;
                
            }
            else
            {
                // Move to the next startIndex
                startIndex++;
            }
        }
        
    }


    /// <summary>
    /// Finds the A* path as a list of Vector3 world positions.
    /// </summary>
    /// <param name="startWorldPosition">Starting position of the path.</param>
    /// <param name="endWorldPosition">Ending position of the path.</param>
    /// <returns>List of Vector3 world positions of the path.</returns>
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

    /// <summary>
    /// Finds the A* path as a list of PathNodes.
    /// </summary>
    /// <param name="startNodePos">Starting node of the path.</param>
    /// <param name="endNodePos">Ending node of the path.</param>
    /// <returns>List of PathNodes of the path.</returns>
    public List<PathNode> FindPath(Vector2Int startNodePos, Vector2Int endNodePos){
        PathNode startNode = grid.GetNode(startNodePos);
        PathNode endNode = grid.GetNode(endNodePos);

        //For some reason the end node is null at times, so this just disables pathfinding in that case
        if(endNode == null || startNode == null){
            return null;
        }

        //The player can theoretically land on a non walkable node in which case the alogrithm will find a the closes walkable
        //node and move to it, if there are no neighbouring walkable nodes then the algorithm will return null
        if(!endNode.isWalkable || endNode.isOccupied){
            endNode = FindAvailableNeighbour(endNode);
            if(endNode == null){
                return null;
            }
        }


        //Reset all nodes, set G cost to infinity and cameFromNode to null
        for(int x = 0; x < grid.width; x++){
            for(int y = 0; y < grid.height; y++){
                PathNode node = grid.GetNode(new Vector2Int(x, y));
                node.gCost = int.MaxValue;
                node.cameFromNode = null;
            }
        }

        //Initialise open and closed lists, add the start node to the open list
        openList = new List<PathNode>(){startNode};
        closedList = new HashSet<PathNode>();
        openList.Add(startNode);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, endNode);

        //A* algorithm
        while(openList.Count > 0){
            PathNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode){
                grid.OccupyNode(new Vector2Int(endNode.x, endNode.y));
                return CalculatePath(endNode);
            }

            //Limit F cost to 1000 to prevent too long or complex paths
            if(currentNode.fCost > 1000){
                return null;
            }


            openList.Remove(currentNode);
            closedList.Add(currentNode);
            List <PathNode> neighbourList = GetNeighbourList(currentNode);
            foreach(PathNode neighbourNode in neighbourList){
                if(closedList.Contains(neighbourNode)) continue;
                if(!neighbourNode.isWalkable || neighbourNode.isOccupied){
                    closedList.Add(neighbourNode);
                    continue;
                }

                int potentialGCost = currentNode.gCost + GetDistance(currentNode, neighbourNode);
                if(potentialGCost < neighbourNode.gCost){
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = potentialGCost;
                    neighbourNode.hCost = GetDistance(neighbourNode, endNode);

                    if(!openList.Contains(neighbourNode)){
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Creates the path using the "cameFromNode" variable in each node, starting from the end node and ending at the start node. Then reverses the list.
    /// </summary>
    /// <param name="endNode">The end node of the path.</param>
    /// <returns>List of path nodes representing the path.</returns>
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

    /// <summary>
    /// Gets the neigbours of the current node, including diagonals.
    /// </summary>
    /// <param name="currentNode">Currently evaluated node.</param>
    /// <returns>List of neighbours.</returns>
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

    /// <summary>
    /// Heuristic function which determines the distance between two nodes.
    /// </summary>
    /// <param name="a">First node.</param>
    /// <param name="b">Second node.</param>
    /// <returns>Cost of getting from node a to node b.</returns>
    private int GetDistance(PathNode a, PathNode b){
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return Mathf.Min(xDistance, yDistance) * 14 + remaining * 10;
    }

    /// <summary>
    /// Gets the lowest F cost node from a list.
    /// </summary>
    /// <param name="pathNodeList">List of path nodes.</param>
    /// <returns>Pathnode with the lowest F cost form the list.</returns>
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList){
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++){
            if(pathNodeList[i].fCost < lowestFCostNode.fCost){
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    /// <summary>
    /// Finds an alternative neighbour to a node if the node is not walkable or is occupied.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
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
