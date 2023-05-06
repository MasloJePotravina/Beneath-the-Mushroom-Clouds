using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

/// <summary>
/// Implements the behaviour of the option menu.
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    /// <summary>
    /// Refeence to the Game options instance which persist between scenes.
    /// </summary>
    private GameOptions gameOptions;

    /// <summary>
    /// List of all available resolutions.
    /// </summary>
    Resolution[] resolutions;

    /// <summary>
    /// Text displaying the amount of FOV raycasts.
    /// </summary>
    private TextMeshProUGUI fovRaycastAmount;

    /// <summary>
    /// Text displaying the master volume.
    /// </summary>
    private TextMeshProUGUI masterVolumeAmount;

    /// <summary>
    /// Text displaying the sfx volume.
    /// </summary>
    private TextMeshProUGUI sfxVolumeAmount;

    /// <summary>
    /// Text displaying the inventory volume.
    /// </summary>
    private TextMeshProUGUI inventoryVolumeAmount;

    /// <summary>
    /// Slider for the FOV raycast amount.
    /// </summary>
    private Slider fovRaycastSlider;
    
    /// <summary>
    /// Slider for the master volume.
    /// </summary>
    private Slider masterVolumeSlider;

    /// <summary>
    /// Slider for the sfx volume.
    /// </summary>
    private Slider sfxVolumeSlider;

    /// <summary>
    /// Slider for the inventory volume.
    /// </summary>
    private Slider inventoryVolumeSlider;

    /// <summary>
    /// Dropdown with available resolutions.
    /// </summary>
    private TMP_Dropdown resolutionDropdown;

    /// <summary>
    /// Dropdown with available quality settings.
    /// </summary>
    private TMP_Dropdown qualityDropdown;

    /// <summary>
    /// Toggle for fullscreen.
    /// </summary>
    private Toggle fullscreenToggle;

    /// <summary>
    /// Gets all necessary references on awake and loads their values from the Game Options instance.
    /// </summary>
    void Awake(){
        gameOptions = GameOptions.Instance;
        Transform videoOptions = transform.Find("Options/VideoOptions").transform;
        resolutionDropdown = videoOptions.Find("ResolutionSetting/ResolutionDropdown").GetComponent<TMP_Dropdown>();
        qualityDropdown = videoOptions.Find("QualitySetting/QualityDropdown").GetComponent<TMP_Dropdown>();
        fullscreenToggle = videoOptions.Find("FullscreenToggle").GetComponent<Toggle>();
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutions[i].width + " X " + resolutions[i].height + " [" + resolutions[i].refreshRate + "Hz]"));
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.value = currentResolutionIndex;

        fovRaycastAmount = videoOptions.Find("FOVRaycastsSetting/Amount").GetComponent<TextMeshProUGUI>();

        Transform audioOptions = transform.Find("Options/AudioOptions").transform;
        masterVolumeAmount = audioOptions.Find("MasterVolume/Amount").GetComponent<TextMeshProUGUI>();
        sfxVolumeAmount = audioOptions.Find("SFXVolume/Amount").GetComponent<TextMeshProUGUI>();
        inventoryVolumeAmount = audioOptions.Find("InventoryVolume/Amount").GetComponent<TextMeshProUGUI>();

        fovRaycastSlider = videoOptions.Find("FOVRaycastsSetting/Slider").GetComponent<Slider>();
        masterVolumeSlider = audioOptions.Find("MasterVolume/Slider").GetComponent<Slider>();
        sfxVolumeSlider = audioOptions.Find("SFXVolume/Slider").GetComponent<Slider>();
        inventoryVolumeSlider = audioOptions.Find("InventoryVolume/Slider").GetComponent<Slider>();

        LoadOptionsUI();
       

    }

    ///<summary>
    /// Sets the master volume.
    /// </summary>
    /// <param name="volume">Master volume</param>
    public void SetMasterVolume(float volume)
    {
        masterVolumeAmount.text = Mathf.RoundToInt(volume * 100).ToString();
        gameOptions.SetMasterVolume(volume);
    }

    /// <summary>
    /// Sets the sfx volume.
    /// </summary>
    /// <param name="volume">SFX Volume</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolumeAmount.text = Mathf.RoundToInt(volume * 100).ToString();
        gameOptions.SetSFXVolume(volume);
    }

    /// <summary>
    /// Sets the inventory volume.
    /// </summary>
    /// <param name="volume"></param>
    public void SetInventoryVolume(float volume)
    {
        inventoryVolumeAmount.text = Mathf.RoundToInt(volume * 100).ToString();
        gameOptions.SetInventoryVolume(volume);
    }

    /// <summary>
    /// Sets the quality.
    /// </summary>
    /// <param name="qualityIndex">Index of the selected quality level</param>
    public void SetQuality(int qualityIndex)
    {
        gameOptions.SetQualityIndex(qualityIndex);
    }

    /// <summary>
    /// Sets the resolution.
    /// </summary>
    /// <param name="resolutionIndex">Index of the selected resolution.</param>
    public void SetResolution(int resolutionIndex)
    {
        gameOptions.SetResolutionIndex(resolutionIndex);
    }

    /// <summary>
    /// Toggles fullscreen.
    /// </summary>
    /// <param name="fullscreen">True if the game should be fullscreen, false for windowed.</param>
    public void SetFullscreen(bool fullscreen)
    {
        gameOptions.SetFullscreen(fullscreen);
    }

    /// <summary>
    /// Sets the amount of FOV raycasts.
    /// </summary>
    /// <param name="amount">New amount of FOV raycasts.</param>
    public void SetFOVRaycasts(float amount)
    {
        int intAmount = Mathf.RoundToInt(amount);
        fovRaycastAmount.text = intAmount.ToString();
        gameOptions.SetFOVRaycasts(intAmount);
        
    }

    /// <summary>
    /// Loads the options UI with the values from the Game Options instance.
    /// </summary>
    public void LoadOptionsUI(){
        float masterVolume = gameOptions.GetMasterVolume();
        float sfxVolume = gameOptions.GetSFXVolume();
        float inventoryVolume = gameOptions.GetInventoryVolume();

        int fovRaycasts = gameOptions.GetFOVRaycasts();
        int qualityIndex = gameOptions.GetQualityIndex();
        int resolutionIndex = gameOptions.GetResolutionIndex();

        bool fullscreen = gameOptions.GetFullscreen();

        masterVolumeSlider.value = masterVolume;
        masterVolumeAmount.text = Mathf.RoundToInt(masterVolume * 100).ToString();

        sfxVolumeSlider.value = sfxVolume;
        sfxVolumeAmount.text = Mathf.RoundToInt(sfxVolume * 100).ToString();

        inventoryVolumeSlider.value = inventoryVolume;
        inventoryVolumeAmount.text = Mathf.RoundToInt(inventoryVolume * 100).ToString();

        fovRaycastSlider.value = fovRaycasts;
        fovRaycastAmount.text = fovRaycasts.ToString();

        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        qualityDropdown.value = qualityIndex;
        qualityDropdown.RefreshShownValue();

        fullscreenToggle.isOn = fullscreen;
    }
}
