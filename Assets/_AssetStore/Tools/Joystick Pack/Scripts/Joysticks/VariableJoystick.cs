using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.InputSystem.OnScreen;

public class VariableJoystick : Joystick
{
    public enum JoystickType
    {
        Fixed, Floating, Dynamic
    }

    [SerializeField] private float moveThreshold = 1;

    [SerializeField]
    private JoystickType joystickType = JoystickType.Fixed;

    [SerializeField]
    OnScreenStick onScreenStick;

    public float MoveThreshold
    {
        get
        {
            return moveThreshold;
        }

        set
        {
            moveThreshold = Mathf.Abs(value);
        }
    }

    private Vector2 fixedPosition = Vector2.zero;

    protected override void Start()
    {
        base.Start();

        fixedPosition = background.anchoredPosition;

        SetMode(joystickType);
    }

    public void SetMode(JoystickType joystickType)
    {
        this.joystickType = joystickType;

        if(joystickType == JoystickType.Fixed)
        {
            background.anchoredPosition = fixedPosition;

            background.gameObject.SetActive(true);
        }

        //else
        //    background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
		if (joystickType != JoystickType.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);

            background.gameObject.SetActive(true);
        }

        if(onScreenStick)
            onScreenStick.OnPointerDown(eventData);

        else
            base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if(onScreenStick)
            onScreenStick.OnDrag(eventData);

        else
            base.OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(onScreenStick)
            onScreenStick.OnPointerUp(eventData);

        else
            base.OnPointerUp(eventData);

        //if(joystickType != JoystickType.Fixed)
        //    background.gameObject.SetActive(false);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if(joystickType == JoystickType.Dynamic && magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;

            background.anchoredPosition += difference;
        }

        base.HandleInput(magnitude, normalised, radius, cam);
    }
}

