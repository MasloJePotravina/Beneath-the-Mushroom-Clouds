//Based on: https://www.youtube.com/watch?v=6OT43pvUyfY
using UnityEngine.Audio;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;


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


                if(s.canOverlap)
                {
                    SwitchSoundAndPlayOneShot(source, s);
                }
                else
                {
                    //Note: Sometimes, a sounds needs to wait for another sound to finish playing before it can play
                    //The problem is that the AudioSource.Play() function will start the sound (presumably) at the beginning
                    // of a frame and therefore, if the sound that is supposed to wait is called in the same frame as the original
                    // sound, the new sound will not wait. This means that simple AudioSource.isPlaying will not work and I had to
                    //use this workaround
                    if(wait)
                    {
                        if(source.clip != null)
                        {
                            float time = source.clip.length - source.time;
                            StartCoroutine(WaitForPreviousClip(source, s, time));
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

    private void SwitchSoundAndPlay(AudioSource source, Sound s)
    {
         
        SourceSetup(source, s);
        source.Play();
        StartCoroutine(ResetAudioSource(source, s.clip));
    }

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

    private IEnumerator WaitForPreviousClip(AudioSource source, Sound s, float time)
    {
        yield return new WaitForSeconds(time);
        SwitchSoundAndPlay(source, s);
    }

    private IEnumerator ResetAudioSource(AudioSource source, AudioClip clip)
    {
        yield return new WaitForSeconds(source.clip.length);
        //Protection against accidentaly resetting the audio source of another sound
        if(source.clip == clip)
        {
            source.clip = null;
        }
    }

}
