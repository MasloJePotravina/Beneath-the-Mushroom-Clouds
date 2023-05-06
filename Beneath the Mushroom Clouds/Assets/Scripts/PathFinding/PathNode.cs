using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Path node used to represent a position in pathfinding.
/// </summary>
public class PathNode
{   
    /// <summary>
    /// Reference to the pathfinding grid.
    /// </summary>
    private PathFindingGrid grid;

    /// <summary>
    /// X position of the node in the grid.
    /// </summary>
    public int x;

    /// <summary>
    /// Y position of the node in the grid.
    /// </summary>
    public int y;
    

    /// <summary>
    /// G cost of the node. Cost of moving from the starting node to this node.
    /// </summary>
    public int gCost = 0;

    /// <summary>
    /// H cost of the node. Cost of moving from this node to the end node.
    /// </summary>
    public int hCost = 0;

    /// <summary>
    /// F cost of the node. Sum of G and H costs.
    /// </summary>
    /// <value></value>
    public int fCost { get { return gCost + hCost; } }

    /// <summary>
    /// Whether the node is walkable or not.
    /// </summary>
    public bool isWalkable;

    /// <summary>
    /// Whether the node is occupied or not.
    /// </summary>
    public bool isOccupied;

    /// <summary>
    /// Reference to the node which came before this node in the pathfinding process.
    /// </summary>
    public PathNode cameFromNode;

    /// <summary>
    /// Constructor of the path node.
    /// </summary>
    /// <param name="grid">Reference to the pathfinding grid.</param>
    /// <param name="x">X position of the path node in the grid.</param>
    /// <param name="y">Y position of the path node in the grid.</param>
    public PathNode(PathFindingGrid grid, int x, int y){
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
        isOccupied = false;
        cameFromNode = null;
    }

}
