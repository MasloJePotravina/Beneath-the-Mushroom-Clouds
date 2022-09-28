using UnityEngine;
using UnityEngine.InputSystem;

public class FirearmAuto : MonoBehaviour
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


    private PlayerStatus playerStatus;
    private FirearmFunctions fireFunc;



    private float fireRate = 10.0f; //Rounds per second
    private float lastShot;
    private int consecShots = 0;
    private bool shooting = false;

    private float cooldownStart = 0.2f;//After how long after the first shot the recoil starts cooling down
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
        playerStatus = player.GetComponent<PlayerStatus>();
        shooterAbility = playerStatus.shooterAbility;
        UpdateConeLines(shooterAbility * (initialDeviation/2));
        fireFunc = gameManager.GetComponent<FirearmFunctions>();

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
                UpdateConeLines(CalcAimCone());
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
                        UpdateConeLines(CalcAimCone());
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
        
        Vector2 aimAngle = transform.parent.transform.up;

        aimAngle = fireFunc.ApplyAimErrorToRaycast(aimAngle, CalcAimCone());

        hits = Physics2D.RaycastAll(muzzle.transform.position, aimAngle);
        float halfWallDistance = -1.0f;//If the bullet passes a half wall we need to store
                                       //the distance from the wall for future calculations
        foreach (RaycastHit2D hit in hits)
        {              
            if(hit.transform.CompareTag("HalfWall"))
            {
                if (!fireFunc.HalfWallPassed(hit.distance)) //If bullet hit the wall draw bullet line
                {
                    fireFunc.BulletLine(hit, muzzle.transform.position);
                    break;
                }
                else //If not proceed with next collider
                {
                    halfWallDistance = hit.distance;
                    continue;
                }
            }
            //TODO:This whole implementation will be moved somewhere else
            if (hit.transform.CompareTag("NPC"))
            {
                if(fireFunc.NPCHit(halfWallDistance, hit))
                {
                    fireFunc.BulletLine(hit, muzzle.transform.position);
                    //Destroy(hit.transform.gameObject);
                    break;
                }
                else
                {
                    continue;
                }
            }
            fireFunc.BulletLine(hit, muzzle.transform.position);
            //After something is hit the bullet does not travel further
            break;
        }
    }

    

    //Calculate aiming error of player based on the characters shooting ability
    //Better shooting ability -> smaller aim cone
    //shooter ability modifier between 0 and 1
    private float CalcAimCone()
    {

        //Summary: The weapon is less accurate with more consecutive shots, this stops after 10 rounds
        // The best shooter is twice as good at controling recoil than the worst shooter
        float consecShotsModifier = (bulletDevIncrement - 0.5f*(1 - shooterAbility)) * consecShots;

        float degrees = initialDeviation * shooterAbility + consecShotsModifier;

        //Degrees are halved here before returining (half the degrees to each side)
        degrees *= 0.5f;

        return degrees;
    }


    private void UpdateConeLines(float degrees)
    { 

        coneLineL.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, degrees);
        coneLineR.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -degrees);

    }

}
