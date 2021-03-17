using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager : MonoBehaviour
{
	public GameObject[] panels;
	public CharacterModel[] characterModel;
	public RuntimeAnimatorController animatorController;
	public int characterNumber;
	public int skinNumber;
	public GameObject basherContainer;
	public GameObject[] tabs;
	public GameObject badgeContainer;
	public GameObject badgePrefab;
	public int myLevel = 1;
	private List<GameObject> characters;
	private static AvatarManager instance;

	public static AvatarManager Instance
	{
		get
		{
			return instance;
		}
	}

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	void Start()
	{
		characters = new List<GameObject>();
		for (int i = 0; i < characterModel.Length; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				GameObject go = Instantiate(characterModel[i].characterModel[j]);
				go.transform.parent = basherContainer.transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
				go.GetComponent<Animator>().enabled = true;
				go.GetComponent<Animator>().runtimeAnimatorController = animatorController;
				go.SetActive(false);
				foreach (Renderer ren in go.GetComponentsInChildren<Renderer>())
				{
					ren.renderingLayerMask = 1;
				}
				characters.Add(go);
			}
		}
		characters[0].SetActive(true);
	}

	public void OpenPanel(int i)
	{
		foreach (GameObject p in panels)
			p.SetActive(false);
		panels[i].SetActive(true);
		if (i == 3)
			SetDefaultPos();
		if (i == 0 || i == 3)
			basherContainer.SetActive(false);
		else
			basherContainer.SetActive(true);
	}

	public void ChooseCharacter(int i)
	{
		foreach (GameObject go in characters)
			go.SetActive(false);
		characters[i * 3].SetActive(true);
		characterNumber = i;
	}

	public void ChooseSkin(int i)
	{
		foreach (GameObject go in characters)
			go.SetActive(false);
		characters[characterNumber * 3 + i].SetActive(true);
		skinNumber = i;
	}

	public void ChooseTab(int i)
	{
		foreach (GameObject go in tabs)
			go.SetActive(false);
		tabs[i].SetActive(true);
	}

	public void SetDefaultPos()
	{
		myLevel = GetComponent<BadgeManager>().myLevel;
		Vector3 newPos = badgeContainer.transform.localPosition;
		newPos.x = -(300 * myLevel - 960 + 150 + 480);
		//newPos.x = -(trophies * 60 - 960 + 150);
		badgeContainer.transform.localPosition = newPos;
	}
}
