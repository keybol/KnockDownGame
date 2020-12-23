using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI Navigation, if set correctly will wander around the area until a target destination has been set (either through a delay or line of sight). Once set the entity will move towards the destination.
/// Once the destination is reached it will randomly switch between 3 behaviours, rotating around the entity, moving away from the entity or moving towards the entity.
/// </summary>
public class AINavigation : MonoBehaviour {



	// ****************** Settings ***************************

	#region Settings 
	public KPlayer kplayer;
    //if Disabled then entity will not navigate
    public bool navEnabled = true;

    /// <summary>
    /// If true then navigation will be disabled on abc activation
    /// </summary>
    public bool disableOnABCActivation = true;


    [Header("Wandering Settings")]

    /// <summary>
    /// If enabled then entity will wander until the destination has been set
    /// </summary>
    public bool wanderTillDestinationSet = true;

    /// <summary>
    /// Time between wander destination being updated
    /// </summary>
    public float wanderInterval = 10f;

    /// <summary>
    /// Area range in which the next random position will be selcted
    /// </summary>
    public float wanderAreaRange = 50f;

    /// <summary>
    /// Max range in which entity will wander
    /// </summary>
    public float wanderRange = 20f;

    [Space(10)]

    /// <summary>
    /// The speed of the entity when wandering 
    /// </summary>
    public float wanderSpeed = 3f;

    /// <summary>
    /// Name of the Animation in the controller to use when wandering
    /// </summary>
    public string wanderAnimatorParameter = "WalkForward";

    [Header("Destination Settings")]

    /// <summary>
    /// Destination we are trying to reach via Tag
    /// </summary>
    public string destinationTag;


    /// <summary>
    /// The radius to search for entities with the correct tag
    /// </summary>
    public float destinationSearchRadius = 40f;

    /// <summary>
    /// The interval before destination is cleared so a new one can be found, if 0 then this functionality is turned off
    /// </summary>
    public float clearDestinationInterval = 0f;


    /// <summary>
    /// The minimum time till the entity will start travelling towards the target 
    /// </summary>
    [Range(0f, 300f)]
    public float setDestinationMinDelay = 1f;

    /// <summary>
    /// The maximum time till the entity will start travelling towards the target 
    /// </summary>
    [Range(0f, 300f)]
    public float setDestinationMaxDelay = 10f;

    [Space(10)]

    /// <summary>
    /// Speed of the entity when travelling to destination
    /// </summary>
    public float speed = 10f;


    /// <summary>
    /// Name of the Animation in the controller to use when moving to the destination
    /// </summary>
    public string destinationAnimatorParameter = "Run";

    [Space(10)]

    /// <summary>
    /// The minimum stopping distance from destination
    /// </summary>
    [Range(1f, 100f)]
    public float minimumStopDistance = 8f;

    /// <summary>
    /// The maximum stopping distance from destination
    /// </summary>
    [Range(1f, 100f)]
    public float maximumStopDistance = 14f;


    /// <summary>
    /// If true then the stopping distance checks will include if the entity agent is currently automatically moving due to nav obstacles so then it won't stop movement animations cause agent velocity is higher then 0 etc
    /// </summary>
    public bool stopDistanceCheckVelocity = false;

    /// <summary>
    /// If true then the entity will always turn towards the destination
    /// </summary>
    public bool alwaysFaceDestination = true;



    [Header("Line of Sight Settings")]

    /// <summary>
    /// If enabled then the entity will only move towards the destination if it's in line of sight 
    /// </summary>
    public bool lineOfSightRequired = false;

    /// <summary>
    /// Line of sight range
    /// </summary>
    public float lineOfSightRange = 40f;



    [Header("Destination Behaviour - Rotate Around Settings")]

    /// <summary>
    /// If enabled then the entity has the potential to rotate around the destination once reached
    /// </summary>
    public bool enableRotationBehaviour = false;

    /// <summary>
    /// The minimum time which must pass between rotations
    /// </summary>
    public float rotationInterval = 20f;

    /// <summary>
    /// The minimum chance for the entity to start rotating around the destination
    /// </summary>
    [Range(0f, 100f)]
    public float rotationMinChance = 0f;


    /// <summary>
    /// The maximum chance for the entity to start rotating around the destination
    /// </summary>
    [Range(0f, 100f)]
    public float rotationMaxChance = 42f;


    [Space(10)]
    /// <summary>
    /// The speed in which the entity will rotate
    /// </summary>
    public float rotationSpeed = 10f;

    /// <summary>
    /// Name of the Animation in the controller to use when rotation around destination to the right
    /// </summary>
    public string rotateAroundLeftAnimatorParameter = "SideStepLeft";

    /// <summary>
    /// Name of the Animation in the controller to use when rotation around destination to the left
    /// </summary>
    public string rotateAroundRightAnimatorParameter = "SideStepRight";


    [Space(10)]


    /// <summary>
    /// The minimum duration the entity will rotate around for
    /// </summary>
    [Range(0f, 100f)]
    public float rotationMinDuration = 5f;


    /// <summary>
    /// The maximum duration the entity will rotate around for
    /// </summary>
    [Range(0f, 100f)]
    public float rotationMaxDuration = 10f;



    [Header("Destination Behaviour - Distance Change Settings")]

    /// <summary>
    /// If enabled then the entity has the potential to rotate around the destination once reached
    /// </summary>
    public bool enableDistanceBehaviour = false;

    /// <summary>
    /// The minimum time which must pass between distance change
    /// </summary>
    public float distanceChangeInterval = 20f;

    /// <summary>
    /// The minimum chance for the entity to start moving forward or back from destination (within the stop distance)
    /// </summary>
    [Range(0f, 100f)]
    public float distanceChangeMinChance = 50f;

    /// <summary>
    /// The maximum chance for the entity to start moving forward or back from destination (within the stop distance)
    /// </summary>
    [Range(0f, 100f)]
    public float distanceChangeMaxChance = 100f;

    [Space(10)]

    /// <summary>
    /// The minimum chance for the entity to start moving back from destination (within the stop distance)
    /// </summary>
    [Range(0f, 100f)]
    public float moveBackMinChance = 0f;

    /// <summary>
    /// The maximum chance for the entity to start moving forward to destination (within the stop distance)
    /// </summary>
    [Range(0f, 100f)]
    public float moveBackMaxChance = 50f;

    /// <summary>
    /// The speed in which the entity will move forward or back
    /// </summary>
    public float moveBackSpeed = 3f;

    /// <summary>
    /// Name of the Animation in the controller to use when rotation around destination
    /// </summary>
    public string moveBackAnimatorParameter = "WalkBack";

    [Space(10)]

    /// <summary>
    /// The minimum chance for the entity to start rotating around the destination (within the stop distance)
    /// </summary>
    [Range(0f, 100f)]
    public float moveForwardMinChance = 51f;

    /// <summary>
    /// The maximum chance for the entity to start rotating around the destination (within the stop distance)
    /// </summary>
    [Range(0f, 100f)]
    public float moveForwardMaxChance = 100f;

    /// <summary>
    /// The speed in which the entity will move forward or back
    /// </summary>
    public float moveTowardsSpeed = 3f;

    /// <summary>
    /// Name of the Animation in the controller to use when rotation around destination
    /// </summary>
    public string moveTowardsAnimatorParameter = "WalkForward";



    #endregion

    // ****************** Variables ***************************

    #region Variables 

    /// <summary>
    /// Transform of our main destination 
    /// </summary>
    private Transform destination;

    /// <summary>
    /// Animator of the entity 
    /// </summary>
    private Animator ani;

    /// <summary>
    /// Entity's NavMeshAgent component
    /// </summary>
    private NavMeshAgent navAgent;

    /// <summary>
    /// Transform of the entity this component is attached too 
    /// </summary>
    private Transform meTransform;

    /// <summary>
    /// Determines if the goal destination has been set
    /// </summary>
    private bool destinationSet = false;

    /// <summary>
    /// Determines if the wander destination has been set 
    /// </summary>
    private bool wanderSet = false;

    /// <summary>
    /// Tracks when we last set the wander destination so wandering isn't repeated before the interval time
    /// </summary>
    private float wanderSetTime = 0f;

    /// <summary>
    /// If true then entity will rotate around current destination
    /// </summary>
    private bool rotateAroundDestination = false;

    /// <summary>
    /// What direction the entity was rotating around (-1 is off, 0 is right, 1 is left)
    /// </summary>
    private int rotateAroundDirection = -1;

    /// <summary>
    /// Duration of the rotation around destination
    /// </summary>
    private float rotateAroundDestinationDuration = 0f;

    /// <summary>
    /// The time in which the entity started to rotate around the destination
    /// </summary>
    private float rotateAroundDestinationStartTime = 0f;

    /// <summary>
    /// The time in which the entity started to change distance from destination
    /// </summary>
    private float changeDestinationDistanceStartTime = 0f;

    /// <summary>
    /// If true then entity will move back from current destination
    /// </summary>
    /// <remarks>Used for when the stopping distance is changed, changing this variable to true will move the entity back so it matches the new stopping distance (if it was higher then before) </remarks>
    private bool changeDestinationMovingAway = false;

    /// <summary>
    /// True if the entity is currently moving towards the destination due to an at destination behaviour
    /// </summary>
    private bool changeDestinationMovingTowards = false;

    /// <summary>
    /// The distance between the entity and the destination when the entity started moving forward. This is so if the destination changes the behaviour can stop
    /// </summary>
    private float changeDestinationMoveTowardsStartingDistance = 0;

    /// <summary>
    /// Records the ABC controller component for events
    /// </summary>
    private ABC_Controller ABCEventsCont;


    #endregion

    // ****************** Private Methods ***************************

    #region Private Methods 

    /// <summary>
    /// Will intergate with ABC by retriving the component from the current follow target and then subscribing to it's ability activation events
    /// </summary>
    private void IntegrateWithABC()
    {

        if (this.disableOnABCActivation == false)
            return; 

        this.ABCEventsCont = transform.GetComponentInChildren<ABC_Controller>();

        if (this.ABCEventsCont != null)
        {
            this.ABCEventsCont.onAbilityActivation += this.DisableNavigation;
            this.ABCEventsCont.onAbilityActivationComplete += this.EnableNavigation;
        }

    }

    /// <summary>
    /// Will enable the always face destination setting 
    /// </summary>
    private void EnableNavigation(string AbilityName, int AbilityID)
    {

        this.navEnabled = true;
    }

    /// <summary>
    /// Will disable the always face destination setting allowing entities to turn to the target
    /// </summary>
    private void DisableNavigation(string AbilityName, int AbilityID)
    {
        this.ClearDestination();
        this.navEnabled = false;
    }




    /// <summary>
    /// Determines if the given value is between the min and max values provided
    /// </summary>
    /// <param name="Min">Minimum value the roll has to be above to return true</param>
    /// <param name="Max">Maximum value the roll has to be below to return true</param>
    /// <returns>True if the dice roll is inbetween the numbers provided, else false</returns>
    private bool DiceRollCheck(float DiceRoll, float Min, float Max) {

        float dice = DiceRoll;

        if (dice >= Min && dice <= Max) {
            return true;
        }
        else {
            return false;
        }
    }

	private void Start()
	{
		kplayer = GetComponent<KPlayer>();
	}

    /// <summary>
    /// Starts an animation for the component depending on the state given
    /// </summary>
    /// <param name="State">The animation to play</param>
    private void StartAnimation(AnimationState State) {

        if (this.ani == null)
            return;

        // set variables to be used later 
        string animatorParameter = "";
        float animatorOnValue = 0;



        switch (State) {
            case AnimationState.Wander:

                animatorParameter = this.wanderAnimatorParameter;
                animatorOnValue = this.wanderSpeed;

                break;
            case AnimationState.ToDestination:

                animatorParameter = this.destinationAnimatorParameter;
                animatorOnValue = this.speed;

                break;
            case AnimationState.RotateAroundRight:

                animatorParameter = this.rotateAroundRightAnimatorParameter;
                animatorOnValue = this.rotationSpeed;

                break;
            case AnimationState.RotateAroundLeft:

                animatorParameter = this.rotateAroundLeftAnimatorParameter;
                animatorOnValue = this.rotationSpeed;

                break;
            case AnimationState.ChangeDistanceForward:

                animatorParameter = this.moveTowardsAnimatorParameter;
                animatorOnValue = this.moveTowardsSpeed;

                break;
            case AnimationState.ChangeDistanceBackward:

                animatorParameter = this.moveBackAnimatorParameter;
                animatorOnValue = this.moveBackSpeed;

                break;
        }


        // if animator parameter is null then  end here. 
        if (animatorParameter == "")
            return;

        //Start Animation
        ani.SetFloat(animatorParameter, animatorOnValue);


    }


    /// <summary>
    /// Ends an animation for the component depending on the state given
    /// </summary>
    /// <param name="State">The animation to play</param>
    private void EndAnimation(AnimationState State) {

        if (this.ani == null || this.ani.gameObject.activeInHierarchy == false)
            return;

        // set variables to be used later 
        string animatorParameter = "";
        float animatorOffValue = 0;



        switch (State) {
            case AnimationState.Wander:

                animatorParameter = this.wanderAnimatorParameter;
                animatorOffValue = 0;

                break;
            case AnimationState.ToDestination:

                animatorParameter = this.destinationAnimatorParameter;
                animatorOffValue = 0;

                break;
            case AnimationState.RotateAroundRight:

                animatorParameter = this.rotateAroundRightAnimatorParameter;
                animatorOffValue = 0;

                break;
            case AnimationState.RotateAroundLeft:

                animatorParameter = this.rotateAroundLeftAnimatorParameter;
                animatorOffValue = 0;

                break;
            case AnimationState.ChangeDistanceForward:

                animatorParameter = this.moveTowardsAnimatorParameter;
                animatorOffValue = 0;

                break;
            case AnimationState.ChangeDistanceBackward:

                animatorParameter = this.moveBackAnimatorParameter;
                animatorOffValue = 0;

                break;
        }


        // if animator parameter is null then  end here. 
        if (animatorParameter == "")
            return;

        //Start Animation
        ani.SetFloat(animatorParameter, animatorOffValue);


    }

    /// <summary>
    /// Ends all known animations for the component
    /// </summary>
    /// <param name="State">The animation to play</param>
    private void EndAllAnimations() {

        if (this.ani == null)
            return;

        foreach (AnimationState state in System.Enum.GetValues(typeof(AnimationState))) {
            this.EndAnimation(state);
        }

    }

    /// <summary>
    /// Will search in a radius and set the destination if the correct tag has been found
    /// </summary>
    private void FindDestination() {

        if (this.destination != null || this.navEnabled == false)
            return;

        // get all objects in a  range
        Collider[] potentialDestinations = Physics.OverlapSphere(meTransform.position, this.destinationSearchRadius);

        //loop through to find correct one that matches tag
        foreach (Collider destination in potentialDestinations) {

            //If tag doesn't match move on
            if (destination.transform.tag != this.destinationTag)
                continue;

            //If entity is not in sight then move on
            if (EntityInSight(destination.transform) == false)
                continue;
          
            //If we got this far then correct object has been found so set destination and end here
            this.destination = destination.transform;

            break;

        }
    }

   

    /// <summary>
    /// Determines if the entity is in sight of the destination. If the entity was already previously aware of the destination then only range is checked else range and line of sight is checked. 
    /// </summary>
    /// <param name="Entity">Object to check if in line of sight</param>
    /// <returns>true if entity is in range and is either facing the destination or is already aware of the target, else false</returns>
    private bool EntityInSight(Transform Entity) {

        if (Entity == null)
            return false;

        //If we don't care about line of sight then return true
        if (this.lineOfSightRequired == false)
            return true;

        // if distance between the entity and destination is greater then the line of sight then return false as we too far away 
        if (navAgent.isActiveAndEnabled && navAgent.remainingDistance > this.lineOfSightRange)
            return false;

        //Check if entity is looking at the destination 
        var dir = (Entity.position - meTransform.position).normalized;
        var dot = Vector3.Dot(dir, meTransform.forward);

        if (dot >= 0.3 || destinationSet == true) {
            //  entity is facing the target or is already aware of the destination presence so return true
            return true;
        }
        else {
            //If the entity is not already aware of the destination or can't see the destination then return false
            return false;
        }

    }

    /// <summary>
    /// Will determine if the entity is within stopping distance of the destination
    /// </summary>
    /// <returns>True if within stopping distance, else false</returns>
    private bool WithinStoppingDistance() {

        // if distance between the entity and nav agents destination is less then the stopping distance then return true  
        if (navAgent.remainingDistance <= navAgent.stoppingDistance && (navAgent.hasPath == false || (this.stopDistanceCheckVelocity == false || this.stopDistanceCheckVelocity == true && navAgent.velocity.sqrMagnitude == 0f)))
            return true;



        // else we are not within the stopping distance
        return false;


    }

    /// <summary>
    /// Will clear the current destination stopping the navagent from moving towards anywhere 
    /// </summary>
    private void ClearDestination() {

        if (navAgent.isOnNavMesh){
            this.navAgent.isStopped = true;
            this.navAgent.ResetPath();
        }
 
        this.destination = null;
       this.destinationSet = false;
       this.wanderSet = false;
       this.EndAllAnimations();
       
    }

    /// <summary>
    /// Will make the entity wander to a random position on the NavMesh
    /// </summary>
    private void WanderManager() {

        if (this.navEnabled == false)
            return; 

        //If not active then end manager here
        if (navAgent.enabled == false || this.meTransform.gameObject.activeInHierarchy == false)
            return;

        //If the entity is moving towards our goal destination then we can return as we don't want to start wandering
        if (this.destinationSet == true)
            return;

        //If we are wandering and within stopping distance then we can stop wandering as we have reached the random point 
        if (this.wanderSet == true && WithinStoppingDistance())
            this.ClearDestination();


        //if it's too early to start wandering then return here
        if (Time.time - this.wanderSetTime < this.wanderInterval)
            return;


        //Get random point on NavMesh 
        Vector3 randomDestination = meTransform.position + Random.insideUnitSphere * this.wanderAreaRange;
        NavMeshHit hit;



        //if Position is not found then return
        if (NavMesh.SamplePosition(randomDestination, out hit, this.wanderRange, 1) == false)
            return;


        // else set destination to new random destination 
        navAgent.SetDestination(hit.position);
		navAgent.speed = this.wanderSpeed;
		kplayer.MovementStickValue = new Vector2(navAgent.velocity.x, navAgent.velocity.z);

		//No longer require a stopping distance as this is a random wander so will set it to a hardcoded value
		navAgent.stoppingDistance = 2f;

        //If the agent was stopped previously start it up again
        if (navAgent.isStopped == true)
            navAgent.isStopped = false;

        //we are wandering
        this.wanderSet = true;
        this.wanderSetTime = Time.time;

        //Start wander animation
        this.StartAnimation(AnimationState.Wander);



    }

    /// <summary>
    /// Sets the destination of the NavMeshAgent to the goal destination defined in settings
    /// </summary>
    private void DestinationManager() {

        if (this.navEnabled == false)
            return;

        //try and find destination
        this.FindDestination();

        //If not active then end manager here
        if (navAgent.enabled == false || meTransform.gameObject.activeInHierarchy == false || this.destination == null)
            return;


        // If we are not in line of sight of the destination or they are out of range or are no longer active then clear the destination 
        if (this.EntityInSight(this.destination) == false || Vector3.Distance(this.destination.transform.position, this.meTransform.position) > this.destinationSearchRadius || this.destination.gameObject.activeInHierarchy == false) {

            //Remove the main destination if previously set 
            if (this.destinationSet == true)
                this.ClearDestination();

            return;
        }


        //If this is first time setting the destination then set the random stopping distance
        if (this.destinationSet == false)
            navAgent.stoppingDistance = Random.Range(this.minimumStopDistance, this.maximumStopDistance);

        //If we got this far the entities knows about the destination so set it and stopping distance
        navAgent.SetDestination(destination.position);

        //Tell rest of component that main destination has been set with this also stop the wandering flag
        this.destinationSet = true;

        if (this.wanderSet) {
            this.wanderSet = false;
            this.EndAnimation(AnimationState.Wander);
        }


        //If we are within the stopping distance then call the at destination behaviour manager else animate movement
        if (WithinStoppingDistance()) {

            this.AtDestinationBehaviourManager();

        } else if (this.changeDestinationMovingTowards && Vector3.Distance(this.meTransform.position, this.destination.position) <= this.changeDestinationMoveTowardsStartingDistance+1) {    
            // else if entity is moving towards destination and the distance between entity and destination has not increased since the entity started moving then start the move forward animation and set the speed 
            this.StartAnimation(AnimationState.ChangeDistanceForward);
			navAgent.speed = this.moveTowardsSpeed;
			kplayer.MovementStickValue = new Vector2(navAgent.velocity.x, navAgent.velocity.z);

		} else { 
            // else move towards destination as normal stopping any previous stop at destination behaviours
            this.StopDestinationBehaviours();

            //set speed and start animation
            this.StartAnimation(AnimationState.ToDestination);
			navAgent.speed = this.speed;
			kplayer.MovementStickValue = new Vector2(navAgent.velocity.x, navAgent.velocity.z);

		}

        //If the agent was stopped previously start it up again
        if (navAgent.isStopped == true)
            navAgent.isStopped = false;


    }

    /// <summary>
    /// Manages the behaviour once the entity reaches the destination includes stop walking animations and special behaviours like rotating round or changing stopping distance to move forward or back
    /// </summary>
    private void AtDestinationBehaviourManager() {

        //Stop main movement animation
        this.EndAnimation(AnimationState.ToDestination);

        if (this.destinationSet == false)
            return;

        //If we was previously in the behaviour of changing distance and moving towards we can turn this off now 
        if (this.changeDestinationMovingTowards) {
            this.changeDestinationMovingTowards = false;
            this.EndAnimation(AnimationState.ChangeDistanceForward);
        }


        float diceRoll = Random.Range(0f, 100f);

        //determine if we can rotate around target by doing a dice roll and making sure the last rotation ended more then interval time ago 
        if (this.enableRotationBehaviour && this.DiceRollCheck(diceRoll, this.rotationMinChance, this.rotationMaxChance) && (Time.time - rotateAroundDestinationStartTime > this.rotationInterval + this.rotateAroundDestinationDuration)) {
            this.rotateAroundDestination = true;
            this.rotateAroundDestinationStartTime = Time.time;
            this.rotateAroundDestinationDuration = Random.Range(this.rotationMinDuration, this.rotationMaxDuration);
        }


        //determine if we can change distance from target by doing a dice roll and making sure the last destination change ended more then interval time ago 
        if (this.enableDistanceBehaviour && this.DiceRollCheck(diceRoll, this.distanceChangeMinChance, this.distanceChangeMaxChance) && (Time.time - changeDestinationDistanceStartTime > this.distanceChangeInterval)) {
            this.changeDestinationDistanceStartTime = Time.time;
            this.ChangeDistanceFromDestination();
        }



    }


    /// <summary>
    /// Will immediately  stop any destination behaviours currently in progress
    /// </summary>
    private void StopDestinationBehaviours() {

        //Stop moving away
        if (this.changeDestinationMovingAway) {
            this.changeDestinationMovingAway = false;
            this.EndAnimation(AnimationState.ChangeDistanceBackward);
        }

        //Stop moving towards
        if (this.changeDestinationMovingTowards) {
            this.changeDestinationMovingTowards = false;
            this.changeDestinationMoveTowardsStartingDistance = 0f;
            this.EndAnimation(AnimationState.ChangeDistanceForward);
        }

        //Stop rotating around
        if (this.rotateAroundDestination) {
            this.rotateAroundDestination = false;
            this.rotateAroundDirection = 0;
            this.EndAnimation(AnimationState.RotateAroundRight);
            this.EndAnimation(AnimationState.RotateAroundLeft);
        }

    }

    /// <summary>
    /// Will rotate the entity around the destination
    /// </summary>
    private void RotateAroundDestination() {

        if (this.rotateAroundDestination == false)
            return;

        //if we have rotated for the duration set then make sure to turn off rotation
        if (Time.time - rotateAroundDestinationStartTime > this.rotateAroundDestinationDuration) {
            this.rotateAroundDestination = false;   
            this.EndAnimation(this.rotateAroundDirection == 1 ? AnimationState.RotateAroundRight : AnimationState.RotateAroundLeft);
            //reset direction
            this.rotateAroundDirection = 0; 
        }


        if (this.destination == null || this.rotateAroundDestination == false)
            return;
        

        //Random direction (1 for right, 2 for left) - if 0 then not set yet
        if (this.rotateAroundDirection == 0)
            this.rotateAroundDirection = Random.Range(1, 3);

        //Rotate around destination by speed set
        transform.RotateAround(this.destination.position, this.rotateAroundDirection == 1 ? Vector3.down : Vector3.up, this.rotationSpeed * Time.deltaTime);

        //Start Animation
        this.StartAnimation(this.rotateAroundDirection == 1 ? AnimationState.RotateAroundRight : AnimationState.RotateAroundLeft);

    }


    /// <summary>
    /// Will change the distance from the destination, either moving closer or away. This is done by changing the stopping distance on the navmesh
    /// </summary>
    private void ChangeDistanceFromDestination() {


        if (this.destination == null)
            return;

        //Loop through dice rolling until logic has decided to move forward or back 
        bool diceLocked = true;

        while (diceLocked) {

            float diceRoll = Random.Range(0f, 100f);

            //If we are moving forwards then set the stopping distance to a number lower then what we are already on (Navagent will move automatically move due to the stopping distance being lower)
            if (this.DiceRollCheck(diceRoll, moveForwardMinChance, moveForwardMaxChance)) {
                //if we are already at lowest distance then can't move forward
                if (navAgent.stoppingDistance == this.minimumStopDistance)
                    break; 
                //Update stopping distance so it is closer
                navAgent.stoppingDistance = Random.Range(navAgent.stoppingDistance, this.minimumStopDistance);
                //Tell rest of component we are moving towards destination
                this.changeDestinationMovingTowards = true;
                //Start the moving forward animation to get a head start
                StartAnimation(AnimationState.ChangeDistanceForward);
                //Track the current distance between destination and entity. If this distance ever grows then this behaviour will stop as entity is no longer at destination and needs to reach it via normal method
                this.changeDestinationMoveTowardsStartingDistance = Vector3.Distance(meTransform.position, destination.position);
                //Stop loop
                diceLocked = false;
            }

            //If we are moving backwards then set the stopping distance to a number higher then what we are already on and also set the boolean to move the entity back away from current destination
            if (this.DiceRollCheck(diceRoll, moveBackMinChance, moveBackMaxChance) && diceLocked == true) {
                //if we are already at highest distance then can't move back
                if (navAgent.stoppingDistance == this.maximumStopDistance)
                    break;
                //update stopping distance so it's further away
                navAgent.stoppingDistance = Random.Range(navAgent.stoppingDistance, this.maximumStopDistance);
                //Set property to true so entity will start moving backwards (is not automatic like moving towards)
                this.changeDestinationMovingAway = true;
                //stop loop
                diceLocked = false;
            }
        }



    }




    /// <summary>
    /// Will move the entity away from the destination, used once the stopping distance is increased to move the entity to that position
    /// </summary>
    private void MoveAwayFromDestination() {

        if (this.changeDestinationMovingAway == false || this.destination == null) 
            return;


        //If we are close to the stopping distance then end the moving away 
        if (Vector3.Distance(this.meTransform.position, this.destination.position) >= navAgent.stoppingDistance - 2.5) {
            this.changeDestinationMovingAway = false;
            this.EndAnimation(AnimationState.ChangeDistanceBackward);
            return;
        }

        //else move back
        transform.position = Vector3.MoveTowards(this.meTransform.position, this.destination.position, -1 * this.moveBackSpeed * Time.deltaTime);

        //Start Animation
        this.StartAnimation(AnimationState.ChangeDistanceBackward);
    }





    #endregion


    // ****************** Game ***************************

    #region Game


    void OnEnable() {

        if (this.enabled == false)
            return;

        this.IntegrateWithABC();

        //Store references to components used in script 
       this.meTransform = this.GetComponent<Transform>();
       this.navAgent = meTransform.GetComponent<NavMeshAgent>();
       this.ani = meTransform.GetComponentInChildren<Animator>();



        //Invoke our wander and set destination methods
        InvokeRepeating("WanderManager", 0.5f, 1f);
        InvokeRepeating("DestinationManager", Random.Range(this.setDestinationMinDelay, this.setDestinationMaxDelay), 0.5f);
        InvokeRepeating("ClearDestination", this.clearDestinationInterval, this.clearDestinationInterval);

    }

    private void OnDisable() {
        //If script disabled then clear destination and stop animations
        this.ClearDestination();

        //desubscribe from events
        if (this.ABCEventsCont != null) {
            this.ABCEventsCont.onAbilityActivation -= this.DisableNavigation;
            this.ABCEventsCont.onAbilityActivationComplete -= this.EnableNavigation;
        }


    }




    void Update() {


        //If enabled then rotate around destination
        if (this.rotateAroundDestination)
            this.RotateAroundDestination();

        //If enabled then move away from destination
        if (this.changeDestinationMovingAway)
            this.MoveAwayFromDestination();

    }

    private void FixedUpdate() {

        //Turn to target if set to always face destination and a destination is set
        if (this.destinationSet == true && this.alwaysFaceDestination) {
            Vector3 lookDirection = destination.transform.position - meTransform.position;
            //Reset y so it's always just turning to the destination
            lookDirection.y = 0;

            meTransform.rotation = Quaternion.Lerp(meTransform.rotation, Quaternion.LookRotation(lookDirection), 20 * Time.deltaTime);

        }
    }

    #endregion



    // ************ ENUMS ***********************************

    #region Enums

    private enum AnimationState{
        Wander,
        ToDestination,
        RotateAroundLeft,
        RotateAroundRight,
        ChangeDistanceForward,
        ChangeDistanceBackward
    }

    #endregion
}
