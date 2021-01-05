using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Audio Clip Database", menuName = "Database/Audio Clip Database")]
public class AudioClipDatabase : ScriptableObject
{
    [SerializeField]
    private List<AudioClip> audioClips;

    public List<AudioClip> AudioClips
    {
        get
        {
            return audioClips;
        }
    }
}
