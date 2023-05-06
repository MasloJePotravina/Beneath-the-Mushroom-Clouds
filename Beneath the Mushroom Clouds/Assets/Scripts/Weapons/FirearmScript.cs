using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Implements the behaviour of the player's firearm.
/// </summary>
public class FirearmScript: MonoBehaviour
{

    /// <summary>
    /// Reference to the selected firearm.
    /// </summary>
    public InventoryItem selectedFirearm;

    //Need to cache firearm when reloading, so that if the weapons are switched the reload is interrupted
    //(current selected firearm is not the same as the one that was reloaded)
    
    /// <summary>
    /// Reference to the firearm that was being reloaded.
    /// </summary>
    private InventoryItem reloadedFirearm;

    /// <summary>
    /// Reference to the inventory screen.
    /// </summary>
    [SerializeField] private GameObject inventoryScreen;

    /// <summary>
    /// Reference to the inventory controller.
    /// </summary>
    private InventoryController inventoryController;


    /// <summary>
    /// Reference to the human animation controller.
    /// </summary>
    private HumanAnimationController humanAnimationController;

    /// <summary>
    /// Reference to the muzzle of the firearm.
    /// </summary>
    public GameObject muzzle;

    /// <summary>
    /// Reference to the player.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Reference to the pivot of the left cone line.
    /// </summary>
    public GameObject coneLineL;

    /// <summary>
    /// Reference to the pivot of the right cone line.
    /// </summary>
    public GameObject coneLineR;

    /// <summary>
    /// Prefab for bullet impact.
    /// </summary>
    public GameObject bulletImpactPrefab;

    /// <summary>
    /// Whether the player is currently reloading.
    /// </summary>
    private bool reloading = false;

    /// <summary>
    /// Whether the player is currently racking the weapon.
    /// </summary>
    private bool racking = false;

    /// <summary>
    /// Whether the player is currently loading a round into the internal magazine.
    /// </summary>
    private bool triggerEnabled = true;

    /// <summary>
    /// Initial accuracy of the selected firearm.
    /// </summary>
    private float initialAccuracy; //Worst case initial bullet deviaiton (degrees)

    /// <summary>
    /// By how much does accuracy decrease with each consecutive shot.
    /// </summary>
    private float bulletAccuracyDecrement; //How many degrees does another fired bullet add at most

    /// <summary>
    /// Currently running reload coroutine.
    /// </summary>
    private Coroutine reloadCoroutine;

    /// <summary>
    /// Currently running rack coroutine.
    /// </summary>
    private Coroutine rackCoroutine;

    /// <summary>
    /// Reference to the status of the player.
    /// </summary>
    private PlayerStatus playerStatus;

    /// <summary>
    /// Whether the firearm is active.
    /// </summary>
    private bool firearmActive;

    /// <summary>
    /// Weapon type of the selected firearm.
    /// </summary>
    private string weaponType;

    /// <summary>
    /// Fire mode of the selected firearm.
    /// </summary>
    private string fireMode;

    /// <summary>
    /// Fire rate of the selected firearm.
    /// </summary>
    private float fireRate = 10.0f; //Rounds per second

    /// <summary>
    /// Fire rate timer.
    /// </summary>
    private float fireRateTimer = 0.0f;
    /// <summary>
    /// When was the last time the weapon fired a shot.
    /// </summary>
    private float lastShot;
    /// <summary>
    /// How many consecutive shots did the gun fire.
    /// </summary>
    private int consecShots = 0;

    /// <summary>
    /// Whether the trigger is pressed.
    /// </summary>
    private bool triggerPressed = false; //Is the trigger pressed

    /// <summary>
    /// Flag used to regulate the firing of semi-automatic weapons.
    /// </summary>
    private bool semiBlock = false; //Has a round been fired on this trigger press

    /// <summary>
    /// Whether the weapon chambers itself automatically.
    /// </summary>
    private bool autoChamber = false; //Whether the firearm should be chambering a round by itself 
                                      //(e.g. the pistol chambers rounds by itself but shouldn't automatically chamber a round after reload) 

    /// <summary>
    /// When should the accuracy cooldown start after firing the last shot.
    /// </summary>
    private float cooldownStart = 0.2f;//After how long after the first shot the recoil starts cooling down

    /// <summary>
    /// Accuracy cooldown timer.
    /// </summary>
    private float cooldownStartTimer = 0.0f;

    /// <summary>
    /// Current accuracy of the firearm.
    /// </summary>
    private float cooldownRate = 0.1f; //How quickly the firearm recoil cools down

    /// <summary>
    /// Cooldown timer.
    /// </summary>
    private float cooldownTimer = 0.0f;

    /// <summary>
    /// Shotgun spread of a shotgun.
    /// </summary>
    private float shotgunSpread = 5.0f;

    /// <summary>
    /// Speed of loading or unloading a magazine.
    /// </summary>
    private float magLoadSpeed = 1.25f;

    /// <summary>
    /// Speed of racking the weapon.
    /// </summary>
    private float rackWeaponSpeed = 1f;

    /// <summary>
    /// Speed of loading a round into the internal magazine of the weapon.
    /// </summary>
    private float loadRoundSpeed = 1f;

    /// <summary>
    /// Reference to the muzzle flash of the weapon.
    /// </summary>
    [SerializeField] private GameObject muzzleFlash;

    /// <summary>
    /// Sprite renderer of the muzzle flash.
    /// </summary>
    private SpriteRenderer muzzleSpriteRenderer;

    /// <summary>
    /// Light component of the muzzle flash.
    /// </summary>
    private UnityEngine.Rendering.Universal.Light2D muzzleLight;

    /// <summary>
    /// Array of different muzzle flash sprites.
    /// </summary>
    [SerializeField] private Sprite[] muzzleFlashSprites;

    /// <summary>
    /// Index of which muzzle flash sprite is currently being used.
    /// </summary>
    private int muzzleFlashCycle = 0;

    /// <summary>
    /// Offset in the array of muzzle flashes for different weapons.
    /// </summary>
    private int muzzleFlashOffset = 0;

    /// <summary>
    /// Reference to the audio manager.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Whether the use of the firearm is blocked by an open inventory screen.
    /// </summary>
    public bool inventoryBlock = false;

    /// <summary>
    /// Layer maksk for the bullet raycast.
    /// </summary>
    private int layerMask;

    /// <summary>
    /// Reference to the noise origin component.
    /// </summary>
    private NoiseOrigin noiseOrigin;



    /// <summary>
    /// Gets all of the necessary references and intializes the firearm values at the start.
    /// </summary>
    void Start()
    {
        playerStatus = player.GetComponent<PlayerStatus>();
        UpdateConeLines(playerStatus.shootingAbility * (initialAccuracy/2));
        inventoryController = inventoryScreen.GetComponent<InventoryController>();
        humanAnimationController = player.GetComponent<HumanAnimationController>();

        muzzleSpriteRenderer = muzzleFlash.GetComponent<SpriteRenderer>();
        muzzleLight = muzzleFlash.GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        audioManager = FindObjectOfType<AudioManager>();

        layerMask = LayerMask.GetMask("NPC", "HalfObstacle", "FullObstacle");

        noiseOrigin = GetComponent<NoiseOrigin>();

    }

    /// <summary>
    /// Each frame performs various checks to determine whether the weapon can be fired, was fired and calculates the recoil accumulation and cooldown.
    /// </summary>
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

        if(inventoryBlock){
            return;
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

        if(selectedFirearm.boltOpen &&  (selectedFirearm.ammoCount > 0 || selectedFirearm.isChambered) && !reloading && !racking){
            StartCoroutine(CloseBolt());
            return;
        }

        if(!selectedFirearm.isChambered && selectedFirearm.ammoCount > 0 && !reloading){
            if(selectedFirearm.itemData.manuallyChambered && !racking){
                rackCoroutine =  StartCoroutine(RackFirearm(rackDelay: true));
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

        //Trigger is pressed
        if (!triggerPressed)
            return;

        //If weapon is semi automatic, only fire one round per trigger press
        if(semiBlock)
            return;

        if(HandsThroughWallCheck()){
            return;
        }

        //Attempt to fire a round (if weapon is empty, this will return false)
        if(!inventoryController.FireRound()){
            return;
        }

        //Round is fired
        audioManager.Play(selectedFirearm.itemData.weaponType + "Gunshot");
        noiseOrigin.GenerateNoise(300f);
        StartCoroutine(MuzzleFlash());
        if(selectedFirearm.itemData.manuallyChambered){
            selectedFirearm.shellInChamber = true;
        }
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

        if(selectedFirearm.ammoCount > 0){
            autoChamber = true;
        }
    }

    /// <summary>
    /// Cooroutine that simmulates muzzle flash by quickly enabling and disabling the muzzle flash sprite and light.
    /// </summary>
    /// <returns>Reference to the coroutine.</returns>
    IEnumerator MuzzleFlash(){
        muzzleSpriteRenderer.sprite = muzzleFlashSprites[muzzleFlashOffset + ((muzzleFlashCycle++) % 3)];
        muzzleLight.enabled = true;
        muzzleSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
        muzzleSpriteRenderer.enabled = false;
    }


    /// <summary>
    /// Adjusts fire rate for weapons that chamber themselves automatically.
    /// </summary>
    /// <returns>True when the weapon is ready to fire.</returns>
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

    
    /// <summary>
    /// Toggles the firearm as active for use.
    /// </summary>
    /// <param name="active">Whether the firearm is active or not.</param>
    public void SetFirearmActive(bool active)
    {
        firearmActive = active;
    }


    /// <summary>
    /// Called when the player is attempting to press the trigger.
    /// </summary>
    /// <param name="value">Input value of the input action the player performing.</param>
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

    /// <summary>
    /// Fires a bullet in the direction the player is aiming at.
    /// </summary>
    /// <param name="BulletDeviation">Deviation of the bullet compared to where the player is aiming.</param>
    private void Shoot(Vector2 BulletDeviation)
    { 
        bool headShot = false;
        bool torsoShot = false;
        bool legsShot = false;
        bool leftArmShot = false;
        bool rightArmShot = false;
        bool npcHitPossible = false;
        bool npctested = false;
        HumanHitbox hitNPCHitbox = null;
        RaycastHit2D[] hits = Physics2D.RaycastAll(muzzle.transform.position, BulletDeviation, Mathf.Infinity, layerMask);
        float halfObstacleDistance = -1.0f;//If the bullet passes a half obstacle we need to store
                                       //the distance from the obstacle for future calculations
        foreach (RaycastHit2D hit in hits)
        {              
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("HalfObstacle"))
            {
                if (!HalfObstaclePassed(hit.distance)) //If bullet hit the obstacle draw bullet line
                {
                    BulletImpact(hit, muzzle.transform.position);
                    break;
                }
                else //If not proceed with next collider
                {
                    halfObstacleDistance = hit.distance;
                }
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {

                if(!npctested){
                    npctested = true;
                    if(NPCHit(halfObstacleDistance, hit)){
                        npcHitPossible = true;
                        BulletImpact(hit, muzzle.transform.position);
                        hitNPCHitbox = hit.transform.root.Find("Torso").GetComponent<HumanHitbox>();
                    }else{
                        npcHitPossible = false;
                    }
                }
                if(npcHitPossible){
                    if(hit.collider.gameObject.name == "Head")
                    {
                        headShot = true;
                    }
                    else if(hit.collider.gameObject.name == "Torso")
                    {
                        torsoShot = true;
                    }
                    else if(hit.collider.gameObject.name == "Legs")
                    {
                        legsShot = true;
                    }else if(hit.collider.gameObject.name == "LeftArm"){
                        leftArmShot = true;
                    }else if(hit.collider.gameObject.name == "RightArm"){
                        rightArmShot = true;
                    }
                }  
            }
            else{
                BulletImpact(hit, muzzle.transform.position);
                break;
            }
        }

        //Debug.Log("Headshot: " + headShot + " TorsoShot: " + torsoShot + " LegsShot: " + legsShot + " LeftArmShot: " + leftArmShot + " RightArmShot: " + rightArmShot);

        if(hitNPCHitbox != null){
            hitNPCHitbox.BulletHit(selectedFirearm.itemData.damage, headShot, torsoShot, legsShot, leftArmShot, rightArmShot);
        } 
    }

    

    //Calculate aiming error of player based on the characters shooting ability
    //Better shooting ability -> smaller aim cone
    //shooter ability modifier between 0 and 1
    /// <summary>
    /// Caluycles the aim cone of the player based on the characters shooting ability. weapon's initial accuracy and bullet accuracy decrement.
    /// </summary>
    /// <returns>Aim cone in degrees.</returns>
    private float CalcAimCone()
    {

        //Summary: The weapon is less accurate with more consecutive shots, this stops after 10 rounds
        // The best shooter is twice as good at controling recoil than the worst shooter
        float consecShotsModifier = (bulletAccuracyDecrement - 0.5f*(1 - playerStatus.shootingAbility)) * consecShots;

        float degrees = initialAccuracy * playerStatus.shootingAbility + consecShotsModifier;

        //Degrees are halved here before returining (half the degrees to each side)
        degrees *= 0.5f;

        return degrees;
    }

    /// <summary>
    /// Updates the debug cone lines.
    /// </summary>
    /// <param name="degrees">Degrees of the cone for each cone line.
    private void UpdateConeLines(float degrees)
    { 

        coneLineL.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, degrees);
        coneLineR.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -degrees);

    }

    //Applies cover to enemy crouching behind a obstacle
    //Enemies STANDING behind a obstacle have no bonus from this since they already get the implied protection from the bulet hitting the obstacle
    //This function is supposed to simulate bulltes flying over half obstacles and over the heads of the enemy or hitting them based on
    //the distances of the shooter and the obstacle and the distance of the enemy and the obstacle
    //This happens because of the fact that the gun is shot from a higher place than the halfobstacle and therefore can still hit enemies that
    //are too far from the obstacle to take cover. See ASCII illustration:
    //
    //   O
    //   |=====> ----------___________   
    //   || \\                        ------------___________
    //   |                                           ||      ----------__________
    //   /\                                          ||                          ----------___________
    //__/__\_________________________________________||___________________________________________________________
    //    <---------Shooter-Wall Distance------------><--Safe-><------------Cover loses effect-------------><---No Cover---->
    //
    //Formulas used to calculate the zones after the obstacle:
    // Safe: between 0 and 0.25 * Shooter-Wall Distance ==> The enemy has zero chance of being hit, full cover
    // Cover loses effect: between 0.25 * SWD and 2 * SWD ==> Linear loss of cover
    // No Cover = 2 * Shooter-Wall Distance and more ==> The enemy is too far behind cover and therefore he is fully visible, no cover
    /// <summary>
    /// Applies cover to the enely based on half obstacles, stances and distances from the half obstacle.
    /// </summary>
    /// <param name="halfObstacleDistance">Distance between the shooter and the half obstacle.</param>
    /// <param name="hit">Raycast hit.</param>
    /// <returns>Percentage mulstiplier of whether the victim could or could not be shot.</returns>
    public float ApplyCoverToEnemy(float halfObstacleDistance, RaycastHit2D hit)
    {
        float percentage;
        if (halfObstacleDistance <= 0)
        {
            percentage = 1.0f;
        }
        else
        {
            if (hit.distance - halfObstacleDistance <= 0.25 * halfObstacleDistance)
            {
                percentage = 0.0f;
            }
            else if (hit.distance - halfObstacleDistance >= 2 * halfObstacleDistance)
            {
                percentage = 1.0f;
            }
            else
            {
                percentage = ((hit.distance - halfObstacleDistance - 0.25f * halfObstacleDistance) / (1.75f * halfObstacleDistance));
            }
        }
        return percentage;
    }

    /// <summary>
    /// Determines whether a victim should be shot based on the distance of the shooter.
    /// </summary>
    /// <param name="halfObstacleDistance">Distance between the shooter and the half obstacle.</param>
    /// <param name="hit">Raycast hit</param>
    /// <returns>Whether the NPC was shot or not.</returns>
    public bool NPCHit(float halfObstacleDistance, RaycastHit2D hit)
    {


        bool isCrouched = hit.transform.root.GetComponent<NPCStatus>().isCrouched;

        if (isCrouched)
        {
            float hitChance = ApplyCoverToEnemy(halfObstacleDistance, hit);
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

    /// <summary>
    /// Determines whether a half obstacle was passed or hit by a bullet.
    /// </summary>
    /// <param name="distance">Distance between the shooter and the half obstacle.</param>
    /// <returns>True if the bullet flew over the half obstacle, false if it hit the half obstacle.</returns>
    public bool HalfObstaclePassed(float distance)
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

    /// <summary>
    /// Applies the aim error to a raycast compared to its original direction
    /// </summary>
    /// <param name="originalDirection">Original direction of the raycast.</param>
    /// <param name="degrees">By how many degrees the raycast can deviate.</param>
    /// <returns>New direction of the raycast.</returns>
    public Vector2 ApplyAimErrorToRaycast(Vector2 originalDirection, float degrees)
    {

        degrees = Random.Range(0, degrees);

        //The bullet either deviates to the right or the left
        if (Random.value <= 0.5f)
            return Quaternion.Euler(0, 0, -degrees) * originalDirection;
        else
            return Quaternion.Euler(0, 0, degrees) * originalDirection;

    }

    /// <summary>
    /// Creates a bullet impact where the ullet hit an obstacle.
    /// </summary>
    /// <param name="hit">Raycast hit.</param>
    /// <param name="muzzlePos">Position of the muzzle.</param>
    public void BulletImpact(RaycastHit2D hit, Vector3 muzzlePos)
    {
        Vector3 dir = hit.point - new Vector2(muzzlePos.x, muzzlePos.y);
        GameObject bullet = Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(Vector3.forward, dir) * Quaternion.Euler(0, 0, 180));
        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC")){
            
            bullet.GetComponent<Animator>().SetBool("BloodSplatter", true);
            bullet.transform.position = bullet.transform.position + (dir.normalized * 5f);
           
        }
    }

    /// <summary>
    /// Changes the selected firearm and all variables associated with it.
    /// </summary>
    /// <param name="firearm">Reference to the newly selected firearm.</param>
    public void ChangeSelectedFirearm(InventoryItem firearm){
        selectedFirearm = firearm;
        if(selectedFirearm == null){
            firearmActive = false;
            return;
        }
        firearmActive = true;
        weaponType = selectedFirearm.itemData.weaponType;
        fireMode = selectedFirearm.GetFiremode();
        MuzzleFlashOffsetSelector();
        if(selectedFirearm.itemData.weaponLength == 0){
            this.transform.localPosition = new Vector3(-0.015f, 0.53f, 0);
        }else if(selectedFirearm.itemData.weaponLength == 1){
            this.transform.localPosition = new Vector3(-0.015f, 0.68f, 0);
        }else{
            //Prepared for different weapon lengths
            this.transform.localPosition = new Vector3(-0.015f, 0.75f, 0);
        }

        initialAccuracy = selectedFirearm.itemData.initialAccuracy;
        bulletAccuracyDecrement = selectedFirearm.itemData.bulletAccuracyDecrement;

        UpdateConeLines(CalcAimCone());

        
    }

    /// <summary>
    /// Selects an offset for the array of muzzle flashes based on the weapon type.
    /// </summary>
    private void MuzzleFlashOffsetSelector(){
        if(weaponType == "Pistol"){
            muzzleFlashOffset = 0;
        }else if(weaponType == "AssaultRifle" || weaponType == "HuntingRifle"){
            muzzleFlashOffset = 3;
        }else if(weaponType == "Shotgun"){
            muzzleFlashOffset = 6;
        }
    }

    /// <summary>
    /// Registers an attempt to reload the selected firearm. Activates reloading coroutine if possible.
    /// </summary>
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

    /// <summary>
    /// Reloading coroutine. Handles the reload of a selected firearm.
    /// </summary>
    /// <returns>Reference to the runnign coroutine.</returns>
    private IEnumerator Reload(){
        reloading = true;
        reloadedFirearm = selectedFirearm;
        triggerEnabled = false;
        noiseOrigin.GenerateNoise(20f);
        if(selectedFirearm.itemData.usesMagazines){
            if(selectedFirearm.hasMagazine){
                inventoryController.ReloadRemoveMagazine(selectedFirearm);
                humanAnimationController.UnloadMagAnimation(selectedFirearm);
                yield return new WaitForSeconds(magLoadSpeed);
                humanAnimationController.LoadMagAnimation(selectedFirearm);
                yield return new WaitForSeconds(magLoadSpeed);
                inventoryController.AttachMagazine(selectedFirearm, true);
            }else{
                humanAnimationController.LoadMagAnimation(selectedFirearm);
                yield return new WaitForSeconds(magLoadSpeed);
                inventoryController.AttachMagazine(selectedFirearm, true);
            }
        }else{
            //Todo: no animation for open bolt yet, load round animation is used instead
            if(!selectedFirearm.isChambered){
                humanAnimationController.OpenBoltAnimation(selectedFirearm);
                selectedFirearm.boltOpen = true;
                yield return new WaitForSeconds(rackWeaponSpeed);
            }
            while(selectedFirearm.ammoCount < selectedFirearm.currentMagazineSize){
                
                humanAnimationController.LoadRoundAnimation(selectedFirearm);
                yield return new WaitForSeconds(loadRoundSpeed);
                if(!inventoryController.LoadRound(selectedFirearm)){
                    break;
                }
            }
            
        }
        noiseOrigin.GenerateNoise(20f);
        reloading = false;
        reloadedFirearm = null;
        triggerEnabled = true;

    }

    //RackDelay for manually chambered weapons so that the racking starts a bit later after taking a shot
    /// <summary>
    /// Racking coroutine. Handles the racking of a selected firearm.
    /// </summary>
    /// <param name="rackDelay">Whether the racking should be delayed. Used for racks after shooting.</param>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator RackFirearm(bool rackDelay = false){
        racking = true;
        reloadedFirearm = selectedFirearm;
        triggerEnabled = false;
        if(rackDelay){
            yield return new WaitForSeconds(0.2f);
        }
        noiseOrigin.GenerateNoise(20f);
        humanAnimationController.RackAnimation(selectedFirearm);
        yield return new WaitForSeconds(rackWeaponSpeed);
        inventoryController.RackFirearm(selectedFirearm);
        racking = false;
        reloadedFirearm = null;
        triggerEnabled = true;
        selectedFirearm.shellInChamber = false;
    }

    /// <summary>
    /// Closing bolt coroutine. Handles the closing of a selected firearm's bolt.
    /// </summary>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator CloseBolt(){
        racking = true;
        reloadedFirearm = selectedFirearm;
        triggerEnabled = false;
        noiseOrigin.GenerateNoise(20f);
        humanAnimationController.CloseBoltAnimation(selectedFirearm);
        yield return new WaitForSeconds(loadRoundSpeed);
        inventoryController.CloseBolt(selectedFirearm);
        racking = false;
        reloadedFirearm = null;
        triggerEnabled = true;
    }

    /// <summary>
    /// Registers an opened inventory and disables the trigger.
    /// </summary>
    public void InventoryOpened(){
        triggerPressed = false;
        inventoryBlock = true;
    }

    /// <summary>
    /// Registers a closed inventory and enables the trigger.
    /// </summary>
    public void InventoryClosed(){
        inventoryBlock = false;
    }

    /// <summary>
    /// Switches the fire mode of the firearm if possible.
    /// </summary>
    public void SwitchFiremode(){
        fireMode = inventoryController.SwitchFiremode(selectedFirearm);
    }

    /// <summary>
    /// Checks whether the player's hands are protruding thoruhg an obstacle when attempting to shoot.
    /// </summary>
    /// <returns>True if the player;s hands are protruding through an obstacle, false otherwise.</returns>
    private bool HandsThroughWallCheck(){
        //Shoot raycast from player's position to the weapon's position, if blocked by a Full Obstacle, cannot shoot
        Vector3 dir = this.transform.position - player.transform.position;
        RaycastHit2D hit;
        if(playerStatus.isCrouched){
            hit = Physics2D.Raycast(player.transform.position, dir, dir.magnitude, LayerMask.GetMask("FullObstacle", "HalfObstacle"));
        }else{
            hit = Physics2D.Raycast(player.transform.position, dir, dir.magnitude, LayerMask.GetMask("FullObstacle"));
        }
        
        if(hit.collider != null){
            return true;
        }else{
            return false;
        }

    }

}
