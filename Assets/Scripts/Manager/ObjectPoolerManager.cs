using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{

	public static ObjectPoolerManager Instance;
	public Transform pooledObjectsParent;
	public List<GameObject> pooledAudioObjects;
	public GameObject audioObjectToPool;
	public int audioAmountToPool;
	public List<GameObject> pooledLandSmokeObjects;
	public GameObject landSmokeObjectToPool;
	public int landSmokeAmountToPool;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		pooledAudioObjects = new List<GameObject>();
		for (int i = 0; i < audioAmountToPool; i++)
		{
			GameObject obj = (GameObject)Instantiate(audioObjectToPool);
			obj.SetActive(false);
			obj.transform.parent = pooledObjectsParent;
			pooledAudioObjects.Add(obj);
		}
		pooledLandSmokeObjects = new List<GameObject>();
		for(int i = 0; i < landSmokeAmountToPool; i++)
		{
			GameObject obj = (GameObject)Instantiate(landSmokeObjectToPool);
			obj.SetActive(false);
			obj.transform.parent = pooledObjectsParent;
			pooledLandSmokeObjects.Add(obj);
		}
	}

	public GameObject GetPooledAudioObject()
	{
		for (int i = 0; i < pooledAudioObjects.Count; i++)
		{
			if (!pooledAudioObjects[i].activeInHierarchy)
			{
				return pooledAudioObjects[i];
			}
		}
		return null;
	}

	public GameObject GetPooledLandSmokeObject()
	{
		for (int i = 0; i < pooledLandSmokeObjects.Count; i++)
		{
			if (!pooledLandSmokeObjects[i].activeInHierarchy)
			{
				return pooledLandSmokeObjects[i];
			}
		}
		return null;
	}
}
