using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{

	public static ObjectPoolerManager Instance;
	public Transform pooledObjectsParent;
	public List<GameObject> pooledObjects;
	public GameObject objectToPool;
	public int amountToPool;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		pooledObjects = new List<GameObject>();
		for(int i = 0; i < amountToPool; i++)
		{
			GameObject obj = (GameObject)Instantiate(objectToPool);
			obj.SetActive(false);
			obj.transform.parent = pooledObjectsParent;
			pooledObjects.Add(obj);
		}        
    }

	public GameObject GetPooledObject()
	{
		for (int i = 0; i < pooledObjects.Count; i++)
		{
			if (!pooledObjects[i].activeInHierarchy)
			{
				return pooledObjects[i];
			}
		}
		return null;
	}
}
