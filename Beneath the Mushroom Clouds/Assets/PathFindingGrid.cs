//Based on https://www.youtube.com/watch?v=waEsGu--9P8
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingGrid
{

    public int width;
    public int height;

    public PathNode[,] gridArray;

    private float worldToGridOffset = 1000f;

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

    public Vector2Int WorldToGridPosition(Vector3 worldPosition){
        Vector2Int positionOnGrid = new Vector2Int((int)((worldPosition.x + worldToGridOffset)/10f), (int)((worldPosition.y + worldToGridOffset)/10f));
        return positionOnGrid;
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition){
        Vector3 worldPosition = new Vector3(gridPosition.x * 10f - worldToGridOffset +5, gridPosition.y * 10f - worldToGridOffset +5, 0);
        return worldPosition;
    }


    public PathNode GetNode(Vector2Int nodePosition){
        if(nodePosition.x < 0 || nodePosition.x >= width || nodePosition.y < 0 || nodePosition.y >= height){
            return null;
        }  
        else
        {
            return gridArray[nodePosition.x, nodePosition.y];
        }
    }

    public void OccupyNode(Vector2Int nodePosition){
        PathNode node = GetNode(nodePosition);
        if(node != null){
            node.isOccupied = true;
        }
    }

    public void UnoccupyNode(Vector2Int nodePosition){
        PathNode node = GetNode(nodePosition);
        if(node != null){
            node.isOccupied = false;
        }
    }
    

}
