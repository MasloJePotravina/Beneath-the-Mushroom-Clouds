using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFirearmScript : MonoBehaviour
{
    private InventoryItem weapon;
    private ItemData weaponData;

    private NPCStatus status;

    private GameObject npc;
    
    private GameObject muzzle;
    private GameObject coneLineL;
    private GameObject coneLineR;

    private AudioManager audioManager;

    private HumanAnimationController animationController;

    [SerializeField] private GameObject bulletImpactPrefab;

    //Values to which the betweenShotsTimers are set between shots
    [SerializeField] private float pistolFireRate = 0.16f;
    [SerializeField] private float assaultRifleFireRate = 0.1f;
    [SerializeField] private float shotgunFireRate = 1.2f;
    [SerializeField] private float huntingRifleFireRate = 1.2f;

    [SerializeField] private Sprite[] muzzleFlashSprites;

    private float fireRate = 0f;
    private string weaponType;
    private float shotgunSpread = 5.0f;

    public float initialDeviation = 5; 
    public float bulletDevIncrement = 1; 

    private float betweenShotsTimer = 0f;
    private float cooldownStart = 0.2f;//After how long after the first shot the recoil starts cooling down
    private float cooldownStartTimer = 0.0f;
    private float cooldownRate = 0.1f; //How quickly the firearm recoil cools down
    private float cooldownTimer = 0.0f;
    private float lastShot = 0f;

    private int muzzleFlashOffset = 0;
    private SpriteRenderer muzzleFlashSpriteRenderer;
    private UnityEngine.Rendering.Universal.Light2D muzzleLight;
    private int muzzleFlashCycle = 0;

    private int consecShots = 0;

    private int layerMask;

    private int currentAmmo = 0;
    private int maxAmmo = 0;

    bool reloading = false;
    bool racking = false;

    private float magLoadSpeed = 1.25f;
    private float rackWeaponSpeed = 1f;
    private float loadRoundSpeed = 1f;

    private bool muzzleFlashEnabled = true;


    


    private void Start()
    {
        npc = transform.parent.gameObject;
        status = npc.GetComponent<NPCStatus>();
        muzzle = transform.Find("Muzzle").gameObject;
        audioManager = GameObject.FindObjectOfType<AudioManager>();

        coneLineL = transform.Find("ConeLineLPivot").gameObject;
        coneLineR = transform.Find("ConeLineRPivot").gameObject;

        muzzleFlashSpriteRenderer = muzzle.transform.Find("MuzzleFlash").GetComponent<SpriteRenderer>();
        muzzleLight = muzzle.transform.Find("MuzzleFlash").GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        layerMask = LayerMask.GetMask("Player", "HalfObstacle", "FullObstacle");

        animationController = npc.GetComponent<HumanAnimationController>();

        

        

    }

    private void Update()
    {
        UpdateConeLines(CalcAimCone());
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

    public void SetWeaponData(InventoryItem weapon){
        this.weapon = weapon;
        this.weaponData = weapon.itemData;
        weaponType = weaponData.weaponType;
        GetWeaponVariables(weaponData);

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

    public void PlayerSpotted(){
        betweenShotsTimer = 1f;
    }

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
                lastShot = Time.time;
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

    private IEnumerator RackWeapon(InventoryItem weapon){
        yield return new WaitForSeconds(0.2f);
        animationController.RackAnimation(weapon);
        racking = false;
    }

    private void GetWeaponVariables (ItemData weaponData)
    {
        switch (weaponData.weaponType)
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

    }



    private void Shoot(Vector2 BulletDeviation)
    { 
        bool headShot = false;
        bool torsoShot = false;
        bool legsShot = false;
        bool leftArmShot = false;
        bool rightArmShot = false;
        bool playerHitPossible = false;
        bool playerTested = false;
        HumanHitbox hitPlayerHitbox = null;
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
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {

                if(!playerTested){
                    playerTested = true;
                    if(PlayerHit(halfWallDistance, hit)){
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
            hitPlayerHitbox.BulletHit(0.5f, headShot, torsoShot, legsShot, leftArmShot, rightArmShot);
        } 
    }
        

    //Calculate aiming error of player based on the characters shooting ability
    //Better shooting ability -> smaller aim cone
    //shooter ability modifier between 0 and 1
    private float CalcAimCone()
    {

        //Summary: The weapon is less accurate with more consecutive shots, this stops after 10 rounds
        // The best shooter is twice as good at controling recoil than the worst shooter
        float consecShotsModifier = (bulletDevIncrement - 0.5f*(1 - status.shooterAbility)) * consecShots;

        float degrees = initialDeviation * status.shooterAbility + consecShotsModifier;

        //Degrees are halved here before returining (half the degrees to each side)
        degrees *= 0.5f;

        return degrees;
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

    public bool PlayerHit(float halfWallDistance, RaycastHit2D hit)
    {
        bool isCrouched = hit.transform.root.GetComponent<PlayerStatus>().isCrouched;
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

    //Creates the bullet raycast
    public void BulletImpact(RaycastHit2D hit, Vector3 muzzlePos)
    {
        Vector3 dir = hit.point - new Vector2(muzzlePos.x, muzzlePos.y);
        GameObject bullet = Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(Vector3.forward, dir));
    }

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

    private void UpdateConeLines(float degrees)
    { 

        coneLineL.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, degrees);
        coneLineR.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -degrees);

    }

    private void MuzzleFlashOffsetSelector(string weaponType){
        if(weaponType == "Pistol"){
            muzzleFlashOffset = 0;
        }else if(weaponType == "AssaultRifle" || weaponType == "HuntingRifle"){
            muzzleFlashOffset = 3;
        }else if(weaponType == "Shotgun"){
            muzzleFlashOffset = 6;
        }
    }

    private IEnumerator MuzzleFlash(){
        muzzleFlashSpriteRenderer.sprite = muzzleFlashSprites[muzzleFlashOffset + ((muzzleFlashCycle++) % 3)];
        muzzleLight.enabled = true;
        muzzleFlashSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
        muzzleFlashSpriteRenderer.enabled = false;
    }

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

    public void SetMuzzleFlashVisibility(bool visible){
        muzzleFlashEnabled = visible;
    }

    



}
