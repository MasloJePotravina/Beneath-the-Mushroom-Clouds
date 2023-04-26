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
    private HumanAnimationController playerAnimationController;

    public GameObject muzzle;
    public GameObject player;
    public GameObject coneLineL;
    public GameObject coneLineR;
    public GameObject bulletImpactPrefab;

    private bool reloading = false;
    private bool racking = false;
    private bool triggerEnabled = true;


    private float initialAccuracy; //Worst case initial bullet deviaiton (degrees)
    private float bulletAccuracyDecrement; //How many degrees does another fired bullet add at most

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

    [SerializeField] private GameObject muzzleFlash;

    private SpriteRenderer muzzleSpriteRenderer;
    private UnityEngine.Rendering.Universal.Light2D muzzleLight;

    [SerializeField] private Sprite[] muzzleFlashSprites;

    private int muzzleFlashCycle = 0;
    private int muzzleFlashOffset = 0;

    private AudioManager audioManager;

    public bool inventoryBlock = false;

    private int layerMask;

    private NoiseOrigin noiseOrigin;



    // Start is called before the first frame update
    void Start()
    {
        playerStatus = player.GetComponent<PlayerStatus>();
        UpdateConeLines(playerStatus.shootingAbility * (initialAccuracy/2));
        inventoryController = inventoryScreen.GetComponent<InventoryController>();
        playerAnimationController = player.GetComponent<HumanAnimationController>();

        muzzleSpriteRenderer = muzzleFlash.GetComponent<SpriteRenderer>();
        muzzleLight = muzzleFlash.GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        audioManager = FindObjectOfType<AudioManager>();

        layerMask = LayerMask.GetMask("NPC", "HalfObstacle", "FullObstacle");

        noiseOrigin = GetComponent<NoiseOrigin>();

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

    IEnumerator MuzzleFlash(){
        muzzleSpriteRenderer.sprite = muzzleFlashSprites[muzzleFlashOffset + ((muzzleFlashCycle++) % 3)];
        muzzleLight.enabled = true;
        muzzleSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
        muzzleSpriteRenderer.enabled = false;
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
        bool headShot = false;
        bool torsoShot = false;
        bool legsShot = false;
        bool leftArmShot = false;
        bool rightArmShot = false;
        bool npcHitPossible = false;
        bool npctested = false;
        HumanHitbox hitNPCHitbox = null;
        RaycastHit2D[] hits = Physics2D.RaycastAll(muzzle.transform.position, BulletDeviation, Mathf.Infinity, layerMask);
        float halfWallDistance = -1.0f;//If the bullet passes a half wall we need to store
                                       //the distance from the wall for future calculations
        foreach (RaycastHit2D hit in hits)
        {              
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("HalfObstacle"))
            {
                if (!HalfWallPassed(hit.distance)) //If bullet hit the wall draw bullet line
                {
                    BulletImpact(hit, muzzle.transform.position);
                    break;
                }
                else //If not proceed with next collider
                {
                    halfWallDistance = hit.distance;
                }
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {

                if(!npctested){
                    npctested = true;
                    if(NPCHit(halfWallDistance, hit)){
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


        bool isCrouched = hit.transform.root.GetComponent<NPCStatus>().isCrouched;

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
    public void BulletImpact(RaycastHit2D hit, Vector3 muzzlePos)
    {
        Vector3 dir = hit.point - new Vector2(muzzlePos.x, muzzlePos.y);
        GameObject bullet = Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(Vector3.forward, dir) * Quaternion.Euler(0, 0, 180));
        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC")){
            
            bullet.GetComponent<Animator>().SetBool("BloodSplatter", true);
            bullet.transform.position = bullet.transform.position + (dir.normalized * 5f);
           
        }
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

    private void MuzzleFlashOffsetSelector(){
        if(weaponType == "Pistol"){
            muzzleFlashOffset = 0;
        }else if(weaponType == "AssaultRifle" || weaponType == "HuntingRifle"){
            muzzleFlashOffset = 3;
        }else if(weaponType == "Shotgun"){
            muzzleFlashOffset = 6;
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
        noiseOrigin.GenerateNoise(20f);
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
            //Todo: no animation for open bolt yet, load round animation is used instead
            if(!selectedFirearm.isChambered){
                playerAnimationController.OpenBoltAnimation(selectedFirearm);
                selectedFirearm.boltOpen = true;
                yield return new WaitForSeconds(rackWeaponSpeed);
            }
            while(selectedFirearm.ammoCount < selectedFirearm.currentMagazineSize){
                
                playerAnimationController.LoadRoundAnimation(selectedFirearm);
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
    private IEnumerator RackFirearm(bool rackDelay = false){
        racking = true;
        reloadedFirearm = selectedFirearm;
        triggerEnabled = false;
        if(rackDelay){
            yield return new WaitForSeconds(0.2f);
        }
        noiseOrigin.GenerateNoise(20f);
        playerAnimationController.RackAnimation(selectedFirearm);
        yield return new WaitForSeconds(rackWeaponSpeed);
        inventoryController.RackFirearm(selectedFirearm);
        racking = false;
        reloadedFirearm = null;
        triggerEnabled = true;
        selectedFirearm.shellInChamber = false;
    }

    private IEnumerator CloseBolt(){
        racking = true;
        reloadedFirearm = selectedFirearm;
        triggerEnabled = false;
        noiseOrigin.GenerateNoise(20f);
        playerAnimationController.CloseBoltAnimation(selectedFirearm);
        yield return new WaitForSeconds(loadRoundSpeed);
        inventoryController.CloseBolt(selectedFirearm);
        racking = false;
        reloadedFirearm = null;
        triggerEnabled = true;
    }


    public void InventoryOpened(){
        triggerPressed = false;
        inventoryBlock = true;
    }

    public void InventoryClosed(){
        inventoryBlock = false;
    }

    public void SwitchFiremode(){
        fireMode = inventoryController.SwitchFiremode(selectedFirearm);
    }

}
