using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Will spawn and respawn a group of entities on the condition that all have been disabled, will simulate waves of enemies (The next group won't spawn until the last of the last bacth is disabled) 
/// </summary>
public class GroupSpawner : MonoBehaviour {

    // ************ Settings *****************************

    #region Settings


    [Header("Spawning Settings")]
    /// <summary>
    /// The list of objects to spawn and respawn
    /// </summary>
    public List<GameObject> spawnEntities = new List<GameObject>();

    /// <summary>
    /// The delay before the objects are spawned/respawned
    /// </summary>
    public float spawnDelay = 4f;

    /// <summary>
    /// How far from current spot can the entity spawn randomly
    /// </summary>
    public float randomSpawnRadius = 5f;



    [Header("Spawn Graphic Settings")]
    /// <summary>
    /// The graphic which is played when the object spawns
    /// </summary>
    public GameObject spawnGraphic;

    /// <summary>
    /// Gameobject which will pool all the graphics
    /// </summary>
    public GameObject objectPool;

    /// <summary>
    /// How long the graphic is activated for
    /// </summary>
    public float graphicDuration = 3f;

    /// <summary>
    /// Offset of the spawn graphic
    /// </summary>
    public Vector3 graphicOffset;

    /// <summary>
    /// The delay between the graphic activating and the entity starting
    /// </summary>
    public float delayBetweenGraphicAndSpawn = 0.2f;
    


    #endregion

    // ********************* Variables ******************

    #region Variables

    /// <summary>
    /// Determines if entities are currently being spawned
    /// </summary>
    private bool spawning = false;

    /// <summary>
    /// Will track all objects placed in the graphic pool
    /// </summary>
    private List<GameObject> graphicPool = new List<GameObject>();

    #endregion



    // ********************* Public Methods ********************

    #region Public Methods

    /// <summary>
    /// Will respawn all entities
    /// </summary>
    public void RespawnEntities() {

        if (spawning == true)
            return;

        foreach (GameObject obj in this.spawnEntities)
            StartCoroutine(this.SpawnObject(obj));

    }

    #endregion


    // ********************* Private Methods ********************

    #region Private Methods

    /// <summary>
    /// Will create and pool the spawn graphics setup 
    /// </summary>
    /// <param name="CreateOne">If true then only one extra graphic will be created and returned</param>
    /// <returns>One graphic gameobject which has been created</returns>
    private GameObject CreateSpawnGraphics(bool CreateOne = false) {

        GameObject graphic = null;

        if (this.spawnGraphic != null) {

            //how many objects to make
            float objCount = CreateOne ? 1 : this.spawnEntities.Count + 3;

            for (int i = 0; i < objCount; i++) {
                // create object particle 
                graphic = (GameObject)(GameObject.Instantiate(this.spawnGraphic));

                // place object under our pool object
                graphic.transform.SetParent(this.objectPool.transform);
                graphic.SetActive(false);

                // add to generic list. 
                this.graphicPool.Add(graphic);
            }
        }

        return graphic;
    }


    /// <summary>
    /// Will activate the spawn graphic at the entities position
    /// </summary>
    private IEnumerator ActivateSpawnGraphic(GameObject spawnedEntity) {

        GameObject graphicObj = this.graphicPool.Where(obj => obj.activeInHierarchy == false).OrderBy(obj => UnityEngine.Random.value).FirstOrDefault();

        if (graphicObj == null)
            graphicObj = CreateSpawnGraphics(true);


        //detatch from pool 
        graphicObj.transform.parent = null;

        //place object at entity position
        graphicObj.transform.position = spawnedEntity.transform.position + this.graphicOffset;

        //Set active 
        graphicObj.SetActive(true);

        //Wait for duration
        yield return new WaitForSeconds(this.graphicDuration);

        //return to pool 
        graphicObj.SetActive(false);
        graphicObj.transform.SetParent(this.objectPool.transform);

    }

    /// <summary>
    /// Function determines if all entites have been disabled 
    /// </summary>
    /// <returns>True if all entities tracked are disabled, else false</returns>
    private bool AllEntitiesDisabled() {

        bool retval = true; 

        foreach (GameObject obj in this.spawnEntities) {
            if (obj.activeInHierarchy)
                retval = false;
        }

        return retval;
    }


    /// <summary>
    /// Function will spawn the object provided in a random position from where it last despawned
    /// </summary>
    /// <param name="SpawnObject"></param>
    /// <returns></returns>
    private IEnumerator SpawnObject(GameObject SpawnObject) {


        this.spawning = true;


        //Wait for spawn delay
        yield return new WaitForSeconds(this.spawnDelay);

        Vector3 randomPosition = SpawnObject.transform.position + SpawnObject.transform.forward * Random.Range(0, this.randomSpawnRadius) + SpawnObject.transform.right * Random.Range(-this.randomSpawnRadius, this.randomSpawnRadius);

        SpawnObject.transform.position = randomPosition;
        StartCoroutine(this.ActivateSpawnGraphic(SpawnObject));

        //Wait for delay between graphic and spawn
        yield return new WaitForSeconds(this.delayBetweenGraphicAndSpawn);

        SpawnObject.SetActive(true);


        this.spawning = false;
    }


    #endregion


    // ********************* Game ********************

    #region Game


    // Use this for initialization
    void Start() {

        //pool graphics
        this.CreateSpawnGraphics();

    }

    // Update is called once per frame
    void Update() {

        if (this.AllEntitiesDisabled())
           this.RespawnEntities();

    }

    #endregion


}
