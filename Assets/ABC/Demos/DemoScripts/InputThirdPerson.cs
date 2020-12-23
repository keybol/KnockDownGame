using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InputThirdPerson : MonoBehaviour {


    // ********************* Settings ********************
    #region Settings

    [Header("Component Settings")]

    /// <summary>
    /// Determines if movement and rotation is allowed
    /// </summary>
    public bool allowMovement = true;

    /// <summary>
    /// Allows for a small delay when turning back on movement
    /// </summary>
    public float enableMovementDelay = 0.3f;

    /// <summary>
    /// Allows for a small delay when turning off movement
    /// </summary>
    public float disableMovementDelay = 0f;

    /// <summary>
    /// If true then the script will integrate with ABC receiving events when movement is prevented
    /// </summary>
    public bool enableABCIntegration = false; 

    [Header("Movement")]

    /// <summary>
    /// Determines how fast the entity will move
    /// </summary>
    public float moveForce = 4f;

    /// <summary>
    /// Determines how fast the entity will move when sprinting
    /// </summary>
    public float sprintForce = 6f;

    /// <summary>
    /// Axis name of sprint button
    /// </summary>
    public string sprintButton = "Sprint";

    [Header("LockOn Movement")]


    /// <summary>
    /// If true then lock on movement will be used 
    /// </summary>
    public bool useLockOnMovement = false;

    /// <summary>
    /// The lock on target - once locked on the entity will always face the target
    /// and move sideways/backwards/forwards. If null the entity will as normal
    /// turn to face the direction of the movement button pressed in relation
    /// to the way the camera is facing. 
    /// </summary>
    public GameObject lockOnTarget;

    /// <summary>
    /// The movement force when locking on to a target
    /// </summary>
    public float lockOnMoveForce = 9f;

    /// <summary>
    /// If enabled the script will integrate with ABC setting the same target as ABC
    /// </summary>
    public bool enableABCTargetIntegration = false;

    /// <summary>
    /// If configured the entity will temporarily not be locked on to the entity 
    /// </summary>
    public KeyCode tempLockOffButton = KeyCode.None;

    /// <summary>
    /// The amount of time to not be locked on to the entity before being locked back on
    /// </summary>
    public float tempLockOffDuration = 1f; 


    [Header("Jump and Gravity")]

    /// <summary>
    /// If enabled then the entity can jump
    /// </summary>
    public bool allowJumping = true; 

    /// <summary>
    /// Determines how far the entity will jump
    /// </summary>
    public float jumpForce = 8f;

    /// <summary>
    /// If enable then gravity will be applied to the entity
    /// </summary>
    public bool allowGravity = true;

    /// <summary>
    /// Determines the gravity applied to the entity
    /// </summary>
    public float gravity = 2f;


    [Header("Rotation")]
    /// <summary>
    /// If enabled then the entity is allowed to rotate
    /// </summary>
    public bool allowRotation = true;

    /// <summary>
    /// Determiens the speed of rotation, the lower the number the longer it takes
    /// </summary>
    public float rotationSpeed = 0.8f;
  
    /// <summary>
    /// Determines how much input is needed by the user before the rotation starts
    /// </summary>
    public float rotationDrag = 0f;


    [Header("Animation")]
    /// <summary>
    /// If enabled then motion is turned off allowing movement to come from animation and only rotation will occur
    /// </summary>
    public bool rootMotionMode = false;

    /// <summary>
    /// Name of the normal movement animation to play
    /// </summary>
    public string animationParameter;

    /// <summary>
    /// Name of the side movement animation to play
    /// </summary>
    public string lockOnSideAniParameter = "SideStep";

    /// <summary>
    /// Name of the front animation to play
    /// </summary>
    public string lockOnForwardAniParameter = "FrontStep";


    #endregion


    // ********************* Private Properties ********************

    #region Private Properties

    /// <summary>
    /// Value which indicates how much vertical velocity is applied to the motion
    /// </summary>
    private float _verticalVelocity;

    /// <summary>
    /// Property which will work out the vertical velocity value to apply to the motion depending on if the user is grounded, jumping or falling
    /// </summary>
    private float VerticalVelocity {
        get {
            //If the entity is grounded
            if (charController.isGrounded == true) {
                if (Input.GetButton("Jump") && this.allowJumping == true)  // If jump key has been pressed to change vertical velocity to the jump force defined
                    _verticalVelocity = this.jumpForce;
                else
                    _verticalVelocity = 0; // else nothing is happening and the entity is grounded so no vertical velocity is applied
            } else {

                //If gravity is disabled then don't apply a gravity force
                if (this.allowGravity == false)
                    _verticalVelocity = 0;
                else 
                    _verticalVelocity -= this.gravity * Time.deltaTime; // else If the entity is not grounded and allow gravity is enabled then apply the gravity value to the vertical velocity
            }
               
            return _verticalVelocity;
        }
    }

    /// <summary>
    /// Records the current input X amount (Horizontal Axis)
    /// </summary>
    float inputX;

    /// <summary>
    /// Records the current Z amount (Vertical Axis)
    /// </summary>
    float inputZ;


    #endregion



    // ********************* Variables ********************
    #region Variables

    /// <summary>
    ///Global variable which indicates how much input force was applied by the user
    /// </summary>
    private float inputForce;

    /// <summary>
    /// Variable which tracks the current speed
    /// </summary>
    private float speedForce;

    /// <summary>
    /// Global variable which tracks which way the entity needs to move calculated from user input
    /// </summary>
    private Vector3 moveDirection;

    /// <summary>
    /// Main Camera
    /// </summary>
    private Camera Cam;

    /// <summary>
    /// Character Controller component attached to entity
    /// </summary>
    private CharacterController charController;

    /// <summary>
    /// Entities transform
    /// </summary>
    private Transform meTransform;

    /// <summary>
    /// Animator attached to entity
    /// </summary>
    private Animator Ani;


    /// <summary>
    /// Records the ABC SM Component if used for events
    /// </summary>
    private ABC_StateManager ABCEventsSM;

    /// <summary>
    /// Records the ABC controller component for events
    /// </summary>
    private ABC_Controller ABCEventsCont;

    #endregion


    // ********************* Private Methods ********************
    #region Private Methods

    /// <summary>
    /// Will check if the Button provided has been setup in the input manager. 
    /// </summary>
    /// <param name="InputName">Name of </param>
    /// <returns>True if input exists, else false.</returns>
    bool IsInputAvailable(string InputName)
    {
        try
        {
            Input.GetAxis(InputName);
            return true;
        }
        catch 
        {
            return false;
        }
    }


    /// <summary>
    /// Will intergate with ABC by retriving the component from the current follow target and then subscribing to it's movement and gravity events
    /// </summary>
    private void IntegrateWithABC() {


        this.ABCEventsSM = meTransform.GetComponentInChildren<ABC_StateManager>();
        this.ABCEventsCont = meTransform.GetComponentInChildren<ABC_Controller>();

        //subscribe to the events
        if (this.ABCEventsSM != null && this.enableABCIntegration == true) {
            this.ABCEventsSM.onEnableMovement += this.EnableMovement;
            this.ABCEventsSM.onDisableMovement += this.DisableMovement;

            this.ABCEventsSM.onEnableGravity += this.EnableGravity;
            this.ABCEventsSM.onDisableGravity += this.DisableGravity;
        }


        if (this.ABCEventsCont != null && this.enableABCTargetIntegration){
            this.ABCEventsCont.onTargetSet += this.LockOnTarget;
        }

    }

    /// <summary>
    /// Will wait for a duration before enabling or disabling movement
    /// </summary>
    /// <param name="AllowMovement">True if movement is enabled, else false</param>
    /// <param name="Duration">Duration to wait before movement is enabled or disabled</param>
    private IEnumerator ToggleMovement(bool AllowMovement, float Duration = 0f) {

        //if already enabled or disabled then return
        if (this.allowMovement == AllowMovement)
            yield break;


        if (Duration > 0f)
        yield return new WaitForSeconds(Duration);

        this.allowMovement = AllowMovement; 


    }



    /// <summary>
    /// Will enable/disable gravity
    /// </summary>
    /// <param name="AllowGravity">True if gravity is enabled, else false</param>
    private IEnumerator ToggleGravity(bool AllowGravity)
    {

        //if already enabled or disabled then return
        if (this.allowGravity == AllowGravity)
            yield break;


        this.allowGravity = AllowGravity;

    }


    /// <summary>
    /// Will rotate the entity by the direction and speed provided, as long as the input force is greater then the rotation drag setup
    /// </summary>
    /// <param name="Direction">Vector3 to rotate the entity in</param>
    /// <param name="RotationSpeed">Float indicating the speed of rotation</param>
    /// <param name="InputForce">The input force applied by the user, needs to be greater then the rotation drag for the rotation to happen</param>
    private void Rotate(Vector3 Direction, float RotationSpeed, float InputForce) {

        //If we are not allowed to rotate or the input force is not greater then the setup drag then return here
        if (InputForce <= this.rotationDrag || this.allowRotation == false)
            return;

        // else rotate 
        meTransform.rotation = Quaternion.Slerp(meTransform.rotation, Quaternion.LookRotation(Direction), RotationSpeed);
    }

    /// <summary>
    /// Will move the entity using the character controller component. 
    /// </summary>
    /// <param name="Motion">Vector3 of the movement to apply</param>
    private void Move(Vector3 Motion) {

        //If root motion mode is on then reset any X and Y coords as this is done by the animation
        if (this.rootMotionMode == true) {
            Motion.x = 0f;
            Motion.z = 0f;
        }



        if (this.charController != null && this.charController.enabled == true)
        charController.Move(Motion * Time.deltaTime);

    }

    /// <summary>
    /// Determines the move direction depending on the axis input made by the user and the direction of the camera
    /// </summary>
    private void DetermineMoveDirection() {

        //Work out the current speed force
        this.speedForce = this.moveForce;

        if (this.sprintButton != string.Empty && this.IsInputAvailable(this.sprintButton) && Input.GetButton(this.sprintButton))
            this.speedForce = this.sprintForce;
        else if (Input.GetKey(KeyCode.LeftShift))
            this.speedForce = this.sprintForce;


        //Get input from user
        this.inputX = Input.GetAxis("Horizontal") * this.speedForce;
        this. inputZ = Input.GetAxis("Vertical") * this.speedForce;

        //Retrieve input force to be used by animations and rotation checks later
        this.inputForce = new Vector2(inputX, inputZ).sqrMagnitude;

        //record the direction of the camera
        Vector3 camRight = Cam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 camForward = Cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        //Work out move direction using the camera direction and the input from user
        this.moveDirection = camRight * inputX + camForward * inputZ;

    }


    /// <summary>
    /// Main method to rotate and move the entity
    /// </summary>
    private void RotateAndMoveEntity() {

        //Call lock off handler 
        StartCoroutine(this.TempLockOffHandler());

        // Only rotate the entity to face the direction we want to travel if we have not got a lock on target
        // else turn the entity to face lock on target
        if (this.lockOnTarget == null)
            this.Rotate(this.moveDirection, this.rotationSpeed, this.inputForce);
        else
            this.meTransform.LookAt(lockOnTarget.transform.position);

        this.moveDirection.y = this.VerticalVelocity;

        this.Move(this.moveDirection);



    }


    /// <summary>
    /// Will animate the entity using settings and the input force
    /// </summary>
    private void Animate() {

        //If no animator has been setup then return here
        if (Ani == null)
            return;

        //If we don't have a lock on target then just use the normal movement animation
        if (lockOnTarget == null){
            //reset lock on animations
            if (this.useLockOnMovement){
                Ani.SetFloat(this.lockOnSideAniParameter, 0, 0.0f, Time.deltaTime * 2f);
                Ani.SetFloat(this.lockOnForwardAniParameter, 0, 0.0f, Time.deltaTime * 2f);
            }

            Ani.SetFloat(this.animationParameter, this.inputForce, 0.0f, Time.deltaTime * 2f);
        } else {
            //reset normal animation
            Ani.SetFloat(this.animationParameter, 0, 0.0f, Time.deltaTime * 2f);

            if (this.useLockOnMovement)  {
                // else if forward (Z axis) is being pressed then stop side animations and use forward animation
                if (this.inputZ != 0){
                    Ani.SetFloat(this.lockOnSideAniParameter, 0, 0.0f, Time.deltaTime * 2f);
                    Ani.SetFloat(this.lockOnForwardAniParameter, this.inputForce * this.inputZ, 0.0f, Time.deltaTime * 2f);
                }else{ // else side (X axis) is being pressed so stop forward animation and use sideways animation
                    Ani.SetFloat(this.lockOnForwardAniParameter, 0, 0.0f, Time.deltaTime * 2f);
                    Ani.SetFloat(this.lockOnSideAniParameter, this.inputForce * this.inputX, 0.0f, Time.deltaTime * 2f);
                }
            }
        }

    }

 
    /// <summary>
    /// Will handle the temporarily lock off so if a configured button is pressed
    /// the entity will for a duration not be locked on to the current target 
    /// after the duration the entity will lock on to the target again
    /// </summary>
    private IEnumerator TempLockOffHandler(){

        // if temp lock off button is pressed then temporarily unlock on the target 
        if (Input.GetKey(this.tempLockOffButton)){

            //store current target 
            GameObject currentLockOnTarget = this.lockOnTarget;

            //turn off lock on
            this.lockOnTarget = null;

            //wait the duration
            yield return new WaitForSeconds(this.tempLockOffDuration);

            //If a new target hasn't been selected then lock back on
            if (this.lockOnTarget == null)
                this.lockOnTarget = currentLockOnTarget;
        }

    }


    #endregion

    // ********************* Public Methods ********************
    #region Public Methods

    /// <summary>
    /// Will enable movement and rotation
    /// </summary>
    public void EnableMovement() {


        //Stop any current toggle movement calls incase one is mid delay
        StopCoroutine("ToggleMovement");

        //Enable movement after delay
        StartCoroutine(this.ToggleMovement(true, this.enableMovementDelay));

    }


    /// <summary>
    /// Will stop movement and rotation
    /// </summary>
    public void DisableMovement() {

        //Stop any current toggle movement calls incase one is mid delay
        StopCoroutine("ToggleMovement");

        //Disable movement without delay
        StartCoroutine(this.ToggleMovement(false, this.disableMovementDelay));
    }


    /// <summary>
    /// Will enable Gravity
    /// </summary>
    public void EnableGravity(){

        //Stop any current toggle movement calls incase one is mid delay
        StopCoroutine("ToggleGravity");

        //Enable movement after delay
        StartCoroutine(this.ToggleGravity(true));

    }


    /// <summary>
    /// Will disable gravity
    /// </summary>
    public void DisableGravity(){

        //Stop any current toggle movement calls incase one is mid delay
        StopCoroutine("ToggleGravity");

        //Enable movement after delay
        StartCoroutine(this.ToggleGravity(false));
    }


    /// <summary>
    /// Will lock on to the target gameobject
    /// </summary>
    /// <param name="Target">Object to lock on too</param>
    public void LockOnTarget(GameObject Target){
        this.lockOnTarget = Target;
    }

    #endregion


    // ********************** Game ******************

    #region Game

    void Start() {


        //Delcare animator
        Ani = this.GetComponentInChildren<Animator>();

   
        //Retrieve main camera
        Cam = Camera.main;

        //Determine character controller
        charController = this.GetComponentInChildren<CharacterController>();


   

    }

    private void OnEnable() {
        //Record entity transform
        meTransform = this.transform;
        this.IntegrateWithABC();
    }


    void Update() {

        //If movement is not allowed then end here
        if (allowMovement == false) 
            return;


        //Determine input and direction
        this.DetermineMoveDirection();

        //Rotate and move entity
        this.RotateAndMoveEntity();

        //Run animations
        this.Animate();

    }



    private void OnDisable() {

        if (this.ABCEventsSM != null) {
            this.ABCEventsSM.onEnableMovement -= this.EnableMovement;
            this.ABCEventsSM.onDisableMovement -= this.DisableMovement;

            this.ABCEventsSM.onEnableGravity -= this.EnableGravity;
            this.ABCEventsSM.onDisableGravity -= this.DisableGravity;
        }

        if (this.ABCEventsCont != null){
            this.ABCEventsCont.onTargetSet -= this.LockOnTarget;
        }
    }


    #endregion
}
