
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the current status of the NPC.
/// </summary>
public class NPCStatus : MonoBehaviour
{

    /// <summary>
    /// Reference to the world status.
    /// </summary>
    private WorldStatus worldStatus;

    /// <summary>
    /// Whether the NPC is crouched.
    /// </summary>
    public bool isCrouched = false;

    /// <summary>
    /// Whether the NPC is moving.
    /// </summary>
    public bool isMoving = false;

    /// <summary>
    /// Whether the NPC is running.
    /// </summary>
    public bool isRunning = false;

    /// <summary>
    /// Walk speed of the NPC.
    /// </summary>
    private float walkSpeed = 50f;

    /// <summary>
    /// Run speed of the NPC.
    /// </summary>
    private float runSpeed = 100f;

    /// <summary>
    /// Crouch speed of the NPC.
    /// </summary>
    private float crouchSpeed = 20f;

    /// <summary>
    /// Current speed of the NPC.
    /// </summary>
    public float currentSpeed = 0f;

    /// <summary>
    /// Whether the NPC is hit by the field of view of the player.
    /// </summary>
    private bool hitByFov = false;

    /// <summary>
    /// Reference to the head of the NPC.
    /// </summary>
    private GameObject head;

    /// <summary>
    /// Reference to the torso of the NPC.
    /// </summary>
    private GameObject torso;

    /// <summary>
    /// Reference to the legs of the NPC.
    /// </summary>
    private GameObject legs;

    /// <summary>
    /// Reference to the sprite renderer of the head of the NPC.
    /// </summary>
    private SpriteRenderer spriteRendererHead;

    /// <summary>
    /// Reference to the sprite renderer of the torso of the NPC.
    /// </summary>
    private SpriteRenderer spriteRendererTorso;

    /// <summary>
    /// Reference to the sprite renderer of the legs of the NPC.
    /// </summary>
    private SpriteRenderer spriteRendererLegs;

    /// <summary>
    /// Reference to the sprite renderer of the firearm of the NPC.
    /// </summary>
    private SpriteRenderer spriteRendererFirearm;

    /// <summary>
    /// Prefab of an inventory item.
    /// </summary>
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// List of possible weapons that the NPC can use. 
    /// </summary>
    [SerializeField] private ItemData[] possibleWeaponsData;

    /// <summary>
    /// NPC's weapon.
    /// </summary>
    public InventoryItem weapon;

    /// <summary>
    /// Currently selected weapon (null if the NPC does not have a weapon equipped).
    /// </summary>
    public InventoryItem selectedWeapon;

    /// <summary>
    /// Shooting ability of the NPC.
    /// </summary>
    public float shootingAbility = 0f;

    /// <summary>
    /// Reference to the NPC firearm script.
    /// </summary>
    private NPCFirearmScript npcFirearmScript;

    /// <summary>
    /// Health of the NPC.
    /// </summary>
    private float health = 100f;

    /// <summary>
    /// Dictionary of body parts and whether they are shot. Used to calculate debuffs from shots for NPCs.
    /// </summary>
    /// <typeparam name="string">Body part</typeparam>
    /// <typeparam name="bool">Whether the body part is shot.</typeparam>
    /// <returns></returns>
    private Dictionary<string, bool> shotBodyParts = new Dictionary<string, bool>(){
        {"Head", false},
        {"Torso", false},
        {"LeftArm", false},
        {"RightArm", false},
        {"LeftLeg", false},
        {"RightLeg", false}
    };

    /// <summary>
    /// Base drain of a bleed.
    /// </summary>
    public float baseHealthDrain = 0.027f;

    /// <summary>
    /// Prefab for a dead body container.
    /// </summary>
    [SerializeField] private GameObject deadBodyPrefab;

    /// <summary>
    /// Current bleed strength.
    /// </summary>
    private float bleedStrength = 0;
    
    /// <summary>
    /// Reference to the particle system for the blood trail.
    /// </summary>
    private ParticleSystem bloodParticles;

    /// <summary>
    /// Get all necessary references and set base values for variables at the start.
    /// </summary>
    void Start()
    {
        torso = transform.Find("Torso").gameObject;
        legs = torso.transform.Find("Legs").gameObject;
        head = torso.transform.Find("HeadPivot").Find("Head").gameObject;
        spriteRendererFirearm = torso.transform.Find("FirearmSprite").GetComponent<SpriteRenderer>();

        npcFirearmScript = transform.Find("Firearm").GetComponent<NPCFirearmScript>();

        spriteRendererHead = head.GetComponent<SpriteRenderer>();
        spriteRendererTorso = torso.GetComponent<SpriteRenderer>();
        spriteRendererLegs = legs.GetComponent<SpriteRenderer>();

        weapon = SpawnNPCWeapon();
        npcFirearmScript.SetWeaponData(weapon);

        currentSpeed = walkSpeed;

        shootingAbility = Random.Range(0f, 0.8f);

        worldStatus = GameObject.FindObjectOfType<WorldStatus>();

        bloodParticles = transform.Find("BloodTrailEmitter").GetComponent<ParticleSystem>();

       
    }

    /// <summary>
    /// Each frame, check if the NPC is hit by the field of view of the player, update bleeding and determine death.
    /// </summary>
    void Update()
    {
        VisibilitySwitch(hitByFov);//TODO change
        hitByFov = false;

        UpdateBleeding();

        BloodTrail();

        if(health <= 0){
            Death();
        }
    }

    /// <summary>
    /// Called by the FOV of the player when they see the NPC.
    /// </summary>
    public void HitByFov()
    {
        hitByFov = true;
    }

    /// <summary>
    /// Toggle the visibility of the NPC.
    /// </summary>
    /// <param name="visible">Whether the NPC should be visible.</param>
    private void VisibilitySwitch(bool visible)
    {
        spriteRendererHead.enabled = visible;
        spriteRendererTorso.enabled = visible;
        spriteRendererLegs.enabled = visible;
        spriteRendererFirearm.enabled = visible;
        npcFirearmScript.SetMuzzleFlashVisibility(visible);
    }

    /// <summary>
    /// Spawn a weapon for the NPC.
    /// </summary>
    /// <returns>The weapon that was spawned for the NPC.</returns>
    private InventoryItem SpawnNPCWeapon(){
        int randomIndex = Random.Range(0, possibleWeaponsData.Length);
        InventoryItem weapon = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        RectTransform rectTransform = weapon.GetComponent<RectTransform>();
        //The weapon is actually an invisible inventory sprite attached to the NPC
        rectTransform.SetParent(transform);
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.localScale = new Vector3(1, 1, 1);
        weapon.Set(possibleWeaponsData[randomIndex]);
        return weapon;
    }

    /// <summary>
    /// Whether the NPC was shot.
    /// </summary>
    /// <param name="damage">Damage of the weapon that shot the NPC.</param>
    /// <param name="bodyPart">Which body part was shot.</param>
    public void GotShot(float damage, string bodyPart){

        //Damage to body parts differs (3.5x for head, 1x for torso, 0.5x for limbs)
        if(bodyPart == "Head"){
            health -= damage * 3.5f;
        }else if(bodyPart == "Torso"){
            health -= damage;
        }else{
            health -= damage * 0.5f;
        }

        shotBodyParts[bodyPart] = true;

        if(bodyPart == "LeftArm" && !shotBodyParts["LeftArm"] || bodyPart == "RightArm" && !shotBodyParts["RightArm"]){
            shootingAbility += 0.25f;
        }

        if(bodyPart == "LeftLeg" && !shotBodyParts["LeftLeg"] || bodyPart == "RightLeg" && !shotBodyParts["RightLeg"]){
            walkSpeed -= 10;
            runSpeed -= 20;
        }
    }

    /// <summary>
    /// Updates the bleed strength of the NPC.
    /// </summary>
    private void UpdateBleeding(){
        bleedStrength = 0;
        foreach(string bodyPart in shotBodyParts.Keys){
            if(shotBodyParts[bodyPart]){
                health -= baseHealthDrain * worldStatus.timeMultiplier * Time.deltaTime;
                bleedStrength += 5;
            }
        }
    }

    /// <summary>
    /// Death of the NPC. Destorys the NPC and instantiates a dead body container in their place.
    /// </summary>
    private void Death(){
        

        GameObject deadBody = InstantiateDeadBody();

        RandomizedItemGenerator itemGenerator = deadBody.GetComponent<RandomizedItemGenerator>();
        itemGenerator.forcedItems = new List<ItemData>(){weapon.itemData};
        if(weapon.itemData.usesMagazines){
            itemGenerator.forcedItems.Add(weapon.itemData.magazineItemData);
        }
        itemGenerator.forcedItems.Add(weapon.itemData.ammoItemData);

        deadBody.GetComponent<DestroyObjectOutOfView>().timeToDestruction = 3600f;//TODO: Change

        //Unoccupy last destination node
        NPCBehaviourHostile behaviour = GetComponent<NPCBehaviourHostile>();
        behaviour.UnoccupyLastNode();
        
        Destroy(gameObject);
    }

    /// <summary>
    /// Emits blood particles from the NPC when it is bleeding, based on the bleed strength.
    /// </summary>
    private void BloodTrail(){
        ParticleSystem.EmissionModule emission = bloodParticles.emission;
        if(bleedStrength <= 0f)
            emission.rateOverTime = 0f;
        else
            emission.rateOverTime = Random.Range(bleedStrength - 2, bleedStrength + 2);
    }

    /// <summary>
    /// Instantiate dead body container in the place of the NPC with proper rotation.
    /// </summary>
    /// <returns></returns>
    public GameObject InstantiateDeadBody(){
        Vector2 direction = transform.up;
        Vector2 deadBodyPosition2D = new Vector2(transform.position.x + direction.x * 10f, transform.position.y + direction.y * 10f);
        Vector3 deadBodyPosition3D = new Vector3(deadBodyPosition2D.x, deadBodyPosition2D.y, -1);
        GameObject deadBody = Instantiate(deadBodyPrefab, deadBodyPosition3D, transform.rotation);
        if(Random.Range(0, 1) < 0.5f){
            deadBody.GetComponent<SpriteRenderer>().flipX = true;
        }

        return deadBody;
    }

    
}
