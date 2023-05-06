using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour NPC's firearm handling.
/// </summary>
public class NPCFirearmScript : MonoBehaviour
{
    /// <summary>
    /// The weapon the NPC is using. This does not change during the game.
    /// </summary>
    private InventoryItem weapon;

    /// <summary>
    /// ItemData of the weapon the NPC is using.
    /// </summary>
    private ItemData weaponData;

    /// <summary>
    /// Reference to the NPC's status.
    /// </summary>
    private NPCStatus status;

    /// <summary>
    /// Reference to the NPC.
    /// </summary>
    private GameObject npc;
    
    /// <summary>
    /// Reference to the firearm's muzzle.
    /// </summary>
    private GameObject muzzle;

    /// <summary>
    /// Reference to the audio manager.
    /// </summary>
    private AudioManager audioManager;

    /// <summary>
    /// Reference to the animation controller.
    /// </summary>
    private HumanAnimationController animationController;

    /// <summary>
    /// Prefab of the bullet impact.
    /// </summary>
    [SerializeField] private GameObject bulletImpactPrefab;

    //Values to which the betweenShotsTimers are set between shots
    /// <summary>
    /// Rate at which the NPC fires the pistol.
    /// </summary>
    [SerializeField] private float pistolFireRate = 0.16f;

    /// <summary>
    /// Rate at which the NPC fires the assault rifle.
    /// </summary>
    [SerializeField] private float assaultRifleFireRate = 0.1f;

    /// <summary>
    /// Rate at which the NPC fires the shotgun.
    /// </summary>
    [SerializeField] private float shotgunFireRate = 1.2f;

    /// <summary>
    /// Rate at which the NPC fires the hunting rifle.
    /// </summary>
    [SerializeField] private float huntingRifleFireRate = 1.2f;

    /// <summary>
    /// Reference to the muzzle flash game object.
    /// </summary>
    [SerializeField] private Sprite[] muzzleFlashSprites;

    /// <summary>
    /// Current fire rate at which the NPC fires their assigned weapon.
    /// </summary>
    private float fireRate = 0f;

    /// <summary>
    /// Weapon type the NPC is using.
    /// </summary>
    private string weaponType;

    /// <summary>
    /// Spread of the shotgun pellets.
    /// </summary>
    private float shotgunSpread = 5.0f;

    /// <summary>
    /// Initial accuracy of the NPC's weapon (actually the spread of the weapon - the higher the number, the lower the accuracy).
    /// </summary>
    public float initialAccuracy = 5; 

    /// <summary>
    /// Accuracy decrement for each shot fired from the NPC's weapon (actually an increment of the spread of the weapon - the higher the number, the lower the accurac)
    /// </summary>
    public float bulletAccuracyDecrement = 1; 

    /// <summary>
    /// Timer between between shots.
    /// </summary>
    private float betweenShotsTimer = 0f;
    /// <summary>
    /// How long before the weapon's recoil starts to cool down.
    /// </summary>
    private float cooldownStart = 0.2f;//After how long after the first shot the recoil starts cooling down
    /// <summary>
    /// Timer for start of the cooldown.
    /// </summary>
    private float cooldownStartTimer = 0.0f;
    /// <summary>
    /// How quickly a firearm cools down its recoil
    /// </summary>
    private float cooldownRate = 0.1f; //How quickly the firearm recoil cools down
    /// <summary>
    /// Timer for recoil cooldown.
    /// </summary>
    private float cooldownTimer = 0.0f;

    /// <summary>
    /// Offset of the muzzle flash based on the length of the weapon.
    /// </summary>
    private int muzzleFlashOffset = 0;
    /// <summary>
    /// Sprite renderer of the muzzle flash.
    /// </summary>
    private SpriteRenderer muzzleFlashSpriteRenderer;
    /// <summary>
    /// 2D light component of the muzzle flash.
    /// </summary>
    private UnityEngine.Rendering.Universal.Light2D muzzleLight;
    /// <summary>
    /// Index to cycle through different muzzle flash sprites.
    /// </summary>
    private int muzzleFlashCycle = 0;

    /// <summary>
    /// How many consecutive shots were fired form the weapon.
    /// </summary>
    private int consecShots = 0;

    /// <summary>
    /// The layer mask for bullet raycasts.
    /// </summary>
    private int layerMask;

    /// <summary>
    /// Current ammunition count of the weapon.
    /// </summary>
    private int currentAmmo = 0;

    /// <summary>
    /// Maximum ammunition count of the weapon.
    /// </summary>
    private int maxAmmo = 0;

    /// <summary>
    /// Whether the NPC is reloading their weapon.
    /// </summary>
    bool reloading = false;

    /// <summary>
    /// Whether the NPC is racking their weapon.
    /// </summary>
    bool racking = false;

    /// <summary>
    /// How quickly the NPC reloads their weapon.
    /// </summary>
    private float magLoadSpeed = 1.25f;

    /// <summary>
    /// How quickly the NPC racks their weapon.
    /// </summary>
    private float rackWeaponSpeed = 1f;

    /// <summary>
    /// How quickly the NPC loads a round into their weapon.
    /// </summary>
    private float loadRoundSpeed = 1f;

    /// <summary>
    /// Whether the muzzle flash is enabled.
    /// </summary>
    private bool muzzleFlashEnabled = true;


    

    /// <summary>
    /// Gets the necessary references and sets the layer mask for bullet raycasts.
    /// </summary>
    private void Start()
    {
        npc = transform.parent.gameObject;
        status = npc.GetComponent<NPCStatus>();
        muzzle = transform.Find("Muzzle").gameObject;
        audioManager = GameObject.FindObjectOfType<AudioManager>();

        muzzleFlashSpriteRenderer = muzzle.transform.Find("MuzzleFlash").GetComponent<SpriteRenderer>();
        muzzleLight = muzzle.transform.Find("MuzzleFlash").GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        layerMask = LayerMask.GetMask("Player", "HalfObstacle", "FullObstacle");

        animationController = npc.GetComponent<HumanAnimationController>();

    }

    /// <summary>
    /// Each frame the weapon recoil is cooled down
    /// </summary>
    private void Update()
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
                    
                }
            }
        }

        betweenShotsTimer -= Time.deltaTime;

        
    }

    /// <summary>
    /// Sets the weapon data of the NPC's firearm. Called when the NPC is first spawned.
    /// </summary>
    /// <param name="weapon"></param>
    public void SetWeaponData(InventoryItem weapon){
        this.weapon = weapon;
        this.weaponData = weapon.itemData;
        weaponType = weaponData.weaponType;
        
        switch (weaponType)
        {
            case "AssaultRifle":
                fireRate = assaultRifleFireRate;
                maxAmmo = 30;
                currentAmmo = 30;
                break;
            case "Pistol":
                fireRate = pistolFireRate;
                maxAmmo = 12;
                currentAmmo = 12;
                break;
            case "Shotgun":
                fireRate = shotgunFireRate;
                maxAmmo = 6;
                currentAmmo = 6;
                break;
            case "HuntingRifle":
                fireRate = huntingRifleFireRate;
                maxAmmo = 5;
                currentAmmo = 5;
                break;
        }

        initialAccuracy = weaponData.initialAccuracy;
        bulletAccuracyDecrement = weaponData.bulletAccuracyDecrement;

        if(weaponData.weaponLength == 0){
            this.transform.localPosition = new Vector3(-0.015f, 0.53f, 0);
        }else if(weaponData.weaponLength == 1){
            this.transform.localPosition = new Vector3(-0.015f, 0.68f, 0);
        }else{
            //Prepared for different weapon lengths
            this.transform.localPosition = new Vector3(-0.015f, 0.75f, 0);
        }

        if(weaponData.manuallyChambered){
            weapon.shellInChamber = true;
        }

    }


    /// <summary>
    /// Called when the player is spotted and gives a small delay before the NPC starts shooting.
    /// </summary>
    public void PlayerSpotted(){
        betweenShotsTimer = 1f;
    }

    /// <summary>
    /// Attempt to fire the weapon if possible.
    /// </summary>
    public void EngageEnemy(){
        if(status.selectedWeapon == null){
            return;
        }

        if(reloading){
            return;
        }

        if(betweenShotsTimer <= 0){
             //Round is fired
            audioManager.Play(status.selectedWeapon.itemData.weaponType + "Gunshot", this.gameObject);
            if(muzzleFlashEnabled)
                StartCoroutine(MuzzleFlash());
           
            Vector2 bulletDeviation = ApplyAimErrorToRaycast(transform.parent.transform.up, CalcAimCone());
            if(weaponType == "Shotgun"){
                for (int i = 0; i < 8; i++)
                {
                    Vector2 pelletBulletDeviation = ApplyAimErrorToRaycast(bulletDeviation, shotgunSpread / 2);
                    Shoot(pelletBulletDeviation);
                }
            }else{
                Shoot(bulletDeviation);
            }

            currentAmmo --;

            if(weaponData.weaponType == "AssaultRifle"){
                if(Random.Range(0, 1f) < 0.2f){
                    betweenShotsTimer = fireRate + Random.Range(0.1f, 0.2f);
                }else{
                    betweenShotsTimer = fireRate;
                }
            }else{
                betweenShotsTimer = fireRate + Random.Range(0f, 0.2f);
            }
            

            if(weaponData.weaponType == "AssaultRifle"){
                if (consecShots < 10)
                    consecShots += 1;
                cooldownStartTimer = 0.0f;
                
            }else if(weaponData.weaponType == "Pistol"){
                if (consecShots < 5)
                    consecShots += 1;
                cooldownStartTimer = 0.0f;
            }

            

            if(weaponData.manuallyChambered && !racking){
                StartCoroutine(RackWeapon(weapon));
                racking = true;
            }
            
        }

        if(currentAmmo <= 0){
            StartCoroutine(Reload());
        }
        
    }

    /// <summary>
    /// Coroutine to rack the NPCs weapon.
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    private IEnumerator RackWeapon(InventoryItem weapon){
        yield return new WaitForSeconds(0.2f);
        animationController.RackAnimation(weapon);
        racking = false;
    }



    /// <summary>
    /// Fires a bullet in the direction the NPC is facing.
    /// </summary>
    /// <param name="BulletDeviation"></param>
    private void Shoot(Vector2 BulletDeviation)
    { 
        //Variables to determine which body part collider the raycast passed through
        bool headShot = false;
        bool torsoShot = false;
        bool legsShot = false;
        bool leftArmShot = false;
        bool rightArmShot = false;

        //Whether the raycast already tested the player collider and if it determined that a hit is possible
        bool playerTested = false;
        bool playerHitPossible = false;
        
        //Reference o the player's hitbox when raycast passes through it
        HumanHitbox hitPlayerHitbox = null;
        RaycastHit2D[] hits = Physics2D.RaycastAll(muzzle.transform.position, BulletDeviation, Mathf.Infinity, layerMask);
        //If the bullet passes a half obstacle the distance from the obstacle needs to be stored for future calculations
        float halfObstacleDistance = -1.0f;
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
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {

                if(!playerTested){
                    playerTested = true;
                    if(PlayerHit(halfObstacleDistance, hit)){
                        playerHitPossible = true;
                        BulletImpact(hit, muzzle.transform.position);
                        hitPlayerHitbox = hit.transform.root.Find("Torso").GetComponent<HumanHitbox>();
                    }else{
                        playerHitPossible = false;
                    }
                }
                if(playerHitPossible){
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

        if(hitPlayerHitbox != null){
            hitPlayerHitbox.BulletHit(weapon.itemData.damage, headShot, torsoShot, legsShot, leftArmShot, rightArmShot);
        } 
    }
        

    //Calculate aiming error of player based on the characters shooting ability
    //Better shooting ability -> smaller aim cone
    //shooter ability modifier between 0 and 1
    /// <summary>
    /// Caluclates the aim cone in degrees based on the NPC's shooting ability, the initial weapon accuracy and the number of consecutive shots fired.
    /// </summary>
    /// <returns>Degrees of the aim cone, halved</returns>
    private float CalcAimCone()
    {

        //Summary: The weapon is less accurate with more consecutive shots, this stops after 10 rounds
        // The best shooter is twice as good at controling recoil than the worst shooter
        float consecShotsModifier = (bulletAccuracyDecrement - 0.5f*(1 - status.shootingAbility)) * consecShots;

        float degrees = initialAccuracy * status.shootingAbility + consecShotsModifier;

        //Degrees are halved here before returining (half the degrees to each side)
        degrees *= 0.5f;

        return degrees;
    }

    //Applies the random error within the confines of the cone to the bullet
    //Degrees represent half the angle of the cone (15 degree -> 30degree cone)
    /// <summary>
    /// Applies an aim error to the raycast.
    /// </summary>
    /// <param name="originalDirection">Original direction the raycast (bullet) was meant to fly in.</param>
    /// <param name="degrees">How many degrees to each side can the bullet deviate.</param>
    /// <returns>New direction the bullet flies towards.</returns>
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
    /// Determines whether the player was hit based on distance from the NPC and possible half cover the player is hiding behind.
    /// </summary>
    /// <param name="halfObstacleDistance">Distance between the shooter and the half obstacle. If there was no half obstacle in between, this is set to a negative value.</param>
    /// <param name="hit">The raycast hit.</param>
    /// <returns>Whether the player was hit or not.</returns>
    public bool PlayerHit(float halfObstacleDistance, RaycastHit2D hit)
    {
        bool isCrouched = hit.transform.root.GetComponent<PlayerStatus>().isCrouched;
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
    /// Determines whether a bullet passed a half obstacle or was stopped by it based on distance.
    /// </summary>
    /// <param name="distance">Dinstance between the shooter and the half obstacle.</param>
    /// <returns>Whether the bullet passed the half obstacle (true) or not (false)</returns>
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

    /// <summary>
    /// Creates a bullet impact effect where the bullet impacted an obstacle or an enemy.
    /// </summary>
    /// <param name="hit">Raycast hit.</param>
    /// <param name="muzzlePos">Position of the muzzle.</param>
    public void BulletImpact(RaycastHit2D hit, Vector3 muzzlePos)
    {
        //Get the direction of the bullet, instantiate the bullet impact effect
        Vector3 dir = hit.point - new Vector2(muzzlePos.x, muzzlePos.y);
        GameObject bullet = Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(Vector3.forward, dir));

        //If it hit the player, switch it for blood splatter and rotate it to be oposite of the shot direction
        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")){
            
            bullet.GetComponent<Animator>().SetBool("BloodSplatter", true);
            bullet.transform.position = bullet.transform.position + (dir.normalized * 5f);
           
        }
    }

    /// <summary>
    /// Applies possible cover to the enemy if they are crouched behinf a half wall based on their distance from each other.
    /// </summary>
    /// <param name="halfObstacleDistance">Distance between the shooter and the half obstacle.</param>
    /// <param name="hit">Raycast hit.</param>
    /// <returns>The chance modifier of whether the hit was possible o not.</returns>
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
    /// Selects which muzzle flashes should be used for which weapon.
    /// </summary>
    /// <param name="weaponType">The type of weapon the NPC is holding.</param>
    private void MuzzleFlashOffsetSelector(string weaponType){
        if(weaponType == "Pistol"){
            muzzleFlashOffset = 0;
        }else if(weaponType == "AssaultRifle" || weaponType == "HuntingRifle"){
            muzzleFlashOffset = 3;
        }else if(weaponType == "Shotgun"){
            muzzleFlashOffset = 6;
        }
    }

    /// <summary>
    /// Coroutine that emulates muzzle flash by turning the mizzle sprite and its light on and off.
    /// </summary>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator MuzzleFlash(){
        muzzleFlashSpriteRenderer.sprite = muzzleFlashSprites[muzzleFlashOffset + ((muzzleFlashCycle++) % 3)];
        muzzleLight.enabled = true;
        muzzleFlashSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
        muzzleFlashSpriteRenderer.enabled = false;
    }

    /// <summary>
    /// Coroutune that simulates the NPC's reload.
    /// </summary>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator Reload(){
        reloading = true;
        if(weaponData.usesMagazines){
            animationController.UnloadMagAnimation(weapon);
            yield return new WaitForSeconds(magLoadSpeed);
            animationController.LoadMagAnimation(weapon);
            yield return new WaitForSeconds(magLoadSpeed);

            animationController.RackAnimation(weapon);
            yield return new WaitForSeconds(rackWeaponSpeed);
            currentAmmo = maxAmmo;
        }else{
            while(currentAmmo < maxAmmo){
                animationController.LoadRoundAnimation(weapon);
                yield return new WaitForSeconds(loadRoundSpeed);
                currentAmmo++;
            }

            animationController.RackAnimation(weapon);
            yield return new WaitForSeconds(rackWeaponSpeed);
            
        }
        
        reloading = false;
    }

    /// <summary>
    /// Sets the visibility of the muzzle flash, used to disable it when the NPC is outside of the FOV of the player.
    /// </summary>
    /// <param name="visible"></param>
    public void SetMuzzleFlashVisibility(bool visible){
        muzzleFlashEnabled = visible;
    }

    



}
