using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// will disable standard movement components on the entity (navagent, character controller, rigidbody etc)
/// </summary>
public class ABC_DisableMovementComponents : MonoBehaviour {


    // ********************* Settings ********************
    #region Settings

    /// <summary>
    /// If true then component will be removed from entity when disabled
    /// </summary>
    public bool removeComponentOnDisable = true;

    /// <summary>
    /// If true then the velocity of the rigidbody will remain at 0, halting movement without disabling the component (so it can still work out collisions etc)
    /// </summary>
    public bool haltRigidbody = true;

    /// <summary>
    /// If true then any nav agent components on the entity will be disabled
    /// </summary>
    public bool haltNavAgent = true;

    /// <summary>
    /// If true then any character controller components on the entity will be disabled
    /// </summary>
    public bool disableCharacterController = true;



    #endregion



    //************************ Variables / Private Properties ****************************************

    #region Variables / Private Properties

    /// <summary>
    /// Transform of the entity 
    /// </summary>
    private Transform meTransform;

    /// <summary>
    /// Rigidbody attached to the entity
    /// </summary>
    private Rigidbody meRigidbody;

    /// <summary>
    /// Nav Agent attached to entity
    /// </summary>
    private NavMeshAgent navAgent;

    /// <summary>
    /// CharacterController attached to entity
    /// </summary>
    private CharacterController charController;

    #endregion

    // ********************* Private Methods ********************

    #region Private Methods

    /// <summary>
    /// Will halt the rigidbody keeping velocity at 0
    /// </summary>
    private void HaltRigidbody() {

        if (this.haltRigidbody == false)
            return;

        //If a rigidbody exists then stop velocity
        if (this.meRigidbody != null) {
            this.meRigidbody.velocity = Vector3.zero;
            this.meRigidbody.angularVelocity = Vector3.zero;
        }

    }



    /// <summary>
    /// Will enable/disable the NavAgent 
    /// </summary>
    private void HaltNavAgent() {

        if (this.navAgent == null)
            return;

        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;
    }

    /// <summary>
    /// Will enable/disable the Character Controller 
    /// </summary>
    /// <param name="Enabled">True to enable NavAgent, else false to disable</param>
    private void ToggleCharacterController(bool Enabled) {

        if (this.charController == null)
            return;


        this.charController.enabled = Enabled;

    }



    #endregion


    //************************ Game ****************************************

    #region Variables / Private Properties


    // Use this for initialization
    void OnEnable () {

        //Record all compoenents 
        this.meTransform = transform;
        this.meRigidbody = meTransform.GetComponentInChildren<Rigidbody>(true);
        this.navAgent = meTransform.GetComponent<NavMeshAgent>();
        this.charController = meTransform.GetComponent<CharacterController>();

        //Disable all components
        this.ToggleCharacterController(false);
	}
	
	// Update is called once per frame
	void Update () {

        //Freeze Rigidbody
        this.HaltRigidbody();

        //Freeze NavAgent
        this.HaltNavAgent();

	}

    private void OnDisable() {

        //Enable all components again        
        this.ToggleCharacterController(true);

        //If setup too then remove component
        if (this.removeComponentOnDisable)
            Destroy(this);
    }

    #endregion
}
