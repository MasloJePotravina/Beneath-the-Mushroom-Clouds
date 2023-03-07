using UnityEngine;
using System.Collections;

public class FirearmScript: MonoBehaviour
{

    public InventoryItem selectedFirearm;
    //Need to cache firearm when reloading, so that if the weapons are switched the reload is interrupted
    //(current selected firearm is not the same as the one that was reloaded)
    private InventoryItem reloadedFirearm;

    [SerializeField] private GameObject mainCamera;
    private InventoryController inventoryController;

    public GameObject muzzle;
    public GameObject player;
    public GameObject coneLineL;
    public GameObject coneLineR;
    public GameObject bulletPrefab;

    private bool reloading = false;


    public float initialDeviation; //Worst case initial bullet deviaiton (degrees)
    public float bulletDevIncrement; //How many degrees does another fired bullet add at most

    private float shooterAbility;

    private Coroutine reloadCoroutine;


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

    private float magSwapSpeed = 2.5f;
    private float rackWeaponSpeed = 0.5f;

    private RaycastHit2D[] hits; //Field of hit objects


    // Start is called before the first frame update
    void Start()
    {
        playerStatus = player.GetComponent<PlayerStatus>();
        shooterAbility = playerStatus.shooterAbility;
        UpdateConeLines(shooterAbility * (initialDeviation/2));
        inventoryController = mainCamera.GetComponent<InventoryController>();

    }

    // Update is called once per frame
    void Update()
    {

        if(reloading){
            if(selectedFirearm == null || selectedFirearm != reloadedFirearm){
                reloading = false;
                reloadedFirearm = null;
                StopCoroutine(reloadCoroutine);
            }
        }
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

        if(!selectedFirearm.isChambered){
            if(WeaponCycled()){
                selectedFirearm.ChamberFromMagazine();
            }
            return;
        }
        if (!triggerPressed)
            return;

        
        //Trigger is pressed
        Vector2 BulletDeviation = ApplyAimErrorToRaycast(transform.parent.transform.up, CalcAimCone());
        switch (firearmMode)
        {
            case 0://Assault Rifle Mode
                Shoot(BulletDeviation);
                lastShot = Time.time;
                if (consecShots < 10)
                    consecShots += 1;
                cooldownStartTimer = 0.0f;
                UpdateConeLines(CalcAimCone());
                break;
            case 1://Pistol Mode
                Shoot(BulletDeviation);
                semiBlock = true;
                if (consecShots < 5)
                    consecShots += 1;
                cooldownStartTimer = 0.0f;
                fireRateTimer = 0.15f;
                UpdateConeLines(CalcAimCone());
                break;
            case 2://Shotgun Mode
                for (int i = 0; i < 8; i++)
                {
                    Vector2 pelletBulletDeviation = ApplyAimErrorToRaycast(BulletDeviation, shotgunSpread / 2);
                    Shoot(pelletBulletDeviation);
                }
                semiBlock = true;
                fireRateTimer = 1.5f;
                UpdateConeLines(CalcAimCone());
                break;
            case 3://Bolt-Action Mode
                Shoot(BulletDeviation);
                semiBlock = true;
                fireRateTimer = 1.5f;
                UpdateConeLines(CalcAimCone());
                break;
        }
    }

    private bool WeaponCycled()
    {
        if(firearmMode == 0){
            if(Time.time - lastShot > 1 / fireRate)
            {
                return true;
            }
        }else{
            if(fireRateTimer <= 0 && !semiBlock)
            {
                return true;
            }
        }
        return false;
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

    private void Shoot(Vector2 BulletDeviation)
    { 

        if(!selectedFirearm.FireRound()){
            return;
        }
        hits = Physics2D.RaycastAll(muzzle.transform.position, BulletDeviation);
        float halfWallDistance = -1.0f;//If the bullet passes a half wall we need to store
                                       //the distance from the wall for future calculations
        foreach (RaycastHit2D hit in hits)
        {              
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("HalfObstacle"))
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
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("NPC"))
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
                if (Random.value < hitChance)
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
    public Vector2 ApplyAimErrorToRaycast(Vector2 BulletDeviation, float degrees)
    {

        degrees = Random.Range(0, degrees);

        //The bullet either deviates to the right or the left
        if (Random.value <= 0.5f)
            return Quaternion.Euler(0, 0, -degrees) * BulletDeviation;
        else
            return Quaternion.Euler(0, 0, degrees) * BulletDeviation;

    }

    //Creates the bullet raycast
    public void BulletLine(RaycastHit2D hit, Vector3 muzzlePos)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        LineRenderer lineRenderer = bullet.GetComponent<LineRenderer>();
        Vector3[] trajectory = { muzzlePos, hit.point };
        lineRenderer.SetPositions(trajectory);
    }

    public void ChangeSelectedFirearm(InventoryItem firearm){
        selectedFirearm = firearm;
        if(selectedFirearm == null){
            firearmActive = false;
            return;
        }
        firearmActive = true;
        switch(firearm.itemData.weaponType){
            case "AssaultRifle":
                firearmMode = 0;
                break;
            case "Pistol":
                firearmMode = 1;
                break;
            case "Shotgun":
                firearmMode = 2;
                break;
            case "HuntingRifle":
                firearmMode = 3;
                break;
        }

        
    }

    public void ReloadButtonPressed(){
        
        if(selectedFirearm == null){
            return;
        }

        if(inventoryController.FindMagazine(selectedFirearm.itemData.weaponType, true) == null){
            return;
        }

        if(!reloading){
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload(){
        reloading = true;
        reloadedFirearm = selectedFirearm;
        
        if(selectedFirearm.hasMagazine){
            inventoryController.ReloadRemoveMagazine(selectedFirearm);
            yield return new WaitForSeconds(magSwapSpeed);
            inventoryController.AttachMagazine(selectedFirearm, true);
        }else{
            yield return new WaitForSeconds(magSwapSpeed/2);
            inventoryController.AttachMagazine(selectedFirearm, true);
        }
        if(!selectedFirearm.isChambered){
            yield return new WaitForSeconds(rackWeaponSpeed);
            Debug.Log("racked");
        }
        reloading = false;
        reloadedFirearm = null;

    }
}
