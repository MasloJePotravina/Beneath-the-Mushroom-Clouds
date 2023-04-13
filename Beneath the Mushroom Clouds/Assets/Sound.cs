//Based on: https://www.youtube.com/watch?v=6OT43pvUyfY
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;

    public bool canOverlap;

    public string name;

    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public bool randomPitch;

    public float randomPitchMin;
    public float randomPitchMax;

    public AudioSource source;


}
