using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class firearmShotgun : MonoBehaviour
{
    private GameInputActions inputActions;

    public GameObject muzzle;
    public GameObject player;
    public GameObject coneLineL;
    public GameObject coneLineR;
    public GameObject gameManager;

    public float initialDeviation; //Worst case initial bullet deviaiton (degrees)
    public float bulletDevIncrement; //How many degrees does another fired bullet add at most

    private float shooterAbility;


    private playerStatus playerStatus;
    private firearmFunctions fireFunc;

    private int consecShots = 0;

    private float cooldownStart = 0.5f;//After how long after the first shot the recoil starts cooling down
    private float cooldownStartTimer = 0.0f;
    private float cooldownRate = 0.1f; //How quickly the firearm recoil cools down
    private float cooldownTimer = 0.0f;

    private float shotgunSpread = 5.0f;

    private float fireRateTimer = 0.0f;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        inputActions.Player.Fire.started += PressTrigger;
        playerStatus = player.GetComponent<playerStatus>();
        fireFunc = gameManager.GetComponent<firearmFunctions>();
        shooterAbility = playerStatus.shooterAbility;
        updateConeLines(shooterAbility * (initialDeviation / 2));

    }

    // Update is called once per frame
    void Update()
    {
        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }


        if (consecShots > 0)
        {
            cooldownStartTimer += Time.deltaTime;
            if (cooldownStartTimer > cooldownStart)
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

    private void PressTrigger(InputAction.CallbackContext context)
    {
        if (fireRateTimer <= 0)
        {
            Shoot();
            if (consecShots < 5)
                consecShots += 1;
            cooldownStartTimer = 0.0f;
            fireRateTimer = 1.5f;
            updateConeLines(CalcAimCone());
        }
    }

    private void Shoot()
    {
        RaycastHit2D[] hits;

        Vector2 aimAngle = transform.parent.transform.up;
        aimAngle = fireFunc.ApplyAimErrorToRaycast(aimAngle, CalcAimCone());

        for(int i = 0; i < 8; i++)
        {
            Vector2 pelletAimAngle = fireFunc.ApplyAimErrorToRaycast(aimAngle, shotgunSpread/2);
            hits = Physics2D.RaycastAll(muzzle.transform.position, pelletAimAngle);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.tag != "Player" && hit.transform.tag != "Weapon")
                {
                    if (hit.transform.tag == "Half Wall")
                    {
                        if (!fireFunc.HalfWallPassed(hit.distance)) //If bullet hit the wall draw bullet line
                        {
                            fireFunc.BulletLine(hit, muzzle.transform.position);
                            break;
                        }
                        else //If not proceed with next collider
                        {
                            continue;
                        }
                    }
                    //This is where a chance to miss intersected object (such as half wall) will be implemented later
                    fireFunc.BulletLine(hit, muzzle.transform.position);
                    //After something is hit the bullet does not travel further
                    break;
                }
            }
        }
    }

    //Calculate aiming error of player based on the characters shooting ability
    //Better shooting ability -> smaller aim cone
    //shooter abulity modifier between 0 and 1
    private float CalcAimCone()
    {

        //Summary: The weapon is less accurate with more consecutive shots, this stops after 10 rounds
        // The best shooter is twice as good at controling recoil than the worst shooter (5 degree versus 10 degree variation after 10 shots)
        float consecShotsModifier = (bulletDevIncrement - 1 * (1 - shooterAbility)) * consecShots;

        //Scenarios for better understansing: 
        //  Best shooter, first shot -> no deviation
        //  Best shooter, after 10 shots -> 5 degree deviation
        //  Worst shooter, first shot -> 5 degree deviation
        //  Worst shooter, after 10 shots -> 15 degree deviation
        float degrees = initialDeviation * shooterAbility + consecShotsModifier;

        //Degrees are halved here before returining (half the degrees to each side)
        degrees = degrees / 2;


        return degrees;
    }


    private void updateConeLines(float degrees)
    {
        coneLineL.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, degrees);
        coneLineR.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -degrees);
    }
}
