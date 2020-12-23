using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Component to switch who the user is playing as, will turn off and on certain components and change camera
/// </summary>
[System.Serializable]
public class PlayerSwitcher : MonoBehaviour {

    /// <summary>
    /// Singleton instance so switchercan be accessed by any part of the game
    /// </summary>
    public static PlayerSwitcher instance = null;

    #region SwitchEntity Class
    [System.Serializable]
    public class SwitchEntity {


        // ************ Settings *****************************

        #region Settings

        /// <summary>
        /// Used for inspector only to toggle the fold out
        /// </summary>
        public bool foldOut = true;

        /// <summary>
        /// Name of the switch entity
        /// </summary>
        public string switchEntityName = "New Switch Entity";

        /// <summary>
        /// Unique ID of the switch entity
        /// </summary>
        public int switchID = 0; 

        /// <summary>
        /// Entity which will be switched too
        /// </summary>
        public GameObject entityObject;

        /// <summary>
        /// If true then instead of deactivating ABC Controllers the AI is turned on instead
        /// </summary>
        public bool enableABCAIOnDisable = false;

        /// <summary>
        /// Components to enable when entity is active
        /// </summary>
        public List<string> enabledComponentsOnActivate = new List<string>();

        /// <summary>
        /// Components to enable when entity is active
        /// </summary>
        public List<string> disabledComponentsOnActivate = new List<string>();

        /// <summary>
        /// Components to disable when entity is active
        /// </summary>
        public List<string> enabledComponentsOnDeactivate = new List<string>();

        /// <summary>
        /// Components to disable when entity is active
        /// </summary>
        public List<string> disabledComponentsOnDeactivate = new List<string>();

        /// <summary>
        /// Components to enable when entity is active
        /// </summary>
        public List<GameObject> enabledObjectsOnActivate = new List<GameObject>();

        /// <summary>
        /// Components to enable when entity is active
        /// </summary>
        public List<GameObject> disabledObjectsOnActivate = new List<GameObject>();

        /// <summary>
        /// Components to disable when entity is active
        /// </summary>
        public List<GameObject> enabledObjectsOnDeactivate = new List<GameObject>();

        /// <summary>
        /// Components to disable when entity is active
        /// </summary>
        public List<GameObject> disabledObjectsOnDeactivate = new List<GameObject>();

        #endregion

        // ********************* Private Methods ********************

        #region Private Methods

        /// <summary>
        /// ABC Integration - Will toggle the ABC AI on/off
        /// </summary>
        /// <param name="Enabled">True to turn AI on, else false</param>
         private void ToggleABCAI(bool Enabled = true) {

            //Retrieve/Create IEntity interface
            ABC_IEntity abcEntity = ABC_Utilities.GetStaticABCEntity(entityObject);

            //Toggle AI restricting ability triggers
            abcEntity.ToggleAI(Enabled);

            if (Enabled == false) // reset target 
                abcEntity.RemoveTarget();

        }

        /// <summary>
        /// Will find and toggle of/on the component
        /// </summary>
        /// <param name="ComponentName">Component to toggle</param>
        /// <param name="Enabled">If true component will be enabled, else false will disable</param>
        private void ToggleComponent(string ComponentName, bool Enabled) {

            switch (ComponentName) {
                case "NavMeshAgent":
                    NavMeshAgent agent = entityObject.GetComponent<NavMeshAgent>();

                    if (agent != null)
                    agent.enabled = Enabled;

                    break;
                default:

                     var component = entityObject.GetComponent(ComponentName) as MonoBehaviour;

                    if (component != null)
                        component.enabled = Enabled;

                    break; 

            }


        }

        #endregion


        // ********************* Public Methods ********************

        #region Public Methods

        /// <summary>
        /// Will switch to this entity 'Activating it', enabling and disabling components and objects set on Activate
        /// </summary>
        public void SwitchToEntity() {

            //Cycle through and enable components defined to be active 'OnActivate'
            foreach (string comp in this.enabledComponentsOnActivate) {


                //If we are toggling ABC AI rather then enabling/disabling then turn off AI as the main Component is active and useable
                if (this.enableABCAIOnDisable && comp == "ABC_Controller") {
                    this.ToggleABCAI(false);
                    continue;
                }

                this.ToggleComponent(comp, true);
            }

            //Cycle through and enable objects defined to be active 'OnActivate'
            foreach (GameObject obj in this.enabledObjectsOnActivate) {

                if (obj != null)
                    obj.SetActive(true);

            }

            //Cycle through and disable components defined to be disabled 'OnActivated'
            foreach (string comp in this.disabledComponentsOnActivate) {

                //If we are toggling ABC AI rather then enabling/disabling then turn on AI as the main Component is unactive and not useable
                if (this.enableABCAIOnDisable && comp == "ABC_Controller") {
                    this.ToggleABCAI(true);
                    continue;
                }

                this.ToggleComponent(comp, false);

            }

            //Cycle through and disable objects defined to be disabled 'OnActivated'
            foreach (GameObject obj in this.disabledObjectsOnActivate) {

                if (obj != null)
                    obj.SetActive(false);

            }

        }

        /// <summary>
        /// Will switch from this entity 'Deactivating it', enabling and disabling components and objects set on Deactivate
        /// </summary>
        public void SwitchFromEntity() {


            //Cycle through and enable components defined to be active 'OnDeactivated'
            foreach (string comp in this.enabledComponentsOnDeactivate) {

                //If we are toggling ABC AI rather then enabling/disabling then turn off AI as the main Component is active and useable
                if (this.enableABCAIOnDisable && comp == "ABC_Controller") {
                    this.ToggleABCAI(false);
                    continue;
                }

                this.ToggleComponent(comp, true);

            }

            //Cycle through and enable objects defined to be active 'OnDeactivated'
            foreach (GameObject obj in this.enabledObjectsOnDeactivate) {

                if (obj != null)
                    obj.SetActive(true);

            }


            //Cycle through and disable components defined to be disabled 'OnDeactivated'
            foreach (string comp in this.disabledComponentsOnDeactivate) {

                //If we are toggling ABC AI rather then enabling/disabling then turn on AI as the main Component is unactive and not useable
                if (this.enableABCAIOnDisable && comp == "ABC_Controller") {
                    this.ToggleABCAI(true);
                    continue;
                }

                this.ToggleComponent(comp, false);

            }

            //Cycle through and disable objects defined to be disabled 'OnDeactivated'
            foreach (GameObject obj in this.disabledObjectsOnDeactivate) {

                if (obj != null)
                    obj.SetActive(false);

            }

        }




        #endregion

    }
#endregion

    // ************ Settings *****************************

    #region Settings

    /// <summary>
    /// The main camera in the game which follows the player
    /// </summary>
    public GameObject playerCamera;

    /// <summary>
    /// List of entities that can be switched too
    /// </summary>
    public List<SwitchEntity> switchEntities = new List<SwitchEntity>();




    #endregion

    // ********************* Variables ******************

    #region Variables

    private SwitchEntity currentPlayer;

    #endregion



    // ********************* Public Methods ********************

    #region Public Methods

    /// <summary>
    /// Will switch to the entity chosen by the ID provided
    /// </summary>
    /// <param name="SwitchEntityID">ID of the entity to switch too</param>
    public void SwitchToEntity(int SwitchEntityID) {

        //Find entity with the matching ID
        SwitchEntity switchEntity = this.switchEntities.Where(s => s.switchID == SwitchEntityID).FirstOrDefault();

        //If doesn't exist then return here
        if (switchEntity == null)
            return;

        //If we already have a current player we have previously switched too, then switch from that entity (enabling and disabling components/objects 'On deactivated')
        if (this.currentPlayer != null)
        this.currentPlayer.SwitchFromEntity();

        //Record the new current player
        this.currentPlayer = switchEntity;
        //Switch to the new current player
        this.currentPlayer.SwitchToEntity();
        //Swap camera focus to the new player
        this.SwapCameraFocus(this.currentPlayer);

    }

    /// <summary>
    /// Will switch to the entity chosen by the name provided
    /// </summary>
    /// <param name="SwitchEntityName">Name of the entity to switch too</param>
    public void SwitchToEntity(string SwitchEntityName) {

        //Find entity with the matching Name
        SwitchEntity switchEntity = this.switchEntities.Where(s => s.switchEntityName == SwitchEntityName).FirstOrDefault();

        if (switchEntity == null)
            return;

        //If doesn't exist then return here
        if (switchEntity == null)
            return;

        //If we already have a current player we have previously switched too, then switch from that entity (enabling and disabling components/objects 'On deactivated')
        if (this.currentPlayer != null)
            this.currentPlayer.SwitchFromEntity();

        //Record the new current player
        this.currentPlayer = switchEntity;
        //Switch to the new current player
        this.currentPlayer.SwitchToEntity();
        //Swap camera focus to the new player
        this.SwapCameraFocus(this.currentPlayer);

    }




    #endregion


    // ********************* Private Methods ********************

    #region Private Methods

    /// <summary>
    /// Will setup the component switching to the first entity setup in the list
    /// </summary>
    private void Setup() {

        //If no entities setup then end here
        if (this.switchEntities.Count == 0)
            return; 

        //Make sure all entities are deactivated to start with to make sure the correct components and objects are enabled/disabled
        foreach (SwitchEntity switchEntity in this.switchEntities) {
            switchEntity.SwitchFromEntity();
        }

        //Switch to the first entity in the list
        this.SwitchToEntity(this.switchEntities.First().switchID);


    }

    /// <summary>
    /// Function will swap the camera focus to the entity provided
    /// </summary>
    /// <param name="SwitchEntity">Entity to swap the camera focus too</param>
    private void SwapCameraFocus(SwitchEntity SwitchEntity) {

        //If a shared camera isn't used end here
        if (this.playerCamera == null)
            return;

        //Find the camera script attached to the camera object
        CameraBaseThirdPerson cameraComp = this.playerCamera.GetComponent<CameraBaseThirdPerson>();

        //If not found end here
        if (cameraComp == null)
            return;

        //Set the follow target of the camera to the entity provided
        cameraComp.SetFollowTarget(SwitchEntity.entityObject);

    }




    #endregion


    // ********************* Game ********************

    #region Game

    private void Awake() {

        //Declare singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Make sure this singleton is accessable all over the game
        DontDestroyOnLoad(gameObject);
    }


    // Use this for initialization
    void Start() {

        //Run setup 
        this.Setup();
    }


    #endregion
}
