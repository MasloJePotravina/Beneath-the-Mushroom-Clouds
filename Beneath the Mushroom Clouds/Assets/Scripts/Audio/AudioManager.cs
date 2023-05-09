//Based on: https://www.youtube.com/watch?v=6OT43pvUyfY
using UnityEngine.Audio;
using System.Collections;
using UnityEngine;
/// <summary>
/// Manages the playing of audio clips.
/// </summary>
public class AudioManager : MonoBehaviour
{

    //Note: Likely to be reworked as this is hardly scalable
    /// <summary>
    /// List of all the sounds in the game.
    /// </summary>
    public Sound[] sounds;

    /// <summary>
    /// Plays a sound.
    /// </summary>
    /// <param name="name">Name of the sound.</param>
    /// <param name="sourceOverride">Overrides the sounds original source.</param>
    /// <param name="wait">Whether a sound should wait for another sound to start playing.</param>
    public void Play(string name, GameObject sourceOverride = null,  bool wait = false)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                AudioSource source = s.source;
                
                if(sourceOverride != null)
                {
                    source = sourceOverride.GetComponent<AudioSource>();
                }   

                //If this is an overlapping sound (such as a gunshot) just play it
                if(s.canOverlap)
                {
                    SwitchSoundAndPlayOneShot(source, s);
                }
                //If this sound should not overlap with other sounds, based on the wait flag, either play it or wait for the previous sound to finish playing
                else
                {
                    //Note: Sometimes, a sounds needs to wait for another sound to finish playing before it can play
                    //The problem is that the AudioSource.Play() function will start the sound (presumably) at the beginning
                    // of a frame and therefore, if the sound that is supposed to wait is called in the same frame as the original
                    // sound, the new sound will not wait. This means that simple AudioSource.isPlaying will not work and I had to
                    //use this workaround
                    if(wait)
                    {
                        //If some other sound is currently playing on the source, wait for it to finish, otherwise just play the sound
                        if(source.clip != null)
                        {
                            float time = source.clip.length - source.time;
                            StartCoroutine(WaitBeforeSwitching(source, s, time));
                        }
                        else
                        {
                            SwitchSoundAndPlay(source, s);
                        }
                    }
                    else
                    {
                        SwitchSoundAndPlay(source, s);
                    }
                }

                return;
            }
        }
    }

    /// <summary>
    /// Switches an audioclip on an audio source and plays it. This cancels the previously played sound.
    /// </summary>
    /// <param name="source">The audio source for the clip.</param>
    /// <param name="s">The sound to be switched to and played.</param>
    private void SwitchSoundAndPlay(AudioSource source, Sound s)
    {
         
        SourceSetup(source, s);
        source.Play();
        StartCoroutine(ResetAudioSource(source, s.clip));
    }

    /// <summary>
    /// Switches an audio clip on an audio source and plays it as a one shot. This does not cancel the previously played sound, the sounds overlap.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="s"></param>
    private void SwitchSoundAndPlayOneShot(AudioSource source, Sound s)
    {
        AudioSource[] sources = source.gameObject.GetComponents<AudioSource>();
        //One Shot sounds have their own audio source so that other sounds using the same gameObject do not 
        //change their volume and pitch values while they still might be playing
        //All objects that can play one shot sounds should have 2 audio sources
        source = sources[1];
        SourceSetup(source, s);
        source.PlayOneShot(s.clip);
    }

    /// <summary>
    /// Sets up the source by setting its clip, volume and pitch.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="s"></param>
    private void SourceSetup(AudioSource source, Sound s)
    {
        source.clip = s.clip;
        source.volume = s.volume;

        if(s.randomPitch)
        {
            source.pitch = Random.Range(s.randomPitchMin, s.randomPitchMax);
        }
        else
        {
            source.pitch = s.pitch;
        }
    }

    /// <summary>
    /// Waits for some time before switching to a sound and playing it. This is used to wait for another sound to finish playing.
    /// </summary>
    /// <param name="source">Audio source for the sound.</param>
    /// <param name="s">The sound to be played.</param>
    /// <param name="time">How long it should wait before switching to the sound and playing it.</param>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator WaitBeforeSwitching(AudioSource source, Sound s, float time)
    {
        yield return new WaitForSeconds(time);
        SwitchSoundAndPlay(source, s);
    }

    /// <summary>
    /// Resets the clip of an audio source after the clip has finished playing.
    /// </summary>
    /// <param name="source">The audio source to reset.</param>
    /// <param name="clip">Audio clip for which the reset happens.</param>
    /// <returns>Reference to the running coroutine.</returns>
    private IEnumerator ResetAudioSource(AudioSource source, AudioClip clip)
    {
        yield return new WaitForSeconds(source.clip.length);
        //Protection against GameObjects that were destroyed trying to play sounds
        if(source == null){
            yield break;
        }
        //Protection against accidentally resetting the audio source of another sound
        if(source.clip == clip)
        {
            source.clip = null;
        }
    }

}
