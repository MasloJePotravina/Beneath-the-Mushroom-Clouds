using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of hostile NPCs.
/// </summary>
public class NPCBehaviourHostile : MonoBehaviour
{
    /// <summary>
    /// Reference t the pathfinding instance.
    /// </summary>
    private PathFinding pathFinding;

    /// <summary>
    /// List of locations that make up the currentp path the NPC is taking.
    /// </summary>
    private List<Vector3> currentPath = new List<Vector3>();

    /// <summary>
    /// Locations the NPC has yet to search during their "Search" state.
    /// </summary>
    private List<Vector3> searchLocations = new List<Vector3>();

    /// <summary>
    /// Locations the NPC should patrol in their "Patrol" state.
    /// </summary>
    public List<Vector3> patrolLocations = new List<Vector3>();

    /// <summary>
    /// Reference to the Human Animation Controller.
    /// </summary>
    private HumanAnimationController animationController;

    /// <summary>
    /// Array of all possible states the NPC can be in.
    /// </summary>
    private string[] states = {"Idle", "Patrol", "Chase", "Attack", "Search"};

    /// <summary>
    /// Current state the NPC is in.
    /// </summary>
    private string currentState;

    /// <summary>
    /// Minumum angle the npc rotates their head when looking around. (maximum left)
    /// </summary>
    public float lookAroundMinAngle = -29f;

    /// <summary>
    /// Maximum angle the npc rotates their head when looking around. (maximum right)
    /// </summary>
    public float lookAroundMaxAngle = 29f;

    /// <summary>
    /// Current look-around angle the NPC is trying to reach.
    /// </summary>
    private float lookAroundRandomAngle;

    /// <summary>
    /// How quickly the NPC rotates their head when looking around.
    /// </summary>
    public float lookAroundLerpSpeed = 5f;

    /// <summary>
    /// Minimal time the NPC should wait between looking around.
    /// </summary>
    public float waitBetweenLookAroundMin = 3f;

    /// <summary>
    /// Maximum time the NPC should wait between looking around.
    /// </summary>
    public float waitBetweenLookAroundMax = 8f;
    

    /// <summary>
    /// Original rotation the NPC had when they spawned.
    /// </summary>
    private Quaternion originalRotation;

    /// <summary>
    /// The rotation the NPC is currently trying to reach.
    /// </summary>
    private Quaternion endRotation;

    /// <summary>
    /// Whether the NPC is currently looking around.
    /// </summary>
    private bool isLookingAround = false;

    /// <summary>
    /// Whether the NPC can again start looking around.
    /// </summary>
    private bool lookAroundAvailable = true;

    /// <summary>
    /// Whether the NPC sees a potential target (player).
    /// </summary>
    public bool targetVisible = false;

    /// <summary>
    /// Reference to the status of the NPC.
    /// </summary>
    private NPCStatus npcStatus;

    /// <summary>
    /// Transform component of the target (player).
    /// </summary>
    private Transform targetTransform;

    /// <summary>
    /// Last known position of the target (player).
    /// </summary>
    private Vector3 lastKnwonTargetPosition;


    /// <summary>
    /// Whether the NPC has reached their destination.
    /// </summary>
    private bool reachedDestination = false;

    /// <summary>
    /// Whether the NPC finished searching an area.
    /// </summary>
    bool searchFinished = false;

    /// <summary>
    /// Whether the NPC is currently searching one of the loactions in the search area.
    /// </summary>
    private bool searchingLocation = false;

    /// <summary>
    /// Timer used to set how long a location in an area should be searched.
    /// </summary>
    private float searchTimer = 0f;

    /// <summary>
    /// Current index in the patrol path.
    /// </summary>
    private int currentPatrolIndex = 0;

    /// <summary>
    /// Currently searched location in the search area.
    /// </summary>
    private Vector3 currentSearchLocation;

    /// <summary>
    /// Reference to the NPCs firearm script.
    /// </summary>
    private NPCFirearmScript firearmScript;

    /// <summary>
    /// Reference to the NPCs torso.
    /// </summary>
    private GameObject torso;

    /// <summary>
    /// Wthether the NPC is positioned for attack.
    /// </summary>
    private bool positionedForAttack = false;

    /// <summary>
    /// Whether the NPC has determined what their attack position is.
    /// </summary>
    private bool attackPositionSet = false;

    /// <summary>
    /// Position of the NPC on the pathfinding grid in the previous frame.
    /// </summary>
    private Vector2Int previousGridPosition = new Vector2Int(-1, -1);

    /// <summary>
    /// Destination node the NPC was heading towards in the previous frame.
    /// </summary>
    private Vector2Int previousDestinationNode = new Vector2Int(-1, -1);

    /// <summary>
    /// Reference to thei nteract range of the NPC.
    /// </summary>
    private GameObject interactRange;

    /// <summary>
    /// Which direction the NPC is currently moving in.
    /// </summary>
    private Vector3 moveDirection;

    /// <summary>
    /// NPC's world position in the previous frame.
    /// </summary>
    private Vector3 previousPosition;

    /// <summary>
    /// NPC's world position when they spawned.
    /// </summary>
    private Vector3 spawnPosition;

    /// <summary>
    /// Whether the NPC is currently returning to their spawn position.
    /// </summary>
    private bool returningToSpawn = false;



    /// <summary>
    /// Sets all of the ncessary values and references. Sets the starting state based on the existence of patrol locations.
    /// </summary>
    void Start()
    {
        animationController = GetComponent<HumanAnimationController>();
        WalkableGrid walkableGrid = GameObject.FindObjectOfType<WalkableGrid>();
        pathFinding = PathFinding.Instance;

        originalRotation = transform.rotation;
        npcStatus = GetComponent<NPCStatus>();

        //If there are no patrol locations, the NPC will be idle
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

        //This is done so that I can freely place NPC spawners, without having to worry about placing the spawner precisely in the center of a grid node
        //If a simple transform.position is used, the NPC will bug out when trying to return to their spawn position and "jitter" in place
        spawnPosition = pathFinding.grid.GridToWorldPosition(pathFinding.grid.WorldToGridPosition(transform.position));


        

    }

    /// <summary>
    /// Each frame the NPC updates their state, updates their position/rotation values and occupy/unoccupy pathfinding nodes.
    /// </summary>
    void Update(){
    
        GameObject headPivot = transform.Find("Torso").Find("HeadPivot").gameObject;
        headPivot.transform.rotation = this.transform.rotation;

        //Go to the specific behaviour based on behaviour state
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
        }


        //If the grid position changed between the frames, occupy the new node and unoccupy the old one
        Vector2Int currentGridPosition = pathFinding.grid.WorldToGridPosition(transform.position);
        if(currentGridPosition != previousGridPosition){
            
            pathFinding.grid.OccupyNode(currentGridPosition);
            pathFinding.grid.UnoccupyNode(previousGridPosition);
            previousGridPosition = currentGridPosition;
        }

        //If the a path is set, unoccupy nodes that were previously on the path and occupy the one's that are the new destination path
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

    /// <summary>
    /// On late update the animations are played.
    /// </summary>
    void LateUpdate(){
        animationController.animateTorso();
        animationController.animateLegs(new Vector2(moveDirection.x, moveDirection.y));
        animationController.setFirearmAnimatorMovementBools();
    }

    
    /// <summary>
    /// Randomly looks around.
    /// </summary>
    private void RandomLookAround(){
        if (!isLookingAround)
        {
            //If the NPC is not looking around, set a ranom new angle to rotate towards
            lookAroundRandomAngle = Random.Range(lookAroundMinAngle, lookAroundMaxAngle);
            endRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 0f, lookAroundRandomAngle));
            isLookingAround = true;
            
        }
        else
        {
            //Smoothly rotate toeards the random angle
            transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, lookAroundLerpSpeed * Time.deltaTime);

            //When the angle is roughly reached
            if (Quaternion.Angle(transform.rotation, endRotation) < 3f)
            {   
                //If this was a rotation back to original rotation, it means this look around has ended and now the NPC waits for next look around
                isLookingAround = false;
                if(endRotation == originalRotation){
                    StartCoroutine(WaitBetweenLookAround());
                    return;
                }

                //Randomly either return back to the original rotation or rotate to a new angle on the oposite side
                //This should create a pretty nice look around mechanic
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

    /// <summary>
    /// Coroutine that waits a specific amount of time before allowing the NPC to look around again.
    /// </summary>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator WaitBetweenLookAround(){
        lookAroundAvailable = false;
        yield return new WaitForSeconds(Random.Range(waitBetweenLookAroundMin, waitBetweenLookAroundMax));
        lookAroundAvailable = true;
    }

    /// <summary>
    /// The behaviour of an idle NPC. Enemy stands in place and occasionally looks around.
    /// </summary>
    private void Idle(){

        //If the player is sighted, equip weapon and switch to attack state
        if(targetVisible){
            if(npcStatus.selectedWeapon == null){
                EquipWeapon();
            }
            positionedForAttack = false;
            attackPositionSet = false;
            returningToSpawn = false;
            SwitchState("Attack");
            return;
        }

        //If the NPC is not returning to spawn but is too far away from it, find a path to it and start moving towards it
        if(!returningToSpawn && Vector3.Distance(transform.position, spawnPosition) > 10f){
            currentPath = pathFinding.FindIdealPath(transform.position, spawnPosition);
            reachedDestination = false;
            returningToSpawn = true;
        }

        //If the NPC did not reach their destination move along the path
        if(!reachedDestination){
            reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 5f);
            return;
        }

        //If this piece of code was reached the NPC is at their spawn position again
        returningToSpawn = false;

        
        //If look around is possible, look around
        if(lookAroundAvailable){
            RandomLookAround();
        }
        
    }
    
    /// <summary>
    /// The behaviour of a patroling NPC. Walks from patrol position to patrol position and briefly looks around each one.
    /// </summary>
    private void Patrol(){

        //If the player is sighted, equip weapon and switch to attack state
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

        //If a destination was reached, set the next patrol location as the destination and set seachingLocation to true
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

        //If the player is not seaching location, move along the path otherwise go to the search location method
        if(!searchingLocation){
            reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 5f);
        }else{
            SearchLocation();
        }
    }

    /// <summary>
    /// The behaviour of a NPC that is attacking the player. NPC moves to an attack position and engages the player.
    /// </summary>
    private void Attack(){

        //If the player is not visible and the NPC is already positioned for attack switch to chase state
        if(!targetVisible && positionedForAttack){
            
            currentPath = pathFinding.FindIdealPath(transform.position, lastKnwonTargetPosition);
            reachedDestination = false;
            SwitchState("Chase");
            return;
        }

        //Each frame update where to rotate to face the player and rotate that way
        Vector2 direction = Vector2.zero;
        if(targetVisible){
            direction = (targetTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), 10f * Time.deltaTime); 
        }

        
        //If the NPC is not positioned for attack, move to the attack position
        if(!positionedForAttack)   
        {
            AttackPosition();
            return;
        }

        

        //If the player is too close to the NPC, move back from them
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

    /// <summary>
    /// The behaviour of a NPC that is chasing the player. NPC moves to the last known position of the player and then searches for them.
    /// </summary>
    private void Chase(){

        //If the player was spotted, switch to attack state
        if(targetVisible){
            positionedForAttack = false;
            attackPositionSet = false;
            SwitchState("Attack");
            firearmScript.PlayerSpotted();
            return;
        }

        //If the destination was reached enter the search state
        if(reachedDestination){
            searchLocations = GetSearchLocations();
            SwitchState("Search");
            return;
        }

        //Move to the destination if it was not reached yet
        reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 5f);
        
    }

    /// <summary>
    /// The behaviour of a NPC that is searching for the player. NPC moves thorough 5 search locations in an area around the last known position of the player and searches them.
    /// </summary>
    private void Search(){

        //If the player was spotted, switch to attack state
        if(targetVisible){
            positionedForAttack = false;
            attackPositionSet = false;
            SwitchState("Attack");
            firearmScript.PlayerSpotted();
            return;
        }

        //If all of the areas were searched, switch back to the NPCs default state (idle or patrol)
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

        //If the are has not yet bee search, search the area
        SearchArea();
        

    }

    /// <summary>
    /// Equips the NPCs weapon
    /// </summary>
    private void EquipWeapon(){
        npcStatus.selectedWeapon = npcStatus.weapon;
        animationController.WeaponSelectAnimation(npcStatus.selectedWeapon, null);
    }

    /// <summary>
    /// Unequips the NPCs weapon
    /// </summary>
    private void UnequipWeapon(){
        
        animationController.WeaponSelectAnimation(null, npcStatus.selectedWeapon);
        npcStatus.selectedWeapon = null;
    }

    /// <summary>
    /// Switches the NPCs state to the given state
    /// </summary>
    /// <param name="state">What state the NPC should switch to.</param>
    private void SwitchState(string state){
        currentState = state;
    }

    /// <summary>
    /// Called when the NPC spots the player.
    /// </summary>
    /// <param name="targetTransform"></param>
    public void SawTarget(Transform targetTransform){
        targetVisible = true;
        this.targetTransform = targetTransform;
        //NotifyFrendlies();
    }

    /// <summary>
    /// Called when the NPC loses sight of the player.
    /// </summary>
    public void LostTarget(){
        targetVisible = false;
        if(targetTransform != null)
            lastKnwonTargetPosition = targetTransform.position;
        targetTransform = null;
    }

    /// <summary>
    /// Moves the NPC along the set path
    /// </summary>
    /// <param name="path">List of positions that represent the currently set path/</param>
    /// <param name="moveSpeed">The speed at which the NPC moves.</param>
    /// <param name="rotateSpeed">Speed at which the NPC rotates.</param>
    /// <returns></returns>
    private bool MoveAlongPath(List<Vector3> path, float moveSpeed, float rotateSpeed){

        if(path == null){
            return true;
        }

        //Determines if the NPC is moving
        npcStatus.isMoving = true;
        if (path.Count == 0){
            npcStatus.isMoving = false;
            return true;
        }


        //Get target position and rotation
        Vector3 targetPosition = path[0]; 
        Vector3 direction = targetPosition - transform.position;
        transform.position += direction.normalized * moveSpeed * Time.deltaTime; 

        //Rotate towards the target rotation, if the rotation towards the target rotation is within 15 degrees, reset the torso rotation so it faces forward
        if(rotateSpeed > 0){
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            if(Quaternion.Angle(transform.rotation, targetRotation) < 15f){
                animationController.ResetTorsoRotation();
                
            }
        }
        

        //If the NPC reached the destination within a certain margin of error, move to the next location in the path
        if (Vector3.Distance(transform.position, targetPosition) < 3f)
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

    /// <summary>
    /// Get the list of search location to search in an area
    /// </summary>
    /// <returns>List of search locations</returns>
    private List<Vector3> GetSearchLocations(){
        List<Vector3> searchLocations = new List<Vector3>();
        for(int i = 0; i < 5; i++){
            float xSignRandom = Random.Range(0f, 1f);
            float ySignRandom = Random.Range(0f, 1f);
            float x;
            float y;
            //Random ranges not too close to the last known position of the player
            if(xSignRandom < 0.5f)
                x = Random.Range(-100f, 20f);
            else
                x = Random.Range(20f, 100f);

            if(ySignRandom < 0.5f)
                y = Random.Range(-100f, 20f);
            else
                y = Random.Range(20f, 100f);

            searchLocations.Add(transform.position + new Vector3(x, y, 0));
        }
        return searchLocations;
    }

    /// <summary>
    /// Called each frame while the NPC is searching an area.
    /// </summary>
    private void SearchArea(){
        
        //If there are no more search locations, return
        if(searchLocations.Count == 0){
            searchFinished = true;
            return;
        }

        //If the destination was reached, get the next search location
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

        //If the NPC has not yet searched this location, search it, otherwise move to the next location
        if(!searchingLocation){
            reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 5f);
        }else{
            SearchLocation();
        }
        

        
        
    }

    /// <summary>
    /// Called each frame while the NPC is searching a location. Causes the NPC to look around the location.
    /// </summary>
    private void SearchLocation(){
        if(searchTimer < 10f){
            searchTimer += Time.deltaTime;
            RandomLookAround();
        }else{
            searchTimer = 0f;
            searchingLocation = false;
        }
        

    }

    /// <summary>
    /// Moves the NPC into the attack position if it not yet in an attack position.
    /// </summary>
    private void AttackPosition(){

        //If the NPC is already in an attack position, return
        if(reachedDestination){
            reachedDestination = false;
            positionedForAttack = true;
            return;
        }

        //If the attack position was not set yet, set it
        if(!attackPositionSet){
            Vector2 direction = Vector2.zero;
            if(targetVisible){
                direction = (targetTransform.position - transform.position).normalized;
            }
            currentPath = pathFinding.FindIdealPath(transform.position, transform.position + (Vector3)direction * 20f);
            attackPositionSet = true;
        }

        //Move to the attack position
        reachedDestination = MoveAlongPath(currentPath, npcStatus.currentSpeed, 0f);
    }

    /// <summary>
    /// Called when the NPC hears a noise coming from the player
    /// </summary>
    /// <param name="noisePosition">World position of the heard noise.</param>
    public void HeardNoise(Vector3 noisePosition){

        //If the NPC is currently in an attack state, nothing happens.
        if(currentState == "Attack")
            return;
        
        //Equip a weapon if the NPC does not have one equipped
        if(npcStatus.selectedWeapon == null){
            EquipWeapon();
        }

        //Switch to the chase state
        lastKnwonTargetPosition = noisePosition;
        currentPath = pathFinding.FindIdealPath(transform.position, lastKnwonTargetPosition);
        reachedDestination = false;
        SwitchState("Chase");
    }


    //TODO: Too inefficient, needs to be optimized, disabled for now
    private void NotifyFrendlies(){
        /*Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 100f);
        foreach(Collider2D collider in colliders){
            if(collider.gameObject.layer == LayerMask.NameToLayer("NoiseReceiver")){
                NPCBehaviourHostile npc = collider.transform.parent.GetComponent<NPCBehaviourHostile>();
                if(npc != null){
                    npc.ReceiveTargetNotification(targetTransform);
                }
            }
        }*/
    }

    /// <summary>
    /// Receive information about the targets position from another NPC. (Currently unused)
    /// </summary>
    /// <param name="targetTransform"></param>
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

    /// <summary>
    /// Unoccupies the last node the NPC uccupied. Used when the NPC dies.
    /// </summary>
    public void UnoccupyLastNode(){
        pathFinding.grid.UnoccupyNode(previousDestinationNode);
    }


}
