using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the pathfinding grid for NPCs.
/// </summary>
public class PathFindingGrid
{

    /// <summary>
    /// Width of the grid.
    /// </summary>
    public int width;
    /// <summary>
    /// Height of the grid.
    /// </summary>
    public int height;

    /// <summary>
    /// Array of path nodes representing the grid.
    /// </summary>
    public PathNode[,] gridArray;

    /// <summary>
    /// Offset of the grid in the world. Half of the dimension.
    /// </summary>
    private float worldToGridOffset = 1000f;

    /// <summary>
    /// Constructor of the pathfinding grid.
    /// </summary>
    /// <param name="width">Width of the pathfinding grid.</param>
    /// <param name="height">Height of the pathfinding grid.</param>
    public PathFindingGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridArray = new PathNode[width, height];

        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                gridArray[x, y] = new PathNode(this, x, y);
            }
        }
    }

    /// <summary>
    /// Converts a world position to a grid position.
    /// </summary>
    /// <param name="worldPosition">Vector3 world position.</param>
    /// <returns>Vector2Int grid position.</returns>
    public Vector2Int WorldToGridPosition(Vector3 worldPosition){
        Vector2Int positionOnGrid = new Vector2Int((int)((worldPosition.x + worldToGridOffset)/10f), (int)((worldPosition.y + worldToGridOffset)/10f));
        return positionOnGrid;
    }

    /// <summary>
    /// Converts a grid position to a world position.
    /// </summary>
    /// <param name="gridPosition">Vector2Int grid position.</param>
    /// <returns>Vector3 world position.</returns>
    public Vector3 GridToWorldPosition(Vector2Int gridPosition){
        Vector3 worldPosition = new Vector3(gridPosition.x * 10f - worldToGridOffset +5, gridPosition.y * 10f - worldToGridOffset +5, 0);
        return worldPosition;
    }

    /// <summary>
    /// Gets the pathfinding node at the given position.
    /// </summary>
    /// <param name="nodePosition">Position of the node on the grid.</param>
    /// <returns>Reference to the pathfinding node.</returns>
    public PathNode GetNode(Vector2Int nodePosition){
        if(nodePosition.x < 0 || nodePosition.x >= width || nodePosition.y < 0 || nodePosition.y >= height){
            return null;
        }  
        else
        {
            return gridArray[nodePosition.x, nodePosition.y];
        }
    }

    /// <summary>
    /// Marks a node as occupied.
    /// </summary>
    /// <param name="nodePosition">Position of the node on the grid.</param>
    public void OccupyNode(Vector2Int nodePosition){
        PathNode node = GetNode(nodePosition);
        if(node != null){
            node.isOccupied = true;
        }
    }

    /// <summary>
    /// Marks a node as unoccupied.
    /// </summary>
    /// <param name="nodePosition">Position of the node on the grid.</param>
    public void UnoccupyNode(Vector2Int nodePosition){
        PathNode node = GetNode(nodePosition);
        if(node != null){
            node.isOccupied = false;
        }
    }
    

}
