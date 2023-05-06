//Based on: https://www.youtube.com/watch?v=6OT43pvUyfY
using UnityEngine.Audio;
using UnityEngine;


/// <summary>
/// Holds information about individual audio clips.
/// </summary>
[System.Serializable]
public class Sound
{   
    /// <summary>
    /// The audio clip asset.
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// Whether the audio clip can overlap with other audio clips.
    /// </summary>
    public bool canOverlap;

    /// <summary>
    /// Name of the audio clip.
    /// </summary>
    public string name;

    /// <summary>
    /// Volume at which the audio clip should be played.
    /// </summary>
    [Range(0f, 1f)]
    public float volume;

    /// <summary>
    /// Pitch at which the audio clip should be played.
    /// </summary>
    [Range(0.1f, 3f)]
    public float pitch;

    /// <summary>
    /// Whether the sound should have randomized pitch. Used for sounds like gunshots.
    /// </summary>
    public bool randomPitch;

    /// <summary>
    /// Minimum pitch for the sound.
    /// </summary>
    public float randomPitchMin;

    /// <summary>
    /// Maximum pitch for the sound.
    /// </summary>
    public float randomPitchMax;

    /// <summary>
    /// Default audio source for the sound. This value is sometimes overriden.
    /// </summary>
    public AudioSource source;


}
