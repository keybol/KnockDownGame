using System.Collections;

using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    public static ScreenShaker Instance
    {
        get; set;
    }

    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;
	Cinemachine.CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeScreen(float shakeDuration)
    {
        if(shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(Shake(shakeDuration));
    }

    private IEnumerator Shake(float shakeDuration)
    {
        float lerpTime = shakeDuration;

        float currentLerpTime = 0;

        float lerpRatio = 0;

        bool isLerping = true;

        while(isLerping)
        {
            if(currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;

                isLerping = false;
            }

            lerpRatio = currentLerpTime / lerpTime;

            virtualCameraNoise.m_AmplitudeGain = Mathf.Lerp(5, 0, lerpRatio);

            currentLerpTime += Time.deltaTime;

            yield return null;
        }

        shakeCoroutine = null;
    }
}
