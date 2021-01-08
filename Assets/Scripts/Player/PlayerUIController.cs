using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI playerNameText;
	[SerializeField] ABC_SliderReference healthSlider;
	[SerializeField] ABC_SliderReference healthOverTimeSlider;
	[SerializeField] CanvasGroup canvasGroup;
	private KPlayer target;
	private Transform targetTransform;
	private Vector3 targetPosition;

	void LateUpdate()
	{
		if (target == null)
			Destroy(this.gameObject);
		if (targetTransform)
		{
			Vector3 finalPosition = targetTransform.position;
			finalPosition.y = targetTransform.position.y + 1.75f;
			transform.position = Camera.main.WorldToScreenPoint(finalPosition);
		}
		playerNameText.text = target.Name;//target.playerIndex + "-" + 
	}

	public void SetTarget(KPlayer _target)
	{
		if (_target == null)
		{
			return;
		}
		target = _target;
		targetTransform = target.transform;
		target.abcState.HealthGUIList[0].healthSlider = healthSlider;
		target.abcState.HealthGUIList[0].healthOverTimeSlider = healthOverTimeSlider;
		if (target.pv.IsMine)
		{
			playerNameText.fontSize = 32;
			playerNameText.fontStyle = FontStyles.Bold;
		}
		transform.parent = KGameManager.Instance.playerUIPanel.transform;
	}
}
