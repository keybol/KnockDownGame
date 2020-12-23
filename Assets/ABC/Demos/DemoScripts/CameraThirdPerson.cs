using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour {


    // ********************* Settings ********************
    #region Settings

    [Header("Camera Distance")]

    /// <summary>
    /// tracks the camera desired distance from the parent entity (Camera holder)
    /// </summary>
    public float desiredDistance = 5f;


    /// <summary>
    /// The minimum distance the camera can be to the parent (Camera holder)
    /// </summary>
    public float minCameraDistance = 3f;

    /// <summary>
    /// The maximum distance the camera can be from the parent entity (Camera holder)
    /// </summary>
    public float maxCameraDistance = 6f;


    [Header("Distance Transition")]

    /// <summary>
    /// The smooth amount to apply when changing camera distance
    /// </summary>
    public float smooth = 5f;


    [Header("Camera Collision")]

    /// <summary>
    /// Objects with the following tags will not setoff camera collision (moving camera closer)
    /// </summary>
    public List<string> IgnoreCollisionTags = new List<string>();

    [Header("Zoom In")]

    /// <summary>
    /// tracks the camera desired distance from the parent entity (Camera holder) when zooming in
    /// </summary>
    public float zoomDesiredDistance = 5f;

    /// <summary>
    /// The key to start zooming in to the desire distance
    /// </summary>
    public KeyCode zoomKey = KeyCode.None;

    #endregion



    // ********************* Variables ********************
    #region Variables

    /// <summary>
    /// Camera direction 
    /// </summary>
    private Vector3 cameraDirection;

    /// <summary>
    /// tracks the cameras current distance from the parent entity (Camera holder), this can change depending on camera collision
    /// </summary>
    private float currentDistance = 2.9f;


    #endregion


    // ********************* Private Methods ********************
    #region Private Methods

    
    /// <summary>
    /// Handles and calculates the desired distance depending on user input and max and min settings
    /// </summary>
    private void DesiredDistanceHandler() {

        //If user scrolls the mouse wheel then change the desired distance 
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            desiredDistance -= 1;

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            desiredDistance += 1;

        //Clamp the desired distance so it never goes over the max or min camera distance
        desiredDistance = Mathf.Clamp(this.desiredDistance, minCameraDistance, maxCameraDistance);

    }

    
    /// <summary>
    /// Will set the current camera distance depending on situations like wall collision
    /// </summary>
    private void SetCurrentDistance() {

        //Set the desired camera position
        Vector3 desiredCameraPos = transform.parent.TransformPoint(cameraDirection * desiredDistance);
        RaycastHit hit;


        //If zoom in is being pressed then set current distance to the zoom in desired distance, else set normal desired distance
        if (Input.GetKey(this.zoomKey)){
            currentDistance = this.zoomDesiredDistance;
        }else{

            //Line cast to check for collision
            if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit)){

                //If we are not ignoring the object we just collided then change distance
                if (this.IgnoreCollisionTags.Contains(hit.transform.tag) == false) {
                    currentDistance = (hit.distance * 0.57f); // collision has occured so move camera to the hit distance 
                    return; //return here as we don't want to clamp 
                } else { 
                    currentDistance = this.desiredDistance; // else no collision so set to desired
                }

            }else{
                currentDistance = this.desiredDistance; // no collision so current distance can be set at the desired distance
            }

            //Clamp the current distance so its within the min and max setting
            currentDistance = Mathf.Clamp(this.currentDistance, minCameraDistance, maxCameraDistance);
        }
    }

    /// <summary>
    /// Will lerp the camera into the correct current distance 
    /// </summary>
    private void MoveCamera() {

        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraDirection * currentDistance, Time.deltaTime * smooth);

    }


    #endregion

    // ********************* Public Methods ********************
    #region Public Methods


    #endregion


    // ********************** Game ******************

    #region Game

    void Awake() {

        //Retrieve current camera direction and distance
        this.cameraDirection = transform.localPosition.normalized;

        this.currentDistance = this.desiredDistance;
    }


    void Update() {

        //Handle any desired distance changes
        this.DesiredDistanceHandler();

        //Set the current distance
        this.SetCurrentDistance();
  
        //Move camera to the current distance 
        this.MoveCamera();

    }




    #endregion
}
