using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private PathFindingGrid grid;
    public int x;
    public int y;
    
    public int gCost = 0;
    public int hCost = 0;
    public int fCost { get { return gCost + hCost; } }

    public bool isWalkable;
    public bool isOccupied;
    public PathNode cameFromNode;


    public PathNode(PathFindingGrid grid, int x, int y){
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
        isOccupied = false;
        cameFromNode = null;
    }

}
