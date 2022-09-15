using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class firearmAuto : MonoBehaviour
{
    private GameInputActions inputActions;

    public GameObject bulletPrefab;
    public GameObject muzzle;
    public GameObject player;
    public GameObject coneLineL;
    public GameObject coneLineR;

    private float shooterAbility;


    private playerStatus playerStatus;



    private float fireRate = 10.0f; //Rounds per second
    private float lastShot;
    private int consecShots = 0;
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
        playerStatus = player.GetComponent<playerStatus>();
        shooterAbility = playerStatus.shooterAbility;
        resetAimCone();

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
                consecShots += 1;
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
            consecShots = 0;
            resetAimCone();
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
        aimAngle = applyAimErrorToRaycast(aimAngle, calcAimCone());

        hits = Physics2D.RaycastAll(muzzle.transform.position, aimAngle);
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

    //Calculate aiming error of player based on the characters shooting ability
    //Better shooting ability -> smaller aim cone
    //shooter abulity modifier between 0 and 1
    private float calcAimCone()
    {

        //Summary: The weapon is less accurate with more consecutive shots, this stops after 10 rounds
        // The best shooter is twice as good at controling recoil than the worst shooter (5 degree versus 10 degree variation after 10 shots)
        float consecShotsModifier = (1.0f - 0.5f*(1 - shooterAbility)) * Mathf.Min(consecShots, 10);

        //Scenarios for better understansing: 
        //  Best shooter, first shot -> no deviation
        //  Best shooter, after 10 shots -> 5 degree deviation
        //  Worst shooter, first shot -> 5 degree deviation
        //  Worst shooter, after 10 shots -> 15 degree deviation
        float degrees = 5.0f * shooterAbility + consecShotsModifier;

        //Degrees are halved here before returining (half the degrees to each side)
        degrees = degrees / 2;

        coneLineL.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, degrees);
        coneLineR.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -degrees);



        return degrees;
    }

    //Applies the random error within the confines of the cone to the bullet
    //Degrees represent half the angle of the cone (15 degree -> 30degree cone)
    //Source for vector rotation: https://answers.unity.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
    private Vector2 applyAimErrorToRaycast(Vector2 aimAngle, float degrees)
    {
        //True if the bullet will deviate to the right, false if to the left
        //Random.value returns numbers between 0 and 1

        bool clockwiseDeviation = (Random.value <= 0.5f);
        degrees = Random.Range(0, degrees);
        

        float tempX = aimAngle.x;
        float tempY = aimAngle.y;

        if (clockwiseDeviation)
        {
            float sin = Mathf.Sin(-degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(-degrees * Mathf.Deg2Rad);
            aimAngle.x = (cos * tempX) - (sin * tempY);
            aimAngle.y = (sin * tempX) + (cos * tempY);
        }
        else
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
            aimAngle.x = (cos * tempX) - (sin * tempY);
            aimAngle.y = (sin * tempX) + (cos * tempY);
        }
        
        return aimAngle;
    }

    private void resetAimCone()
    {
        coneLineL.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, shooterAbility * 2.5f);
        coneLineR.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -(shooterAbility * 2.5f));
    }
}
