using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{

    public GameObject player;
    private GameInputActions inputActions;
    private PlayerStatus status;
    public float distanceFromPlayer;
    public float smoothness;

    private void Awake()
    {
        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        status = player.GetComponent<PlayerStatus>();
    }

    // Update is called once per frame, after all else is updated
    void FixedUpdate()
    {
        if (status.playerAiming)
        {
            //Get the world coordinates of the mouse
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(inputActions.Player.MousePosition.ReadValue<Vector2>());
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
            Vector3 onPlayer3D = new(player.transform.position.x, player.transform.position.y, distanceFromPlayer);
            transform.position = Vector3.Lerp(transform.position, onPlayer3D, smoothness);
        }
    }

}
