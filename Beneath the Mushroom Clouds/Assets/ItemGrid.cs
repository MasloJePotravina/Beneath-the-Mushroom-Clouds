using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{

    const float tileDimension = 64;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    Vector2 positionOnGrid = new Vector2();
    Vector2Int tileOnGrid = new Vector2Int();

    public Vector2Int GetTilePosition(Vector2 mousePosition)
    {

        //The game is designed to be played in 1920x1080 resolution
        //If the resolution is different, the calculations for tiles in the grid will not work properly due to different mouse and grid positions and have to be normalized
        //This is MUCH less efficient and therefore the 1920x1080 resolution is HIGHLY recommended
        //1920x1080: ~290FPS, different resolution: ~250FPS => ~40FPS difference 
        if(Screen.width == 1920 && Screen.height == 1080){
            positionOnGrid.x = mousePosition.x - rectTransform.position.x;
            positionOnGrid.y = rectTransform.position.y - mousePosition.y;

            tileOnGrid.x = (int)(positionOnGrid.x / tileDimension);
            tileOnGrid.y = (int)(positionOnGrid.y / tileDimension);

        }else{
            //Normalize mouse position
            mousePosition.x = mousePosition.x/Screen.width;
            mousePosition.y = mousePosition.y/Screen.height;

            //Normalize width and height of the grid to reference resolution of 1920x1080
            float normalizedGridWidth = rectTransform.rect.width/1920;
            float normalizedGridHeight = rectTransform.rect.height/1080;
            
            //Normalize position of the grid
            Vector2 normalizedGridPosition = new Vector2(rectTransform.position.x/Screen.width, rectTransform.position.y/Screen.height);


            //Upper left corner and lower right corner of the grid normalized
            Vector2 upperLeftCorner = new Vector2(normalizedGridPosition.x, normalizedGridPosition.y);
            Vector2 lowerRightCorner = new Vector2(normalizedGridPosition.x + normalizedGridWidth, normalizedGridPosition.y - normalizedGridHeight);


            positionOnGrid.x = (mousePosition.x - upperLeftCorner.x)/(lowerRightCorner.x - upperLeftCorner.x);
            positionOnGrid.y = (upperLeftCorner.y - mousePosition.y)/(upperLeftCorner.y - lowerRightCorner.y);

            tileOnGrid.x = (int)(positionOnGrid.x / (tileDimension/rectTransform.rect.width));
            tileOnGrid.y = (int)(positionOnGrid.y / (tileDimension/rectTransform.rect.height));
        }
            

        

        return tileOnGrid;
    }
}
