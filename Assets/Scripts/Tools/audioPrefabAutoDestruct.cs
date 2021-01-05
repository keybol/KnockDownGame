using UnityEngine;
using System.Collections;

/// <summary>
/// Simple auto destruct component for effect GameObjects like sparks / blood that use particle systems that helps them to be used with the ObjectPoolManager.
/// </summary>
public class audioPrefabAutoDestruct : MonoBehaviour
{
	AudioSource audioSource;
	float wait = 3f;
	// Use this for initialization
	void Start()
	{
	}

	public void WaitAndDeactivate()
	{
		wait = 3f;
		//if(audioSource.clip != null) wait = audioSource.clip.length;
	}
	// Update is called once per frame
	void Update()
	{
		wait -= Time.deltaTime; //reverse count
		if (wait < 0f)
		{
			gameObject.SetActive(false);
		}
	}
}
