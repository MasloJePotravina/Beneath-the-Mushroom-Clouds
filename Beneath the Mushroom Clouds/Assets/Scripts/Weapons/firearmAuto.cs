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

    private float cooldownStart = 0.5f;//After how long after the first shot the recoil starts cooling down
    private float cooldownStartTimer = 0.0f;
    private float cooldownRate = 0.1f; //How quickly the firearm recoil cools down
    private float cooldownTimer = 0.0f;

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
        updateConeLines(shooterAbility * 2.5f);//2.5f because we need half of the worst shooter initial deviation (5 degrees)

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
                if(consecShots < 10)
                    consecShots += 1;
                cooldownStartTimer = 0.0f;
            }
        }
        else
        {
            if(consecShots > 0)
            {
                cooldownStartTimer += Time.deltaTime;
                if(cooldownStartTimer > cooldownStart)
                {
                    cooldownTimer += Time.deltaTime;
                    if (cooldownTimer > cooldownRate)
                    {
                        consecShots -= 1;
                        cooldownTimer = 0;
                        updateConeLines(CalcAimCone());
                    }
                }
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
        aimAngle = ApplyAimErrorToRaycast(aimAngle, CalcAimCone());

        hits = Physics2D.RaycastAll(muzzle.transform.position, aimAngle);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag != "Player" && hit.transform.tag != "Weapon")
            {
                if(hit.transform.tag == "Half Wall")
                {
                    if (!HalfWallPassed(hit.distance)) //If bullet hit the wall draw bullet line
                    {
                        BulletLine(hit);
                        break;
                    }
                    else //If not proceed with next collider
                    {
                        continue;
                    }
                }
                //This is where a chance to miss intersected object (such as half wall) will be implemented later
                BulletLine(hit);
                //After something is hit the bullet does not travel further
                break;
            }
        }
            

    }

    private void BulletLine(RaycastHit2D hit)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        LineRenderer lineRenderer = bullet.GetComponent<LineRenderer>();
        Vector3[] trajectory = { muzzle.transform.position, hit.point };
        lineRenderer.SetPositions(trajectory);
    }

    //Calculate aiming error of player based on the characters shooting ability
    //Better shooting ability -> smaller aim cone
    //shooter abulity modifier between 0 and 1
    private float CalcAimCone()
    {

        //Summary: The weapon is less accurate with more consecutive shots, this stops after 10 rounds
        // The best shooter is twice as good at controling recoil than the worst shooter (5 degree versus 10 degree variation after 10 shots)
        float consecShotsModifier = (1.0f - 0.5f*(1 - shooterAbility)) * consecShots;

        //Scenarios for better understansing: 
        //  Best shooter, first shot -> no deviation
        //  Best shooter, after 10 shots -> 5 degree deviation
        //  Worst shooter, first shot -> 5 degree deviation
        //  Worst shooter, after 10 shots -> 15 degree deviation
        float degrees = 5.0f * shooterAbility + consecShotsModifier;

        //Degrees are halved here before returining (half the degrees to each side)
        degrees = degrees / 2;

        updateConeLines(degrees);



        return degrees;
    }

    //Applies the random error within the confines of the cone to the bullet
    //Degrees represent half the angle of the cone (15 degree -> 30degree cone)
    //Source for vector rotation: https://answers.unity.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
    private Vector2 ApplyAimErrorToRaycast(Vector2 aimAngle, float degrees)
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


    //distance - distance between the shooter and the wall
    private bool HalfWallPassed(float distance)
    {
        Debug.Log(distance);
        if(distance < 50.0f)
        {
            return true; //bullet passes 100% of the time if shooter is less than 5 metres from HW
        }
        else if (distance > 150.0f)
        {
            return (Random.value > 0.5); //bullet passes 50% of the time if shooter is further than 15 metres from HW
        }
        else
        {
            return (Random.value > (distance - 50.0f) * 0.005);//linear dropoff between 5 metres and 15 metres (from 100% to 50%)
        }
    }


    private void updateConeLines(float degrees)
    {
        coneLineL.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, degrees);
        coneLineR.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -degrees);
    }
}
