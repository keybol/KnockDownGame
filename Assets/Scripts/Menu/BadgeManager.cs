using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BadgeManager : MonoBehaviour
{
	[SerializeField] bool notMenu;
	[SerializeField] int[] expLevel;
	[SerializeField] GameObject badgeContainer;
	[SerializeField] GameObject badgePrefab;
	[SerializeField] GameObject currentTrophies;
	[SerializeField] TextMeshProUGUI trophyCountText;
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] TextMeshProUGUI EXPText;
	[SerializeField] TextMeshProUGUI levelRankText;
	[SerializeField] TextMeshProUGUI levelText;
	[SerializeField] Sprite crown;
	[SerializeField] Sprite coins;
	[SerializeField] int[] coinRewards = new int[10];
	[SerializeField] int[] crownRewards = new int[10];
	[SerializeField] AvatarManager avatarManager;
	[SerializeField] Image EXPFill;
	[SerializeField] public Sprite diamondColored;
	[SerializeField] public Sprite diamondBlack;
	[SerializeField] public Slider sliderValue;
	GamePlayer gamePlayerInstance;
	public List<BadgeVariables> allBadges;
	int trophies;
	int totalTrophies;
	int level;
	public int myLevel = 1;
	int nextDiff;
	int excessTrophies;

	private static BadgeManager instance;
	public static BadgeManager Instance
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
		totalTrophies = expLevel[expLevel.Length - 1];
		gamePlayerInstance = GamePlayer.Instance;
		if (!notMenu)
			InitiateRanksRewards();
	}

	private void InitiateRanksRewards()
	{
		for (int i = 0; i <= totalTrophies; i += 10)
		{
			if (i == expLevel[level])
			{
				GameObject badge = Instantiate(badgePrefab, badgeContainer.transform);
				BadgeVariables badgeVariables = badge.GetComponent<BadgeVariables>();
				badgeVariables.trophiesText.text = "" + i;
				if (i == expLevel[expLevel.Length - 1])
					badgeVariables.blackBar.gameObject.SetActive(false);
				allBadges.Add(badgeVariables);
				level += 1;
				if (level == 1)
				{
					badgeVariables.rewardImage.gameObject.SetActive(false);
					badgeVariables.rewardQuantity.gameObject.SetActive(false);
				}
				else
				{
					if ((level + 2) % 2 == 0)
					{
						badgeVariables.freeUnlock.SetActive(false);
					}
					if ((level + 3) % 4 == 0)
					{
						badgeVariables.rewardImage.sprite = crown;
						badgeVariables.rewardQuantity.text = crownRewards[level / 4].ToString();
					}
					if ((level + 2) % 4 == 0)
					{
						badgeVariables.rewardImage.sprite = coins;
						badgeVariables.rewardQuantity.text = coinRewards[level / 4].ToString();
					}
					if ((level + 1) % 4 == 0)
					{
						//badgeVariables.rewardImage.sprite = tauntModel.danceSprite[Mathf.RoundToInt(level / 4)];
						//badgeVariables.rewardQuantity.gameObject.SetActive(false);
					}
					if (level % 4 == 0)
					{
						//badgeVariables.rewardImage.sprite = characterPack.characterSprites[262 + Mathf.RoundToInt(level / 4)];
						//badgeVariables.rewardQuantity.gameObject.SetActive(false);
					}
				}
				badgeVariables.thisLevel.text = level.ToString();
			}
		}
	}

	public void SetDefaultPos()
	{
		Vector3 newPos = badgeContainer.transform.localPosition;
		newPos.x = -(300 * myLevel - 960 + 150);
		//newPos.x = -(trophies * 60 - 960 + 150);
		badgeContainer.transform.localPosition = newPos;
	}

	void Update()
	{
		trophies = gamePlayerInstance.EXP;
		trophies = Mathf.Clamp(trophies, 0, expLevel[expLevel.Length - 1]);
		if (trophyCountText)
			trophyCountText.text = trophies + "XP";
		if (myLevel < expLevel.Length)
		{
			nextDiff = expLevel[myLevel] - expLevel[myLevel - 1];
			excessTrophies = trophies - expLevel[myLevel - 1];
		}
		EXPText.text = excessTrophies + "/" + nextDiff;
		float fillRatio = (float)excessTrophies / (float)nextDiff;
		EXPFill.fillAmount = fillRatio;
		if (myLevel >= expLevel.Length)
		{
			EXPText.text = "MAX LEVEL";
		}
		for (int i = 0; i < 40; i += 1)
		{
			float diff = 0;
			if (i < allBadges.Count - 1)
				diff = expLevel[i + 1] - expLevel[i];
			float barValue = (trophies - expLevel[i]) / diff;
			if (!notMenu)
			{
				allBadges[i].barFilled.fillAmount = barValue;
				if (i < allBadges.Count - 1)
				{
					if (allBadges[i].barFilled.fillAmount == 1)
						allBadges[i + 1].levelDiamond.sprite = diamondColored;
					else
						allBadges[i + 1].levelDiamond.sprite = diamondBlack;
				}
			}
			if (trophies >= expLevel[i])
			{
				myLevel = i + 1;
				levelText.text = myLevel.ToString();
				if (levelRankText)
					levelRankText.text = myLevel.ToString();
				if (!notMenu)
				{
					float ratio = 0;
					if (trophies < expLevel[expLevel.Length - 1])
						ratio = 300 * barValue;
					float posX = 300 * i + ratio + badgeContainer.transform.localPosition.x - 960 + 150;
					currentTrophies.transform.localPosition = new Vector3(posX, currentTrophies.transform.localPosition.y, 0);
					if (gamePlayerInstance.levelsUnlocked[i] == 0)
					{
						UnlockReward(myLevel);
						gamePlayerInstance.levelsUnlocked[i] = 1;
						gamePlayerInstance.SavePlayer();
					}
				}
			}
		}
		gamePlayerInstance.EXP = Mathf.FloorToInt(sliderValue.value);
		var beltNumber = Mathf.Clamp((myLevel / 40f) * 6f, 0, 5);
		avatarManager.Belt.sprite = avatarManager.belts[Mathf.FloorToInt(beltNumber)];
	}

	private void UnlockReward(int val)
	{
		if ((val + 3) % 4 == 0)
		{
			gamePlayerInstance.trophies += crownRewards[val / 4];
		}
		if ((val + 2) % 4 == 0)
		{
			gamePlayerInstance.coins += coinRewards[val / 4];
		}
		if ((val + 1) % 4 == 0)
		{
			int taunt = (val - 3) / 4;
			//gamePlayerInstance.unlockedTaunts[taunt] = 1;
			//DanceButtonController tauntButton = avatarManager.danceButtonList[taunt + 22].transform.GetComponent<DanceButtonController>();
			//tauntButton.unlocked = true;
			//tauntButton.lockObject.SetActive(false);
			//tauntButton.rankUnlocked.SetActive(false);
		}
		if (val % 4 == 0)
		{
			int epicSkin = (val - 4) / 4;
			//gamePlayerInstance.unlockedEpicSkins[epicSkin] = 1;
			//SkinButtonController skinButton = avatarManager.skinButtons[263 + epicSkin].transform.GetComponent<SkinButtonController>();
			//skinButton.unlocked = true;
			//skinButton.lockObject.SetActive(false);
			//skinButton.rankUnlocked.SetActive(false);
		}
		//AudioManager.Instance.PlaySFX(22, Vector3.zero);
		gamePlayerInstance.SavePlayer();
	}
}
