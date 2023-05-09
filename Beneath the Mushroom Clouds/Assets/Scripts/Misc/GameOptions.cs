using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps track of currently selected options. Is not destroyed on scene load and only one instance of this class exists at runtime.
/// </summary>
public class GameOptions : MonoBehaviour
{
    /// <summary>
    /// Instance of this class. Only one instance of this class exists at runtime.
    /// </summary>
    public static GameOptions Instance;

    /// <summary>
    /// Reference to the Audio Mixer asset.
    /// </summary>
    [SerializeField] private AudioMixer audioMixer;

    /// <summary>
    /// Array of all available resolutions.
    /// </summary>
    Resolution[] resolutions;

    /// <summary>
    /// Current master volume.
    /// </summary>
    private float masterVolume = 1f;

    /// <summary>
    /// Current SFX volume.
    /// </summary>
    private float sfxVolume = 1f;

    /// <summary>
    /// Current inventory volume.
    /// </summary>
    private float inventoryVolume = 1f;

    /// <summary>
    /// Current amount of raycasts used for the Field of View.
    /// </summary>
    private int fovRaycasts = 50;

    /// <summary>
    /// Current quality setting index.
    /// </summary>
    private int qualityIndex = 5;

    /// <summary>
    /// Current resolution index.
    /// </summary>
    private int resolutionIndex = 0;

    /// <summary>
    /// Current fullscreen setting.
    /// </summary>
    private bool fullscreen = true;



    /// <summary>
    /// On Awake, initializes the instance of this class and sets the resolutions array. Marks the instance as DontDestroyOnLoad.
    /// </summary>
    void Awake()
    {
        
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        resolutions = Screen.resolutions;
        Instance = this;
        DontDestroyOnLoad(this);

        //Set current resolution as the default one
        Resolution currentResolution = Screen.currentResolution;
        for(int i = 0; i < resolutions.Length; i++)
        {
            if(resolutions[i].width == currentResolution.width && resolutions[i].height == currentResolution.height)
            {
                resolutionIndex = i;
                break;
            }
        }


    }

    /// <summary>
    /// Subscribes to the sceneLoaded event.
    /// </summary>
     void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the sceneLoaded event.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    /// <summary>
    /// Set game all game options when the scene is loaded.
    /// </summary>
    /// <param name="scene">Loaded scene (unused but required to receive the information about scene load)</param>
    /// <param name="mode">Load scene mode (unused but required to receive the information about scene load)</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetAllOptions();
    }

    /// <summary>
    /// Converts a volume value between 0 and 1 to decibels.
    /// </summary>
    /// <param name="volume">Volume value ranging from 0 to 1.</param>
    /// <returns>Volume in decibels.</returns>
    private float VolumeToDecibels(float volume)
    {
        if(volume == 0f)
            return -80f;
        else
            return Mathf.Log10(volume) * 20f;
    }

    /// <summary>
    /// Sets the master volume.
    /// </summary>
    /// <param name="volume">Master volume between 0 and 1.</param>
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        audioMixer.SetFloat("masterVolume", VolumeToDecibels(masterVolume));
    }

    /// <summary>
    /// Sets the SFX volume.
    /// </summary>
    /// <param name="volume">SFX volume between 0 and 1.</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("sfxVolume", VolumeToDecibels(sfxVolume));
    }

    /// <summary>
    /// Sets the inventory volume.
    /// </summary>
    /// <param name="volume">Inventory volume between 0 and 1.</param>
    public void SetInventoryVolume(float volume)
    {
        inventoryVolume = volume;
        audioMixer.SetFloat("inventoryVolume", VolumeToDecibels(inventoryVolume));
    }

    /// <summary>
    /// Sets the new amount of player's FOV raycasts.
    /// </summary>
    /// <param name="amount">New amount of raycasts.</param>
    public void SetFOVRaycasts(int amount)
    {
        fovRaycasts = amount;
        FieldOfView fov = GameObject.FindObjectOfType<FieldOfView>();
        if(fov != null)
        {
            fov.SetFOVRayCount(fovRaycasts);
        }
    }

    /// <summary>
    /// Sets the new quality setting of the game (mostly insignificant in current version).
    /// </summary>
    /// <param name="quality">Quality index.</param>
    public void SetQualityIndex(int quality)
    {
        qualityIndex = quality;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    /// <summary>
    /// Sets the new resolution of the game.
    /// </summary>
    /// <param name="resolution">Resolution index.</param>
    public void SetResolutionIndex(int resolution)
    {
        resolutionIndex = resolution;
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    /// <summary>
    /// Sets the new fullscreen setting of the game.
    /// </summary>
    /// <param name="fullscreen">True for fullscreen, false for windowed mode.</param>
    public void SetFullscreen(bool fullscreen)
    {
        this.fullscreen = fullscreen;
        Screen.fullScreen = fullscreen;
    }

    /// <summary>
    /// Returns the current master volume.
    /// </summary>
    /// <returns>Current master volume</returns>
    public float GetMasterVolume()
    {
        return masterVolume;
    }

    /// <summary>
    /// Returns the current SFX volume.
    /// </summary>
    /// <returns>Current SFX volume</returns>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// Returns the current inventory volume.
    /// </summary>
    /// <returns>Current inventory volume</returns>
    public float GetInventoryVolume()
    {
        return inventoryVolume;
    }

    /// <summary>
    /// Returns the current amount of FOV raycasts.
    /// </summary>
    /// <returns>Current amount of FOV raycasts</returns>
    public int GetFOVRaycasts()
    {
        return fovRaycasts;
    }

    /// <summary>
    /// Returns the current quality index.
    /// </summary>
    /// <returns>Current quality index</returns>
    public int GetQualityIndex()
    {
        return qualityIndex;
    }

    /// <summary>
    /// Returns the current resolution index.
    /// </summary>
    /// <returns>Current resolution index</returns>
    public int GetResolutionIndex()
    {
        return resolutionIndex;
    }

    /// <summary>
    /// Returns the current fullscreen setting.
    /// </summary>
    /// <returns>Current fullscreen setting</returns>
    public bool GetFullscreen()
    {
        return fullscreen;
    }

    /// <summary>
    /// Sets all game options to their current values. This happens on scene load.
    /// </summary>
    public void SetAllOptions()
    {
        FieldOfView fov = GameObject.FindObjectOfType<FieldOfView>();
        if(fov != null)
        {
            fov.SetFOVRayCount(fovRaycasts);
        }

        
        audioMixer.SetFloat("sfxVolume", VolumeToDecibels(sfxVolume));
        audioMixer.SetFloat("inventoryVolume", VolumeToDecibels(inventoryVolume));

        QualitySettings.SetQualityLevel(qualityIndex);
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
        Screen.fullScreen = fullscreen;

    }
}
