using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFirstPerson : MonoBehaviour {


    // ********************* Settings ********************
    #region Settings

    [Header("Component Settings")]


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


    [Header("Rotation")]

    /// <summary>
    /// The input sensitivity, higher the number the faster the camera rotation
    /// </summary>
    public float inputSensitivity = 5f;

    /// <summary>
    /// The angle restriction, the higher the number the more the camera will rotate up and down 
    /// </summary>
    public float clampAngle = 20f;

    /// <summary>
    /// If true then the Y Axis will be reverted (can swap between up rotating up and up rotating down etc)
    /// </summary>
    public bool invertYAxis = false;

    /// <summary>
    /// Determines if the mouse is locked and hidden
    /// </summary>
    public bool hideLockMouse = true;

    [Header("Movement")]

    /// <summary>
    /// Determines if movement and rotation is allowed
    /// </summary>
    public bool allowMovement = true;

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
            } else  {

                //If gravity is disabled then don't apply a gravity force
                if (this.allowGravity == false)
                    _verticalVelocity = 0;
                else
                    _verticalVelocity -= this.gravity * Time.deltaTime; // else If the entity is not grounded and allow gravity is enabled then apply the gravity value to the vertical velocity
            }

            return _verticalVelocity;
        }
    }
    #endregion


    // ********************* Variables ********************
    #region Variables



    /// <summary>
    /// Main Camera
    /// </summary>
    private Transform meTransform;

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
    /// Tracks the current X rotation so this can be modified via input
    /// </summary>
    private float currentXRot = 0;


    /// <summary>
    /// Tracks the current Y rotation so this can be modified via input
    /// </summary>
    private float currentYRot = 0;

    /// <summary>
    /// Character Controller component attached to entity
    /// </summary>
    private CharacterController charController;

    /// <summary>
    /// Records the ABC Component if used
    /// </summary>
    private ABC_StateManager ABCEvents;

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
    /// Will intergate with ABC by retriving the component from the current follow target and then subscribing to it's on movement and gravity events
    /// </summary>
    private void IntegrateWithABC() {

        if (this.enableABCIntegration == false)
            return;


        this.ABCEvents = meTransform.GetComponentInChildren<ABC_StateManager>();

        //subscribe to the event 
        if (this.ABCEvents != null) {
            this.ABCEvents.onEnableMovement += this.EnableMovement;
            this.ABCEvents.onDisableMovement += this.DisableMovement;


            this.ABCEvents.onEnableGravity += this.EnableGravity;
            this.ABCEvents.onDisableGravity += this.DisableGravity;
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
    /// Will rotate the entity depending on user input
    /// </summary>
    private void Rotate() {

        //Record mouse y now so we can invert it if set too
        float mouseY = -Input.GetAxis("Mouse Y");
        //If set to invert the y axis then turn the positive to a negative or vice versa
        if (this.invertYAxis)
            mouseY = mouseY * -1;


        //Retrieve both the controller and mouse inputs and add them together to get the axis changes (quick way to determine which is being used)
        this.currentXRot += ((this.IsInputAvailable("RightStickVertical") ? Input.GetAxis("RightStickVertical") : 0) + mouseY) * this.inputSensitivity;
        this.currentYRot += ((this.IsInputAvailable("RightStickHorizontal") ? Input.GetAxis("RightStickVertical") : 0) + Input.GetAxis("Mouse X")) * this.inputSensitivity;


        //Apply the angle clamp on the X axis
        this.currentXRot = Mathf.Clamp(currentXRot, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(this.currentXRot, this.currentYRot, 0f);
        transform.rotation = localRotation;


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
        float inputX = Input.GetAxis("Horizontal") * this.speedForce;
        float inputZ = Input.GetAxis("Vertical") * this.speedForce;

        //Retrieve input force to be used by animations and rotation checks later
        this.inputForce = new Vector2(inputX, inputZ).sqrMagnitude;

        //record the direction of the entity
        Vector3 entityRight = this.meTransform.right;
        entityRight.y = 0f;
        entityRight.Normalize();

        Vector3 entityForward = this.meTransform.forward;
        entityForward.y = 0f;
        entityForward.Normalize();

        //Work out move direction using the camera direction and the input from user
        this.moveDirection = entityRight * inputX + entityForward * inputZ;

        //Apply any gravity/jump force
        this.moveDirection.y = this.VerticalVelocity;


    }

    /// <summary>
    /// Will move the entity using the character controller component. 
    /// </summary>
    /// <param name="Motion">Vector3 of the movement to apply</param>
    private void Move() {

        //Determine move direction
        this.DetermineMoveDirection();

        // move entity
        if (this.charController != null && this.charController.enabled == true)
            charController.Move(this.moveDirection * Time.fixedDeltaTime);

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


    #endregion


    // ********************** Game ******************

    #region Game

    private void OnEnable() {
        //Hide and lock mouse
        if (this.hideLockMouse) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //Record transform
        this.meTransform = this.transform;

        this.IntegrateWithABC();
    }

    void Start() {

 

        //Retrieve starting rotation
        Vector3 rot = meTransform.localRotation.eulerAngles;
        currentYRot = rot.y;
        currentXRot = rot.x;


        //Determine character controller
        charController = this.GetComponentInChildren<CharacterController>();

    }


    void LateUpdate() {

        //Rotate entity
        this.Rotate();

        //If movement is not allowed then end here
        if (allowMovement == false)
            return;

        this.Move();


    }


    private void OnDisable() {

        if (this.ABCEvents != null) {
            this.ABCEvents.onEnableMovement -= this.EnableMovement;
            this.ABCEvents.onDisableMovement -= this.DisableMovement;

            this.ABCEvents.onEnableGravity -= this.EnableGravity;
            this.ABCEvents.onDisableGravity -= this.DisableGravity;
        }

    }



    #endregion
}
