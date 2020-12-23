using UnityEngine; 

public class CameraBaseThirdPerson : MonoBehaviour {

    // ********************* Settings ********************
    #region Settings


    [Header("Camera Settings")]

    /// <summary>
    /// The speed in which the camera will follow the entity
    /// </summary>
    public float cameraFollowSpeed = 120f;

    /// <summary>
    /// The entity which the camera will follow
    /// </summary>
    public GameObject followTarget;

    /// <summary>
    /// The offset of the follow target where the camera will be placed
    /// </summary>
    public Vector3 followTargetOffset; 

    /// <summary>
    /// The input sensitivity, higher the number the faster the camera rotation
    /// </summary>
    public float inputSensitivity = 5f;

    /// <summary>
    /// The angle restriction, the higher the number the more the camera will rotate up and down 
    /// </summary>
    public float clampAngle = 20f;

    [Header("Zoom In Settings")]

    /// <summary>
    /// The entity which the camera will follow
    /// </summary>
    public GameObject zoomTarget;

    /// <summary>
    /// The offset of the follow target where the camera will be placed
    /// </summary>
    public Vector3 zoomTargetOffset;

    /// <summary>
    /// The key to start zooming in 
    /// </summary>
    public KeyCode zoomKey = KeyCode.None;


    [Header("Lock On Settings")]

    /// <summary>
    /// If assigned then the camera will always look at this object
    /// </summary>
    public GameObject lockOnTarget;

    /// <summary>
    /// The offset of the lock on target, which changes the position where the camera looks at 
    /// </summary>
    public Vector3 lockOnTargetOffset;


    /// <summary>
    /// If true then the script will integrate with ABC by using the ABC target as the lock on target
    /// </summary>
    public bool enableABCIntegration; 


    [Header("Misc Settings")]

    /// <summary>
    /// Determines if the mouse is locked and hidden
    /// </summary>
    public bool hideLockMouse = true;


    #endregion



    // ********************* Variables ********************
    #region Variables


    /// <summary>
    /// Main Camera
    /// </summary>
    private Camera Cam;


    /// <summary>
    /// Main Camera
    /// </summary>
    private Transform meTransform;

    /// <summary>
    /// Tracks the current X rotation so this can be modified via input
    /// </summary>
    private float currentXRot = 0;


    /// <summary>
    /// Tracks the current Y rotation so this can be modified via input
    /// </summary>
    private float currentYRot = 0;

    /// <summary>
    /// Records the ABC Component if used
    /// </summary>
    private ABC_Controller followTargetABC;


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
    /// Will intergate with ABC by retriving the component from the current follow target and then subscribing to it's on target set event
    /// </summary>
    private void IntegrateWithABC() {

        if (this.enableABCIntegration == false || this.followTarget == null)
            return; 


        this.followTargetABC = followTarget.GetComponentInChildren<ABC_Controller>();

        //subscribe to the event 
        if (this.followTargetABC != null)
            this.followTargetABC.onTargetSet += this.SetLockOnTarget;   

    }
    

    /// <summary>
    /// Will rotate the camera depending on user input
    /// </summary>
    private void RotateCamera() {

        //Retrieve both the controller and mouse inputs and add them together to get the axis changes (quick way to determine which is being used)
        this.currentXRot += ((this.IsInputAvailable("RightStickVertical") ? Input.GetAxis("RightStickVertical") : 0) + Input.GetAxis("Mouse Y") * this.inputSensitivity); 
        this.currentYRot += ((this.IsInputAvailable("RightStickHorizontal") ? Input.GetAxis("RightStickVertical") : 0) + Input.GetAxis("Mouse X")) * this.inputSensitivity;
        
        //Apply the angle clamp on the X axis
        this.currentXRot = Mathf.Clamp(currentXRot, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(this.currentXRot, this.currentYRot, 0f);
        transform.rotation = localRotation;


    }

    /// <summary>
    /// Will move the camera towards the follow target
    /// </summary>
    private void MoveCamera() {

        //If no follow target exists then end here
        if (followTarget == null)
            return;

        //If zooming input is being pressed then move position to the zoom target instantly
        if (this.zoomTarget != null && Input.GetKey(this.zoomKey) == true){
            transform.position = this.zoomTarget.transform.position + this.zoomTargetOffset;
        } else {
            // else move towards are normal follow target over a duration determined by the camera follow speed
            float distance = cameraFollowSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, this.followTarget.transform.position + this.followTargetOffset, distance);
        }

    }


    /// <summary>
    /// Will lock on to an object by making sure the camera is always rotated to face the current target
    /// </summary>
    private void LockOnTargetHandler() {

        //If no lock on target exists then end here
        if (lockOnTarget == null)
            return;


        //Look at lock on target
        transform.LookAt(lockOnTarget.transform.position + lockOnTargetOffset);

    }





    #endregion

    // ********************* Public Methods ********************
    #region Public Methods

    /// <summary>
    /// Will follow the new object provided, setting up any ABC integrations
    /// </summary>
    /// <param name="NewFollowTarget">New Object to follow</param>
    public void SetFollowTarget(GameObject NewFollowTarget) {

        if (this.followTarget == NewFollowTarget)
            return;

        //Remove current follow target
        this.ClearFollowTarget();

        //Set the new follow target
        this.followTarget = NewFollowTarget;
        this.IntegrateWithABC();



    }

    /// <summary>
    /// Will clear the current follow target removing any event subscriptions to ABC
    /// </summary>
      public void ClearFollowTarget() {

            if (this.followTarget == null)
                return;

        this.followTarget = null;

        //Unsubscribe to any events
        if (this.followTargetABC != null)
            this.followTargetABC.onTargetSet -= this.SetLockOnTarget;


    }

    /// <summary>
    /// Sets the lock on target
    /// </summary>
    /// <param name="Target">Object to lock on too</param>
    public void SetLockOnTarget(GameObject NewLockOnTarget) {

        //If the target provided was null or not active then clear the lock on target
        if (NewLockOnTarget == null || NewLockOnTarget.activeInHierarchy == false)
            this.ClearLockOnTarget();


        this.lockOnTarget = NewLockOnTarget; 


    }

    /// <summary>
    /// Clears the lock on target
    /// </summary>
    public void ClearLockOnTarget() {

        if (this.lockOnTarget == null)
            return;


        //Look at lock on target
        transform.LookAt(lockOnTarget.transform);

        Vector3 rot = transform.localRotation.eulerAngles;
        currentYRot = rot.y;
        currentXRot = rot.x;

        //Clear target
        this.lockOnTarget = null;

    }


    #endregion


    // ********************** Game ******************

    #region Game

    void Start() {

        //Record transform
        this.meTransform = this.transform;

        //Retrieve starting rotation to place camera behind the follow target
        Vector3 rot = followTarget.transform.localRotation.eulerAngles;
        currentYRot = rot.y;
        currentXRot = rot.x;

        //Hide and lock mouse
        if (this.hideLockMouse) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }




    }

    private void OnEnable() {

        this.IntegrateWithABC();
    }


    void Update() {

        //Rotate Camera
        this.RotateCamera();

        //Lock on to any targets
        this.LockOnTargetHandler();

    }


    private void LateUpdate() {

        //Move camera
        MoveCamera();

    }


    private void OnDisable() {

        //Unsubscribe to any events
        if (this.followTargetABC != null)
        this.followTargetABC.onTargetSet -= this.SetLockOnTarget;

    }

    #endregion
}
