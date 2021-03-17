using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
	public int EXP = 0;
	public int[] levelsUnlocked = new int[40];
	public int gems = 0;
	public int coins = 0;
	public int trophies = 0;
	private static GamePlayer instance;
	public static GamePlayer Instance
	{
		get
		{
			if (instance == null)
			{
				GamePlayer resource = Resources.Load<GamePlayer>("Managers/GamePlayerManager");
				instance = Instantiate(resource);
			}
			return instance;
		}
	}

	void Awake()
	{
		if (instance == null)
		{
			instance = this;

			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
			Destroy(gameObject);
			Debug.Log("Error instance is not null", instance);
		}
	}

	public void SavePlayer()
	{
	}

	public void LoadPlayer()
	{
	}
}
