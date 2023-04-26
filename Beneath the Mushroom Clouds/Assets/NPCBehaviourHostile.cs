using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviourHostile : MonoBehaviour
{
    private PathFinding pathFinding;
    private List<Vector3> currentPath = new List<Vector3>();
    private List<Vector3> currentIdealPath = new List<Vector3>();
    private List<Vector3> searchLocations = new List<Vector3>();
    [SerializeField] private List<Vector3> patrolLocations = new List<Vector3>();

    private HumanAnimationController animationController;


    private string[] states = {"Idle", "Patrol", "Chase", "Attack", "Search", "TakeCover"};
    private string currentState;

    
    public float lookAroundMinAngle = -29f;
    public float lookAroundMaxAngle = 29f;
    private float lookAroundRandomAngle;
    public float lookAroundLerpSpeed = 5f;
    public float waitBetweenLookAroundMin = 3f;
    public float waitBetweenLookAroundMax = 8f;
    
    private Quaternion originalRotation;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private bool isLookingAround = false;
    private bool lookAroundAvailable = true;

    public bool targetVisible = false;

    private NPCStatus npcStatus;

    private Transform targetTransform;

    private Vector3 lastKnwonTargetPosition;

    private bool reachedDestination = false;

    bool searchFinished = false;

    private bool searchingLocation = false;

    private float searchTimer = 0f;

    private int currentPatrolIndex = 0;

    private Vector3 currentSearchLocation;

    private NPCFirearmScript firearmScript;


    private GameObject torso;

    private bool positionedForAttack = false;

    private bool attackPositionSet = false;

    private Vector2Int previousGridPosition = new Vector2Int(-1, -1);

    private Vector2Int previousDestinationNode = new Vector2Int(-1, -1);

    private GameObject interactRange;

    private Vector3 moveDirection;

    private Vector3 previousPosition;


    



    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<HumanAnimationController>();
        WalkableGrid walkableGrid = GameObject.FindObjectOfType<WalkableGrid>();
        pathFinding = PathFinding.Instance;

        originalRotation = transform.rotation;
        startRotation = transform.rotation;
        npcStatus = GetComponent<NPCStatus>();

        if(patrolLocations.Count > 0){
            SwitchState("Patrol");
        }else{
            SwitchState("Idle");
        }


        firearmScript = transform.Find("Firearm").GetComponent<NPCFirearmScript>();

        torso = transform.Find("Torso").gameObject;

        interactRange = transform.Find("InteractRange").gameObject;

        previousPosition = transform.position;
        moveDirection = transform.position - previousPosition;

        

    }

    void Update(){
    
        GameObject headPivot = transform.Find("Torso").Find("HeadPivot").gameObject;
        headPivot.transform.rotation = this.transform.rotation;

        if(currentPath != null){
            for(int i = 0; i < currentPath.Count - 1; i++){
                Debug.DrawLine(currentPath[i], currentPath[i+1], Color.green);
            }
        }

        
        if(currentState == "Idle"){
            Idle();
        }
        else if(currentState == "Patrol"){
            Patrol();
        }
        else if(currentState == "Chase"){
            Chase();
        }
        else if(currentState == "Attack"){
            Attack();
        }
        else if(currentState == "Search"){
            Search();
        }/*
        else if(currentState == "TakeCover"){
            TakeCover();
        }*/

        Vector2Int currentGridPosition = pathFinding.grid.WorldToGridPosition(transform.position);
        if(currentGridPosition != previousGridPosition){
            
            pathFinding.grid.OccupyNode(currentGridPosition);
            pathFinding.grid.UnoccupyNode(previousGridPosition);
            previousGridPosition = currentGridPosition;
        }

        if(currentPath != null){
            if(currentPath.Count > 0){
                Vector2Int currentDestinationNode = pathFinding.grid.WorldToGridPosition(currentPath[currentPath.Count - 1]);
                if(currentDestinationNode != previousDestinationNode){
                    pathFinding.grid.OccupyNode(currentDestinationNode);
                    pathFinding.grid.UnoccupyNode(previousDestinationNode);
                    previousDestinationNode = currentDestinationNode;
                }
            }
        }


        moveDirection = transform.position - previousPosition;
        previousPosition = transform.position;
        
    }

    void LateUpdate(){
        animationController.animateTorso();
        animationController.animateLegs(new Vector2(moveDirection.x, moveDirection.y));
        animationController.setFirearmAnimatorMovementBools();
    }

    

    private void RandomLookAround(){
        if (!isLookingAround)
        {
            lookAroundRandomAngle = Random.Range(lookAroundMinAngle, lookAroundMaxAngle);
            endRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 0f, lookAroundRandomAngle));
            isLookingAround = true;
            
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, lookAroundLerpSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, endRotation) < 0.1f)
            {   
                isLookingAround = false;
                if(endRotation == originalRotation){
                    StartCoroutine(WaitBetweenLookAround());
                    return;
                }
                float randomChance = Random.Range(0f, 1f);

                if (randomChance < 0.5f)
                {
                    isLookingAround = true;
                    endRotation = originalRotation;
                    
                }else{
                    isLookingAround = true;
                    if(lookAroundRandomAngle < 0){
                        lookAroundRandomAngle = Random.Range(-lookAroundRandomAngle, -lookAroundRandomAngle + lookAroundMaxAngle);
                    }else{
                        lookAroundRandomAngle = Random.Range(lookAroundMinAngle - lookAroundRandomAngle, -lookAroundRandomAngle);
                    }
                    endRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 0f, lookAroundRandomAngle));
                }
            }
        }
    }

    private IEnumerator WaitBetweenLookAround(){
        lookAroundAvailable = false;
        yield return new WaitForSeconds(Random.Range(waitBetweenLookAroundMin, waitBetweenLookAroundMax));
        lookAroundAvailable = true;
    }

    private void Idle(){
        if(targetVisible){
            if(npcStatus.selectedWeapon == null){
                EquipWeapon();
            }
            positionedForAttack = false;
            attackPositionSet = false;
            SwitchState("Attack");
            return;
        }

        if(lookAroundAvailable){
            RandomLookAround();
        }
        
    }

    private void Patrol(){
        if(targetVisible){
            if(npcStatus.selectedWeapon == null){
                EquipWeapon();
            }
            positionedForAttack = false;
            attackPositionSet = false;
            SwitchState("Attack");
            firearmScript.PlayerSpotted();
            return;
        }

        if(reachedDestination){
            currentPatrolIndex++;
            if(currentPatrolIndex >= patrolLocations.Count){
                currentPatrolIndex = 0;
            }
            
            currentPath = pathFinding.FindIdealPath(transform.position, patrolLocations[currentPatrolIndex]);
            reachedDestination = false;
            searchingLocation = true;
            originalRotation = transform.rotation;
        }

        if(!searchingLocation){
            reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 5f);
        }else{
            SearchLocation();
        }
    }

    private void Attack(){


        if(!targetVisible && positionedForAttack){
            
            currentPath = pathFinding.FindIdealPath(transform.position, lastKnwonTargetPosition);
            reachedDestination = false;
            SwitchState("Chase");
            return;
        }


        Vector2 direction = Vector2.zero;

        
        if(targetVisible){
            direction = (targetTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), 10f * Time.deltaTime); 
        }

        

        if(!positionedForAttack)
        {
            AttackPosition();
            return;
        }

        


        float distance = Vector2.Distance(transform.position, targetTransform.position);

        if(distance < 30f){
            if(Physics2D.Raycast(transform.position, -direction, distance, LayerMask.GetMask("FullObstacle", "HalfObstacle"))){
                firearmScript.EngageEnemy();
            }else{
                transform.position = Vector2.MoveTowards(transform.position, targetTransform.position, -npcStatus.currentSpeed * Time.deltaTime);
            }
        }else{
            firearmScript.EngageEnemy();
        }



    }

    private void Chase(){

        if(targetVisible){
            positionedForAttack = false;
            attackPositionSet = false;
            SwitchState("Attack");
            firearmScript.PlayerSpotted();
            return;
        }
        if(reachedDestination){
            searchLocations = GetSearchLocations();
            SwitchState("Search");
            return;
        }

        reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 5f);
        
    }

    private void Search(){
        if(targetVisible){
            positionedForAttack = false;
            attackPositionSet = false;
            SwitchState("Attack");
            firearmScript.PlayerSpotted();
            return;
        }
        if(searchFinished){
            searchFinished = false;
            if(npcStatus.selectedWeapon != null){
                UnequipWeapon();
            }
            if(patrolLocations.Count > 0){
                SwitchState("Patrol");
            }else{
                SwitchState("Idle");
            }

            return;
        }

        
        SearchArea();
        

    }

    private void EquipWeapon(){
        npcStatus.selectedWeapon = npcStatus.weapon;
        animationController.WeaponSelectAnimation(npcStatus.selectedWeapon, null);
    }

    private void UnequipWeapon(){
        
        animationController.WeaponSelectAnimation(null, npcStatus.selectedWeapon);
        npcStatus.selectedWeapon = null;
    }

    private void SwitchState(string state){
        currentState = state;
    }

    public void SawTarget(Transform targetTransform){
        targetVisible = true;
        this.targetTransform = targetTransform;
        NotifyFrendlies();
    }

    public void LostTarget(){
        targetVisible = false;
        if(targetTransform != null)
            lastKnwonTargetPosition = targetTransform.position;
        targetTransform = null;
    }

    private bool MoveAlongPath(List<Vector3> path, float moveSpeed, float rotateSpeed){

        if(path == null){
            return true;
        }


        npcStatus.isMoving = true;
        if (path.Count == 0){
            npcStatus.isMoving = false;
            return true;
        }



        Vector3 targetPosition = path[0]; 
        Vector3 direction = targetPosition - transform.position;
        transform.position += direction.normalized * moveSpeed * Time.deltaTime; 

        if(rotateSpeed > 0){
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            if(Quaternion.Angle(transform.rotation, targetRotation) < 15f){
                animationController.ResetTorsoRotation();
                
            }
        }
        


        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            path.RemoveAt(0);
            if (path.Count == 0) 
            {
                npcStatus.isMoving = false;
                return true;
            }
        }

        return false;
    }

    private List<Vector3> GetSearchLocations(){
        List<Vector3> searchLocations = new List<Vector3>();
        for(int i = 0; i < 5; i++){
            float xSignRandom = Random.Range(0f, 1f);
            float ySignRandom = Random.Range(0f, 1f);
            float x;
            float y;
            if(xSignRandom < 0.5f)
                x = Random.Range(-40f, 20f);
            else
                x = Random.Range(20f, 40f);

            if(ySignRandom < 0.5f)
                y = Random.Range(-40f, 20f);
            else
                y = Random.Range(20f, 40f);

            searchLocations.Add(transform.position + new Vector3(x, y, 0));
        }
        return searchLocations;
    }

    private void SearchArea(){
        
        if(searchLocations.Count == 0){
            searchFinished = true;
            return;
        }

        if(reachedDestination){
            reachedDestination = false;
            searchLocations.RemoveAt(0);
            if(searchLocations.Count == 0){
                searchFinished = true;
                return;
            }
            currentSearchLocation = searchLocations[0];
            
            currentPath = pathFinding.FindIdealPath(transform.position, currentSearchLocation);
            searchingLocation = true;
            originalRotation = transform.rotation;
        }

        if(!searchingLocation){
            reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 5f);
        }else{
            SearchLocation();
        }
        

        
        
    }

    private void SearchLocation(){
        if(searchTimer < 10f){
            searchTimer += Time.deltaTime;
            RandomLookAround();
        }else{
            searchTimer = 0f;
            searchingLocation = false;
        }
        

    }

    private void ClearReservation(){
        if(currentPath != null && currentPath.Count > 0){
            
        }
    }

    private void AttackPosition(){
        if(reachedDestination){
            reachedDestination = false;
            positionedForAttack = true;
            return;
        }

        if(!attackPositionSet){
            Vector2 direction = Vector2.zero;
            if(targetVisible){
                direction = (targetTransform.position - transform.position).normalized;
            }
            currentPath = pathFinding.FindIdealPath(transform.position, transform.position + (Vector3)direction * 20f);
            attackPositionSet = true;
        }

        reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 0f);
    }

    public void HeardNoise(Vector3 noisePosition){
        if(currentState == "Attack")
            return;
        
        if(npcStatus.selectedWeapon == null){
            EquipWeapon();
        }
        lastKnwonTargetPosition = noisePosition;
        currentPath = pathFinding.FindIdealPath(transform.position, lastKnwonTargetPosition);
        reachedDestination = false;
        SwitchState("Chase");
    }

    private void NotifyFrendlies(){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 100f);
        foreach(Collider2D collider in colliders){
            if(collider.gameObject.layer == LayerMask.NameToLayer("NoiseReceiver")){
                NPCBehaviourHostile npc = collider.transform.parent.GetComponent<NPCBehaviourHostile>();
                if(npc != null){
                    npc.ReceiveTargetNotification(targetTransform);
                }
            }
        }
    }

    public void ReceiveTargetNotification(Transform targetTransform){
        if(currentState == "Attack")
            return;
        
        if(npcStatus.selectedWeapon == null){
            EquipWeapon();
        }
        lastKnwonTargetPosition = targetTransform.position;
        currentPath = pathFinding.FindIdealPath(transform.position, lastKnwonTargetPosition);
        reachedDestination = false;
        SwitchState("Chase");
    }


}
