using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingDebug : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject pathFindingTilePrefab;
    private PathFindingGrid grid;
    private Vector2Int previousPlayerPositionOnGrid;
    private List<TextMesh> tileTextList = new List<TextMesh>();
    private PathFinding pathFinding;
    
    private int debugGridDimension = 40;
    private int debugGridOffset = 20;
    // Start is called before the first frame update
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

    void Update(){
        Vector2Int currentPlayerPositionOnGrid = grid.WorldToGridPosition(player.transform.position);
        if(currentPlayerPositionOnGrid != previousPlayerPositionOnGrid){
            previousPlayerPositionOnGrid = currentPlayerPositionOnGrid;
            Vector3 newDebugGridPosition = grid.GridToWorldPosition(currentPlayerPositionOnGrid);
            this.transform.position = new Vector3(newDebugGridPosition.x-5f, newDebugGridPosition.y-5f, -20);
            UpdatePathFindingTiles(currentPlayerPositionOnGrid);
        }

    }

    

    private void InstantiateText(Vector2Int gridPosition){
        Vector3 localPosition = new Vector3((gridPosition.x-debugGridOffset)*10, (gridPosition.y-debugGridOffset)*10, -20);
        GameObject text = Instantiate(pathFindingTilePrefab, localPosition, Quaternion.identity, this.transform);
        TextMesh textElement = text.transform.Find("Text").GetComponent<TextMesh>();
        tileTextList.Add(textElement);
    }

    

    private void UpdatePathFindingTiles(Vector2Int currentPlayerPositionOnGrid){
        for(int x = 0; x < debugGridDimension; x++)
        {
            for(int y = 0; y < debugGridDimension; y++)
            {   
                PathNode node = grid.GetNode(currentPlayerPositionOnGrid + new Vector2Int(x-debugGridOffset, y-debugGridOffset));
                SetWalkableValues(x*debugGridDimension+y,node.isWalkable, node.isOccupied);
            }
        }
    }

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
