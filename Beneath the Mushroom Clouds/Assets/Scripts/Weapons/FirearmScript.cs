using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class FirearmScript: MonoBehaviour
{

    public InventoryItem selectedFirearm;
    //Need to cache firearm when reloading, so that if the weapons are switched the reload is interrupted
    //(current selected firearm is not the same as the one that was reloaded)
    private InventoryItem reloadedFirearm;

    [SerializeField] private GameObject inventoryScreen;
    private InventoryController inventoryController;
    private PlayerAnimationController playerAnimationController;

    public GameObject muzzle;
    public GameObject player;
    public GameObject coneLineL;
    public GameObject coneLineR;
    public GameObject bulletPrefab;

    private bool reloading = false;
    private bool racking = false;
    private bool triggerEnabled = true;


    public float initialDeviation; //Worst case initial bullet deviaiton (degrees)
    public float bulletDevIncrement; //How many degrees does another fired bullet add at most

    private float shooterAbility;

    private Coroutine reloadCoroutine;
    private Coroutine rackCoroutine;


    private PlayerStatus playerStatus;

    private bool firearmActive;
    private string weaponType;
    private string fireMode;


    private float fireRate = 10.0f; //Rounds per second
    private float fireRateTimer = 0.0f;
    private float lastShot;
    private int consecShots = 0;
    private bool triggerPressed = false; //Is the trigger pressed
    private bool semiBlock = false; //Has a round been fired on this trigger press
    private bool autoChamber = false; //Whether the firearm should be chambering a round by itself 
                                      //(e.g. the pistol chambers rounds by itself but shouldn't automatically chamber a round after reload) 

    private float cooldownStart = 0.2f;//After how long after the first shot the recoil starts cooling down
    private float cooldownStartTimer = 0.0f;
    private float cooldownRate = 0.1f; //How quickly the firearm recoil cools down
    private float cooldownTimer = 0.0f;

    private float shotgunSpread = 5.0f;

    private float magLoadSpeed = 1.25f;
    private float rackWeaponSpeed = 1f;
    private float loadRoundSpeed = 1f;

    private RaycastHit2D[] hits; //Field of hit objects


    // Start is called before the first frame update
    void Start()
    {
        playerStatus = player.GetComponent<PlayerStatus>();
        shooterAbility = playerStatus.shooterAbility;
        UpdateConeLines(shooterAbility * (initialDeviation/2));
        inventoryController = inventoryScreen.GetComponent<InventoryController>();
        playerAnimationController = player.GetComponent<PlayerAnimationController>();

    }

    // Update is called once per frame
    void Update()
    {
        if(reloading || racking){
            if(selectedFirearm == null || selectedFirearm != reloadedFirearm){
                reloading = false;
                racking = false;
                reloadedFirearm = null;
                triggerEnabled = true;
                if(reloadCoroutine != null)
                    StopCoroutine(reloadCoroutine);
                if(rackCoroutine != null)
                    StopCoroutine(rackCoroutine);
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

        if(!selectedFirearm.isChambered && selectedFirearm.ammoCount > 0 && !reloading){
            if(selectedFirearm.itemData.manuallyChambered && !racking){
                rackCoroutine =  StartCoroutine(RackFirearm());
            }else{
                if(FireRateCheck()){
                    if(autoChamber){
                        //NOTE: To clarify, this instantly chambers a round without any animation (the firearm racks itself as automatic and semi-auto weapons do)
                        inventoryController.RackFirearm(selectedFirearm);
                        autoChamber = false;
                    }else if(!racking){
                        rackCoroutine =  StartCoroutine(RackFirearm());
                    }
                }
            }
            return;
        }
        if (!triggerPressed)
            return;

        if(semiBlock)
            return;

        
        if(!inventoryController.FireRound()){
            return;
        }

        //Trigger is pressed
        Vector2 BulletDeviation = ApplyAimErrorToRaycast(transform.parent.transform.up, CalcAimCone());
        if(weaponType == "Shotgun"){
            for (int i = 0; i < 8; i++)
            {
                Vector2 pelletBulletDeviation = ApplyAimErrorToRaycast(BulletDeviation, shotgunSpread / 2);
                Shoot(pelletBulletDeviation);
            }
        }else{
            Shoot(BulletDeviation);
        }
        UpdateConeLines(CalcAimCone());

        if(fireMode == "automatic"){
            lastShot = Time.time;
            if (consecShots < 10)
                consecShots += 1;
            cooldownStartTimer = 0.0f;
        }else if(fireMode == "semi-auto"){
            if (consecShots < 5)
                consecShots += 1;
            cooldownStartTimer = 0.0f;
            semiBlock = true;
            fireRateTimer = 0.15f;
        }


        autoChamber = true;
    }


    //Adjusts the rate of fire for weapons that cycle automatically (automatic and semi-automatic)
    private bool FireRateCheck()
    {
        switch(fireMode){
            case "automatic":
                if(Time.time - lastShot > 1 / fireRate)
                {
                    return true;
                }
                break;
            case "semi-auto":
                if(fireRateTimer <= 0)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    
    /*Enables Firearm for use*/
    public void SetFirearmActive(bool active)
    {
        firearmActive = active;
    }


    public void PressTrigger(InputValue value)
    {


        if (value.isPressed)
        {
            if(!triggerEnabled)
                return;
            triggerPressed = true;
        }
        else
        {
            triggerPressed = false;
            semiBlock = false;
        }
        
    }

    private void Shoot(Vector2 BulletDeviation)
    { 

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
        weaponType = selectedFirearm.itemData.weaponType;
        fireMode = selectedFirearm.GetFiremode();
        if(selectedFirearm.itemData.weaponLength == 0){
            this.transform.localPosition = new Vector3(0, 0.6f, 0);
        }else if(selectedFirearm.itemData.weaponLength == 1){
            this.transform.localPosition = new Vector3(0, 0.75f, 0);
        }else{
            //Prepared for different weapon lengths
            this.transform.localPosition = new Vector3(0, 0.75f, 0);
        }


        
    }

    public void ReloadButtonPressed(){
        
        if(selectedFirearm == null){
            return;
        }

        if(reloading){
            return;
        }

        if(selectedFirearm.itemData.usesMagazines){
            if(inventoryController.FindMagazine(selectedFirearm.itemData.weaponType, true) == null){
                return;
            }
        }else{
            if(inventoryController.FindAmmo(selectedFirearm.itemData.weaponType, true) == null){
                return;
            }
        }

        

        

        reloadCoroutine = StartCoroutine(Reload());
    }

    private IEnumerator Reload(){
        reloading = true;
        reloadedFirearm = selectedFirearm;
        triggerEnabled = false;
        if(selectedFirearm.itemData.usesMagazines){
            if(selectedFirearm.hasMagazine){
                inventoryController.ReloadRemoveMagazine(selectedFirearm);
                playerAnimationController.UnloadMagAnimation(selectedFirearm);
                yield return new WaitForSeconds(magLoadSpeed);
                playerAnimationController.LoadMagAnimation(selectedFirearm);
                yield return new WaitForSeconds(magLoadSpeed);
                inventoryController.AttachMagazine(selectedFirearm, true);
            }else{
                playerAnimationController.LoadMagAnimation(selectedFirearm);
                yield return new WaitForSeconds(magLoadSpeed);
                inventoryController.AttachMagazine(selectedFirearm, true);
            }
        }else{
            while(selectedFirearm.ammoCount < selectedFirearm.currentMagazineSize){
                playerAnimationController.LoadRoundAnimation(selectedFirearm);
                yield return new WaitForSeconds(loadRoundSpeed);
                if(!inventoryController.LoadRound(selectedFirearm)){
                    break;
                }
            }
        }
        
        
        if(!selectedFirearm.isChambered){
            playerAnimationController.RackAnimation(selectedFirearm);
            yield return new WaitForSeconds(rackWeaponSpeed);
            inventoryController.RackFirearm(selectedFirearm);
        }
        reloading = false;
        reloadedFirearm = null;
        triggerEnabled = true;

    }

    private IEnumerator RackFirearm(){
        racking = true;
        reloadedFirearm = selectedFirearm;
        triggerEnabled = false;
        
        playerAnimationController.RackAnimation(selectedFirearm);
        yield return new WaitForSeconds(rackWeaponSpeed);
        inventoryController.RackFirearm(selectedFirearm);
        racking = false;
        reloadedFirearm = null;
        triggerEnabled = true;
    }


    public void InventoryOpened(){
        triggerPressed = false;
    }

    public void SwitchFiremode(){
        fireMode = inventoryController.SwitchFiremode(selectedFirearm);
    }

}
