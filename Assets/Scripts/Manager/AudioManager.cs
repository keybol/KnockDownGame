using System.Collections;

using System.Collections.Generic;

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if(instance == null)
            {
                AudioManager resource = Resources.Load<AudioManager>("Managers/AudioManager");

                instance = Instantiate(resource);

                instance.Initialize();
            }

            return instance;
        }
    }

    [SerializeField]
    private AudioSource bgmAudioSource;

    [SerializeField]
    private AudioSource sfxAudioSource;

    [SerializeField]
    private AudioSource voiceAudioSource;

    [SerializeField]
    private AudioSource ambientAudioSource;

    [SerializeField]
    private AudioClipDatabase bgmAudioDatabase;

    [SerializeField]
    private List<AudioClip> sfxAudioClip;

    [SerializeField]
    private List<AudioClip> voiceAudioClip;

    [SerializeField]
    private List<AudioClip> ambientAudioClip;

    private float bgmCurrentVolume = 1;
    private float sfxCurrentVolume = 1;
    private float voiceCurrentVolume = 1;
    private float ambientCurrentVolume = 1;

    private Coroutine fadeBGMCoroutine;
    private Coroutine fadeSFXCoroutine;
    private Coroutine fadeVoiceCoroutine;
    private Coroutine fadeAmbientCoroutine;

    private bool initialized;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        else if(instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(initialized)
            return;

        //bgmCurrentVolume = bgmAudioSource.volume;
        //sfxCurrentVolume = sfxAudioSource.volume;
        //voiceCurrentVolume = voiceAudioSource.volume;
        //ambientCurrentVolume = ambientAudioSource.volume;

        initialized = true;
    }

    //BGM
    public void PlayBGM(int index, Vector3 position, bool loop = true)
    {
        PlayAudio(bgmAudioSource, bgmAudioDatabase.AudioClips, bgmCurrentVolume, index, position);
    }

    public void PlayBGM(string clipName, Vector3 position, bool loop = true)
    {
		bgmAudioSource.loop = loop;
		PlayAudio(bgmAudioSource, bgmAudioDatabase.AudioClips, bgmCurrentVolume, clipName, position);
    }

    public void PauseBGM()
    {
        PauseAudio(bgmAudioSource);
    }

    public void UnpauseBGM()
    {
        UnpauseAudio(bgmAudioSource);
    }

    public void MuteBGM()
    {
        MuteAudio(bgmAudioSource);
    }

    public void UnmuteBGM()
    {
        UnmuteAudio(bgmAudioSource);
    }

    public void FadeBGM(float fadeTime, float startVolume, float endVolume)
    {
        if(fadeBGMCoroutine != null)
            StopCoroutine(fadeBGMCoroutine);

        fadeBGMCoroutine = StartCoroutine(FadeAudio(bgmAudioSource, fadeTime, startVolume, endVolume));
    }

    public void StopBGM()
    {
        if(fadeBGMCoroutine != null)
        {
            StopCoroutine(fadeBGMCoroutine);

            fadeBGMCoroutine = null;
        }

        StopAudio(bgmAudioSource);
    }

    //SFX
    public void PlaySFX(int index, Vector3 position)
    {
        PlayAudio(sfxAudioSource, sfxAudioClip, sfxCurrentVolume, index, position);
    }

    public void PlaySFX(string clipName, Vector3 position)
    {
        PlayAudio(sfxAudioSource, sfxAudioClip, sfxCurrentVolume, clipName, position);
    }

    public void PauseSFX()
    {
        PauseAudio(sfxAudioSource);
    }

    public void UnpauseSFX()
    {
        UnpauseAudio(sfxAudioSource);
    }

    public void MuteSFX()
    {
        MuteAudio(sfxAudioSource);
		sfxCurrentVolume = 0;
	}

    public void UnmuteSFX()
    {
        UnmuteAudio(sfxAudioSource);
		sfxCurrentVolume = 1;
	}

    public void FadeSFX(float fadeTime, float startVolume, float endVolume)
    {
        if(fadeSFXCoroutine != null)
            StopCoroutine(fadeSFXCoroutine);

        fadeSFXCoroutine = StartCoroutine(FadeAudio(sfxAudioSource, fadeTime, startVolume, endVolume));
    }

    public void StopSFX()
    {
        if(fadeSFXCoroutine != null)
        {
            StopCoroutine(fadeSFXCoroutine);

            fadeSFXCoroutine = null;
        }

        StopAudio(sfxAudioSource);
    }

    //Voice
    public void PlayVoice(int index, Vector3 position)
    {
        PlayAudio(voiceAudioSource, voiceAudioClip, voiceCurrentVolume, index, position);
    }

    public void PlayVoice(string clipName, Vector3 position)
    {
        PlayAudio(voiceAudioSource, voiceAudioClip, voiceCurrentVolume, clipName, position);
    }

    public void PauseVoice()
    {
        PauseAudio(voiceAudioSource);
    }

    public void UnpauseVoice()
    {
        UnpauseAudio(voiceAudioSource);
    }

    public void MuteVoice()
    {
        MuteAudio(voiceAudioSource);
    }

    public void UnmuteVoice()
    {
        UnmuteAudio(voiceAudioSource);
    }

    public void FadeVoice(float fadeTime, float startVolume, float endVolume)
    {
        if(fadeVoiceCoroutine != null)
            StopCoroutine(fadeVoiceCoroutine);

        fadeVoiceCoroutine = StartCoroutine(FadeAudio(voiceAudioSource, fadeTime, startVolume, endVolume));
    }

    public void StopVoice()
    {
        if(fadeVoiceCoroutine != null)
        {
            StopCoroutine(fadeVoiceCoroutine);

            fadeVoiceCoroutine = null;
        }

        StopAudio(voiceAudioSource);
    }

    //Ambient
    public void PlayAmbient(int index, Vector3 position)
    {
        PlayAudio(ambientAudioSource, ambientAudioClip, ambientCurrentVolume, index, position);
    }

    public void PlayAmbient(string clipName, Vector3 position)
    {
        PlayAudio(ambientAudioSource, ambientAudioClip, ambientCurrentVolume, clipName, position);
    }

    public void PauseAmbient()
    {
        PauseAudio(ambientAudioSource);
    }

    public void UnpauseAmbient()
    {
        UnpauseAudio(ambientAudioSource);
    }

    public void MuteAmbient()
    {
        MuteAudio(ambientAudioSource);
    }

    public void UnmuteAmbient()
    {
        UnmuteAudio(ambientAudioSource);
    }

    public void FadeAmbient(float fadeTime, float startVolume, float endVolume)
    {
        if(fadeAmbientCoroutine != null)
            StopCoroutine(fadeAmbientCoroutine);

        fadeAmbientCoroutine = StartCoroutine(FadeAudio(ambientAudioSource, fadeTime, startVolume, endVolume));
    }

    public void StopAmbient()
    {
        if(fadeAmbientCoroutine != null)
        {
            StopCoroutine(fadeAmbientCoroutine);

            fadeAmbientCoroutine = null;
        }

        StopAudio(ambientAudioSource);
    }

    //Audio Functions
    private void PlayAudio(AudioSource audioSource, List<AudioClip> clips, float volume, int index, Vector3 position)
    {
        if(audioSource == null)
        {
            Debug.LogError("Audio Source is Null");

            return;
        }

        if(clips == null)
        {
            Debug.LogError("Audio Clips is Null");

            return;
        }

        if(index >= clips.Count)
        {
            Debug.LogError("Index Out of Bounds");

            return;
        }

        if(clips[index] == null)
        {
            Debug.LogError("Audio Clip is Null");

            return;
        }

		GameObject audioPrefab = ObjectPoolerManager.Instance.GetPooledAudioObject();
		audioPrefab.transform.position = position;
		audioPrefab.SetActive(true);
		audioPrefab.GetComponent<AudioSource>().clip = clips[index];
		audioPrefab.GetComponent<AudioSource>().volume = volume;
		audioPrefab.GetComponent<AudioSource>().Play();
		audioPrefab.GetComponent<audioPrefabAutoDestruct>().WaitAndDeactivate();

		/*audioSource.clip = clips[index];

        audioSource.volume = volume;

        audioSource.Play();*/
	}

    private void PlayAudio(AudioSource audioSource, List<AudioClip> clips, float volume, string clipName, Vector3 position)
    {
        if(audioSource == null)
        {
            Debug.LogError("Audio Source is Null");

            return;
        }

        if(clips == null)
        {
            Debug.LogError("Audio Clips is Null");

            return;
        }

        AudioClip clip = clips.Find(c => c.name == clipName);

        if(clip == null)
        {
            Debug.Log("Clip Not Found");

            return;
        }

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.Play();
    }

    private void PauseAudio(AudioSource audioSource)
    {
        if(audioSource == null)
        {
            Debug.LogError("Audio Source is Null");

            return;
        }

        if(!audioSource.isPlaying)
            return;

        audioSource.Pause();
    }

    private void UnpauseAudio(AudioSource audioSource)
    {
        if(audioSource == null)
        {
            Debug.LogError("Audio Source is Null");

            return;
        }

        audioSource.UnPause();
    }

    private void MuteAudio(AudioSource audioSource)
    {
        if(audioSource == null)
        {
            Debug.LogError("Audio Source is Null");

            return;
        }

        audioSource.mute = true;
    }

    private void UnmuteAudio(AudioSource audioSource)
    {
        if(audioSource == null)
        {
            Debug.LogError("Audio Source is Null");

            return;
        }

        audioSource.mute = false;
    }

    private void StopAudio(AudioSource audioSource)
    {
        if(audioSource == null)
        {
            Debug.LogError("Audio Source is Null");

            return;
        }

        if(!audioSource.isPlaying)
            return;

        audioSource.Stop();
    }

    private IEnumerator FadeAudio(AudioSource audioSource, float fadeTime, float startVolume, float endVolume)
    {
        float lerpTime = fadeTime;

        float currentLerpTime = 0;

        float time = 0;

        bool isLerping = true;

        while(isLerping && audioSource.isPlaying)
        {
            if(currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;

                isLerping = false;
            }

            time = currentLerpTime / lerpTime;

            audioSource.volume = Mathf.Lerp(startVolume, endVolume, time);

            currentLerpTime += Time.deltaTime;

            yield return null;
        }
    }
}


