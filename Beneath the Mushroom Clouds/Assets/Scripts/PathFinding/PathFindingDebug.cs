using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a debug grid, used to debug the pathfinding of enemies and build the map.
/// </summary>
public class PathFindingDebug : MonoBehaviour
{
    /// <summary>
    /// Reference to the player.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Prefab of a pathfinding debug tile.
    /// </summary>
    [SerializeField] private GameObject pathFindingTilePrefab;

    /// <summary>
    /// Reference to the pathfinding grid.
    /// </summary>
    private PathFindingGrid grid;

    /// <summary>
    /// Previous position of the player on the grid.
    /// </summary>
    private Vector2Int previousPlayerPositionOnGrid;

    /// <summary>
    /// List of all the text objects in the debug grid.
    /// </summary>
    private List<TextMesh> tileTextList = new List<TextMesh>();

    /// <summary>
    /// Reference to pathfinding.
    /// </summary>
    private PathFinding pathFinding;
    
    /// <summary>
    /// Dimension of the debug grid which moves with the player.
    /// </summary>
    private int debugGridDimension = 40;

    /// <summary>
    /// Offset of the debug grid which moves with the player. Half of the dimension.
    /// </summary>
    private int debugGridOffset = 20;
    
    /// <summary>
    /// Gets the necessary references and initializes the debug grid on start.
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        pathFinding = PathFinding.Instance;
        grid = pathFinding.grid;
        previousPlayerPositionOnGrid = grid.WorldToGridPosition(player.transform.position);
        
        Vector2Int currentPlayerPositionOnGrid = grid.WorldToGridPosition(player.transform.position);
        for(int x = 0; x < debugGridDimension; x++)
        {
            for(int y = 0; y < debugGridDimension; y++)
            {
                InstantiateText(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// Each frame updates the position of the grid and its values, if the player moved.
    /// </summary>
    void Update(){
        Vector2Int currentPlayerPositionOnGrid = grid.WorldToGridPosition(player.transform.position);
        if(currentPlayerPositionOnGrid != previousPlayerPositionOnGrid){
            previousPlayerPositionOnGrid = currentPlayerPositionOnGrid;
            Vector3 newDebugGridPosition = grid.GridToWorldPosition(currentPlayerPositionOnGrid);
            this.transform.position = new Vector3(newDebugGridPosition.x-5f, newDebugGridPosition.y-5f, -20);
            UpdatePathFindingTiles(currentPlayerPositionOnGrid);
        }

    }

    
    /// <summary>
    /// Instantiates the text for a node in the debug grid.
    /// </summary>
    /// <param name="gridPosition">The position of the tile which is being instantiated.</param>
    private void InstantiateText(Vector2Int gridPosition){
        Vector3 localPosition = new Vector3((gridPosition.x-debugGridOffset)*10, (gridPosition.y-debugGridOffset)*10, -20);
        GameObject text = Instantiate(pathFindingTilePrefab, localPosition, Quaternion.identity, this.transform);
        TextMesh textElement = text.transform.Find("Text").GetComponent<TextMesh>();
        tileTextList.Add(textElement);
    }

    
    /// <summary>
    /// Updates the pathfinding tiles in the debug grid.
    /// </summary>
    /// <param name="currentPlayerPositionOnGrid">Current position of the player on the grid.</param>
    private void UpdatePathFindingTiles(Vector2Int currentPlayerPositionOnGrid){
        for(int x = 0; x < debugGridDimension; x++)
        {
            for(int y = 0; y < debugGridDimension; y++)
            {   
                PathNode node = grid.GetNode(currentPlayerPositionOnGrid + new Vector2Int(x-debugGridOffset, y-debugGridOffset));
                if(node == null){
                    continue;
                }
                SetWalkableValues(x*debugGridDimension+y,node.isWalkable, node.isOccupied);
            }
        }
    }

    /// <summary>
    /// Sets the symbol on a tile in a debug grid to: 0 if walkable, X if not walkable, Y if occupied.
    /// </summary>
    /// <param name="textIndex">Text index of the node.</param>
    /// <param name="isWalkable">Whether the node is walkable.</param>
    /// <param name="isOccupied">Whether the node is occupied.</param>
    private void SetWalkableValues(int textIndex, bool isWalkable, bool isOccupied){
        //currentPlayerPositionOnGrid + new Vector2Int(x-debugGridOffset, y-debugGridOffset)
        //currentPlayerPositionOnGrid + new Vector2Int(x-debugGridOffset, y-debugGridOffset)
        if(isWalkable && !isOccupied){
            tileTextList[textIndex].text = "0";
            tileTextList[textIndex].color = Color.white;
        }else if(!isWalkable){
            tileTextList[textIndex].text = "X";
            tileTextList[textIndex].color = Color.red;
        }else if(isOccupied){
            tileTextList[textIndex].text = "Y";
            tileTextList[textIndex].color = Color.yellow;
        }
    }
        

}
