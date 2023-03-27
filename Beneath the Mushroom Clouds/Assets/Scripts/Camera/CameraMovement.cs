using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Implements the behaviour of the main camera.
/// </summary>
public class CameraMovement : MonoBehaviour
{

    /// <summary>
    /// Reference to the player.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Reference to the Player Status script.
    /// </summary>
    private PlayerStatus status;

    /// <summary>
    /// How far the camera is from the player in the Z axis.
    /// </summary>
    public float distanceFromPlayer;

    /// <summary>
    /// Smoothness of the camera movement.
    /// </summary>
    public float smoothness;

    /// <summary>
    /// Current mouse position. Set by the PlayerControls script.
    /// </summary>
    private Vector2 mousePosition;

    /// <summary>
    /// Initialize the Player Status script.
    /// </summary>
    void Start()
    {
        status = player.GetComponent<PlayerStatus>();
    }

    /// <summary>
    /// Each frame, update the camera position based on the player's position, mouse position and aiming status.
    /// </summary>
    void FixedUpdate()
    {
        if (status.playerAiming)
        {
            //Get the world coordinates of the mouse
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
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
            Vector3 positionAbovePlayer = new Vector3(player.transform.position.x, player.transform.position.y, distanceFromPlayer);
            transform.position = Vector3.Lerp(transform.position, positionAbovePlayer, smoothness);
        }
    }


    public void SetMousePosition(Vector2 mousePosition)
    {
        this.mousePosition = mousePosition;
    }

}
