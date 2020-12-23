using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif
public static class ABC_Utilities {

#if UNITY_EDITOR
    static ABC_Utilities() {
        //Every time a file updates then refresh custom references for save/load
        EditorApplication.projectChanged += RefreshCustomReferences;
        EditorApplication.playModeStateChanged += LogPlayModeState;

        //Refresh custom references
        RefreshCustomReferences();
    }
#endif


    // ********************* Variables ********************
    #region Variables


    // The ABC Pool in the hiearchy which contains all the ABC Objects
    private static GameObject _abcPool;

    /// <summary>
    /// The ABC Pool in the hiearchy which contains all the ABC Objects
    /// </summary>
    public static GameObject abcPool {

        get {
            if (_abcPool == null)
            {
                if (GameObject.Find("ABC*_Pool") == null)
                {
                    _abcPool = new GameObject("ABC*_Pool made from Code");
                    Debug.Log("A pool of ability objects (ABC*_Pool) has been created automatically");
                    _abcPool.name = "ABC*_Pool";
                }
                else
                {

                    _abcPool = GameObject.Find("ABC*_Pool");
                }

                //Add the MbSurrogate script if it doesn't already exist
                if (_abcPool.GetComponent<ABC_MbSurrogate>() == null)
                    _abcPool.AddComponent<ABC_MbSurrogate>();
            }

            return _abcPool;
        }
    }

    /// <summary>
    ///  surrogate MonoBehaviour which is used to call courotines
    /// </summary>
    private static MonoBehaviour _mbSurrogate;

    /// <summary>
    /// A property which allows the (non MonoBehaviour) Ability class to use and cache a MonoBehaviour surrogate from the running instance to allow for coroutine calls.
    /// </summary>
    public static MonoBehaviour mbSurrogate {
        get {
            // if no surrogate is current cached or the current one is no longer active then we need to find one (preferably ABC_Controller) 
            if (_mbSurrogate == null || _mbSurrogate.gameObject.activeInHierarchy == false)
            {

                //Find our mbsurrogate script first as this will never be destroyed
                _mbSurrogate = GameObject.FindObjectOfType<ABC_MbSurrogate>();

                //if mbsurrogate can't be found from controller then find any monobehaviour
                if (_mbSurrogate == null)
                    _mbSurrogate = GameObject.FindObjectOfType<MonoBehaviour>();

            }

            // if a surrogate is still not gounf then an error has occured, tell user and return null.
            if (_mbSurrogate == null)
            {
                Debug.Log("No MonoBehaviour object was found in the scene to start coroutine from ability non monobehaviour class.");
                return null;

            }

            return _mbSurrogate;

        }


    }




    #endregion


    // ********************* Private Variables ********************
    #region Private Variables

    /// <summary>
    /// Unique ID counter used to generate a unique ID 
    /// </summary>
    private static int uniqueIDCounter = 0;

    /// <summary>
    /// Will keep track of all IEntity objects created in the game to be retrieved by any component at any time, helps with performance stopping scripts from having to remake entity objects in real time situations (surrounding checks)
    /// </summary>
    private static Dictionary<GameObject, ABC_IEntity> StaticABCEntities = new Dictionary<GameObject, ABC_IEntity>();

    /// <summary>
    /// The time that the StaticABCEntities list was last cleared
    /// </summary>
    private static float timeOfLastStaticABCEntitiesClear = 0f;

    /// <summary>
    /// How often in seconds that the StaticABCEntities list will be cleared (If 0 then list will never clear)
    /// </summary>
    private static float clearStaticABCEntitiesInterval = 600f;

    /// <summary>
    /// List of bones ignored by utilities functions
    /// </summary>
    private static List<int> ignoredBones = new List<int>{17, 18, 19, 20, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 
        38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 55};
    

    #endregion

    // ********************* Private Methods ********************
    #region Private Methods

#if UNITY_EDITOR
    /// <summary>
    /// Event handler to call functions when entering/exiting edit mode
    /// </summary>
    /// <param name="state">state of editor</param>
    private static void LogPlayModeState(PlayModeStateChange state) {
        switch (state) {
            case PlayModeStateChange.EnteredEditMode:
                RefreshCustomReferences();
                break;
            case PlayModeStateChange.ExitingEditMode:
                RefreshCustomReferences();
                break;
        }
    }

    /// <summary>
    /// Will invoke the method passed in the parameter for any properties on the object which has the method
    /// </summary>
    /// <param name="obj">Object to run property methods</param>
    /// <param name="MethodName">Name of method to run</param>
    private static void InvokeMethodOnAllProperties(object obj, string MethodName) {

        //Get type of the object
        var type = obj.GetType();

        //Search through all fields in the object
        foreach (FieldInfo fi in type.GetFields()) {

            //Grab the property value of the field
            object propertyValue = fi.GetValue(obj);

            //If property value is null then continue to next field
            if (propertyValue == null)
                continue;

            //Get method from the property value if it exists
            MethodInfo mi = propertyValue.GetType().GetMethod(MethodName);

            //If method exists then invoke it
            if (mi != null) {
                mi.Invoke(propertyValue, new object[] { });
            }

            //If the property is a list then recursivley call this function to check and run the method (if it exists) on all the properties of the objects
            if (propertyValue.GetType().IsGenericType) {

                //Get the list collection
                var collection = (IEnumerable)propertyValue;

                //Cycle through the list recalling this method recursivley to check the properties of the object to call the method
                foreach (var item in collection) {
                    //If object exists call method
                    if (item != null)
                        InvokeMethodOnAllProperties(item, MethodName);

                }
            }
        }
    }
#endif

    /// <summary>
    /// Will disable the object and place it back into the ABC Pool after the delay. This is private as it's called from PoolObject public method which handles the courotine
    /// </summary>
    /// <param name="Obj">Object to disable and pool</param>
    /// <param name="Delay">(Optional)Delay until object is disabled and placed in the pool</param> 
    private static IEnumerator PoolObjectAfterDuration(GameObject Obj, float Delay) {

        // if gameobject is not active then we can finish the method here
        if (Obj.activeInHierarchy == false)
            yield break;

        yield return new WaitForSeconds(Delay);

        Obj.transform.SetParent(abcPool.transform);

        Obj.SetActive(false);


    }

    #endregion

    // ********************* Public Methods ********************
    #region Public Methods


    /// <summary>
    /// Will call the refresh method on all custom references, updating their refname to match the value (refname is used for save/loading)
    /// <remarks>Called for example when a asset name changes, this will in turn call this method refreshing all the custom references resetting the refname to the new name of the asset</remarks>
    /// </summary>
    public static void RefreshCustomReferences() {
#if UNITY_EDITOR
        //Get all objects with ABC Components (in scene and files)
        foreach (GameObject obj in GetAllABCObjects(true)) {

            //Get controller component
            ABC_Controller abcController = obj.GetComponent<ABC_Controller>();

            //If controller found then call the refresh method on all custom reference properties
            if (abcController != null) {
                InvokeMethodOnAllProperties(abcController, "RefreshCustomReference");
                //Set dirty to save the changes
                EditorUtility.SetDirty(abcController);
            }

            //Get state manager component
            ABC_StateManager abcStateManager = obj.GetComponent<ABC_StateManager>();

            //If state manager found then call the refresh method on all custom reference properties
            if (abcStateManager != null) {
                InvokeMethodOnAllProperties(abcStateManager, "RefreshCustomReference");
                //Set dirty to save the changes
                EditorUtility.SetDirty(abcStateManager);
            }

        }
#endif
    }


    /// <summary>
    /// Generates a unique ID
    /// </summary>
    /// <returns>ID integer</returns>
    public static int GenerateUniqueID() {

        //Increment the uniqueid counter
        uniqueIDCounter += 1; 

        //Return ID made up of (1) 
        //last number of year  (0)
        //Day of the year      (140)
        //last number of minute (5)
        //UniqueIDCounter       (4)
        return int.Parse(("1") + System.DateTime.Now.ToString("yy").Substring(1, 1) + "" + System.DateTime.Now.DayOfYear + "" + System.DateTime.Now.ToString("mm").Substring(1, 1) + uniqueIDCounter.ToString());

    }

    /// <summary>
    /// Will add the ABC entity objects to a dictionary, tracked via the object as a key
    /// </summary>
    /// <param name="Obj">GameObject key</param>
    /// <param name="Entity">IEntity object which was made from the object</param>
    public static void AddStaticABCEntity(GameObject Obj, ABC_IEntity Entity) {

        //If IEntity object has not already been added then add it
        if (StaticABCEntities.ContainsKey(Obj) == false)
            StaticABCEntities.Add(Obj, Entity);

    }

    /// <summary>
    /// Will retrieve the IEntity object which links to the gameobject provided. If the IEntity object doesn't exist then one is created and returned
    /// </summary>
    /// <param name="Obj">Gameobject of the IEntity object</param>
    /// <returns>The IEntity Object linked to the gameobject, else creates a new one and returns that</returns>
    public static ABC_IEntity GetStaticABCEntity(GameObject Obj) {

        //If the interval defined is over 0 and the interval has passed since the last clean out then  clear the StaticABCEntities list to stop it growing too big with objects no longer in game. 
        if (clearStaticABCEntitiesInterval > 0 && Time.time - timeOfLastStaticABCEntitiesClear > clearStaticABCEntitiesInterval) {
            //clear list to be made fresh 
            StaticABCEntities.Clear();
            //Record time of last clear
            timeOfLastStaticABCEntitiesClear = Time.time; 
        }


        if (StaticABCEntities.ContainsKey(Obj) == true)
            return StaticABCEntities[Obj];
        else if (Obj.transform.parent != null && StaticABCEntities.ContainsKey(Obj.transform.parent.gameObject))
            return StaticABCEntities[Obj.transform.parent.gameObject];
        else
            return new ABC_IEntity(Obj);


    }

    /// <summary>
    /// Will refresh all ABC entities ensuring that all properties are up to date
    /// </summary>
    public static void RefreshAllABCEntities() {

        foreach (ABC_IEntity entity in StaticABCEntities.Values)
            entity.RefreshEntity();

    }

    /// <summary>
    /// Will return all ABC Entities that are in range of the starting position provided
    /// </summary>
    /// <param name="StartingPosition">Starting position to check distance to</param>
    /// <param name="Range">The range in which entities have to be within the starting position to be returned</param>
    /// <returns></returns>
    public static List<ABC_IEntity> GetAllABCEntitiesInRange(Vector3 StartingPosition, float Range) {

        //Find all entities that are in range of the starting position provided
        return StaticABCEntities.Values.Where(e => e.transform != null && Vector3.Distance(StartingPosition, e.transform.position) <= Range).ToList();


    }

    /// <summary>
    /// Returns a bool indicating if the object has any tags that match the tags in the list provided
    /// </summary>
    /// <remarks>
    /// Will check the objects tags and ABC tags created in the statemanager component
    /// </remarks>
    /// <param name="Obj">Object which will be checked to see if it has a tag matching any elements in the list provided</param>
    /// <param name="TagList">List of string tags</param>
    /// <returns>True if the object's tags match any element in the taglist provided, else false.</returns>
    public static bool ObjectHasTag(GameObject Obj, List<string> TagList) {

        //If object not provided then return false
        if (Obj == null)
            return false; 

        // get iEntity to check for ABC tags
        ABC_IEntity iEntity = GetStaticABCEntity(Obj);


        // check if list matches any ABC tags
        if (iEntity.HasABCTag(TagList))
            return true;


        // loop through taglist and return true if object has a normal tag
        foreach (string element in TagList) {

            if (Obj.tag == element)
                return true;

        }


        // went through tag list and no match was found so we return false; 
        return false;
    }

    /// <summary>
    /// Returns a bool indicating if the object has any tags that match the tag provided
    /// </summary>
    /// <remarks>
    /// Will check the objects tags and ABC tags created in the statemanager component
    /// </remarks>
    /// <param name="Obj">Object which will be checked to see if it has a tag matching any elements in the list provided</param>
    /// <param name="Tag">string tag to check</param>
    /// <returns>True if the object's tags match any element in the tag provided, else false.</returns>
    public static bool ObjectHasTag(GameObject Obj, string Tag) {

        //If object not provided then return false
        if (Obj == null)
            return false;

        // get iEntity to check for ABC tags
        ABC_IEntity iEntity = GetStaticABCEntity(Obj);

        // we can only deal with objects which have a state manager script
        if (iEntity.HasABCTag(Tag))
            return true;


        if (Obj.tag == Tag)
            return true;

       

        // went through tag list and no match was found so we return false; 
        return false;
    }


    /// <summary>
    /// Will search the hiearchy within the object provided returning the first object to match the tag provided. 
    /// </summary>
    /// <param name="ObjectToTraverse">Parent Object which will be looked through for a tag</param>
    /// <param name="Tag">Tag to match</param>
    /// <param name="IncludeParent">(Optional) If true then parent object will be checked to see if it has the correct tag, else function will only focus on children</param>
    /// <returns>First Gameobject encountered which has the matching tag </returns>
    public static GameObject TraverseObjectForTag(GameObject ObjectToTraverse, string Tag, bool IncludeParent = true){

        //Get children
        List<Transform> linkedObjects = new List<Transform>();
        linkedObjects = ObjectToTraverse.GetComponentsInChildren<Transform>().ToList();

        //If parameter set then include parent
        if (IncludeParent == true && ObjectToTraverse.transform.parent != null)
            linkedObjects.Add(ObjectToTraverse.transform.parent.GetComponent<Transform>());

        //Then search through all object returning the first object matching the tag
        foreach (Transform obj in linkedObjects) if (obj.CompareTag(Tag)) 
                return obj.gameObject;
     
        //If reached this far then tag can't be found so return null
        return null;
    }



    /// <summary>
    /// Will search the hiearchy within the object provided searching for the component provided in the parameter, returning the first one found
    /// </summary>
    /// <remarks>Will look in following order: Object, Parent of Object, Root of Object, Children in Parent, Children in Root</remarks>
    /// <param name="ObjectToTraverse">Parent Object which will be looked through for a tag</param>
    /// <returns>First statemanager component encountered</returns>
    public static Component TraverseObjectForComponent(GameObject ObjectToTraverse, System.Type ComponentToFind){


        // find state script on the object 
        Component retVal =  ObjectToTraverse.transform.GetComponent(ComponentToFind);

        //If doesn't exist then try children
        if (retVal == null)
            retVal = ObjectToTraverse.GetComponentInChildren(ComponentToFind);

        //return what we have found
        return retVal;
    }

    /// <summary>
    /// Will disable the object and place it back into the ABC Pool
    /// </summary>
    /// <param name="Obj">Object to disable and pool</param>
    /// <param name="Delay">(Optional)Delay until object is disabled and placed in the pool</param> 
    public static void PoolObject(GameObject Obj, float Delay = 0f){

        if (Obj == null)
            return; 


        //If a duration has been given then pool object after the duration
        if (Delay > 0f){
            mbSurrogate.StartCoroutine(PoolObjectAfterDuration(Obj, Delay));
            return;
        }

        // else no duration so we can set inactive and pool straight away
        Obj.transform.SetParent(abcPool.transform); 
        Obj.SetActive(false);


    }

    /// <summary>
    /// Will retrieve all objects in the game which contain either the ABC_Controller or ABC_StateManager component
    /// </summary>
    /// <param name="GetObjectsNotInScene">If true then objects not in scene (prefabs etc) are also returned</param>
    /// <returns></returns>
    public static List<GameObject> GetAllABCObjects(bool GetObjectsNotInScene = false){

        //Collect all objects in scene that relate to ABC (Making sure not to repeat ourselves)
        List<GameObject> retval = new List<GameObject>();
        retval.AddRange((Resources.FindObjectsOfTypeAll(typeof(ABC_Controller)) as ABC_Controller[]).Select(a => a.gameObject).ToList());
        retval.AddRange((Resources.FindObjectsOfTypeAll(typeof(ABC_StateManager)) as ABC_StateManager[]).Select(b => b.gameObject).Except(retval).ToList());
        retval.AddRange((Resources.FindObjectsOfTypeAll(typeof(ABC_WeaponPickUp)) as ABC_WeaponPickUp[]).Select(c => c.gameObject).Except(retval).ToList());


        //Remove all persistant objects (prefabs etc)
        if (GetObjectsNotInScene == false)
            retval = retval.Select(item => item.gameObject).Where(obj => obj.scene.name != null).ToList();

        return retval;

    }


    /// <summary>
    /// Will find all objects in the current scene
    /// </summary>
    /// <returns>List of GameObjects in the scene</returns>
    public static List<GameObject> GetAllObjectsInScene() {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;

            objectsInScene.Add(go);
        }

        objectsInScene = objectsInScene.Select(item => item.gameObject).Where(obj => obj.scene.name != null).ToList();

        return objectsInScene;
    }

    /// <summary>
    /// Will find all objects with the tag
    /// </summary>
    /// <returns>List of GameObjects in the scene</returns>
    public static List<GameObject> GetAllObjectsWithTag(string Tag)
    {

        return GameObject.FindGameObjectsWithTag(Tag).ToList();
    }

    /// <summary>
    /// From the position provided the function will return the closest mesh surface on the entity
    /// </summary>
    /// <param name="Entity">Entity to compare mesh surface distance</param>
    /// <param name="Position">Position to compare which mesh surface is closest to it</param>
    /// <returns>The transform of the closest mesh surface found, if not found then the same position is returned</returns>
    public static Vector3 GetClosestSurfaceFromPosition(ABC_IEntity Entity, Vector3 Position)  {

        MeshFilter entityMeshFilter = Entity.transform.GetComponentInChildren<MeshFilter>();

        if (entityMeshFilter == null)
            return Position;


        // convert position to local space
        Vector3 localPosition = Entity.transform.InverseTransformPoint(Position);

        float currentClosestDistance = 0f;
        Vector3 retVal = Vector3.zero;

        // scan all vertices to find nearest
        foreach (Vector3 vertex in entityMeshFilter.mesh.vertices){

            
            if (currentClosestDistance == 0 || Vector3.Distance(localPosition, vertex) < currentClosestDistance){
                currentClosestDistance = Vector3.Distance(localPosition, vertex);
                retVal = vertex;
            }
        }

        // convert nearest vertex back to world space
        retVal = Entity.transform.TransformPoint(retVal);

        //If distance is too big then the model has not got detailed vertices so we will just return the same position
        if (Vector3.Distance(retVal, Position) > 3)
            return Position;
        else
            return retVal; 

    }




    /// <summary>
    /// From the position provided the function will return the closest bone on the entity
    /// </summary>
    /// <param name="Entity">Entity to compare bone distance</param>
    /// <param name="Position">Position to compare which bone is closest to it</param>
    /// <returns>The transform of the closest bone found, if not found then null is returned</returns>
    public static Transform GetClosestBoneFromPosition(ABC_IEntity Entity, Vector3 Position){

        //If entity hasn't got state manager then finish here
        if (Entity.HasABCStateManager() == false)
            return null;

        //Store animator as we will be referencing it a lot
        Animator Ani = Entity.animator;

        //If entity doesn't have an animator then return null
        if (Ani == null)
            return null;

        Transform retVal = null;
        float currentClosestDistance = 0f;

        //Loop through every bone
        foreach (HumanBodyBones boneID in System.Enum.GetValues(typeof(HumanBodyBones)))  {

            //If we reached last bone then continue (GetBoneTransform function errors if last bone enum inputted)
            if (ignoredBones.Contains((int)boneID))
                continue; 


            //Get bone from transform
            Transform bone = Ani.GetBoneTransform(boneID);

            //If no bone found then continue
            if (bone == null)
                continue;

            //If first time storing (closest current distance is 0) or the new bone is closer then record it ready to be returned
            if (currentClosestDistance == 0f || Vector3.Distance(bone.position, Position) < currentClosestDistance){
                retVal = bone;
                currentClosestDistance = Vector3.Distance(bone.position, Position);
            }

        }


        return retVal; 

    }


    /// <summary>
    /// Will apply a new shader to the object provided, returning the old shader which was replaced.
    /// </summary>
    /// <param name="Obj">Object to apply shader too</param>
    /// <param name="Shader">Shader to apply</param>
    /// <returns>The old shader which was replaced by the new shader</returns>
    /// <remarks>Old shader is returned incase this is to be reapplied at a later date</remarks>
    public static Shader ApplyShader(GameObject Obj, Shader Shader) {

        //If either the object or shader is null then end here
        if (Obj == null || Shader == null)
            return null;

        //Collect the renderer
        Renderer objRenderer = Obj.GetComponentInChildren<Renderer>(true);

        if (objRenderer == null)
            return null;


        //Track the current shader to return
        Shader retVal = objRenderer.material.shader;

        //Apply the shader
        objRenderer.material.shader = Shader;

        return retVal;

    }


    /// <summary>
    /// Rolls a dice to generate a number. If the number is between the min and max then it will return true, else false
    /// </summary>
    /// <param name="Min">Minimum value the roll has to be above to return true</param>
    /// <param name="Max">Maximum value the roll has to be below to return true</param>
    /// <returns>True if the dice roll was between the min and max value, else false</returns>
    public static bool DiceRoll(float Min, float Max) {
        float dice = Random.Range(0f, 100f);

        if (dice >= Min && dice <= Max) {
            return true;
        } else {
            return false;
        }


    }


    /// <summary>
    /// Will modify the game speed for the duration given
    /// </summary>
    /// <param name="SpeedFactor">Value to change the game speed to (lower the number the slower the speed)</param>
    /// <param name="Duration">How long to modify the game speed for</param>
    /// <param name="Delay">Delay before the game speed is modified</param>
    public static IEnumerator ModifyGameSpeed(float SpeedFactor, float Duration, float Delay = 0f) {

        //If we are already in the middle of a slow motion then end here 
        if (Time.timeScale != 1f) 
            yield break;

        //wait for any delay
        if (Delay > 0f)
            yield return new WaitForSeconds(Delay);

        Time.timeScale = SpeedFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        while (Time.timeScale != 1f) {
            yield return new WaitForEndOfFrame();

            //If we slowed down the game time then gradually increase the speed till we get to 1
            if (Time.timeScale < 1f) {
                Time.timeScale += (1f / Duration) * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            } else {
            //If we have sped the game up then gradually decrease speed till we get to 1
                Time.timeScale -= (1f / Duration) * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Max(Time.timeScale, 1f);
            }

            Time.fixedDeltaTime = Time.timeScale * 0.02f;


        }


    }

    #endregion


    // ********************* Asset Integration - Editor Only Code ********************


#if UNITY_EDITOR
   

    /// <summary>
    /// Returns a bool indicating if the define symbol has been added to the project
    /// </summary>
    /// <param name="Define">Name of the scripting define symbol i.e ABC_GC_Integration</param>
    /// <returns>True if the define symbol exists, else false</returns>
    public static bool IntegrationDefineSymbolExists(string Define) {

        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = defines.Split(';').ToList();

        if (allDefines.Contains(Define))
            return true;
        else
            return false; 

    }

    /// <summary>
    /// Add scripting define symbols for integration - if correct symbol is added then code for integrations will compile
    /// else it is ignored
    /// </summary>
    /// <param name="Define">Name of the scripting define symbol i.e ABC_GC_Integration</param>
    public static void AddIntegrationDefineSymbols(string Define)
    {

        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = defines.Split(';').ToList();

        if (allDefines.Contains(Define)){
            Debug.LogWarning("Selected build target (" + EditorUserBuildSettings.activeBuildTarget.ToString() + ") already contains <b>" + Define + "</b> <i>Scripting Define Symbol</i>.");
            return;
        }

        allDefines.Add(Define);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));

        Debug.LogWarning("<b>" + Define + "</b> added to <i>Scripting Define Symbols</i> for selected build target (" + EditorUserBuildSettings.activeBuildTarget.ToString() + ").");
        
    }

    /// <summary>
    /// remove scripting define symbols for integration - if correct symbol is added then code for integrations will compile
    /// else it is ignored
    /// </summary>
    /// <param name="Define">Name of the scripting define symbol i.e ABC_GC_Integration</param>
    public static void RemoveIntegrationDefineSymbols(string Define)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = defines.Split(';').ToList();

        if (allDefines.Contains(Define)){
            allDefines.Remove(Define);
        } else {
            //Wasn't added already so can just end here
            return; 
        }


        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));

        Debug.LogWarning("<b>" + Define + "</b> removed from <i>Scripting Define Symbols</i> for selected build target (" + EditorUserBuildSettings.activeBuildTarget.ToString() + ").");
    }
#endif
}





// ********************* Shared ENUMS ********************
#region Shared ENUMS


//What ability is doing - the event 
public enum AbilityEvent {
    Activation,
    Collision
}


// only used in ability class
public enum AbilityPositionGraphicType {
    SpawnObject,
    End
}




public enum AbilityType {
    Projectile = 0,
    RayCast = 1,
    Melee = 2
}


public enum InputType {
    Key = 0,
    Button = 1
}

public enum TriggerType { 
    Input = 0,
    InputCombo = 1
}


public enum AnimationMethod{
    ABCAnimationRunner = 0, 
    Animator = 1
}


public enum AnimatorParameterType {
    Float = 0,
    integer = 1,
    Bool = 2,
    Trigger = 3
}



public enum TravelType {
    Forward = 0,
    SelectedTarget = 1,
    Self = 2,
    ToWorld = 3, // fires to ground in world (maybe rename - think TSW ground target spells) 
    Mouse2D = 4,
    NoTravel = 5,
    Crosshair = 6, // fires to crosshair position setup on screen
    MouseForward = 7, // rotates to mouse position and fires forward (like forward) 
    CustomScript = 8, // custom script made by user 
    MouseTarget = 9, // uses selected target to travel to mouse position (without forcing character to rotate like mouse forward)
    NearestTag = 10 // uses selected target to travel to the nearest tag within a range 
}



// Starting postion where particle will spawn 
public enum StartingPosition {
    Self = 0,
    Target = 1,
    OnObject = 2,
    OnWorld = 3,
    CameraCenter = 4,
    OnTag = 5,
    OnSelfTag = 6
}


// how do spells act when collidng with other spells
public enum AbilityCollisionIgnores {
    IgnoreAll = 0,
    IgnoreForeignAbilities = 1,
    IgnoreSelfAbilities = 2,
    IgnoreNone = 3,
    AlwaysCollideAll = 4,
    AlwaysCollideForeignAbilities = 5,
    AlwaysCollideSelfAbilities = 6
}

// on Impact do we destroy
public enum ImpactDestroy {
    DestroyOnAll = 0,
    DestroyOnABCProjectiles = 1,
    DestroyOnABCStateManagers = 2,
    DestroyOnAllABC = 3,
    DestroyOnAllNotABC = 4,
    DestroyOnTargetOnly = 5,
    DestroyOnTerrainOnly = 6,
    DontDestroy = 7,
    DestroyOnAllNotABCProjectile = 8
}



public enum TargetSelectType {
    None = 0,
    Mouse = 1,
    Crosshair = 2
}

public enum TargetType {
    Target,
    Mouse,
    World
}

// type of bounce targeting 
public enum BounceTarget {
    NearestABCStateManager = 0,
    NearestObject = 1,
    NearestTag = 2
}


public enum LoggingType {


    ReadyToCastAgain = 0,
    Range = 1, 
    FacingTarget = 2,
    FpsSelection = 3,
    TargetSelection = 4,
    SoftTargetSelection = 5,
    WorldSelection = 6,
    AbilityActivationError = 7,
    NoMana = 8,
    Preparing = 9,
    AbilityInterruption = 10,
    Initiating = 11,
    AbilityActivation = 12,
    ScrollAbilityEquip = 13,
    AmmoInformation = 14, 
    BlockingInformation = 15, 
    WeaponInformation = 16, 
    ParryInformation = 17

}


public enum CollisionType {
    OnStay,
    OnExit,
    OnEnter,
    Particle,
    None
}


//integrations 

public enum ABCIntegrationType{
    ABC = 0, 
    GameCreator = 1,
    EmeraldAI = 2
}

//Game Creator Integration
public enum GCStatType
{
    Stat = 1,
    Attribute = 2
}

#endregion