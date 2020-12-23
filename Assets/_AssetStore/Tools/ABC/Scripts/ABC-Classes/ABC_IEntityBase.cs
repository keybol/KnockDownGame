using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

#if ABC_GC_Stats_Integration
using GameCreator.Stats;
#endif

/// <summary>
/// A lightweight interface (not strictly typed as an interface) which holds information regarding entity objects using ABC. IEntity holds properties which are access by Abilities and Effects when performing actions like activating 
/// </summary>
/// <remarks>
/// All ABC components access this interface when dealing with non ABC Generated Objects (Entities) any property in this class can be changed as long as it returns the same type of value it expects. 
/// An example of this is if a user doesn't want to use StateManager for Health Management so they can plug in their own health component property here.
/// Then when ABC calls this interface to reduce health it will simply effect the new component. 
/// This also allows for different target systems to be used if desired. As long as these methods returns the same type of property then any of this class can be changed and it will still easily fit in with the rest of ABC.
/// 
/// Originator is a IEntity property which is used in many functions and generally means the entity which activates the ability. 
/// 
/// If you are making amendments it is recommend to override these methods on the ABC_IEntity class (which inherits this) so any future updates don't overwrite your changes: https://docs.microsoft.com/en-us/visualstudio/modeling/overriding-and-extending-the-generated-classes?view=vs-2019
/// </remarks>
[System.Serializable]
public class ABC_IEntityBase {

    //************************ Entity Components ****************************************


#region Entity Components

    /// <summary>
    /// Transform of the entity
    /// </summary>
    [NonSerialized]
    protected Transform _entityTransform;

    /// <summary>
    /// GameObject of the entity
    /// </summary>
    [NonSerialized]
    protected GameObject _entityGameObj;

    /// <summary>
    /// Rigidbody of the entity
    /// </summary>
    [NonSerialized]
    protected Rigidbody _entityRigidbody;

    /// <summary>
    /// ABC Controller component attached to the entity (includes mana, logs and target information)
    /// </summary>
    [NonSerialized]
    protected ABC_Controller _entityABC;

    /// <summary>
    /// ABC Statemanger component attached to the entity (handles effect activation, health)
    /// </summary>
    [NonSerialized]
    protected ABC_StateManager _entitySM;

    /// <summary>
    /// The ABC entity object from the inherited class
    /// </summary>
    [NonSerialized]
    protected ABC_IEntity _entity;

    /// <summary>
    /// The current main camera
    /// </summary>
    [NonSerialized]
    protected Camera _mainCamera;

    /// <summary>
    /// The animator for the entity 
    /// </summary>
    [NonSerialized]
    protected Animator _ani;

    /// <summary>
    /// The ABC Animation Runner for the entity (allows for animations without animator)
    /// </summary>
    [NonSerialized]
    protected ABC_AnimationRunner _aniRunner;


    #endregion

    //************************ Variables / protected Properties ****************************************

    #region Variables / protected Properties

    /// <summary>
    /// Will record the latest time that specific functions were called to stop any call overlaps 
    /// (i.e if toggle movement was re-disabled on a next attack, previous toggle will not continue as function has been overwritten)
    /// </summary>
    protected Dictionary<string, float> functionRequestTimeTracker = new Dictionary<string, float>();




    #endregion


    //************************ public virtual Properties  ****************************************

    #region public virtual Properties

    /// <summary>
    /// Constructor to make the entity object. Add other scripts here if required. 
    /// </summary>
    /// <param name="Obj">object to be created into an entity object</param>
    /// <param name="StaticTracking">If true then the entity object will be added to a global static dictionary which can be retrieved later, stopping the need to make another IEntity for this gameobject</param>
    public ABC_IEntityBase(GameObject Obj, bool StaticTracking = true) {


    }

    /// <summary>
    /// Will refresh the entity re-calling the setup method so the latest components are retrieved 
    /// </summary>
    public virtual void RefreshEntity() {

        this.SetupEntity(this.gameObject, this._entity);
    }
     

    /// <summary>
    /// Returns the entities GameObject
    /// </summary>
    public virtual GameObject gameObject {

        get {
            return _entityGameObj;
        }
    }



    /// <summary>
    /// Returns the main Camera
    /// </summary>
    public virtual Camera Camera {

        get {

            if (this._mainCamera == null) {
                if (this._entityABC == null)
                    _mainCamera = Camera.main;
                else
                    _mainCamera = this._entityABC.GetEntitiesCamera();
            }


            return _mainCamera;
        }

    }

    /// <summary>
    /// Returns the entities rigidbody
    /// </summary>
    public virtual Rigidbody rigidBody {
        get {
            return _entityRigidbody;
        }
    }

    /// <summary>
    /// Returns the entities transform
    /// </summary>
    public virtual Transform transform {

        get {
            return _entityTransform; 
        }
    }

    /// <summary>
    /// Returns the entities Animator
    /// </summary>
    public virtual Animator animator {
        get {
            return _ani;
        }

    }

    /// <summary>
    /// Returns the entities ABC animation runner component 
    /// </summary>
    public virtual ABC_AnimationRunner animationRunner {
        get {

            //If the entity doesn't have the animation runner component then add it 
            if (this.animator != null && this._ani.gameObject.GetComponent<ABC_AnimationRunner>() == null)
                this._aniRunner = this.animator.gameObject.AddComponent<ABC_AnimationRunner>();

            //If ani runner is null for any reason then reassign the component
            if (this._aniRunner == null && this.animator != null)
                this._aniRunner = this.animator.gameObject.GetComponent<ABC_AnimationRunner>();

            return this._aniRunner;
        }
    }

    /// <summary>
    /// How the entity is currently selecting targets
    /// </summary>
    public virtual TargetSelectType targetSelectType {
        get {
            return _entityABC.targetSelectType;
        }
    }

    /// <summary>
    /// returns current target of entity
    /// </summary>
    public virtual GameObject target {
        get {
            return _entityABC.GetCurrentTarget();
        }
    }

    /// <summary>
    /// returns current soft target of entity
    /// </summary>
    public virtual GameObject softTarget {
        get {
            return _entityABC.GetCurrentSoftTarget();
        }

    }


    /// <summary>
    /// Returns Current world target of the entity
    /// </summary>
    public virtual  GameObject worldTargetObj {

        get {
            return this._entityABC.worldTargetObject;
        }

    }

    /// <summary>
    /// Returns Current world target position of the entity
    /// </summary>
    public virtual Vector3 worldTargetPos {
        get {
            return this._entityABC.worldTargetPosition;
        }


    }

    /// <summary>
    /// Returns entites Max Health Value
    /// </summary>
    public virtual float maxHealthValue {
        get {
            return _entitySM.currentMaxHealth;
        }

    }

    /// <summary>
    /// Returns entities Current Health Value
    /// </summary>
    public virtual float healthValue {
        get {
            return _entitySM.currentHealth;
        }

    }

    /// <summary>
    /// Will return the % of melee damage mitigation the entity currently has
    /// </summary>
    /// <returns>Float value which determines the current melee damage mitigation</returns>
    public virtual float meleeDamageMitigationPercentage {
        get {
            return _entitySM.GetMeleeDamageMitigationPercentage();
        }

    }

    /// <summary>
    /// Will return the % of projectile and raycast damage mitigation the entity currently has
    /// </summary>
    /// <returns>Float value which determines the current projectile and raycast damage mitigation</returns>
    public virtual float projectileDamageMitigationPercentage {
        get {
            return _entitySM.GetProjectileAndRayCastDamageMitigationPercentage();
        }
    }



    /// <summary>
    /// Returns entities Max Mana Value
    /// </summary>
    public virtual float maxManaValue {

        get {
            return _entityABC.currentMaxMana;
        }

    }

    /// <summary>
    /// Returns entities current Mana value
    /// </summary>
    public virtual float manaValue {
        get {
            return _entityABC.currentMana;
        }

    }

    /// <summary>
    /// Return the mana regen value of the entity
    /// </summary>
    public virtual float manaRegenValue {
        get {
            return _entityABC.manaRegenPerSecond;
        }
    }


    /// <summary>
    ///  Current max block durability of the entity
    /// </summary>
    public virtual float maxBlockDurability {
        get {
            return _entityABC.maxBlockDurability;
        }
    }

    /// <summary>
    ///  Current block durability of the entity
    /// </summary>
    public virtual float blockDurability {
        get {
            return _entityABC.currentBlockDurability;
        }
    }

    /// <summary>
    /// Returns All Weapons setup on the entity
    /// </summary>
    public virtual List<ABC_Controller.Weapon> AllWeapons {
        get {
            return _entityABC.Weapons;
        }
    }

    /// <summary>
    /// Returns All Ability Groups setup on the entity
    /// </summary>
    public virtual List<ABC_Controller.AbilityGroup> AllAbilityGroups {
        get {
            return _entityABC.AbilityGroups;
        }
    }

    /// <summary>
    /// Returns All Abilities setup on the entity
    /// </summary>
    public virtual List<ABC_Ability> AllAbilities {
        get {
            return _entityABC.Abilities;
        }
    }

    /// <summary>
    /// Returns the weapon which is currently equipped 
    /// </summary>
    public virtual ABC_Controller.Weapon currentEquippedWeapon {
        get {
            return _entityABC.CurrentEquippedWeapon;
        }
    }

    /// <summary>
    /// Will return a list of the ABC Animation Runners attached to the entities current equipped weapon
    /// </summary>
    /// <returns>List of ABC Animation Runner Components</returns>
    public virtual List<ABC_AnimationRunner> GetCurrentEquippedWeaponAnimationRunners() {

        return _entityABC.GetCurrentEquippedWeaponAnimationRunners();

    }

    /// <summary>
    /// Will return a list of the Animators attached to the entities current equipped weapon
    /// </summary>
    /// <returns>List of Animator Components</returns>
    public virtual List<Animator> GetCurrentEquippedWeaponAnimators() {

        return _entityABC.GetCurrentEquippedWeaponAnimators();

    }

    /// <summary>
    /// Will return the entities current recorded key input history 
    /// </summary>
    /// <returns>List of recent key inputs made by the entity</returns>
    public virtual List<KeyCode> GetRecordedKeyInputHistory() {
       
        return _entityABC.recordedKeyInputHistory;
    }

    /// <summary>
    /// Returns the ability which is the current scroll ability (used for comparing if abilities are the current ability)
    /// </summary>
    public virtual ABC_Ability currentScrollAbility {
        get {
            return _entityABC.CurrentScrollAbility;
        }
    }

    /// <summary>
    /// Will determine if the entity is in the process of activating an ability
    /// </summary>
    /// <returns></returns>
    public virtual bool activatingAbility {

        get {
            return this._entityABC.IsActivatingAbility();
        }

    }

    /// <summary>
    /// Returns a bool indicating whether the entity can currently activate abilities.
    /// </summary>
    /// <value><c>true</c> if ability activation permitted; otherwise, <c>false</c>.</value>
    public virtual bool abilityActivationPermitted {

        get {
            return this._entityABC.AbilityActivationPermitted();
        }

    }

    /// <summary>
    /// Returns a bool indicating wheter the entity is currently ignoring ability collision
    /// </summary>
    /// <value><c>true</c> if entity is ignoring ability collision; otherwise, <c>false</c>.</value>
    public virtual bool ignoringAbilityCollision {

        get {
            return this._entitySM.ignoreAbilityCollision;
        }

    }


    /// <summary>
    /// returns a bool indicating if ability activation has been restricted by the entity (from a recent hit stagger etc) 
    /// </summary>
    public virtual bool abilityActivationHitRestricted {
        get {
            return this._entityABC.hitRestrictsAbilityActivation;
        }
    }



    /// <summary>
    /// gets entities current ability combokey 
    /// </summary>
    public virtual KeyCode currentComboKey {
        get {
            return _entityABC.currentComboKey;
        }
    }


    /// <summary>
    /// gets entities current combo button
    /// </summary>
    public virtual string currentComboButton {
        get {
            return _entityABC.currentComboButton;
        }
    }

    /// <summary>
    /// If true then the entity will never turn into a surrounding Object 
    /// </summary>
    public virtual bool blockSurroundingObjectStatus {
        get {
            return this._entitySM.blockSurroundingObjectStatus;
        }
        set {
            this._entitySM.blockSurroundingObjectStatus = value; 
        }
    }

    /// <summary>
    /// If this entity is converted to a SurroundingObject then this property tracks what Projectile the entity is linked too
    /// </summary>
    public virtual GameObject surroundingObjectLinkedProjectile {
        get {
            return this._entitySM.surroundingObjectLinkedProjectile;
        }
        set{
            this._entitySM.surroundingObjectLinkedProjectile = value;
        }
      }


    /// <summary>
    /// Records if this entity converted to a surrounding object was a kinematic rigibody, so it can be reverted back to the same status once it's done
    /// </summary>
    public virtual bool surroundingObjectLinkIsKinematic {
        get {
            return this._entitySM.surroundingObjectLinkIsKinematic;
        }
        set {
            this._entitySM.surroundingObjectLinkIsKinematic = value;
        }
    }


    /// <summary>
    ///  Records the rigidbody interpolate state of the converted surrunding object, so it can be reverted back to the same status once it's done
    /// </summary>
    public virtual RigidbodyInterpolation surroundingObjectLinkInterpolateState {
        get {
            return this._entitySM.surroundingObjectLinkInterpolateState;
        }
        set {
            this._entitySM.surroundingObjectLinkInterpolateState = value;
        }
    }


    /// <summary>
    /// Records if this entity converted to a surrounding object collider was a trigger, so it can be reverted back to the same status once it's done
    /// </summary>
    public virtual bool surroundingObjectLinkIsTrigger {
        get {
            return this._entitySM.surroundingObjectLinkIsTrigger;
        }
        set {
            this._entitySM.surroundingObjectLinkIsTrigger = value;
        }
    }


    /// <summary>
    /// Returns Vector3 of the CrossHairPosition on entity screen
    /// </summary>
    public virtual Vector3 crossHairPosition {

        get {
            return new Vector3(_entityABC.crosshairPositionX, _entityABC.crosshairPositionY, 0f);
        }

    }

    /// <summary>
    /// Returns a bool determining if the crosshair is currently active on the entity 
    /// </summary>
    public virtual bool crosshairOverrideActive {
        get {
            return this._entityABC.showCrossHairOverride;
        }
    }

    /// <summary>
    /// Returns the entities ability before target graphic indicator
    /// </summary>
    public virtual GameObject abilityWorldTargetIndicator {
        get {
            return _entityABC.abilityWorldTargetIndicatorObj;
        }
    }

    /// <summary>
    /// Returns entities ability before target graphic indicator default size
    /// </summary>
    public virtual Vector3 abilityWorldTargetIndicatorDefaultSize {
        get {
            return _entityABC.abilityWorldTargetIndicatorDefaultSize;
        }
    }

    /// <summary>
    /// Returns the entities ability before target out of range graphic indicator
    /// </summary>
    public virtual GameObject abilityWorldTargetOutRangeIndicator {
        get {
            return _entityABC.abilityWorldTargetOutRangeIndicatorObj;
        }
    }

    /// <summary>
    /// Returns entities ability before target out of range graphic indicator default size
    /// </summary>
    public virtual Vector3 abilityWorldTargetOutRangeIndicatorDefaultSize {
        get {
            return _entityABC.abilityWorldTargetOutRangeIndicatorDefaultSize;
        }
    }

    /// <summary>
    /// ability icon image GUI attached to the entity
    /// </summary>
    /// <returns></returns>
    public virtual RawImage abilityImageGUI {
        get {
            return this._entityABC.scrollAbilityImageGUI.RawImage;
        }
    }

    /// <summary>
    /// AmmoGUI attached to the entity
    /// </summary>
    public virtual Text abilityAmmoGUI {
        get {
            return this._entityABC.scrollAbilityammoGUIText.Text;
        }
    }


    /// <summary>
    /// ability icon image GUI attached to the entity
    /// </summary>
    /// <returns></returns>
    public virtual RawImage weaponImageGUI {
        get {
            return this._entityABC.weaponImageGUI.RawImage;
        }
    }

    /// <summary>
    /// AmmoGUI attached to the entity
    /// </summary>
    public virtual Text weaponAmmoGUI {
        get {
            return this._entityABC.weaponAmmoGUIText.Text;
        }
    }

    /// <summary>
    /// Preparing Progress GUI Bar attached to entity
    /// </summary>
    public virtual Slider preparingGUIBar {
        get {
            return this._entityABC.preparingAbilityGUIBar.Slider;
        }
    }

    /// <summary>
    /// Preparing ability text object attached to entity (normally overlayed over progress bar)
    /// </summary>
    public virtual Text preparingGUIText {
        get {
            return this._entityABC.preparingAbilityGUIText.Text;
        }
    }


    /// <summary>
    /// Effect Graphic Text attached to the entity 
    /// </summary>
    public virtual GameObject effectText {
        get {

            if (this._entitySM == null)
                return null;

            return this._entitySM.GetEffectTextObject();
        }

    }





    #endregion


    //************************ protected Methods ****************************************

    #region protected Methods

    /// <summary>
    /// Will add the request and latest time to the tracker
    /// </summary>
    /// <param name="Request">The Function Request we are tracking latest time for</param>
    /// <param name="LatestTime">The time the function request was called</param>
    protected void AddToFunctionRequestTimeTracker(string Request, float LatestTime) {

        //If the function request doesn't yet exist then add it
        if (this.functionRequestTimeTracker.ContainsKey(Request))
            this.functionRequestTimeTracker[Request] = LatestTime;
        else 
            this.functionRequestTimeTracker.Add(Request, LatestTime); // else update to latest time

    }

    /// <summary>
    /// Determines if the time provided is currently the most recent request to stop any call overlaps (i.e if toggle movement was disabled on a next attack, previous toggle can be stopped from running)
    /// </summary>
    /// <param name="Request">The Function Request to compare time against</param>
    /// <param name="TimeToCheck">The time we are checking against</param>
    /// <returns>True if the time provided is the most recent function request call, else false</returns>
    protected bool IsLatestFunctionRequestTime(string Request, float TimeToCheck) {

        bool retVal = true;

        //If the time provided is not the latest time recorded for that request, i.e the request has since been called then return false. 
        if (this.functionRequestTimeTracker.ContainsKey(Request) && TimeToCheck < this.functionRequestTimeTracker[Request])
            retVal = false;

        return retVal; 
    }

    /// <summary>
    /// Will setup the Entity finding and storing all components to be referenced and used. 
    /// </summary>
    /// <param name="Obj">object to be created into an entity object</param>
    protected void SetupEntity(GameObject Obj, ABC_IEntity Entity){

        // game object 
        this._entityGameObj = Obj;

        //get the ABC Controller so we can retrieve settings
        this._entityABC = ABC_Utilities.TraverseObjectForComponent(Obj, typeof(ABC_Controller)) as ABC_Controller;


        //get the ABC State Manager so we can retrieve settings
        this._entitySM = ABC_Utilities.TraverseObjectForComponent(Obj, typeof(ABC_StateManager)) as ABC_StateManager;

        // set entity transform to save on process
        this._entityTransform = this._entityGameObj.transform;

        // set rigidbody so we can save on process
        this._entityRigidbody = ABC_Utilities.TraverseObjectForComponent(Obj, typeof(Rigidbody)) as Rigidbody;

        // set Animator 
        this._ani = ABC_Utilities.TraverseObjectForComponent(Obj, typeof(Animator)) as Animator;

        //Update entity property with the actual entity object created throughout the system 
        this._entity = Entity; 


    }


    /// <summary>
    /// Will freeze/unfreeze movement by enabling or disabling the freeze position component
    /// </summary>
    /// <param name="FreezeMovement">True if movmement is to be frozen, else false</param>
    protected void FreezeMovement(bool FreezeMovement) {

        ABC_FreezePosition freezePosComponent = this.gameObject.GetComponent<ABC_FreezePosition>();

        //If we are not freezing movement then disable the component if it exists
        if (FreezeMovement == false && freezePosComponent != null)
            freezePosComponent.enabled = false;


        //Else if we are freezing movement then make sure the freeze position component is added and enabled
        if (FreezeMovement == true) {

            //If component hasn't already been added then add one
            if (freezePosComponent == null)
                freezePosComponent = gameObject.AddComponent<ABC_FreezePosition>();

            freezePosComponent.enableFreezePosition = true;
            freezePosComponent.enabled = true; // else component already added so enable it

        }    

    }



    /// <summary>
    /// Stops/Starts entity from moving by turning off/on all movement compoenents
    /// </summary>
    /// <param name="StopMovement">If true then movement is disabled, else it is enabled</param>
    protected void StopMovement(bool StopMovement) {

        //If entity was disabled during the call then we can end here
        if (_entityGameObj.activeInHierarchy == false)
            return;

        ABC_DisableMovementComponents disableMovementComponents = this.gameObject.GetComponent<ABC_DisableMovementComponents>();

        //If we are not stopping movement then disable the component if it exists
        if (StopMovement == false && disableMovementComponents != null)
            disableMovementComponents.enabled = false;


        //If stopping movement
        if (StopMovement == true) {

            //If component hasn't already been added then add one
            if (disableMovementComponents == null)
                disableMovementComponents = this.gameObject.AddComponent<ABC_DisableMovementComponents>();

            disableMovementComponents.haltRigidbody = true;
            disableMovementComponents.haltNavAgent = true;
            disableMovementComponents.disableCharacterController = true;

            disableMovementComponents.enabled = true;

        }

       

    }


    /// <summary>
    /// Will raise the enable movement delegate event letting any subscribed components know that movement is enabled 
    /// </summary>
    protected void RaiseEnableMovementEvent() {

        //Invokes the enable movement event for any scripts listening to stop movement
        if (_entitySM != null)
            this._entitySM.RaiseEnableMovementEvent();
    }

    /// <summary>
    /// Will raise the disable movement delegate event letting any subscribed components know that movement is disabled 
    /// </summary>
    protected void RaiseDisableMovementEvent() {

        //Invokes the disable movement event for any scripts listening to stop movement
        if (_entitySM != null)
            this._entitySM.RaiseDisableMovementEvent();
    }



    #endregion



    //************************ public virtual Methods  ****************************************

    #region public virtual Methods

    /// <summary>
    /// Will bulk import groups, abilities, weapons and icons all at once. Running the reload component functionality if used during play
    /// </summary>
    /// <param name="GroupsToImport">List of groups to import</param>
    /// <param name="AbilitiesToImport">List of abilities to import</param>
    /// <param name="WeaponsToImport">List of weapons to import</param>
    /// <param name="IconsToImport">List of icons to import</param>
    /// <param name="EnableIfExists">(Optional) If true then if the import item already exists then it will be enabled</param>
    /// <param name="WeaponEquipID">(Optional) if a ID is provided then that weapon will be equipped after import</param>
    /// <param name="ScrollAbilityEquipID">(Optional) if a ID is provided then that scroll ability will be equipped after import</param>
    public virtual IEnumerator ABCBulkImport(List<ABC_Controller.AbilityGroup> GroupsToImport = null, List<ABC_Ability> AbilitiesToImport = null, List<ABC_Controller.Weapon> WeaponsToImport = null, List<ABC_IconUI> IconsToImport = null, bool EnableIfExists = true, int WeaponEquipID = -1, int ScrollAbilityEquipID = -1) {       

        yield return ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.ABCBulkImport(GroupsToImport, AbilitiesToImport, WeaponsToImport, IconsToImport, EnableIfExists,  WeaponEquipID, ScrollAbilityEquipID));

    }

    /// <summary>
    /// Will determine if the entity has the ABC statemanager component attached
    /// </summary>
    /// <returns>True if the entity has the ABC StateManager component, else false</returns>
    public virtual bool HasABCStateManager() {

        if (this._entitySM != null && this._entitySM.enabled == true)
            return true;
        else
            return false; 
    }

    /// <summary>
    /// Return the object which has the statemanager script applied
    /// </summary>
    public virtual GameObject GetABCStateManagerObject() {

        if (this._entitySM != null && this._entitySM.enabled == true)
            return this._entitySM.gameObject;
        else
            return null;
    }

    /// <summary>
    /// Will determine if the entity has the ABC Controller component attached
    /// </summary>
    /// <returns>True if the entity has the ABC Controller component, else false</returns>
    public virtual bool HasABCController() {

        if (this._entityABC != null && this._entityABC.enabled == true)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Return the object which has the ABC Controller script applied
    /// </summary>
    public virtual GameObject GetABCControllerObject()
    {
        if (this._entityABC != null && this._entityABC.enabled == true)
            return this._entityABC.gameObject;
        else
            return null;
    }

    /// <summary>
    /// Returns a bool indicating if the entity has any ABC tags that match the tags in the list provided
    /// </summary>
    /// <remarks>
    /// Will check the entities ABC tags created in the statemanager component
    /// </remarks>
    /// <param name="TagList">List of string tags</param>
    /// <returns>True if the object's tags match any element in the taglist provided, else false.</returns>
    public virtual bool HasABCTag(List<string> TagList) {

        // we can only deal with objects which have a state manager script
        if (this.HasABCStateManager() == false)
            return false; 

        // loop through taglist and return true if object has a  statemanager tag
        foreach (string element in TagList) {

            if (this._entitySM.ABCTag.Contains(element))
                return true;

        }

        //If this far then no ABC tags matched
        return false; 
    }



    /// <summary>
    /// Returns a bool indicating if the entity has any tags that match the tag provided
    /// </summary>
    /// <remarks>
    /// Will check the entities ABC tags created in the statemanager component
    /// </remarks>
    /// <param name="Tag">string tag to check</param>
    /// <returns>True if the object's tags match any element in the tag provided, else false.</returns>
    public virtual bool HasABCTag(string Tag) {

        // we can only deal with objects which have a state manager script
        if (this.HasABCStateManager() == false)
            return false;

        // we can only deal with objects which have a state manager script
        if (this._entitySM.ABCTag.Contains(Tag))
            return true;


        // went through tag list and no match was found so we return false; 
        return false;
    }



    /// <summary>
    /// Will add the text provided to the originators diagnostic log which can be displayed in the editor
    /// </summary>
    /// <param name="TextLog">Text to add to the diagnostic log</param>
    public virtual void AddToDiagnosticLog(string TextLog) {

        //If turned on then write to the diagnostic log
        if (this._entityABC != null)
            this._entityABC.AddToDiagnosticLog(TextLog);


    }

    /// <summary>
    /// Determines if information is allowed to be display for the logging type given
    /// </summary>
    /// <param name="LoggingType">Type of logging (Ability Activation/Interruption etc)</param>
    /// <returns>True if information is allowed to be displayed, else false.</returns>
    public virtual bool LogInformationAbout(LoggingType LoggingType) {

        return this._entityABC.LogInformationAbout(LoggingType);

    }

    /// <summary>
    /// Adds the string provided to the ability log if setup correctly. If log is currently at or greater then the max lines provided then the oldest message will be removed.
    /// </summary>
    /// <param name="Text">Message to add to log</param>
    public virtual void AddToAbilityLog(string Text) {

        ABC_Utilities.mbSurrogate.StartCoroutine(_entityABC.AddToAbilityLog(Text));

    }

    /// <summary>
    /// Adds the string provided to the effect log if setup correctly. If log is currently at or greater then the max lines provided then the oldest message will be removed.
    /// </summary>
    /// <param name="Text">Message to add to log</param>
    public virtual void AddToEffectLog(string Text) {

        ABC_Utilities.mbSurrogate.StartCoroutine(_entitySM.AddToEffectLog(Text));

    }

    /// <summary>
    /// Will enable or disable idle mode
    /// </summary>
    /// <param name="Enabled">True to toggle into idle mode, else false to toggle out of idle mode </param>
    /// <param name="Instant">(Optional) If set to <c>true</c> then idle mode is switched instantly else it waits a certain time defined in settings.</param>
    public virtual IEnumerator ToggleIdleMode(bool Enabled, bool Instant = false) {

         yield return ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.ToggleIdleMode(Enabled, Instant));

    }


    /// <summary>
    /// Determines if the object provided is linked to the originator (I.e a child or parent in hiearchy)
    /// </summary>
    /// <param name="ObjectToCheck">Object which is going to be checked for any relation</param>
    /// <returns>True if object is linked to the originator, else false</returns>
    public virtual bool OriginatorLinkedToGameObject(GameObject ObjectToCheck){

        //Is originator linked?
        bool retVal = false; 

        List<Transform> entityLinkedGameObjs = new List<Transform>();

        //add parent if it exist
        if (this.transform.parent != null)
            entityLinkedGameObjs.Add(this.transform.parent);

        //Get childs
        entityLinkedGameObjs.AddRange(this.gameObject.GetComponentsInChildren<Transform>());

        //Check if any of the transforms found so far are linked
        retVal = entityLinkedGameObjs.Any(obj => obj.transform == ObjectToCheck.transform);


        return retVal;

    }


    /// <summary>
    /// Will raise the entities ability activation delegate event
    /// </summary>
    /// <param name="AbilityName">Name of ability that is activating</param>
    /// <param name="AbilityID">ID of ability that is activating</param>
    public virtual void RaiseAbilityActivationEvent(string AbilityName, int AbilityID){
        if (this._entityABC != null)
        this._entityABC.RaiseAbilityActivationEvent(AbilityName, AbilityID);
    }

    /// <summary>
    /// Will raise the entities ability activation completed delegate event
    /// </summary>
    /// <param name="AbilityName">Name of ability that completed activating</param>
    /// <param name="AbilityID">ID of ability that completed activating</param>
    public virtual void RaiseAbilityActivationCompleteEvent(string AbilityName, int AbilityID){
        if (this._entityABC != null)
            this._entityABC.RaiseAbilityActivationCompleteEvent(AbilityName, AbilityID);
    }


    /// <summary>
    /// Will raise the target set delegate event. 
    /// </summary>
    /// <param name="Target">target gameobject</param>
    public virtual void RaiseTargetSetEvent(GameObject Target) {

        this._entityABC.RaiseTargetSetEvent(Target);

    }

    /// <summary>
    /// Will raise the soft target set delegate event. 
    /// </summary>
    /// <param name="SoftTarget">target gameobject</param>
    public virtual void RaiseSoftTargetSetEvent(GameObject SoftTarget) {

        this._entityABC.RaiseSoftTargetSetEvent(SoftTarget);

    }

    /// <summary>
    /// Will raise the entities effect activation delegate event. 
    /// </summary>
    /// <param name="effect">Effect activated</param>
    public virtual void RaiseEffectActivationEvent(Effect Effect, ABC_IEntity Target, ABC_IEntity Originator) {
        this._entitySM.RaiseEffectActivationEvent(Effect, Target, Originator);

    }

    /// <summary>
    /// Will raise the entities effect deactivation delegate event. 
    /// </summary>
    /// <param name="effect">Effect activated</param>
    public virtual void RaiseEffectDeactivationEvent(Effect Effect, ABC_IEntity Target, ABC_IEntity Originator) {
        this._entitySM.RaiseEffectDeactivationEvent(Effect, Target, Originator);
    }


    /// <summary>
    /// Will raise the Scroll ability set and unset delegate event informing subscribers that a scroll ability has been initialised ('equipped') or deinitialised ('unequipped')
    /// </summary>
    /// <param name="AbilityID">ID of the scroll ability initialised ('equipped') or deinitialised ('unequipped')</param>
    /// <param name="AbilityName">Name of the scroll ability initialised ('equipped') or deinitialised ('unequipped')</param>
    /// <param name="Set">True if the scroll ability was initialised ('equipped') else, false if the scroll ability was deinitialised ('uneqipped')</param>
    public virtual void RaiseScrollAbilitySetAndUnsetEvent(int ScrollAbilityID, string ScrollAbilityName, bool Set) {

        if (this._entityABC != null)
            this._entityABC.RaiseScrollAbilitySetAndUnsetEvent(ScrollAbilityID, ScrollAbilityName, Set);

    }

    /// <summary>
    /// Will deactivate and disable the entity object
    /// </summary>
    public virtual IEnumerator Disable(float delay = 0f) {

        //If ABC Controller is enabled then disable this whilst entity is being disabled

        if (_entityABC != null)
            _entityABC.enabled = false;

        //wait for delay then disable
        yield return new WaitForSeconds(delay);

        if (_entityABC != null)
            _entityABC.enabled = true;

        if (this.gameObject.activeInHierarchy == true)
        this.gameObject.SetActive(false);

    }

  

    /// <summary>
    /// Function to show or hide the entity Health GUI 
    /// </summary>
    /// <param name="Enabled">If true will show Health, else will hide it</param>
    public virtual void ShowHealthGUI(bool Enabled) {

        this._entitySM.ShowHealthGUI(Enabled);

    }


    /// <summary>
    /// Will activate/disable the Mana GUI for the entity 
    /// </summary>
    /// <param name="Enabled">If true will show Mana, else will hide it</param>
    public virtual void ShowManaGUI(bool Enabled = true) {

        this._entityABC.ShowManaGUI(Enabled);

    
    }

    /// <summary>
    /// Function to show or hide the entity Stats GUI 
    /// </summary>
    /// <param name="Enabled">If true will show Stats GUI, else will hide it</param>
    public virtual void ShowStatsGUI(bool Enabled) {

        this._entitySM.ShowStatsGUI(Enabled);

    }

    /// <summary>
    /// Will activate/disable all the ability groups GUI for the entity 
    /// </summary>
    /// <param name="Enabled">If true will show the Ability Group GUI, else will hide it</param>
    public virtual void ShowAbilityGroupsGUI(bool Enabled = true) {

        this._entityABC.ShowAbilityGroupsGUI(Enabled);

    }

    /// <summary>
    /// Will display the text as a graphic in game for the duration provided
    /// </summary>
    /// <param name="Text">Text to display</param>
    /// <param name="Duration">Duration of the graphic</param>
    /// <param name="Color">Color of the graphic</param>
    /// <param name="Originator">Entity that applied the effect</param>
    public virtual void DisplayTextGraphic(string Text, float Duration, Color Color, ABC_IEntity Originator) {

        ABC_Utilities.mbSurrogate.StartCoroutine(_entitySM.DisplayTextGraphic(Text, Duration, Color, Originator));

    }

    /// <summary>
    /// Will find and return an ability Grou[
    /// </summary>
    /// <param name="AbilityGroupID">ID of the ability group to find</param>
    /// <returns>Ability Group found from the ID given</returns>
    public virtual ABC_Controller.AbilityGroup FindAbilityGroup(int AbilityGroupID) {

        return this._entityABC.FindAbilityGroup(AbilityGroupID);

    }

    /// <summary>
    /// Will find and return an ability Group
    /// </summary>
    /// <param name="AbilityGroupName">name of the ability group to find</param>
    /// <returns>Ability Group found from the name given</returns>
    public virtual ABC_Controller.AbilityGroup FindAbilityGroup(string AbilityGroupName) {

        return this._entityABC.FindAbilityGroup(AbilityGroupName);

    }


    /// <summary>
    /// Will find and return an IconUI
    /// </summary>
    /// <param name="IconID">ID of the icon to find</param>
    /// <returns>IconUI found from the ID given</returns>
    public virtual ABC_IconUI FindIconUI(int IconID) {

        return this._entityABC.FindIconUI(IconID);

    }

    /// <summary>
    /// Will find and return an IconUI
    /// </summary>
    /// <param name="IconName">Name of the icon to find</param>
    /// <returns>IconUI found from the ID given</returns>
    public virtual ABC_IconUI FindIconUI(string IconName) {

        return this._entityABC.FindIconUI(IconName);

    }

    /// <summary>
    /// Will add a new UI Icon to the entity
    /// </summary>
    /// <param name="Icon">Icon object to add</param>
    public virtual void AddIconUI(ABC_IconUI Icon) {

        _entityABC.AddIconUI(Icon);

    }

    /// <summary>
    /// Will remove the UI Icon from the entity
    /// </summary>
    /// <param name="Icon">Icon object to remove</param>
    public virtual void RemoveIconUI(ABC_IconUI Icon) {

        _entityABC.RemoveIconUI(Icon);

    }

    /// <summary>
    /// Will delete an Icon  
    /// </summary>
    /// <param name="IconName">Name of the Icon  to delete</param>
    public virtual void DeleteIconUI(string IconName) {

        this._entityABC.DeleteIconUI(IconName);
    }

    /// <summary>
    /// Will delete an Icon  
    /// </summary>
    /// <param name="IconID">ID of the Icon  to delete</param>
    public virtual void DeleteIconUI(int IconID) {

        this._entityABC.DeleteIconUI(IconID);
    }


    /// <summary>
    /// Adjusts weapons's ammo by the value provided. Will also update originators ammo GUI if provided.
    /// </summary>
    /// <remarks>
    /// If an originator is provided then the method will retrieve the entitys Ammo Text GUI and update it reflecting the weapons's current ammo. 
    /// </remarks>
    /// <param name="Value">Value to adjust ammo by (positive and negative possible)</param>
    /// <param name="AdjustAmmoOnly">(Optional) If true then only ammo will be modified, else it's up to logic to decide if ammo or current clip count is changed. </param>
    public virtual void AdjustEquippedWeaponAmmo(int Value, bool AdjustAmmoOnly = false) {
        this._entityABC.AdjustCurrentEquippedWeaponAmmo(Value, AdjustAmmoOnly);
    }

    /// <summary>
    /// Returns a bool indicating if the current equipped weapon has ammo
    /// </summary>
    /// <returns>True if equipped weapon has ammo, else false</returns>
    public virtual bool EquippedWeaponHasAmmo() {

        return this._entityABC.CurrentEquippedWeaponHasAmmo();
    }

    /// <summary>
    /// Determines if a reload is required on the current equipped weapon
    /// </summary>
    /// <returns>Returns true if a reload is required, else false</returns>
    public virtual bool EquippedWeaponReloadRequired() {

        return this._entityABC.CurrentEquippedWeaponReloadRequired();
    }

    /// <summary>
    /// Will find and return an weapon
    /// </summary>
    /// <param name="WeaponID">ID of the weapon to find</param>
    /// <returns>Weapon found from the ID given</returns>
    public virtual ABC_Controller.Weapon FindWeapon(int WeaponID) {

        return this._entityABC.FindWeapon(WeaponID);

    }

    /// <summary>
    /// Will find and return a Weapon
    /// </summary>
    /// <param name="WeaponName">Name of the Weapon to find</param>
    /// <returns>Weapon found from the Name given</returns>
    public virtual ABC_Controller.Weapon FindWeapon(string WeaponName) {

        return this._entityABC.FindWeapon(WeaponName);

    }


    /// <summary>
    /// Will find all icons that are setup to equip the weapon passed in the parameter
    /// </summary>
    /// <param name="WeaponID">ID of the weapon to find links to UI icons</param>
    /// <returns>List of ID's of Icons which equip the weapon passed through in parameters</returns>
    public virtual List<int> FindIconsLinkedToWeapon(int WeaponID) {

        return this._entityABC.FindIconsLinkedToWeapon(WeaponID);
    }


    /// <summary>
    /// Returns a dictionary detailing information about the weapon requested including name, description etc
    /// </summary>
    /// <param name="WeaponID">ID of the weapon</param>weapon</returns>
    public virtual Dictionary<string, string> GetWeaponDetails(int WeaponID) {
        return this._entityABC.GetWeaponDetails(WeaponID);
    }



    /// <summary>
    /// Will return the texture Icon for the weapon passed in the method
    /// </summary>
    /// <param name="WeaponID">ID of the weapon</param>
    /// <returns>Texture2D Icon of the weapon passed</returns>
    public virtual Texture2D GetWeaponIcon(int WeaponID) {

        return this._entityABC.GetWeaponIcon(WeaponID);

    }


    /// <summary>
    /// Will return if the weapon provided is enabled
    /// </summary>
    /// <param name="WeaponID">ID of the weapon</param>
    /// <returns>True if enabled, else false</returns>
    public virtual bool IsWeaponEnabled(int WeaponID) {

        return this._entityABC.IsWeaponEnabled(WeaponID);
    }

    /// <summary>
    /// Will return if the weapon provided is enabled
    /// </summary>
    /// <param name="WeaponName">Name of weapon</param>
    /// <returns>True if enabled, else false</returns>
    public virtual bool IsWeaponEnabled(string WeaponName) {

        return this._entityABC.IsWeaponEnabled(WeaponName);
    }

    /// <summary>
    /// Returns the key/button which will equip the weapon
    /// </summary>
    /// <param name="WeaponID">ID of the Weapon</param>
    /// <returns>String of the key or button which will equip the weapon</returns>
    public virtual string GetWeaponEquipKey(int WeaponID) {

        return this._entityABC.GetWeaponEquipKey(WeaponID);
    }

    /// <summary>
    /// Returns the key/button which which will equip the weapon
    /// </summary>
    /// <param name="WeaponName">Name of the Weapon</param>
    /// <returns>String of the key or button which will equip the weapon</returns>
    public virtual string GetWeaponEquipKey(string WeaponName) {

        return this._entityABC.GetWeaponEquipKey(WeaponName);
    }


    /// <summary>
    /// Will delete a weapon 
    /// </summary>
    /// <param name="WeaponName">Name of the weapon to equip</param>
    public virtual void DeleteWeapon(string WeaponName) {
        this._entityABC.DeleteWeapon(WeaponName);
    }


    /// <summary>
    /// Will delete a weapon 
    /// </summary>
    /// <param name="WeaponID">ID of the weapon to delete</param>
    public virtual void DeleteWeapon(int WeaponID) {
        this._entityABC.DeleteWeapon(WeaponID);
    }


    /// <summary>
    /// Will equip a weapon 
    /// </summary>
    /// <param name="WeaponID">ID of the weapon to equip</param>
    /// <param name="QuickToggle">True if the weapon should equip instantly, else false.</param>
    public virtual void EquipWeapon(int WeaponID, bool QuickToggle = false) {
        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.EquipWeapon(WeaponID, QuickToggle));
    }

    /// <summary>
    /// Will equip a weapon
    /// </summary>
    /// <param name="WeaponName">Name of the weapon to equip</param>
    /// <param name="QuickToggle">True if the weapon should equip instantly, else false.</param>
    public virtual void EquipWeapon(string WeaponName, bool QuickToggle = false) {

        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.EquipWeapon(WeaponName, QuickToggle));

    }


    /// <summary>
    /// Will enable and equip the next scroll ability in the list
    /// </summary>
    /// <param name="QuickToggle">True if the weapon should equip instantly, else false.</param>
    public virtual void EquipNextWeapon(bool QuickToggle = true) {
        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.EquipNextWeapon(QuickToggle));

    }



    /// <summary>
    /// Will enable and equip the next scroll ability in the list
    /// </summary>
    /// <param name="QuickToggle">True if the weapon should equip instantly, else false.</param>
    public virtual void EquipPreviousWeapon(bool QuickToggle = true) {

        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.EquipPreviousWeapon(QuickToggle));

    }

    /// <summary>
    /// Will initiate weapon parry, setting the current weapon to parry
    /// </summary>
    public virtual void ActivateWeaponParry(){

        this._entityABC.ActivateWeaponParry();



    }


    /// <summary>
    /// Will prevent the entity from parrying for the duration provided
    /// </summary>
    /// <param name="Duration">Amount of time to prevent entity from being able to parry</param>
    public virtual void PreventWeaponParryForDuration(float Duration) {

        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.PreventWeaponParryForDuration(Duration));


    }


    /// <summary>
    /// Will activate the parry handler on the entities current equipped weapon (i.e handles the parrying of an ability)
    /// </summary>
    /// <param name="BlockedEntity">Entity that had an ability blocked</param>
    /// <param name="AbilityType">The type of ability that was blocked</param>
    /// <param name="AbilityHitPoint">The vector position where the ability collided</param>
    /// <param name="IgnoreWeaponParry">(Optional)If True then the weapon parry will be ignored</param>
    /// <returns>True if ability was successfully parried, else false</returns>
    public virtual bool ActivateCurrentEquippedWeaponParryHandler(ABC_IEntity BlockedEntity, AbilityType AbilityType, Vector3 AbilityHitPoint = default(Vector3), bool IgnoreWeaponParry = false) {

        if (this._entityABC == null)
            return false; 

        return this._entityABC.ActivateCurrentEquippedWeaponParryHandler(BlockedEntity, AbilityType, AbilityHitPoint, IgnoreWeaponParry);

    }


    /// <summary>
    /// Will activate weapon block, which will make the entity automatically toggle blocking for the duration provided. 
    /// </summary>
    /// <param name="Duration">The duration for the weapon block, if 0 is provided then the entity will block for an infinite duration</param>
    /// <param name="Cooldown">Once the duration is over the cooldown will stop entity blocking again for the time provided</param>
    public virtual void ActivateWeaponBlock(float Duration, float Cooldown){

        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.ActivateAutoWeaponBlock(Duration, Cooldown));

    }


    /// <summary>
    /// Will stop weapon blocking, stopping the current weapon from blocking 
    /// </summary>
    public virtual void StopWeaponBlocking() {

        this._entityABC.StopWeaponBlocking();

    }

    /// <summary>
    /// Will prevent the entity from blocking for the duration provided
    /// </summary>
    /// <param name="Duration">Amount of time to prevent entity from being able to start blocking</param>
    public virtual void PreventWeaponBlockingForDuration(float Duration) {

        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.PreventWeaponBlockingForDuration(Duration));

    }


    /// <summary>
    /// Will activate the block handler on the entities current equipped weapon (i.e handles the blocking of an ability)
    /// </summary>
    /// <param name="BlockedEntity">Entity that had an ability blocked</param>
    /// <param name="AbilityType">The type of ability that was blocked</param>
    /// <param name="AbilityHitPoint">The vector position where the ability collided</param>
    /// <param name="InterruptWeaponBlock">(Optional)If True then the weapon blocking will be cancelled stopping the entity from blocking</param>
    /// <param name="ReduceWeaponBlockDurability">(Optional)If True then the block durability will be decreased due to an ability hit</param>
    /// <returns>True if ability was successfully blocked, else false</returns>
    public virtual bool ActivateCurrentEquippedWeaponBlockHandler(ABC_IEntity BlockedEntity, AbilityType AbilityType, Vector3 AbilityHitPoint = default(Vector3), bool InterruptWeaponBlock = false, bool ReduceWeaponBlockDurability = true) {

        if (this._entityABC == null)
            return false;

        return  this._entityABC.ActivateCurrentEquippedWeaponBlockHandler(BlockedEntity, AbilityType, AbilityHitPoint, InterruptWeaponBlock, ReduceWeaponBlockDurability);
    }
     

    /// <summary>
    /// Will activate the entities weapon melee attack blocked reaction (i.e sword knockback animation when weapon is blocked)
    /// </summary>
    public virtual void ActivateCurrentEquippedWeaponMeleeAttackBlockedReaction() {

        this._entityABC.ActivateCurrentEquippedWeaponMeleeAttackBlockedReaction();

    }


    /// <summary>
    /// Will drop the weapon the entity currently has equipped, running animations, showing the drop graphic and disabling/deleting the graphic
    /// </summary>
    /// <param name="SkipNextWeaponEquip">If true then once current weapon is dropped the next weapon is not automatically equipped</param>
    public virtual IEnumerator DropCurrentWeapon(bool SkipNextWeaponEquip = false) {
        yield return (ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.DropCurrentWeapon(SkipNextWeaponEquip)));
    }


    /// <summary>
    /// Set the key of the ability 
    /// </summary>
    /// <param name="AbilityID">Ability to modify</param>
    /// <param name="Key">Key to set the ability too</param>
    public virtual void SetAbilityKey(int AbilityID, KeyCode Key) {
        this._entityABC.SetAbilityKey(AbilityID, Key, this._entity);
    }

    /// <summary>
    /// Set the button of the ability 
    /// </summary>
    /// <param name="AbilityID">Ability to modify</param>
    /// <param name="KeyButton">Button to set the ability too</param>
    public virtual void SetAbilityKey(int AbilityID, string KeyButton) {
        this._entityABC.SetAbilityKey(AbilityID, KeyButton, this._entity);
    }


    /// <summary>
    /// Will trigger the ability starting the activation
    /// </summary>
    /// <param name="AbilityID">ID of ability to trigger</param>
    public virtual void TriggerAbility(int AbilityID) {
         this._entityABC.TriggerAbility(AbilityID);

    }

    /// <summary>
    /// Will trigger the ability starting the activation
    /// </summary>
    /// <param name="AbilityName">Name of ability to trigger</param>
    public virtual void TriggerAbility(string AbilityName) {
        this._entityABC.TriggerAbility(AbilityName);
    }

    

    /// <summary>
    /// Will force ability activation ignoring all trigger and activation permitted checks
    /// </summary>
    /// <param name="AbilityID">ID of ability to force activation</param>
    public virtual void ForceAbilityActivation(int AbilityID) {

        this._entityABC.ForceAbilityActivation(AbilityID);


    }

    /// <summary>
    /// Will force ability activation ignoring all trigger and activation permitted checks
    /// </summary>
    /// <param name="AbilityID">name of the ability to force activation</param>
    public virtual void ForceAbilityActivation(string AbilityName) {

        this._entityABC.ForceAbilityActivation(AbilityName);


    }

    /// <summary>
    /// Will trigger the current scroll ability if one is active
    /// </summary>
    public virtual void TriggerCurrentScrollAbility() {

        this._entityABC.TriggerCurrentScrollAbility();

    }

    /// <summary>
    /// Determines if an effect is currently active on the entity. 
    /// </summary>
    /// <param name="EffectList">List of effect names to determine if they are active or not</param>
    /// <returns>True if effect is active, else false.</returns>
    public virtual bool IsEffectActive(List<string> EffectList) {

       return this._entitySM.IsEffectActive(EffectList);

    }

    /// <summary>
    /// Adds all effects provided to this entity. 
    /// </summary>
    /// <param name="effects">Effects to add</param>
    /// <param name="CastingOriginator">Entity that applied the effect</param>
    /// <param name="AbilityType">The type of ability which applied the effect (projectile, raycast, melee)</param>
    /// <param name="IsEffectLink">If true then the effect has been applied via ability effect link</param>
    /// <param name="Projectile">(Optional) Object that we collided with which started the process of adding an effect - Used in some range settings etc</param>
    /// <param name="ObjectHit">The object which was hit which effects will be applied on</param>
    /// <param name="EffectHitPoint">(Optional) The vector position where the object collided, if setup correctly this will play the effect graphics at that position</param>
    /// <param name="AbilityName">(Optional) The name of the ability which applied the effect (can be used later on to retrieve effects by ability name to dispel etc)</param>
    /// <param name="ActivateAnimationFromHit">(Optional) If true then hit animation will activate from effect being applied</param>
    /// <param name="SpecificAnimationToActivate">(Optional) The specific hit animation to activate, if left blank then a random hit animation setup will activate (using probability checks and if configured to activate on hit etc)</param>
    /// <param name="OverrideWeaponBlocking">(Optional)If True then the ability will ignore any weapon blocking, stopping the entity hit from blocking</param>
    /// <param name="ReduceWeaponBlockDurability">(Optional)If True then the block durability will be decreased on the entity that blocks this ability</param>
    /// <param name="OverrideWeaponParrying">(Optional)If True then the weapon parry will be ignored</param>
    public virtual void AddEffectsToEntity(List<Effect> effects, ABC_IEntity CastingOriginator, AbilityType AbilityType, bool IsEffectLink, GameObject Projectile = null, GameObject ObjectHit = null, Vector3 EffectHitPoint = default(Vector3), string AbilityName = "", bool ActivateAnimationFromHit = true, string SpecificAnimationToActivate = "", bool OverrideWeaponBlocking = false, bool ReduceWeaponBlockDurability = true, bool OverrideWeaponParrying = false) {


        this._entitySM.AddEffects(effects, CastingOriginator, AbilityType, IsEffectLink, Projectile, ObjectHit, EffectHitPoint, AbilityName, ActivateAnimationFromHit, SpecificAnimationToActivate, OverrideWeaponBlocking, ReduceWeaponBlockDurability, OverrideWeaponParrying);

    }



    /// <summary>
    /// Will get the effects from the ability ID provided 
    /// </summary>
    /// <param name="AbilityID">ID of ability whose effects will be applied</param>
    public List<Effect> GetAbilitiesEffects(int AbilityID) {

        return this._entityABC.GetAbilitiesEffects(AbilityID);
    }

    /// <summary>
    /// Will get the effects from the ability name provided 
    /// </summary>
    /// <param name="AbilityName">Name of ability whose effects will be applied</param>
    public List<Effect> GetAbilitiesEffects(string AbilityName) {

        return this._entityABC.GetAbilitiesEffects(AbilityName);

    }


    /// <summary>
    /// Will apply the effects from the ability ID (ability which is linked to this entity) provided to the target object provided
    /// </summary>
    /// <param name="AbilityID">ID of ability whose effects will be applied</param>
    /// <param name="EffectTarget">Target Object of the effects</param>
    /// <param name="AddedBySplash">(Optional) If true then effect was added by splash functionality </param>
    public virtual void ApplyEffectsFromAbility(int AbilityID, GameObject EffectTarget, bool AddedBySplash = false) {

        this._entityABC.ApplyAbilitiesEffects(AbilityID, EffectTarget, AddedBySplash);

    }

    /// <summary>
    /// Will apply the effects from the ability name (ability which is linked to this entity) provided to the target object provided
    /// </summary>
    /// <param name="AbilityName">Name of ability whose effects will be applied</param>
    /// <param name="EffectTarget">Target Object of the effects</param>
    public virtual void ApplyEffectsFromAbility(string AbilityName, GameObject EffectTarget) {

        this._entityABC.ApplyAbilitiesEffects(AbilityName, EffectTarget);

    }

    /// <summary>
    /// Will remove the effects inflicted by the ability ID provided
    /// </summary>
    /// <param name="AbilityID">ID of the ability whos effects will be removed</param>
    /// <param name="BypassDispelRestriction">(Optional) If true then the dispellable restriction will be ignored</param>
    public virtual void RemoveAbilitiesEffects(int AbilityID, bool BypassDispelRestriction = false) {
        this._entitySM.RemoveAbilitiesEffects(AbilityID, BypassDispelRestriction);
    }

    /// <summary>
    /// Will remove the effects inflicted by the ability ID provided
    /// </summary>
    /// <param name="AbilityName">Name of the ability whos effects will be removed</param>
    /// <param name="BypassDispelRestriction">(Optional) If true then the dispellable restriction will be ignored</param>
    public virtual void RemoveAbilitiesEffects(string AbilityName, bool BypassDispelRestriction = false) {
        this._entitySM.RemoveAbilitiesEffects(AbilityName, BypassDispelRestriction);
    }

    /// <summary>
    /// Will return a bool indicating if ability can be activated via key/button triggers
    /// </summary>
    /// <returns>True if ability can be activated by key/button triggers, else false</returns>
    public virtual bool CanActivateAbilitiesFromTriggers() {

        return this._entityABC.CanActivateAbilitiesFromTriggers();

    }

    /// <summary>
    /// Will return if the ability provided is enabled
    /// </summary>
    /// <param name="AbilityID">ID of the ability</param>
    /// <returns>True if enabled, else false</returns>
    public virtual bool IsAbilityEnabled(int AbilityID) {

        return this._entityABC.IsAbilityEnabled(AbilityID);
    }

    /// <summary>
    /// Will return if the ability provided is enabled
    /// </summary>
    /// <param name="AbilityName">Name of ability</param>
    /// <returns>True if enabled, else false</returns>
    public virtual bool IsAbilityEnabled(string AbilityName) {

        return this._entityABC.IsAbilityEnabled(AbilityName);
    }


    /// <summary>
    /// Will return if the ability provided is enabled and not on cooldown
    /// </summary>
    /// <param name="AbilityID">ID of ability</param>
    /// <returns>Yes if ability is enabled and ready, else false</returns>
    public virtual bool IsAbilityPrimed(int AbilityID) {
        return this._entityABC.IsAbilityPrimed(AbilityID);
    }

    /// <summary>
    /// Will return if the ability provided is enabled and not on cooldown
    /// </summary>
    /// <param name="AbilityName">Name of ability</param>
    /// <returns>Yes if ability is enabled and ready, else false</returns>
    public virtual bool IsAbilityPrimed(string AbilityName) {
        return this._entityABC.IsAbilityPrimed(AbilityName);
    }

    /// <summary>
    /// Will return a list of all ability names 
    /// </summary>
    /// <returns>List of all ability names</returns>
    public virtual List<string> GetAllAbilityNames(bool EnabledOnly = false) {

        return this._entityABC.GetAllAbilityNames(EnabledOnly);

    }

    /// <summary>
    /// Will return a string which represents what key/button will trigger the current scroll ability. 
    /// </summary>
    /// <returns>String of the key or button which will trigger the the current scroll ability</returns>
    public virtual string GetScrollAbilityActivationKey() {

        return this._entityABC.GetScrollAbilityActivationKey();

    }

    /// <summary>
    /// Returns the key/button which will trigger the ability
    /// </summary>
    /// <param name="AbilityID">ID of the ability</param>
    /// <returns>String of the key or button which will trigger the ability</returns>
    public virtual string GetAbilityKey(int AbilityID) {

        return this._entityABC.GetAbilityKey(AbilityID);
    }

    /// <summary>
    /// Returns the key/button which will trigger the ability
    /// </summary>
    /// <param name="AbilityName">Name of ability</param>
    /// <returns>String of the key or button which will trigger the ability</returns>
    public virtual string GetAbilityKey(string AbilityName) {

        return this._entityABC.GetAbilityKey(AbilityName);
    }

    /// <summary>
    /// Will find and return an ability
    /// </summary>
    /// <param name="AbilityID">ID of the ability to find</param>
    /// <returns>Ability found from the ID given</returns>
    public virtual ABC_Ability FindAbility(int AbilityID) {

        return this._entityABC.FindAbility(AbilityID);

    }

    /// <summary>
    /// Will find and return an ability
    /// </summary>
    /// <param name="AbilityName">Name of the ability to find</param>
    /// <returns>Ability found from the Name given</returns>
    public virtual ABC_Ability FindAbility(string AbilityName) {

        return this._entityABC.FindAbility(AbilityName);

    }

    /// <summary>
    /// Returns a dictionary detailing information about the ability requested including name, description etc
    /// </summary>
    /// <param name="AbilityID">ID of the ability</param>
    /// <returns>Dictionary holding information regarding the ability</returns>
    public virtual Dictionary<string, string> GetAbilityDetails(int AbilityID) {
        return this._entityABC.GetAbilityDetails(AbilityID);
    }

    /// <summary>
    /// Returns a dictionary detailing information about the ability requested including name, description etc
    /// </summary>
    /// <param name="AbilityName">Name of ability</param>
    /// <returns>Dictionary holding information regarding the ability</returns>
    public virtual Dictionary<string, string> GetAbilityDetails(string AbilityName) {
        return this._entityABC.GetAbilityDetails(AbilityName);
    }

    /// <summary>
    /// Will return the texture Icon for the ability passed in the method
    /// </summary>
    /// <param name="AbilityID">ID of the ability</param>
    /// <returns>Texture2D Icon of the ability passed</returns>
    public virtual Texture2D GetAbilityIcon(int AbilityID) {

        return this._entityABC.GetAbilityIcon(AbilityID);

    }

    /// <summary>
    /// Will use the ability ID provided to find the next avaliable (not combo blocked) ability ID in it's combo chain
    /// </summary>
    /// <param name="AbilityID">ID of the ability used to find the next combo ability</param>
    /// <returns>ID of the next ability in the combo chain or -1 if not found</returns>
    public virtual int GetAbilityNextAvaliableComboID(int AbilityID) {

        return this._entityABC.GetAbilityNextAvaliableComboID(AbilityID);

    }

    /// <summary>
    /// Will use the ability ID provided to find the next avaliable (not combo blocked) ability ID in it's combo chain
    /// </summary>
    /// <param name="AbilityName">Name of the ability used to find the next combo ability</param>
    /// <returns>ID of the next ability in the combo chain</returns>
    public virtual int GetAbilityNextAvaliableComboID(string AbilityName) {

        return this._entityABC.GetAbilityNextAvaliableComboID(AbilityName);

    }

    /// <summary>
    /// Will return the entities global ability prepare time adjustment (can be positive or negative)
    /// </summary>
    /// <returns>Ability prepare time adjustment</returns>
    public virtual float GetAbilityGlobalPrepareTimeAdjustment()
    {

        return this._entityABC.GetAbilityGlobalPrepareTimeAdjustment();
    }

    /// <summary>
    /// Will return the entities global ability cooldown adjustment (can be positive or negative)
    /// </summary>
    /// <returns>Ability cooldown adjustment</returns>
    public virtual float GetAbilityGlobalCooldownAdjustment() {

        return this._entityABC.GetAbilityGlobalCooldownAdjustment();
    }

    /// <summary>
    /// Will return a value which represents the remaining cooldown for the ability provided
    /// </summary>
    /// <param name="AbilityName">Name of ability</param>
    /// <param name="ReturnPercentage">If true then the reamining cooldown value returned will be a percentage</param>
    /// <returns>Remaining cooldown of the ability</returns>
    public virtual float GetAbilityRemainingCooldown(string AbilityName, bool ReturnPercentage = false) {

        return this._entityABC.GetAbilityRemainingCooldown(AbilityName, ReturnPercentage);

    }

    /// <summary>
    /// Will return a value which represents the remaining cooldown for the ability provided
    /// </summary>
    /// <param name="AbilityID">ID of ability</param>
    /// <param name="ReturnPercentage">If true then the reamining cooldown value returned will be a percentage</param>
    /// <returns>Remaining cooldown of the ability</returns>
    public virtual float GetAbilityRemainingCooldown(int AbilityID, bool ReturnPercentage = false) {

        return this._entityABC.GetAbilityRemainingCooldown(AbilityID, ReturnPercentage);

    }


    /// <summary>
    /// Returns a bool determining if the entity has triggered the cancel event
    /// </summary>
    /// <returns></returns>
    public virtual bool CancelTriggered() {

        return this._entityABC.CancelTriggered();

    }

    /// <summary>
    /// Will return a position offset which will be added on to the ability target position to make the ability miss. A dice roll is performed first to determine
    /// if the ability will miss or not
    /// </summary>
    /// <returns>Vector3 offset to be applied to ability target position to make it miss or not (not if dice roll determines ability won't miss)</returns>
    public virtual Vector3 GetMissChancePositionOffset() {

        return this._entityABC.GetMissChancePositionOffset();
    }

    /// <summary>
    /// Determines if the object provided can target this entity, will check through all limit lists
    /// </summary>
    /// <param name="Targeter">Object which is attempting to target us</param>
    /// <returns>True if the targeter provided can target this entity, else false</returns>
    public virtual bool CanBeTargetedByObject(GameObject Targeter) {

        if (this._entitySM == null)
            return false;

        return this._entitySM.CanBeTargetedByObject(Targeter);


    }


    /// <summary>
    /// Will add the gameobject provided to a priority list which will allow the Targeter to take a spot in the tracking list if requested
    /// </summary>
    /// <param name="Targeter">Object to track</param>
    public virtual void AddObjectAsPriorityTargeter(GameObject Targeter) {


        if (this._entitySM != null)
            this._entitySM.AddObjectAsPriorityTargeter(Targeter);

    }

    /// <summary>
    /// Will add the gameobject provided to a list which tracks who is targeting the entity
    /// </summary>
    /// <param name="Targeter">Object to track</param>
    public virtual void AddObjectToTargeterTracker(GameObject Targeter) {


        if (this._entitySM != null)
            this._entitySM.AddObjectToTargeterTracker(Targeter);

    }

    /// <summary>
    /// Will remove the gameobject provided from all lists which tracks who is targeting the entity
    /// </summary>
    /// <param name="Targeter">Object to track</param>
    public virtual void RemoveObjectFromTargeterTracker(GameObject Targeter) {

        if (this._entitySM != null)
            this._entitySM.RemoveObjectFromTargeterTracker(Targeter);

    }



    /// <summary>
    /// Will set the gameobject provided as the controllers target
    /// </summary>
    /// <param name="target">target gameobject</param>
    public virtual void SetNewTarget(GameObject target) {

        // set target
            this._entityABC.SetNewTarget(target);

    }


    /// <summary>
    /// Will request for the entity to select a new target 
    /// </summary>
    /// <param name="Type">target select type (target/mouse/world)</param>
    /// <param name="SoftTargetOverride">when selecting a new target should softtarget be applied first for confirmation of target</param>
    public virtual void SelectNewTarget(TargetType Type, bool SoftTargetOverride = false) {
        _entityABC.SelectNewTarget(Type, SoftTargetOverride);
    }

    /// <summary>
    /// Returns a bool determining if a new target has been selected yet since requested
    /// </summary>
    /// <param name="Type">target select type (target/mouse/world)</param>
    /// <returns>True if a new target has been selected, else false</returns>
    public virtual bool NewTargetSelected(TargetType Type) {
        return _entityABC.NewTargetSelected(Type);

    }


 

    /// <summary>
    /// Removes entities current target
    /// </summary>
    public virtual void RemoveTarget() {
        _entityABC.RemoveTarget(true);

    }


    /// <summary>
    /// Removes entities current soft target 
    /// </summary>
    public virtual void RemoveSoftTarget() {

        _entityABC.RemoveSoftTarget(true);

    }

    /// <summary>
    /// Will go through all the originators abilities enabling any which have been set to become enabled due to the ability determined in parameters performing an event determined in parameters. 
    /// </summary>
    /// <remarks>
    /// Will link abilities to each other. When 1 ability does a specific action like activation of collision then any abilities linked will become enabled. 
    /// This can be used to  make combos i.e Enable attack 2 once attack 1 activates
    /// </remarks>
    /// <param name="AbilityID">Ability that was activated</param>
    /// <param name="AbilityEvent">Event this ability is performing (activation/collision)</param>
    public virtual void EnableOriginatorsAbilitiesAfterEvent(int AbilityID, AbilityEvent AbilityEvent) {

        this._entityABC.EnableAbilitiesAfterEvent(AbilityID, AbilityEvent);


    }



    /// <summary>
    /// Will temporarily adjust the entities ability activation interval. This is reset once the interval wait is over.
    /// </summary>
    /// <param name="IntervalAdjustment">Amount to adjust ability activation interval by</param>
    public virtual void TemporarilyAdjustAbilityActivationInterval(float IntervalAdjustment) {

        this._entityABC.TemporarilyAdjustAbilityActivationInterval(IntervalAdjustment);

    }


    /// <summary>
    /// Will temporarily restrict the entity from activating an ability for the duration given
    /// </summary>
    /// <param name="Duration">How long to restrict entity from activating the ability</param>
    public virtual void TemporarilyDisableAbilityActivation(float Duration = 1f) {

        this._entityABC.TemporarilyDisableAbilityActivation(Duration);
    }

    /// <summary>
    /// Will add the ability to a list which tracks all abilities that the entity is currently activating
    /// </summary>
    /// <param name="Ability">Ability to add to the activating list</param>
    public virtual void AddToActivatingAbilities(ABC_Ability Ability) {

        this._entityABC.AddToActivatingAbilities(Ability);

    }

    /// <summary>
    /// Will remove the ability from a list which tracks all abilities that the entity is currently activating
    /// </summary>
    /// <param name="Ability">Ability to remove from the activating list</param>
    public virtual void RemoveFromActivatingAbilities(ABC_Ability Ability) {

        this._entityABC.RemoveFromActivatingAbilities(Ability);

    }

    /// <summary>
    /// Will add the ability to a list which tracks all active melee abilities, used when the activation is over but the attack is still going and a reference is needed when interrupting etc (unlike projectiles melee stop instantly)
    /// </summary>
    /// <param name="Ability">Active melee Ability to track</param>
    public virtual void AddToActiveMeleeAbilities(ABC_Ability Ability) {

        this._entityABC.AddToActiveMeleeAbilities(Ability);

    }

    /// <summary>
    /// Will remove the ability from a list which tracks all active melee abilities, used when the activation is over but the attack is still going and a reference is needed when interrupting etc (unlike projectiles melee stop instantly)
    /// </summary>
    /// <param name="Ability">Active melee Ability to track</param>
    public virtual void RemoveFromActiveMeleeAbilities(ABC_Ability Ability) {

        this._entityABC.RemoveFromActiveMeleeAbilities(Ability);

    }

    /// <summary>
    /// Will interupt any abilities that are currently being activated by the entity. 
    /// </summary>
    public virtual void InterruptAbilityActivation() {


        if (_entityABC != null)
            this._entityABC.InterruptAbilityActivation();

    }



    /// <summary>
    /// Will stop the entity from moving due to a hit
    /// </summary>
    public virtual void HitStopMovement() {

        if (_entitySM != null)
            this._entitySM.HitStopMovement();

    }

    /// <summary>
    /// Will interupt any abilities that are currently being activated by the entity due to a hit. Hit interruption is determined by the method this method calls. 
    /// </summary>
    public virtual void HitInterruptAbilityActivation() {
        if (_entityABC != null)
            this._entityABC.ActivateHitInterruptsAbilityActivation();
    }

    /// <summary>
    /// Will restrict the entity from activating abilities due to a hit. Duration of the restriction is determined by the hit prevent duration setup 
    /// </summary>
    public virtual void HitRestrictAbilityActivation() {

        if (_entityABC != null)
            this._entityABC.ActivateHitRestrictsAbilityActivation();

    }


    /// <summary>
    /// restricts entities from activiating ability due to a toggle ability being active
    /// </summary>
    public virtual void ToggledAbilityRestrictsAbilityActivation(bool Enabled) {

        this._entityABC.toggledAbilityRestrictsAbilityActivation = Enabled;

    }


    /// <summary>
    /// Sets the entities current ability combo key
    /// </summary>
    /// <param name="Key"></param>
    public virtual void SetCurrentComboKey(KeyCode Key) {

        _entityABC.currentComboKey = Key;
    }

    /// <summary>
    /// Sets the entities current current combobutton
    /// </summary>
    /// <param name="Button"></param>
    public virtual void SetCurrentComboButton(string Button) {

        _entityABC.currentComboButton = Button;

    }

    /// <summary>
    /// Will setup the weapons for the entity, useful to recall if weapons have changed drastically. 
    /// </summary>
    /// <param name="EquipWeaponID">(Optional) if entered then the weapon matching the ID will be equipped after the weapons have been setup, if -1 is entered then the first weapon configured will be equipped</param>
    /// <param name="QuickToggle">True if the weapon should equip instantly, else false.</param>
    public virtual void SetupWeaponsAndScrollAbilities(int EquipWeaponID = -1, bool QuickToggle = true) {
       ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.SetupWeaponsAndScrollAbilities(EquipWeaponID, QuickToggle));
    }



    /// <summary>
    /// If the 'CurrentScrollAbility' button has been pressed (This is one key that is always setup by the entity not the ability, like swapping weapons but mouse always being the button to fire)
    /// </summary>
    /// <param name="OnPress">True if we are checking if the button is pressed, else false for hold </param>
    /// <returns></returns>
    public virtual bool CurrentScrollAbilityActivationButtonPressed(bool OnPress = true) {

    // if on press is false then we want to see if key is being held down 
    return this._entityABC.CurrentScrollAbilityButtonPressed(OnPress);
}


    /// <summary>
    /// Stops entity from using gravity (sometimes using abilities allows the entity to defy gravity)
    /// </summary>
    public virtual void UseGravity(bool Enabled = true) {

        if (this.rigidBody != null) {

            this.rigidBody.useGravity = Enabled;

        }

    }

    /// <summary>
    /// Will raise the enable gravity delegate event letting any subscribed components know that gravity is enabled 
    /// </summary>
    public virtual void RaiseEnableGravityEvent()
    {

        //Invokes the enable gravity event for any scripts listening to gravity status
        if (_entitySM != null)
            this._entitySM.RaiseEnableGravityEvent();
    }


    /// <summary>
    /// Will raise the disable gravity delegate event letting any subscribed components know that gravity is disabled 
    /// </summary>
    public virtual void RaiseDisableGravityEvent()
    {

        //Invokes the enable gravity event for any scripts listening to gravity status
        if (_entitySM != null)
            this._entitySM.RaiseDisableGravityEvent();
    }

    /// <summary>
    /// Stops entity velocity (sometimes when using ability you stop velocity to fire)
    /// </summary>
    public virtual void StopVelocity() {

        if (this.rigidBody != null) {
            this.rigidBody.velocity = new Vector3(0, 0, 0);

        }

    }

    /// <summary>
    /// Will stop the entity from moving for the duration given
    /// </summary>
    /// <param name="duration">duration to stop movement for</param>
    /// <param name="RaiseEvent">If true then the start and stop movement event delgates will be invoked</param>
    /// <param name="FreezePosition">If true then originator entity movement will be enabled/disabled by freezing the transform position</param>
    /// <param name="DisableComponents">If true then originator entity movement will be enabled/disabled by changing the active state of movement components (Character Controller, Rigidbody, NavAgent etc)
    public virtual IEnumerator StopMovementForDuration(float duration = 1f, bool RaiseEvent = true, bool FreezePosition = true, bool DisableComponents = false) {

        //Track what time this method was called
        //Stops overlapping i.e if another part of the system toggled movement 
        //this function would not continue after duration
        float functionRequestTime = Time.time;

        //If we are to raise the event then call disable movement
        if (RaiseEvent)
            this.ToggleMovementRaiseEvent(functionRequestTime, false);

        //Toggle movement off
        this.ToggleMovement(functionRequestTime, false, FreezePosition, DisableComponents);

        //wait duration
        yield return new WaitForSeconds(duration);

        //toggle movement on
        this.ToggleMovement(functionRequestTime, true, FreezePosition, DisableComponents);

        //If we are to raise the event then call enable movement
        if (RaiseEvent)
            this.ToggleMovementRaiseEvent(functionRequestTime, true);

    }



    /// <summary>
    /// Stops entity from moving until the global IEntity variable stopMovement has been turned to false
    /// </summary>
    /// <param name="Enable">If false then the originator entity movement will be stopped, else if true then originator entity will be allowed to move again</param>
    /// <param name="FreezePosition">If true then originator entity movement will be enabled/disabled by freezing the transform position</param>
    /// <param name="DisableComponents">If true then originator entity movement will be enabled/disabled by changing the active state of movement components (Character Controller, Rigidbody, NavAgent etc)
    public virtual void ToggleMovement(float FunctionRequestTime, bool Enabled = true,  bool FreezePosition = true, bool DisableComponents = false) {

        //If toggle movement has already been called by another part of the system, making this request time not the latest then return here to stop overlapping calls 
        if (this.IsLatestFunctionRequestTime("ToggleMovement", FunctionRequestTime) == false)
            return;

        //Record new latest time this request was called
        this.AddToFunctionRequestTimeTracker("ToggleMovement", FunctionRequestTime);


        //Call freezemovement
        if (FreezePosition) {
            if (Enabled == true)
                this.FreezeMovement(false);
            else
                this.FreezeMovement(true);
        }


        //Turn off all movement components
        if (DisableComponents) {

            //If movement is enabled set the stopMovement variable to false (stopping stopMovement method) else call the stopmovement method which will turn off all movement componenets and stop rigidbody velocity in a loop
            if (Enabled == true)
                this.StopMovement(false);
            else
                this.StopMovement(true);
        }



    }


    /// <summary>
    /// Makes the entity look at the Vector3 position given
    /// </summary>
    /// <param name="Position">Vector3 position to look at</param>
    public virtual void LookAt(Vector3 Position) {

        transform.LookAt(Position);

    }


    /// <summary>
    /// Turns entity to face target object. Locks the Y position so it will always turn realistically. 
    /// </summary>
    /// <param name="Target">Object the entity will turn to face</param>
    public virtual void TurnTo(GameObject Target) {

        // uses look at functionality but keeps the same height as entity so it turns rather then moves whole body.
        this.LookAt(new Vector3(Target.transform.position.x,
                               transform.position.y,
                               Target.transform.position.z));

    }



    /// <summary>
    /// Moves entity to the offset provided 
    /// </summary>
    /// <param name="OffSet">From current position what offset the entity will move to</param>
    /// <param name="ForwardOffset">From current position what forward offset entity will move to</param>
    /// <param name="RightOffset">From current position what right offset entity will move to</param>
    /// <param name="Duration">How long it will take entity to reach the offset</param>
    /// <param name="Delay">Delay before entity starts moving</param>
    /// <param name="ManuallyDetectEnvironmentCollision"> Will manually detect environment collisions in the circumstance where colliders have not been applied to the entity, will only collide with entities named "Terrain", "Environment" or have an "Environment" Tag</param>
    public virtual IEnumerator MoveSelfByOffset(Vector3 OffSet, float ForwardOffset = 0f, float RightOffset = 0f, float Duration = 3f, float Delay = 0f, bool ManuallyDetectEnvironmentCollision = true) {

        // setup script with the right parameters 

        yield return new WaitForSeconds(Delay);

        // check if script exists
        ABC_ObjectToDestination objToDestination = this.transform.GetComponent<ABC_ObjectToDestination>();

        // if it doesn't exist then add it 
        if (objToDestination == null) 
            objToDestination = this.transform.gameObject.AddComponent<ABC_ObjectToDestination>();

        //Disable script whilst we add parameters (stop remove script as disabling will just remove the script)
        objToDestination.removeScript = false;
        objToDestination.enabled = false;


        objToDestination.destinationObj = this.gameObject;
        objToDestination.positionOverride = Vector3.zero;
        objToDestination.destinationParent = null;
        objToDestination.positionOffset = OffSet;
        objToDestination.positionForwardOffset = ForwardOffset;
        objToDestination.positionRightOffset = RightOffset;
        objToDestination.secondsToTarget = Duration;
        objToDestination.stopDistance = 0.6f;
        //For now we will stop movement on collision as this is just used at moment to move when entity performs initiating/preparation animation (If set to move). 
        objToDestination.stopMovementToDestinationOnCollision = true;
        objToDestination.rotateToTarget = false;
        objToDestination.travelDelay = 0f;
        objToDestination.continuouslyCalculateDestination = false;
        objToDestination.hoverOnSpot = false;
        objToDestination.hoverDistance = 0f;
        objToDestination.removeScript = true;

        objToDestination.enabled = true;



    }

    /// <summary>
    /// Moves entity to the Object provided 
    /// </summary>
    /// <param name="DestinationObject">Object to move to</param>
    /// <param name="StopDistance">The distance between entity and destination in which the movement is stopped</param>
    /// <param name="OffSet">Offset from destination which entity will move too</param>
    /// <param name="ForwardOffset">Forward Offset from destination which entity will move too</param>
    /// <param name="RightOffset">Right from destination which entity will move too</param>
    /// <param name="Duration">How long it will take entity to reach the final offset position</param>
    /// <param name="Delay">Delay before entity starts moving</param>
    public virtual IEnumerator MoveSelfToObject(GameObject DestinationObject, float StopDistance = 0.6f,  Vector3 OffSet = default(Vector3), float ForwardOffset = 0f, float RightOffset = 0f, float Duration = 3f, float Delay = 0f) {

        // setup script with the right parameters 

        yield return new WaitForSeconds(Delay);

        // check if script exists
        ABC_ObjectToDestination objToDestination = this.transform.GetComponent<ABC_ObjectToDestination>();

        // if it doesn't exist then add it 
        if (objToDestination == null)
            objToDestination = this.transform.gameObject.AddComponent<ABC_ObjectToDestination>();


        //Disable script whilst we add parameters (stop remove script as disabling will just remove the script)
        objToDestination.removeScript = false;
        objToDestination.enabled = false;

        objToDestination.destinationObj = DestinationObject;
        objToDestination.positionOverride = Vector3.zero;
        objToDestination.destinationParent = null;
        objToDestination.positionOffset = OffSet;
        objToDestination.positionForwardOffset = ForwardOffset;
        objToDestination.positionRightOffset = RightOffset;
        objToDestination.secondsToTarget = Duration;
        objToDestination.stopDistance = StopDistance;
        //For now we will stop movement on collision as this is just used at moment to move when entity performs initiating/preparation animation (If set to move). 
        objToDestination.stopMovementToDestinationOnCollision = true;
        objToDestination.travelDelay = 0f;
        objToDestination.rotateToTarget = false;
        objToDestination.continuouslyCalculateDestination = true;
        objToDestination.hoverOnSpot = false;
        objToDestination.hoverDistance = 0f;
        objToDestination.removeScript = true;

        objToDestination.enabled = true;



    }

    /// <summary>
    /// Moves entity to the position provided 
    /// </summary>
    /// <param name="DestinationPosition">Position to move to</param>
    /// <param name="StopDistance">The distance between entity and destination in which the movement is stopped</param> 
    /// <param name="OffSet">Offset from destination which entity will move too</param>
    /// <param name="ForwardOffset">Forward Offset from destination which entity will move too</param>
    /// <param name="RightOffset">Right from destination which entity will move too</param>
    /// <param name="Duration">How long it will take entity to reach the final offset position</param>
    /// <param name="Delay">Delay before entity starts moving</param>
    public virtual IEnumerator MoveSelfToPosition(Vector3 DestinationPosition, Vector3 OffSet, float ForwardOffset = 0f, float RightOffset = 0f, float Duration = 3f, float Delay = 0f) {

        // setup script with the right parameters 

        yield return new WaitForSeconds(Delay);

        // check if script exists
        ABC_ObjectToDestination objToDestination = this.transform.GetComponent<ABC_ObjectToDestination>();

        // if it doesn't exist then add it 
        if (objToDestination == null)
            objToDestination = this.transform.gameObject.AddComponent<ABC_ObjectToDestination>();


        //Disable script whilst we add parameters (stop remove script as disabling will just remove the script)
        objToDestination.removeScript = false;
        objToDestination.enabled = false;

        objToDestination.destinationObj = null;
        objToDestination.positionOverride = DestinationPosition;
        objToDestination.destinationParent = null;
        objToDestination.positionOffset = OffSet;
        objToDestination.positionForwardOffset = ForwardOffset;
        objToDestination.positionRightOffset = RightOffset;
        objToDestination.secondsToTarget = Duration;
        objToDestination.stopDistance = 0.6f;
        //For now we will stop movement on collision as this is just used at moment to move when entity performs initiating/preparation animation (If set to move). 
        objToDestination.stopMovementToDestinationOnCollision = true;
        objToDestination.rotateToTarget = false;
        objToDestination.travelDelay = 0f;
        objToDestination.continuouslyCalculateDestination = false;
        objToDestination.hoverOnSpot = false;
        objToDestination.hoverDistance = 0f;
        objToDestination.removeScript = true;

        objToDestination.enabled = true;



    }

    /// <summary>
    /// Stops the entity from moving by offset or to an object
    /// </summary>
    public virtual void StopMoveSelf() {
        ABC_ObjectToDestination objToDestination = this.transform.GetComponent<ABC_ObjectToDestination>();

        // if it exists then we can stop the script by setting the distance reached to true
        if (objToDestination != null) {
            objToDestination.DisableScript();

        }

    }


    /// <summary>
    /// Will modify the game speed for the duration given
    /// </summary>
    /// <param name="SpeedFactor">Value to change the game speed to (lower the number the slower the speed)</param>
    /// <param name="Duration">How long to modify the game speed for</param>
    public virtual void ModifyGameSpeed(float SpeedFactor, float Duration) {

        ABC_Utilities.mbSurrogate.StartCoroutine(ABC_Utilities.ModifyGameSpeed(SpeedFactor, Duration));
    }


    /// <summary>
    /// Will shake the main camera for the duration, amount and speed provided
    /// </summary>
    /// <param name="Duration">Duration to shake camera for</param>
    /// <param name="Amount">Amount to shake camera by</param>
    /// <param name="Speed">The speed of the camera shake</param>
    public virtual void ShakeCamera(float Duration, float Amount, float Speed) {

        if (this.Camera == null)
            return; 

        ABC_CameraShake cameraShaker = this.Camera.GetComponent<ABC_CameraShake>();

        if (cameraShaker == null)
           cameraShaker = this.Camera.gameObject.AddComponent<ABC_CameraShake>();

        cameraShaker.ActivateCameraShake(Duration, Amount, Speed);

    }

    /// <summary>
    /// Will return the stat value asked for in the parameter
    /// </summary>
    /// <param name="StatName">Stat to find value for</param>
    /// <param name="IntegrationType">The integration type for stat functionality - if ABC is picked then normal stat functionality is used else stat from another integration system i.e game creator is used</param>>
    /// <returns>Value of the stat</returns>
    public virtual float GetStatValue(string StatName, ABCIntegrationType IntegrationType = ABCIntegrationType.ABC) {

        return this._entitySM.GetStatValue(StatName, IntegrationType);

    }

#endregion


    //************************ public virtual Effect Methods  ****************************************

    #region public virtual Effect Methods

    /// <summary>
    /// Will Adjust the entities health
    /// </summary>
    /// <param name="Potency">Amount to adjust health by (can be positive or negative)</param>
    public virtual void AdjustHealth(float Potency) {
        this._entitySM.AdjustHealth(Potency);

    }

    /// <summary>
    /// Will Adjust the entities mana
    /// </summary>
    /// <param name="Potency">Amount to adjust mana by (can be positive or negative)</param>
    public virtual void AdjustMana(float Potency) { 
        this._entityABC.AdjustMana(Potency);
    }




    /// <summary>
    /// Will adjust the current Block Durability by the amount given can be positive or negative
    /// </summary>
    /// <param name="Potency">Amount to adjust Block Durability by</param>
    public virtual void AdjustBlockDurability(float Potency) {

        this._entityABC.AdjustBlockDurability(Potency);

    }

    /// <summary>
    /// Will activate the HitAnimation Effect
    /// </summary>
    public virtual void RandomHitAnimationEffect() {

        this._entitySM.ActivateRandomHitAnimationEffect();


    }

    /// <summary>
    /// Will activate the HitAnimation Effect from the name provided
    /// </summary>
    public virtual void HitAnimationEffect(string HitAnimationName) {

        this._entitySM.ActivateHitAnimationEffect(HitAnimationName);


    }


    /// <summary>
    /// Will modify a stats value by the amount provided
    /// </summary>
    /// <param name="StatName">Stat which will have its value modified</param>
    /// <param name="Amount">Amount to increase or decrease the stat value by</param>
    public virtual void AdjustStatValue(string StatName, float Amount) {

         this._entitySM.AdjustStatValue(StatName, Amount);

    }


    /// <summary>
    /// Will increase max health
    /// </summary>
    /// <param name="Amount">Amount to increase max health by</param>
    /// <param name="RestoreHealth">If true then health will be restored to full</param>
    public virtual void AdjustMaxHealth(float Amount, bool RestoreHealth = false) {

        this._entitySM.AdjustMaxHealth(Amount, RestoreHealth);
    }

    /// <summary>
    /// Will adjust max Mana
    /// </summary>
    /// <param name="Amount">Amount to adjust max Mana by</param>
    /// <param name="RestoreHealth">If true then Mana will be restored to full</param>
    public virtual void AdjustMaxMana(float Amount, bool RestoreMana = false) {

        this._entityABC.AdjustMaxMana(Amount, RestoreMana);
    }

    /// <summary>
    /// Will adjust max Block Durability
    /// </summary>
    /// <param name="Amount">Amount to adjust max block durability by</param>
    /// <param name="RestoreBlockDurability">If true then block durability will be restored to full</param>
    public virtual void AdjustMaxBlockDurability(float Amount, bool RestoreBlockDurability = false) {

        this._entityABC.AdjustMaxBlockDurability(Amount, RestoreBlockDurability);

    }


    /// <summary>
    /// Will adjust health regen rate
    /// </summary>
    /// <param name="Amount">Amount to adjust regen by</param>
    public virtual void AdjustHealthRegen(float Amount) {

        this._entitySM.AdjustHealthRegen(Amount);

    }

    /// <summary>
    /// Will adjust mana regen rate
    /// </summary>
    /// <param name="Amount">Amount to adjust regen by</param>
    public virtual void AdjustManaRegen(float Amount) {

        this._entityABC.AdjustManaRegen(Amount);

    }


    /// <summary>
    /// Will adjust block duration regen rate
    /// </summary>
    /// <param name="Amount">Amount to adjust regen by</param>
    public virtual void AdjustBlockDurabilityRegen(float Amount) {
        this._entityABC.AdjustBlockDurabilityRegen(Amount);

    }

    /// <summary>
    /// Will adjust the % of projectile and raycast damage mitigation the entity currently has
    /// </summary>
    /// <param name="Potency">Float value of the amount to adjust the current of projectile and raycast damage mitigation by</param>
    public virtual void AdjustProjectileMitigationPercentage(float Potency) {

        this._entitySM.AdjustProjectileAndRayCastDamageMitigationPercentage(Potency);

    }

    /// <summary>
    /// Will adjust the % of melee damage mitigation the entity currently has
    /// </summary>

    /// <param name="Potency">Float value of the amount to adjust the current melee damage mitigation by</param>
    public virtual void AdjustMeleeDamageMitigationPercentage(float Potency) {

        this._entitySM.AdjustMeleeDamageMitigationPercentage(Potency);

    }

    /// <summary>
    /// Toggles the prevent melee effect status 
    /// </summary>
    /// <param name="Enabled">If true prevent melee effects will be active, else false will disable it</param>
    public virtual void TogglePreventMeleeEffects(bool Enabled) {
        this._entitySM.TogglePreventMeleeEffects(Enabled);
    }


    /// <summary>
    /// Toggles the prevent projectile and raycast effect status 
    /// </summary>
    /// <param name="Enabled">If true prevent projectile and raycast effects will be active, else false will disable it</param>
    public virtual void TogglePreventProjectileAndRayCastEffects(bool Enabled) {
        this._entitySM.TogglePreventProjectileAndRayCastEffects(Enabled);
    }




    /// <summary>
    /// Adjust the ammo of the Entities Ability
    /// </summary>
    /// <param name="AbilityID">ID of the ability to enable</param>
    /// <param name="Value">Amount to adjust the ammo by</param>
    public virtual void AdjustAbilityAmmo(int AbilityID, int Value) {

        this._entityABC.AdjustAbilityAmmo(AbilityID, Value);

    }

    /// <summary>
    /// Will adjust the ammo of the weapon ID provided
    /// </summary>
    /// <param name="WeaponID">ID of the weapon to adjust ammo</param>
    /// <param name="Value">Value to adjust ammo by</param>
    public virtual void AdjustWeaponAmmo(int WeaponID, int Value) {

        this._entityABC.AdjustWeaponAmmo(WeaponID, Value);

    }

    /// <summary>
    /// Will set the weapons ammo to the value provided, recalculating weapon clip count
    /// </summary>
    /// <param name="Value">Value to change weapon ammo too</param>
    public virtual void SetWeaponAmmo(int WeaponID, int Value) {

        this._entityABC.SetWeaponAmmo(WeaponID, Value);
    }


    /// <summary>
    /// Will adjust the global ability miss chance by by the value provided  (can be positive or negative)
    /// </summary>
    /// <param name="Value">% Value to adjust the global ability miss chance by (can be positive or negative)</param>
    public virtual void AdjustGlobalAbilityMissChance(float Value) {

        this._entityABC.AdjustGlobalAbilityMissChance(Value);

    }


    /// <summary>
    /// Will adjust the ability global prepare time by the value provided
    /// </summary>
    /// <param name="Value">% Value to adjust global prepare time by (can be positive or negative)</param>
    public virtual void AdjustAbilityGlobalPrepareTime(float Value)
    {

        this._entityABC.AdjustAbilityGlobalPrepareTime(Value);

    }


    /// <summary>
    /// Will adjust the ability global cooldown by the value provided
    /// </summary>
    /// <param name="Value">% Value to adjust global cooldown by (can be positive or negative)</param>
    public virtual void AdjustAbilityGlobalCooldown(float Value) {

        this._entityABC.AdjustAbilityGlobalCooldown(Value);

    }

    /// <summary>
    /// Will add an ABC tag to the entity
    /// </summary>
    /// <param name="TagName">Name of the tag to add</param>
    public virtual void AddABCTag(string TagName) {
        this._entitySM.AddABCTag(TagName);
    }

    /// <summary>
    /// Will remove an ABC tag to the entity
    /// </summary>
    /// <param name="TagName">Name of the tag to remove</param>
    public virtual void RemoveABCTag(string TagName) {
        this._entitySM.RemoveABCTag(TagName);
    }

    /// <summary>
    /// Will enable or disable effect protection on the entity, effect protection will stop any ability effects from activating on the entity
    /// </summary>
    /// <param name="Enabled">True if to enable effect protection, else false to disable</param>
    public virtual void ToggleEffectProtection(bool Enabled = true) {
        this._entitySM.ToggleEffectProtection(Enabled);

    }

    /// <summary>
    /// Will enable or disable ability activation on the entity, 
    /// </summary>
    /// <param name="Enabled">True if ability activation is enabled, else false</param>
    public virtual void ToggleAbilityActivation(bool Enabled = true) {

        if (this._entityABC == null)
            return; 

        if (Enabled)
            this._entityABC.EnableAbilityActivation();
        else
            this._entityABC.DisableAbilityActivation();

    }

    /// <summary>
    /// Will disable the entity
    /// </summary>
    public virtual void DestroyEntity() {

        this._entityGameObj.SetActive(false);

    }


    /// <summary>
    /// Will enable or disable ability collision on the entity, if true then no ability will collide or apply effects to the entity
    /// </summary>
    /// <param name="Enabled">True if to enable ignoring ability collision, else false to disable</param>
    public virtual void ToggleIgnoreAbilityCollision(bool Enabled = true) {

        this._entitySM.ToggleIgnoreAbilityCollision(Enabled);

    }


    /// <summary>
    /// Will display the setup health reduction image for the duration configured 
    /// </summary>
    /// <param name="BypassRestriction">If true then the image will show even if setup not too</param>
    public virtual void ShowHealthReductionImage(bool BypassRestriction = false) {

        this._entitySM.ShowHealthReductionImage(BypassRestriction);


    }

    /// <summary>
    /// Will clear the current destination stopping the navagent from moving towards anywhere 
    /// </summary>
    public virtual void ClearAINavigation(){

        if (this._entityABC != null)
            this._entityABC.ClearAINavigationDestination();

    }

    /// <summary>
    /// Will Freeze the entity in position 
    /// </summary>
    public virtual void RestrictMovement() {

        this.ToggleMovement(Time.time, false);
    }

    /// <summary>
    /// Will enable the entity to move again (Unfreezing)
    /// </summary>
    public virtual void EnableMovement() {

        this.ToggleMovement(Time.time, true);
    }

    /// <summary>
    /// Will restrict entity movement  (by turning off components)
    /// </summary>
    public virtual void RestrictMovementComponents() {

        this.ToggleMovement(Time.time, false, false, true);
    }

    /// <summary>
    /// Will enable  entity movement  (by turning on components)
    /// </summary>
    public virtual void EnableMovementComponents() {

        this.ToggleMovement(Time.time, true, false, true);
    }


    /// <summary>
    /// Will raise the enable movement or disable movement event
    /// </summary>
    /// <param name="Enable">If false then the disable movement event will be invoked, else if true then the enable movement event will be invoked</param>
    public virtual void ToggleMovementRaiseEvent(float FunctionRequestTime, bool Enabled = true) {

        //If toggle movement has already been called by another part of the system, making this request time not the latest then return here to stop overlapping calls 
        if (this.IsLatestFunctionRequestTime("ToggleMovementRaiseEvent", FunctionRequestTime) == false)
            return;

        //Record new latest time this request was called
        this.AddToFunctionRequestTimeTracker("ToggleMovementRaiseEvent", FunctionRequestTime);


        if (Enabled == true)
            this.RaiseEnableMovementEvent();
        else
            this.RaiseDisableMovementEvent();

    }

    /// <summary>
    /// Will find and toggle of/on the component
    /// </summary>
    /// <param name="ComponentName">Component to toggle</param>
    /// <param name="Enabled">If true component will be enabled, else false will disable</param>
    public virtual void ToggleComponent(string ComponentName, bool Enabled) {

        switch (ComponentName) {
            case "NavMeshAgent":
                NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();

                if (agent != null)
                    agent.enabled = Enabled;

                break;
            case "Animator":

                if (this.animator != null)
                    this.animator.enabled = Enabled;

                break; 

            default:

                var component = gameObject.GetComponent(ComponentName) as MonoBehaviour;

                if (component != null)
                    component.enabled = Enabled;

                break;

        }


    }


    /// <summary>
    /// Will enable or disable the AI for the entity
    /// </summary>
    /// <param name="Enabled">True if to enable AI, else false</param>
    public virtual void ToggleAI(bool Enabled = true) {

        this._entityABC.ToggleAI(Enabled);

    }

    /// <summary>
    /// Enables the Entities Ability 
    /// </summary>
    /// <param name="AbilityName">Name of the ability to enable</param>
    public virtual void EnableAbility(string AbilityName) {
        this._entityABC.EnableAbility(AbilityName);
    }


    /// <summary>
    /// Enables the Entities Ability 
    /// </summary>
    /// <param name="AbilityID">ID of the ability to enable</param>
    public virtual void EnableAbility(int AbilityID) {
        this._entityABC.EnableAbility(AbilityID);
    }

    /// <summary>
    /// Enables the Entities Ability for the duration provided
    /// </summary>
    /// <param name="AbilityID">ID of the ability to enable</param>
    /// <param name="Duration">The duration to enable the ability for</param>
    public virtual void EnableAbilityForDuration(int AbilityID, float Duration) {
        ABC_Utilities.mbSurrogate.StartCoroutine(this._entityABC.EnableAbilityForDuration(AbilityID, Duration));
    }

    /// <summary>
    /// Disables the Entities Ability 
    /// </summary>
    /// <param name="AbilityName">Name of the ability to enable</param>
    public virtual void DisableAbility(string AbilityName) {
        this._entityABC.DisableAbility(AbilityName);
    }

    /// <summary>
    /// Disables the Entities Ability 
    /// </summary>
    /// <param name="AbilityID">ID of the ability to enable</param>
    public virtual void DisableAbility(int AbilityID) {
        this._entityABC.DisableAbility(AbilityID);
    }


    /// <summary>
    /// Will swap the enable state of an ability, disabling if enabled, else enabling if disabled
    /// </summary>
    /// <param name="AbilityName">name of the ability to toggle</param>
    public virtual void ToggleAbilityEnableState(string AbilityName) {
        this._entityABC.ToggleAbilityEnableState(AbilityName);
    }


    /// <summary>
    /// Will swap the enable state of an ability, disabling if enabled, else enabling if disabled
    /// </summary>
    /// <param name="AbilityID">ID of the ability to toggle</param>
    public virtual void ToggleAbilityEnableState(int AbilityID) {
        this._entityABC.ToggleAbilityEnableState(AbilityID);

    }




    /// <summary>
    /// Will delete an ability  
    /// </summary>
    /// <param name="AbilityName">Name of the ability  to delete</param>
    public virtual void DeleteAbility(string AbilityName) {
        this._entityABC.DeleteAbility(AbilityName);
    }

    /// <summary>
    /// Will delete a weapon 
    /// </summary>
    /// <param name="AbilityID">ID of the weapon to delete</param>
    public virtual void DeleteAbility(int AbilityID) {
        this._entityABC.DeleteAbility(AbilityID);
    }


    /// <summary>
    /// Enables the Entities Weapon 
    /// </summary>
    /// <param name="WeaponID">ID of the weapon to enable</param>
    /// <param name="EquipWeapon">If true then the weapon enabled will be equipped</param>
    /// <returns>True if weapon was successfully enabled, else false</returns>
    public virtual bool EnableWeapon(int WeaponID, bool EquipWeapon = false) {
       return this._entityABC.EnableWeapon(WeaponID, EquipWeapon);
    }

    /// <summary>
    /// Enables the Entities Weapon 
    /// </summary>
    /// <param name="WeaponName">Name of the weapon to enable</param>
    /// <param name="EquipWeapon">If true then the weapon enabled will be equipped</param>
    /// <returns>True if weapon was successfully enabled, else false</returns>
    public virtual bool EnableWeapon(string WeaponName, bool EquipWeapon = false) {
       return this._entityABC.EnableWeapon(WeaponName, EquipWeapon);
    }

    /// <summary>
    /// Disables the Entities Weapon 
    /// </summary>
    /// <param name="WeaponID">ID of the weapon to Disable</param>
    /// <returns>True if weapon was successfully disabled, else false</returns>
    public virtual bool DisableWeapon(int WeaponID) {
        return this._entityABC.DisableWeapon(WeaponID);
    }

    /// <summary>
    /// Disables the Entities Weapon 
    /// </summary>
    /// <param name="WeaponName">Name of the weapon to enable</param>
    /// <returns>True if weapon was successfully disabled, else false</returns>
    public virtual bool DisableWeapon(string WeaponName) {
       return this._entityABC.DisableWeapon(WeaponName);
    }


    /// <summary>
    /// Will swap the enable state of an weapon, disabling if enabled, else enabling if disabled
    /// </summary>
    /// <param name="WeaponName">name of the weapon to toggle</param>
    public virtual void ToggleWeaponEnableState(string WeaponName) {

        this._entityABC.ToggleWeaponEnableState(WeaponName);

    }

    /// <summary>
    /// Will swap the enable state of an weapon, disabling if enabled, else enabling if disabled
    /// </summary>
    /// <param name="WeaponID">name of the weapon to toggle</param>
    public virtual void ToggleWeaponEnableState(int WeaponID) {

        this._entityABC.ToggleWeaponEnableState(WeaponID);

    }



    /// <summary>
    /// Will return the number of the entities active abilities set up to be a ScrollAbility. 
    /// </summary>
    /// <returns></returns>
    public virtual int ScrollAbilityCount() {

        return this._entityABC.ScrollAbilityCount();

    }

    /// <summary>
    /// Equips the Entities Scroll Ability 
    /// </summary>
    /// <param name="AbilityID">ID of the scroll ability to equip</param>
    public virtual void EquipScrollAbilityByID(int AbilityID)
    {
        this._entityABC.EquipScrollAbility(AbilityID);
    }

    /// <summary>
    /// Equips the Entities Scroll Ability 
    /// </summary>
    /// <param name="AbilityID">ID of the scroll ability to equip</param>
    public virtual void EquipScrollAbilityByName(string AbilityName) {
        this._entityABC.EquipScrollAbility(AbilityName);
    }

    /// <summary>
    /// Will equip the next scroll ability
    /// </summary>
    /// <param name="ActivateAesthetic">True if the disable animation and graphic should activate, else false.</param> 
    public virtual void EquipNextScrollAbility(bool ActivateAesthetic = true) {

        this._entityABC.EquipNextScrollAbility(ActivateAesthetic);

    }

    /// <summary>
    /// Will equip the previous scroll ability
    /// </summary>
    /// <param name="ActivateAesthetic">True if the disable animation and graphic should activate, else false.</param>
    public virtual void EquipPreviousScrollAbility(bool ActivateAesthetic = true) {

        this._entityABC.EquipPreviousScrollAbility(ActivateAesthetic);

    }

    /// <summary>
    /// Will disable the current scroll ability
    /// </summary>
    /// <param name="MinNumberOfActiveConstraint">If true then the scroll ability will only be disabled if a minimum number of scroll abilities are currently active i.e only disable if 2 scroll abilities are currently enabled</param>
    /// <param name="MinNumberOfActiveToDisable">Number of scroll abilities that need to be enabled to disable the currently scroll ability</param>
    public virtual void DisableCurrentScrollAbility(bool MinNumberOfEnabledConstraint = false, int MinNumberOfEnabledToDisable = 2) {

        this._entityABC.DisableCurrentScrollAbility(MinNumberOfEnabledConstraint, MinNumberOfEnabledToDisable);

    }

    /// <summary>
    /// Will 'swap' the current scroll ability with another scroll ability defined by ID provided. The current scroll ability will be disabled and the new scroll ability will beenabled
    /// </summary>
    /// <param name="NewScrollAbilityID">ID of the new scroll ability to enable and swap with the current scroll ability which will be disabled</param>
    /// <param name="MinNumberOfActiveConstraint">If true then the current scroll ability will only be disabled if a minimum number of scroll abilities are currently active i.e only disable if 2 scroll abilities are currently enabled</param>
    /// <param name="MinNumberOfActiveToDisable">Number of scroll abilities that need to be enabled to disable the currently scroll ability</param>
    public virtual void SwapCurrentScrollAbility(int NewScrollAbilityID, bool MinNumberOfEnabledConstraint = false, int MinNumberOfEnabledToDisable = 2) {

        this._entityABC.SwapCurrentScrollAbility(NewScrollAbilityID, MinNumberOfEnabledConstraint, MinNumberOfEnabledToDisable);
    }

    /// <summary>
    /// Will adjust the current enable points by the value provided for the ability group defined by the ID passed in the parameters
    /// </summary>
    /// <param name="GroupID">ID of the ability group </param>
    /// <param name="AdjustmentValue">Amount to adjust the enable points by</param>
    public virtual void AdjustAbilityGroupEnablePoint(int GroupID, float AdjustmentPoint) {

    this._entityABC.AdjustAbilityGroupsEnablePoints(GroupID, AdjustmentPoint);

    }

    /// <summary>
    /// Will enable/disable the ability group linked to the ID provided
    /// </summary>
    /// <param name="GroupID">ID of the ability group </param>
    /// <param name="Enabled">True to enable the ability group, else false to disable</param>
    public virtual void ToggleAbilityGroup(int GroupID, bool Enabled = true) {

        this._entityABC.ToggleAbilityGroup(GroupID, Enabled);

    }

    /// <summary>
    /// Will delete an ability group  
    /// </summary>
    /// <param name="GroupName">Name of the Group to delete</param>
    public virtual void DeleteAbilityGroup(string GroupName) {

        this._entityABC.DeleteAbility(GroupName);
    }

    /// <summary>
    /// Will delete an ability group  
    /// </summary>
    /// <param name="GroupID">ID of the Group to delete</param>
    public virtual void DeleteAbilityGroup(int GroupID) {

        this._entityABC.DeleteAbility(GroupID);
    }

    /// <summary>
    /// Pushes the entity back depending on the source of impact. 
    /// </summary>
    /// <param name="Distance">Amount to push back</param>
    /// <param name="HitPoint">The impact source</param>
    /// <param name="Duration">How long it takes for entity to reach the push distance</param>
    /// <param name="LiftDistance">(Optional) The amount of lift produced by the push</param>
    public virtual void Push(float Distance,  Vector3 HitPoint, float Duration, float LiftDistance = 0) {

    // Angle Between the collision point and the entity
    Vector3 dir = HitPoint - transform.position;
    // Get the opposite direction and normalize it
    dir = -dir.normalized;
    // calculate lift distance
    dir.y = LiftDistance;

    //Move entity
    ABC_Utilities.mbSurrogate.StartCoroutine(this.MoveSelfByOffset(dir* Distance, 0f, 0f, Duration));

}

    /// <summary>
    /// Pushes the entity to the side
    /// </summary>
    /// <param name="RightDistance">Amount to push to side (Positive will push right, Negative left)</param>
    /// <param name="Duration">How long it takes for entity to reach the push distance</param>
    /// <param name="LiftDistance">(Optional) The amount of lift produced by the push</param>
    public virtual void PushSide(float RightDistance, float Duration, float LiftDistance = 0) {


        //Move entity
        ABC_Utilities.mbSurrogate.StartCoroutine(this.MoveSelfByOffset(new Vector3(0, LiftDistance, 0), 0f, RightDistance, Duration));

    }

    /// <summary>
    /// Pushes the entity to the right or left randomly
    /// </summary>
    /// <param name="Distance">Amount to push to side</param>
    /// <param name="Duration">How long it takes for entity to reach the push distance</param>
    /// <param name="LiftDistance">(Optional) The amount of lift produced by the push</param>
    public virtual void PushRandomSide(float Distance, float Duration, float LiftDistance = 0) {

        int diceRoll = (int)UnityEngine.Random.Range(0f, 100f);

        //Push Left by making the distance a negative number
        if (diceRoll >= 0 && diceRoll <= 49)
            Distance = -Distance;

        //Else (50/100) it will remain positive and push right

        //Move entity
        ABC_Utilities.mbSurrogate.StartCoroutine(this.MoveSelfByOffset(new Vector3(0, LiftDistance, 0), 0f, Distance, Duration));

    }

    /// <summary>
    /// Lifts the entity up depending on the source of impact. If negative distance is provided then entity will drop
    /// </summary>
    /// <param name="Distance">Amount to push up</param>
    /// <param name="Duration">How long it takes for entity to reach the lift distance</param>
    public virtual void Lift(float Distance, float Duration = 2f) {

        //Lift entity
        ABC_Utilities.mbSurrogate.StartCoroutine(this.MoveSelfByOffset(new Vector3(0, Distance, 0), 0f, 0f, Duration));

    }

    /// <summary>
    /// Pull the entity forward depending on the source of impact. 
    /// </summary>
    /// <param name="Distance">Amount to Pull</param>
    /// <param name="HitPoint">The impact source</param>
    /// <param name="Duration">How long it takes for entity to reach the pull distance</param>
    /// <param name="LiftDistance">(Optional) The amount of lift produced by the pull</param>
    public virtual void Pull(float Distance, Vector3 HitPoint, float Duration, float LiftDistance = 0) {

        // Angle Between the collision point and the entity
        Vector3 dir = HitPoint - transform.position;
        //  normalize it
        dir = dir.normalized;
        // Apply lift distance
        dir.y = LiftDistance;

        //Move entity
        ABC_Utilities.mbSurrogate.StartCoroutine(this.MoveSelfByOffset(dir * Distance, 0f, 0f, Duration));

    }

    /// <summary>
    /// Moves entity over time to the originator that activated the ability 
    /// </summary>
    /// <param name="Originator">Entity that activated the ability</param>
    /// <param name="Duration">How long it will take entity to reach the final offset position</param>
    /// <param name="StopDistance">The distance between entity and destination in which the movement is stopped</param>
    /// <param name="ForwardOffset">Forward Offset from destination which entity will move too</param>
    /// <param name="RightOffset">Right from destination which entity will move too</param>
    public virtual void GravitateToOriginator(ABC_IEntity Originator, float Duration, float StopDistance, float ForwardOffset = 0f, float RightOffset = 0f) {


        //Move entity
        ABC_Utilities.mbSurrogate.StartCoroutine(this.MoveSelfToObject(Originator.gameObject, StopDistance, new Vector3(0,0,0), ForwardOffset, RightOffset, Duration));

    }

    /// <summary>
    /// Moves entity over time to the projectile object which collided to activate effects 
    /// </summary>
    /// <param name="Projectile">Ability Projectile Gameobject which collided to activate the effects on the target</param>
    /// <param name="Duration">How long it will take entity to reach the final offset position</param>
    /// <param name="StopDistance">The distance between entity and destination in which the movement is stopped</param>
    /// <param name="ForwardOffset">Forward Offset from destination which entity will move too</param>
    /// <param name="RightOffset">Right from destination which entity will move too</param>
    public virtual void GravitateToProjectile(GameObject Projectile, float Duration, float StopDistance, float ForwardOffset = 0f, float RightOffset = 0f) {


        //Move entity
        ABC_Utilities.mbSurrogate.StartCoroutine(this.MoveSelfToObject(Projectile, StopDistance, new Vector3(0, 0, 0), ForwardOffset, RightOffset, Duration));

    }

    /// <summary>
    /// Will make the entity face the originator who activated the ability
    /// </summary>
    /// <param name="Originator">Entity that activated the ability</param>
    public virtual void LookAtOriginator(ABC_IEntity Originator) {

        this.LookAt(Originator.transform.position);

    }

    /// <summary>
    /// Will add an explosive force at the position provided
    /// </summary>
    /// <param name="ExplosionForce">Power of the explosion</param>
    /// <param name="ExplosionPosition">Position of the explosion</param>
    /// <param name="ExplosionRadius">Radius of the explosion</param>
    /// <param name="UpwardsModifier">Upwards Force</param>
    public virtual void ExplosionForce(float ExplosionForce, Vector3 ExplosionPosition, float ExplosionRadius, float UpwardsModifier) {

        if (this.rigidBody != null)
        this.rigidBody.AddExplosionForce(ExplosionForce, ExplosionPosition, ExplosionRadius, UpwardsModifier);
    }


    /// <summary>
    /// Will toggle character visibility by enabling or disabling  the entities mesh renderers
    /// </summary>
    /// <param name="Enabled">True if to make the entity visible, else false</param>
    public virtual void ToggleVisibility(bool Enabled = true) {

        //Get all renderers
        Renderer[] renderers = this.transform.GetComponentsInChildren<Renderer>();

        if (renderers.Count() == 0)
            return; 

        //Cycle through renderer enabling or disabling
        foreach (Renderer rend in renderers) 
            rend.enabled = Enabled;


    }

    /// <summary>
    /// Will enable or disable hovering compoenent
    /// </summary>
    /// <param name="Enabled">If true then hover will be enabled, else if false then disabled</param>
    /// <param name="HoverDistance">(Optional) the hover distance</param>
    public virtual void ToggleHover(bool Enabled = true, float HoverDistance = 0.3f) {

        ABC_Hover hoverComp = null;

        //Add component if enabled

        if (Enabled) {
            hoverComp = this.gameObject.AddComponent<ABC_Hover>();
            hoverComp.hoverDistance = HoverDistance;
        } else {
         //else destroy the component

            hoverComp = this.gameObject.GetComponent<ABC_Hover>();
            if (hoverComp != null)
                UnityEngine.Object.Destroy(hoverComp);
        }
    }

    /// <summary>
    /// Will teleport the entity forward/back by the distance provided (positive value for forward, negative for back)
    /// </summary>
    /// <param name="Distance">Distance to teleport</param>
    public virtual void TeleportForward(float Distance) {

        // move forward by distance
        _entityTransform.Translate(Vector3.forward * Distance);

    }

    /// <summary>
    /// Will teleport the entity Right/Left by the distance provided (positive value for right, negative for left)
    /// </summary>
    /// <param name="Distance">Distance to teleport</param>
    public virtual void TeleportSide(float Distance) {

        // move forward by distance
        _entityTransform.Translate(Vector3.right * Distance);

    }

    /// <summary>
    /// Will teleport the entity behind it's current target (if the target exists)
    /// </summary>
    /// <param name="DistanceInfront">How far infront the target the entity will appear</param>
    /// <param name="AuxiliarySoftTarget">If true and the entity doesn't have a current target then then the entity will instead teleport behind the current softarget </param>
    public virtual void TeleportInfrontCurrentTarget(float DistanceInfront, bool AuxiliarySoftTarget = false) {

        //record target
        GameObject currentTarget = this.target;

        //If target was null but AuxiliarySoftTarget is true then use the softtarget instead
        if (currentTarget == null && AuxiliarySoftTarget == true)
            currentTarget = this.softTarget;

        //If the target is still null then end here
        if (currentTarget == null)
            return;

        //Teleport Distance Behind the target
        _entityTransform.position = currentTarget.transform.position + (currentTarget.transform.forward * DistanceInfront);

        //Turn entity to face target
        this.TurnTo(currentTarget);


    }

    /// <summary>
    /// Will teleport the entity infront of it's current target (if the target exists)
    /// </summary>
    /// <param name="DistanceBehind">How far behind the target the entity will appear</param>
    /// <param name="AuxiliarySoftTarget">If true and the entity doesn't have a current target then then the entity will instead teleport infront of the current softarget </param>
    public virtual void TeleportBehindCurrentTarget(float DistanceBehind, bool AuxiliarySoftTarget = false) {

        //record target
        GameObject currentTarget = this.target;

        //If target was null but AuxiliarySoftTarget is true then use the softtarget instead
        if (currentTarget == null && AuxiliarySoftTarget == true)
            currentTarget = this.softTarget;

        //If the target is still null then end here
        if (currentTarget == null)
            return;

        //Teleport Distance Behind the target
        _entityTransform.position = currentTarget.transform.position - (currentTarget.transform.forward * DistanceBehind);

        //Turn entity to face target
        this.TurnTo(currentTarget);


    }

    /// <summary>
    /// Will teleport the entity to the side of it's current target (if the target exists)
    /// </summary>
    /// <param name="RightDistance">Right distance which the entity will appear on the targets side (positive value for Right, negative for Left)</param>
    /// <param name="AuxiliarySoftTarget">If true and the entity doesn't have a current target then then the entity will instead teleport to the side of the current softarget </param>
    public virtual void TeleportSideCurrentTarget(float RightDistance, bool AuxiliarySoftTarget = false) {

        //record target
        GameObject currentTarget = this.target;

        //If target was null but AuxiliarySoftTarget is true then use the softtarget instead
        if (currentTarget == null && AuxiliarySoftTarget == true)
            currentTarget = this.softTarget;

        //If the target is still null then end here
        if (currentTarget == null)
            return;

        //Teleport Distance Behind the target
        _entityTransform.position = currentTarget.transform.position + (currentTarget.transform.right * RightDistance);

        //Turn entity to face target
        this.TurnTo(currentTarget);


    }

    /// <summary>
    /// Will teleport the entity to either infront, behind or either side of the current target randomly
    /// </summary>
    /// <param name="Distance">The distance which the entity will appear on the random side</param>
    /// <param name="AuxiliarySoftTarget">If true and the entity doesn't have a current target then then the entity will instead teleport to the current softarget </param>
    public virtual void TeleportRandomSideCurrentTarget(float Distance, bool AuxiliarySoftTarget = false) {

        //Roll Dice
        int diceRoll = (int)UnityEngine.Random.Range(0f, 100f);

        //Infront
        if (diceRoll >= 0 && diceRoll <= 24)
            this.TeleportInfrontCurrentTarget(Distance, AuxiliarySoftTarget);

        //Behind
        if (diceRoll >= 25 && diceRoll <= 49)
            this.TeleportBehindCurrentTarget(Distance, AuxiliarySoftTarget);

        //Right side
        if (diceRoll >= 50 && diceRoll <= 74)
            this.TeleportSideCurrentTarget(Distance, AuxiliarySoftTarget); // right side

        //Left side
        if (diceRoll >= 75 && diceRoll <= 100)
            this.TeleportSideCurrentTarget(-Distance, AuxiliarySoftTarget); // left side

    }




    #endregion


    //************************ Integrations   ***************************************


    #region EmeraldAI Integration 

    /// <summary>
    /// Will return the max health (starting health) set on Emerald AI
    /// </summary>
    /// <returns>Int value representing the current max health</returns>
    public virtual int GetEmeraldAIMaxHealth() {


#if ABC_EmeraldAI_Integration
        EmeraldAI.EmeraldAISystem emAISystem = gameObject.GetComponent<EmeraldAI.EmeraldAISystem>();
        return emAISystem.StartingHealth;
#endif

        Debug.Log("Emerald AI Integration is not correctly setup.");
        return 0;

    }


    /// <summary>
    /// Will set the max health (starting health) on Emerald AI
    /// </summary>
    /// <param name="Amount">Amount to set max health by</param>
    public virtual void SetEmeraldMaxHealth(int Amount) {

#if ABC_EmeraldAI_Integration
        EmeraldAI.EmeraldAISystem emAISystem = gameObject.GetComponent<EmeraldAI.EmeraldAISystem>();
        emAISystem.StartingHealth = Amount;

        return; 
#endif

        Debug.Log("Emerald AI Integration is not correctly setup.");

    }

    /// <summary>
    /// Will return the current health set on Emerald AI
    /// </summary>
    /// <returns>Int value representing the current health</returns>
    public virtual int GetEmeraldAICurrentHealth() {

#if ABC_EmeraldAI_Integration
        EmeraldAI.EmeraldAISystem emAISystem = gameObject.GetComponent<EmeraldAI.EmeraldAISystem>();
        return emAISystem.CurrentHealth;
#endif

        Debug.Log("Emerald AI Integration is not correctly setup.");
        return 0;

    }


    /// <summary>
    /// Will set the current health on Emerald AI
    /// </summary>
    /// <param name="Amount">Amount to set the health by</param>
    public virtual void SetEmeraldAICurrentHealth(int Amount) {

#if ABC_EmeraldAI_Integration
        EmeraldAI.EmeraldAISystem emAISystem = gameObject.GetComponent<EmeraldAI.EmeraldAISystem>();
        emAISystem.CurrentHealth = Amount;

        return; 
#endif

        Debug.Log("Emerald AI Integration is not correctly setup.");

    }

    /// <summary>
    /// Will activate the EmeraldAI damage event, damaging the entity using Emerald AI system
    /// </summary>
    /// <param name="Amount">Amount of damage to apply</param>
    /// <param name="AttackingEntity">Entity applying the damage</param>
    /// <param name="TargetTypePlayer">Target type, if true then Player type, else if false then AI type</param>
    public virtual void ActivateEmeraldAIDamage(int Amount, ABC_IEntity AttackingEntity, bool TargetTypePlayer = true) {


#if ABC_EmeraldAI_Integration
        EmeraldAI.EmeraldAISystem emAISystem = gameObject.GetComponent<EmeraldAI.EmeraldAISystem>();
        emAISystem.Damage(Amount, TargetTypePlayer == true ? EmeraldAI.EmeraldAISystem.TargetType.Player : EmeraldAI.EmeraldAISystem.TargetType.AI, AttackingEntity.transform);

        return; 
#endif

        Debug.Log("Emerald AI Integration is not correctly setup.");


    }


    #endregion


    #region GameCreator Integrations

    /// <summary>
    /// Returns the max value for the GC attribute
    /// </summary>
    /// <param name="AttributeID">ID of the attribute to get the max value for</param>
    /// <returns>Float value representing the max value of the attribute</returns>
    public virtual float GetGCMaxAttributeValue(string AttributeID)    {

#if ABC_GC_Stats_Integration
        Stats gcStats = gameObject.GetComponent<Stats>();
        return gcStats.GetAttrMaxValue(AttributeID);

#endif

        Debug.Log("Game Creator Integration is not correctly setup.");
        return 0;
    }



    /// <summary>
    /// Returns the current value for the GC attribute/stat
    /// </summary>
    /// <param name="StatID">ID of the stat/attribute to get the current value for</param>
    /// <param name="GCStatType">The GC stat type: Stat or Attribute</param>
    /// <returns>Float value representing the current value of the stat/attribute</returns>
    public virtual float GetGCStatValue(string StatID, GCStatType GCStatType = GCStatType.Stat) {

#if ABC_GC_Stats_Integration
        Stats gcStats = gameObject.GetComponent<Stats>();

        switch (GCStatType)
        {
            case GCStatType.Attribute:
                return gcStats.GetAttrValue(StatID);
            case GCStatType.Stat:
                return gcStats.GetStat(StatID);
        }
           
#endif

        Debug.Log("Game Creator Integration is not correctly setup.");
            return 0; 
    }


    /// <summary>
    /// (ABC Integration) Will set a Game Creator stat/attribute value by the amount provided
    /// </summary>
    /// <param name="StatID">ID of Stat which will have its value modified</param>
    /// <param name="Amount">Amount to increase or decrease the stat value by</param>
    /// <param name="GCStatType">The GC stat type: Stat or Attribute</param>
    public virtual void SetGCStatValue(string StatID, float Value, GCStatType GCStatType = GCStatType.Stat)
    {

#if ABC_GC_Stats_Integration
        Stats gcStats = gameObject.GetComponent<Stats>();

        switch (GCStatType)
        {
            case GCStatType.Attribute:
                 gcStats.SetAttrValue(StatID, Value, true);
                break; 
            case GCStatType.Stat:
                 gcStats.SetStatBase(StatID, Value, true);
                break; 
        }

        return; 

#endif

        Debug.Log("Game Creator Integration is not correctly setup.");
    }




    /// <summary>
    /// (ABC Integration) Will adjust a Game Creator stats value by the amount provided
    /// </summary>
    /// <param name="StatID">ID of Stat which will have its value modified</param>
    /// <param name="Amount">Amount to increase or decrease the stat value by</param>
    /// <param name="GCStatType">The GC stat type: Stat or Attribute</param>
    public virtual void AdjustGCStatValue(string StatID, float Value, GCStatType GCStatType = GCStatType.Stat) {

#if ABC_GC_Stats_Integration
        Stats gcStats = gameObject.GetComponent<Stats>();

        switch (GCStatType)
        {
            case GCStatType.Attribute:
                 gcStats.AddAttrValue(StatID, Value, true);
                break; 
            case GCStatType.Stat:
                 gcStats.AddStatBase(StatID, Value, true);
                break; 
        }

        return; 

#endif

        Debug.Log("Game Creator Integration is not correctly setup.");
    }

    /// <summary>
    /// (ABC Integration) Will add a Game Creator status effect matching the status effect name/ID provided
    /// </summary>
    /// <param name="StatusEffectID">GC Status Effect ID/Name of the effect to add</param>
    public virtual void AddGCStatusEffect(string StatusEffectID)
    {

#if ABC_GC_Stats_Integration
        Stats gcStats = gameObject.GetComponent<Stats>();

           StatusEffectAsset gcStatusEffect = DatabaseStats.Load().GetStatusEffectAssets().Where(e => e.statusEffect.uniqueName == StatusEffectID).FirstOrDefault();

        if (gcStatusEffect != null)
            gcStats.AddStatusEffect(gcStatusEffect);
#endif

    }


    /// <summary>
    /// (ABC Integration) Will remove a Game Creator status effect matching the status effect name/ID provided
    /// </summary>
    /// <param name="StatusEffectID">GC Status Effect ID/Name of the effect to remove</param>
    public virtual void RemoveGCStatusEffect(string StatusEffectID)
    {

#if ABC_GC_Stats_Integration
        Stats gcStats = gameObject.GetComponent<Stats>();

        StatusEffectAsset gcStatusEffect = DatabaseStats.Load().GetStatusEffectAssets().Where(e => e.statusEffect.uniqueName == StatusEffectID).FirstOrDefault();

        if (gcStatusEffect != null)
            gcStats.RemoveStatusEffect(gcStatusEffect);
#endif

    }


#endregion
}

