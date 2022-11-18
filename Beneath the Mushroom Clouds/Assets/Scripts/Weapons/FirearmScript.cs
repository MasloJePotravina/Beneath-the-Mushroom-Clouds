using UnityEngine;

public class FirearmScript: MonoBehaviour
{

    public GameObject muzzle;
    public GameObject player;
    public GameObject coneLineL;
    public GameObject coneLineR;
    public GameObject bulletPrefab;


    public float initialDeviation; //Worst case initial bullet deviaiton (degrees)
    public float bulletDevIncrement; //How many degrees does another fired bullet add at most

    private float shooterAbility;


    private PlayerStatus playerStatus;

    private bool firearmActive;
    private int firearmMode;


    private float fireRate = 10.0f; //Rounds per second
    private float fireRateTimer = 0.0f;
    private float lastShot;
    private int consecShots = 0;
    private bool triggerPressed = false; //Is the trigger pressed
    private bool semiBlock = false; //Has a semi/pump/bolt-action fired already on this press

    private float cooldownStart = 0.2f;//After how long after the first shot the recoil starts cooling down
    private float cooldownStartTimer = 0.0f;
    private float cooldownRate = 0.1f; //How quickly the firearm recoil cools down
    private float cooldownTimer = 0.0f;

    private float shotgunSpread = 5.0f;

    private RaycastHit2D[] hits; //Field of hit objects


    // Start is called before the first frame update
    void Start()
    {
        playerStatus = player.GetComponent<PlayerStatus>();
        shooterAbility = playerStatus.shooterAbility;
        UpdateConeLines(shooterAbility * (initialDeviation/2));

    }

    // Update is called once per frame
    void Update()
    {
        //Consecutive shots accuracy cooldown
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
                    UpdateConeLines(CalcAimCone());
                }
            }
        }
        //Weapon fire rate timer (shotguns and bolt-actions)
        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }
        //Early exit if the firearm is not active or is not being shot
        if (!firearmActive)
            return;
        if (!triggerPressed)
            return;
        
        //Trigger is pressed
        Vector2 aimAngle = ApplyAimErrorToRaycast(transform.parent.transform.up, CalcAimCone());
        switch (firearmMode)
        {
            case 0://Assault Rifle Mode
                //Source: https://answers.unity.com/questions/761026/automatic-shooting-script.html
                if (Time.time - lastShot > 1 / fireRate)
                {
                    Shoot(aimAngle);
                    lastShot = Time.time;
                    if (consecShots < 10)
                        consecShots += 1;
                    cooldownStartTimer = 0.0f;
                    UpdateConeLines(CalcAimCone());
                }
                break;
            case 1://Pistol Mode
                if (fireRateTimer <= 0 && !semiBlock)
                {
                    Shoot(aimAngle);
                    semiBlock = true;
                    if (consecShots < 5)
                        consecShots += 1;
                    cooldownStartTimer = 0.0f;
                    fireRateTimer = 0.15f;
                    UpdateConeLines(CalcAimCone());
                }
                break;
            case 2://Shotgun Mode
                if (fireRateTimer <= 0 && !semiBlock)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 pelletAimAngle = ApplyAimErrorToRaycast(aimAngle, shotgunSpread / 2);
                        Shoot(pelletAimAngle);
                    }
                    semiBlock = true;
                    fireRateTimer = 1.5f;
                    UpdateConeLines(CalcAimCone());
                }
                break;
            case 3://Bolt-Action Mode
                if (fireRateTimer <= 0 && !semiBlock)
                {
                    Shoot(aimAngle);
                    semiBlock = true;
                    fireRateTimer = 1.5f;
                    UpdateConeLines(CalcAimCone());
                }
                break;
        }
    }

    
    /*Enables Firearm for use*/
    public void SetFirearmActive(bool active)
    {
        firearmActive = active;
    }

    //Sets the mode of the firearm
    //Modes:
    // 0 - Auto
    // 1 - Semi
    // 2 - Shotgun
    // 3 - Bolt Action
    public void SetFirearmMode(int mode)
    {
        firearmMode = mode;
    }

    public void PressTrigger()
    {
        if (!triggerPressed)
        {
            triggerPressed = true;
            semiBlock = false;
        }
        else
        {
            triggerPressed = false;
        }
        
    }

    private void Shoot(Vector2 aimAngle)
    { 
        hits = Physics2D.RaycastAll(muzzle.transform.position, aimAngle);
        float halfWallDistance = -1.0f;//If the bullet passes a half wall we need to store
                                       //the distance from the wall for future calculations
        foreach (RaycastHit2D hit in hits)
        {              
            if(hit.transform.CompareTag("Half Wall"))
            {
                if (!HalfWallPassed(hit.distance)) //If bullet hit the wall draw bullet line
                {
                    BulletLine(hit, muzzle.transform.position);
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
                if(NPCHit(halfWallDistance, hit))
                {
                    BulletLine(hit, muzzle.transform.position);
                    //Destroy(hit.transform.gameObject);
                    break;
                }
                else
                {
                    continue;
                }
            }
            BulletLine(hit, muzzle.transform.position);
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

    //Applies cover to enemy crouching behind a wall
    //Enemies STANDING behind a wall have no bonus from this since they already get the implied protection from the bulet hitting the wall
    //This function is supposed to simulate bulltes flying over half walls and over the heads of the enemy or hitting them based on
    //the distances of the shooter and the wall and the distance of the enemy and the wall
    //This happens because of the fact that the gun is shot from a higher place than the halfwall and therefore can still hit enemies that
    //are too far from the wall to take cover. See ASCII illustration:
    //
    //   O
    //   |=====> ----------___________   
    //   || \\                        ------------___________
    //   |                                           ||      ----------__________
    //   /\                                          ||                          ----------___________
    //__/__\_________________________________________||___________________________________________________________
    //    <---------Shooter-Wall Distance------------><--Safe-><------------Cover loses effect-------------><---No Cover---->
    //
    //Formulas used to calculate the zones after the wall:
    // Safe: between 0 and 0.25 * Shooter-Wall Distance ==> The enemy has zero chance of being hit, full cover
    // Cover loses effect: between 0.25 * SWD and 2 * SWD ==> Linear loss of cover
    // No Cover = 2 * Shooter-Wall Distance and more ==> The enemy is too far behind cover and therefore he is fully visible, no cover
    public float ApplyCoverToEnemy(float halfWallDistance, RaycastHit2D hit)
    {
        float percentage;
        if (halfWallDistance <= 0)
        {
            percentage = 1.0f;
        }
        else
        {
            if (hit.distance - halfWallDistance <= 0.25 * halfWallDistance)
            {
                percentage = 0.0f;
            }
            else if (hit.distance - halfWallDistance >= 2 * halfWallDistance)
            {
                percentage = 1.0f;
            }
            else
            {
                percentage = ((hit.distance - halfWallDistance - 0.25f * halfWallDistance) / (1.75f * halfWallDistance));
            }
        }
        return percentage;
    }
    public bool NPCHit(float halfWallDistance, RaycastHit2D hit)
    {
        bool isCrouched = hit.transform.gameObject.GetComponent<NPCStatus>().isCrouched;
        if (isCrouched)
        {
            float hitChance = ApplyCoverToEnemy(halfWallDistance, hit);
            if (hit.distance <= 50.0f) //A crouched enemy will be hit 100% of the time from distance < 5m
            {
                Debug.Log(hitChance);
                if (Random.value < 1.0f * hitChance)
                    return true;
                else
                    return false;
            }
            else if (hit.distance >= 150.0f)
            {
                if (Random.value < 0.8 * hitChance) //A crouched enemy will be hit 80% of the time from distance > 15m 
                    return true;
                else
                    return false;
            }
            else //linear dropoff between 5 metres and 15 metres (from 100% to 80%)
            {
                Debug.Log(hitChance);
                if (Random.value < (1 - (hit.distance - 50.0f) * 0.002) * hitChance)
                    return true;
                else
                    return false;
            }
        }
        else
        {
            return true; //Standing enemy will get hit 100% of the time regardless of distance. TODO: Prone to future balancing
        }

    }

    //distance - distance between the shooter and the wall
    public bool HalfWallPassed(float distance)
    {
        if (distance < 50.0f)
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
    //Applies the random error within the confines of the cone to the bullet
    //Degrees represent half the angle of the cone (15 degree -> 30degree cone)
    public Vector2 ApplyAimErrorToRaycast(Vector2 aimAngle, float degrees)
    {

        degrees = Random.Range(0, degrees);

        //The bullet either deviates to the right or the left
        if (Random.value <= 0.5f)
            return Quaternion.Euler(0, 0, -degrees) * aimAngle;
        else
            return Quaternion.Euler(0, 0, degrees) * aimAngle;

    }

    //Creates the bullet raycast
    public void BulletLine(RaycastHit2D hit, Vector3 muzzlePos)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        LineRenderer lineRenderer = bullet.GetComponent<LineRenderer>();
        Vector3[] trajectory = { muzzlePos, hit.point };
        lineRenderer.SetPositions(trajectory);
    }
}
