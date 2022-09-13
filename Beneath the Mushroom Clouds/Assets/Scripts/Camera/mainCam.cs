using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCam : MonoBehaviour
{

    public GameObject player;
    public InputProvider playerInput;
    public float distanceFromPlayer = -1;
    public float smoothness = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = new InputProvider();
        playerInput.Enable();
    }

    // Update is called once per frame, after all else is updated
    void FixedUpdate()
    {
        if (playerInput.AimingInput())
        {
            //Get the world coordinates of the mouse
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(playerInput.MousePosition());
            //Get the player position (automatically cuts off the Z coordinate)
            Vector2 playerWorldPos = player.transform.position;

            //Midpoint between the mouse and the player in 2D
            Vector2  midPoint2D = Vector2.Lerp(playerWorldPos, mouseWorldPos, 0.5f);
            //Midpoint between the mouse and the player in 3d
            Vector3 midPoint3D = new (midPoint2D.x, midPoint2D.y, distanceFromPlayer);
            //Smooth camera movement
            transform.position = Vector3.Lerp(transform.position, midPoint3D, smoothness);
        }
        else
        {
            //Smooth player follow and smooth snap back to player after aiming
            Vector3 onPlayer3D = new Vector3(player.transform.position.x, player.transform.position.y, distanceFromPlayer);
            transform.position = Vector3.Lerp(transform.position, onPlayer3D, smoothness);
        }
    }

}
