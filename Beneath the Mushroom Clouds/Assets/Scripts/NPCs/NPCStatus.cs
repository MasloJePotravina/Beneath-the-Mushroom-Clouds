
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NPCStatus : MonoBehaviour
{

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

    public float shooterAbility = 0f;

    private NPCFirearmScript npcFirearmScript;

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

        shooterAbility = Random.Range(0f, 0.8f);


       
    }

    void Update()
    {
        VisibilitySwitch(hitByFov);//TODO change
        hitByFov = false;
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

    
}
