using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BadgeVariables : MonoBehaviour
{
	[SerializeField] public GameObject rank;
	[SerializeField] public GameObject freeUnlock;
	[SerializeField] public TextMeshProUGUI trophiesText;
	[SerializeField] public Image barFilled;
	[SerializeField] public Image blackBar;
	[SerializeField] public Image levelDiamond;
	[SerializeField] public TextMeshProUGUI thisLevel;
	[SerializeField] public Image rewardImage;
	[SerializeField] public TextMeshProUGUI rewardQuantity;
	[SerializeField] public Image freeRewardImage;
	[SerializeField] public TextMeshProUGUI freeRewardQuantity;
}
