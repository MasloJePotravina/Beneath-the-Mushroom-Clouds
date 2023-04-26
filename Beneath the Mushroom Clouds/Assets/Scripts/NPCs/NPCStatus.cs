
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NPCStatus : MonoBehaviour
{

    private WorldStatus worldStatus;

    public bool isCrouched = false;
    public bool isMoving = false;
    public bool isRunning = false;

    private float walkSpeed = 50f;
    private float runSpeed = 100f;
    private float crouchSpeed = 20f;

    public float currentSpeed = 0f;

    private bool hitByFov = false;

    private GameObject head;
    private GameObject torso;
    private GameObject legs;


    private SpriteRenderer spriteRendererHead;
    private SpriteRenderer spriteRendererTorso;
    private SpriteRenderer spriteRendererLegs;
    private SpriteRenderer spriteRendererFirearm;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemData[] possibleWeaponsData;
    public InventoryItem weapon;
    public InventoryItem selectedWeapon;

    public float shootingAbility = 0f;

    private NPCFirearmScript npcFirearmScript;

    private float health = 100f;
    private Dictionary<string, bool> shotBodyParts = new Dictionary<string, bool>(){
        {"Head", false},
        {"Torso", false},
        {"LeftArm", false},
        {"RightArm", false},
        {"LeftLeg", false},
        {"RightLeg", false}
    };
    public float baseHealthDrain = 0.027f;

    [SerializeField] private GameObject deadBodyPrefab;

    private float bleedStrength = 0;
    
    private ParticleSystem bloodParticles;

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

        bloodParticles = GetComponent<ParticleSystem>();

       
    }

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

    public void HitByFov()
    {
        hitByFov = true;
    }

    private void VisibilitySwitch(bool visible)
    {
        spriteRendererHead.enabled = visible;
        spriteRendererTorso.enabled = visible;
        spriteRendererLegs.enabled = visible;
        spriteRendererFirearm.enabled = visible;
        npcFirearmScript.SetMuzzleFlashVisibility(visible);
    }

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

    public void GotShot(float damage, string bodyPart){
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

    private void UpdateBleeding(){
        bleedStrength = 0;
        foreach(string bodyPart in shotBodyParts.Keys){
            if(shotBodyParts[bodyPart]){
                health -= baseHealthDrain * worldStatus.timeMultiplier * Time.deltaTime;
                bleedStrength += 5;
            }
        }
    }

    private void Death(){
        Vector2 direction = transform.up;
        Vector2 deadBodyPosition2D = new Vector2(transform.position.x + direction.x * 10f, transform.position.y + direction.y * 10f);
        Vector3 deadBodyPosition3D = new Vector3(deadBodyPosition2D.x, deadBodyPosition2D.y, -1);
        GameObject deadBody = Instantiate(deadBodyPrefab, deadBodyPosition3D, transform.rotation);
        if(Random.Range(0, 1) < 0.5f){
            deadBody.GetComponent<SpriteRenderer>().flipX = true;
        }
        
        //TODO: Items
        Destroy(gameObject);
    }

    private void BloodTrail(){
        ParticleSystem.EmissionModule emission = bloodParticles.emission;
        if(bleedStrength <= 0f)
            emission.rateOverTime = 0f;
        else
            emission.rateOverTime = Random.Range(bleedStrength - 2, bleedStrength + 2);
    }

    
}
