using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class firearmAuto : MonoBehaviour
{
    private GameInputActions inputActions;

    public GameObject bulletPrefab;
    public GameObject muzzle;

    private float fireRate = 10.0f; //Rounds per second
    private float lastShot;
    private bool shooting = false;

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
        inputActions.Player.Fire.started -= PressTrigger;
        inputActions.Player.Fire.canceled -= PressTrigger;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputActions.Player.Fire.started += PressTrigger;
        inputActions.Player.Fire.canceled += PressTrigger;
    }

    // Update is called once per frame
    void Update()
    {
        //Source: https://answers.unity.com/questions/761026/automatic-shooting-script.html
        if (shooting)
        {
            if(Time.time - lastShot > 1 / fireRate)
            {
                Shoot();
                lastShot = Time.time;
            }
        }
    }

    private void PressTrigger(InputAction.CallbackContext context)
    {
        if (!shooting)
        {
            shooting = true;
        }
        else
        {
            shooting = false;
        }
        
    }

    private void Shoot()
    {
        RaycastHit2D[] hits;
        
        //This whole aimAngle thing is used to fire a raycast in the direction the player is facing
        //instead of the direction towards the mouse. You might wonder why since the player always faces the mouse anyways.
        //It's because when the player decides to place the mouse between humself and the muzzle of the weapon the ray
        //would be cast towards the player, not out of the gun.
        //How it works: Directional vector to the right of the player is rotated 90 degrees cc using the (x,y) -> (-y,x) trick
        Vector2 aimAngle = transform.parent.transform.right;
        float tmpXCoord = aimAngle.x;
        aimAngle.x = -aimAngle.y;
        aimAngle.y = tmpXCoord;
        hits = Physics2D.RaycastAll(transform.parent.transform.position, aimAngle);

        Debug.Log(transform.parent.transform.right);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag != "Player" && hit.transform.tag != "Weapon")
            {
                //This is where a chance to miss intersected object (such as half wall) will be implemented later
                bulletLine(hit);
                //After something is hit the bullet does not travel further
                break;
            }
        }
            

    }

    private void bulletLine(RaycastHit2D hit)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        LineRenderer lineRenderer = bullet.GetComponent<LineRenderer>();
        Vector3[] trajectory = { muzzle.transform.position, hit.point };
        lineRenderer.SetPositions(trajectory);
    }
}
