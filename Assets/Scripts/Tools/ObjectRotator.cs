using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectRotator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	public Vector3 mPrevPos;
	public Vector3 mPosDelta;
	public Vector2 input;
	[SerializeField] private Canvas canvas;
	[SerializeField] private AvatarManager avatarManager;
	private Camera cam;
	public GameObject targetObject;
	private KInputActions controls;

	void Awake()
	{
		controls = new KInputActions();
	}

	public void OnEnable()
	{
		if (controls != null)
			controls.Enable();
	}

	public void OnDisable()
	{
		if (controls != null)
			controls.Disable();
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		mPrevPos = eventData.position / (canvas.scaleFactor);
		OnDrag(eventData);
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		input = eventData.position / (canvas.scaleFactor);
		mPosDelta = (new Vector3(input.x, 0f, 0f) - mPrevPos);
		targetObject.transform.Rotate(transform.up, -Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
		mPrevPos = input;
	}

	private void OpenSkinsPanel()
	{
		if (mPosDelta.x == 0)
		{
			MultiFunction.DoVibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
		}
	}

	private void Update()
	{
		input = controls.UI.Rotate.ReadValue<Vector2>();
		if (input.x < 0)
		{
			targetObject.transform.Rotate(transform.up, 10, Space.World);
		}
		else if (input.x > 0)
		{
			targetObject.transform.Rotate(transform.up, -10, Space.World);
		}
	}
	public virtual void OnPointerUp(PointerEventData eventData)
	{

	}
}
