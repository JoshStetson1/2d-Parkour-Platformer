using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;

    public AudioClip clip;

    [Range(0, 1)]
    public float volume;
    [Range(0.1f, 2)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
