using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Freezes the entity by keeping transform position the same or keeping rigidbody velocity at 0
/// </summary>
public class ABC_FreezePosition : MonoBehaviour {


    // ********************* Settings ********************
    #region Settings

    /// <summary>
    /// If true then component will be removed from entity when disabled
    /// </summary>
    public bool removeComponentOnDisable = true;


    /// <summary>
    /// If true then the entities position will always remain the same as where it started, freezing the entity in position
    /// </summary>
    public bool enableFreezePosition = true;

    #endregion



    //************************ Variables / Private Properties ****************************************

    #region Variables / Private Properties

    /// <summary>
    /// Transform of the entity 
    /// </summary>
    private Transform meTransform;


    /// <summary>
    /// The Vector3 position to freeze the entity too
    /// </summary>
    private Vector3 freezePosition;


    #endregion

    // ********************* Private Methods ********************

    #region Private Methods
    
    /// <summary>
    /// Will keep the entity at the position recorded when the component was enabled
    /// </summary>
    private void FreezePosition() {

        if (this.enableFreezePosition == false)
            return; 

        //Keep entity position at the frozen transform
        this.meTransform.position = this.freezePosition;

    }

  

    #endregion


    //************************ Game ****************************************

    #region Variables / Private Properties


    // Use this for initialization
    void OnEnable () {
        this.meTransform = transform;
        this.freezePosition = transform.position;	
	}
	
	// Update is called once per frame
	void Update () {

        //Freeze position
        this.FreezePosition();

	}

    private void OnDisable() {

        //If setup too then remove component
        if (this.removeComponentOnDisable)
            Destroy(this);
    }

    #endregion
}
