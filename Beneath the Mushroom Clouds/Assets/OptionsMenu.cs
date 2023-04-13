//Based on: https://www.youtube.com/watch?v=YOaYQrN1oYQ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private FieldOfView fov;

    Resolution[] resolutions;

    private TextMeshProUGUI fovRaycastAmount;

    private TextMeshProUGUI masterVolumeAmount;
    private TextMeshProUGUI sfxVolumeAmount;
    private TextMeshProUGUI inventoryVolumeAmount;

    TMP_Dropdown resolutionDropdown;
    void Awake(){
        fov = GameObject.FindObjectOfType<FieldOfView>();
        Transform videoOptions = transform.Find("Options").Find("VideoOptions").transform;
        resolutionDropdown = videoOptions.Find("ResolutionSetting").Find("ResolutionDropdown").GetComponent<TMP_Dropdown>();
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

        fovRaycastAmount = videoOptions.Find("FOVRaycastsSetting").Find("Amount").GetComponent<TextMeshProUGUI>();

        Transform audioOptions = transform.Find("Options").Find("AudioOptions").transform;
        masterVolumeAmount = audioOptions.Find("MasterVolume").Find("Amount").GetComponent<TextMeshProUGUI>();
        sfxVolumeAmount = audioOptions.Find("SFXVolume").Find("Amount").GetComponent<TextMeshProUGUI>();
        inventoryVolumeAmount = audioOptions.Find("InventoryVolume").Find("Amount").GetComponent<TextMeshProUGUI>();


       

    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", VolumeToDecibels(volume));
        masterVolumeAmount.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("sfxVolume", VolumeToDecibels(volume));
        sfxVolumeAmount.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void SetInventoryVolume(float volume)
    {
        audioMixer.SetFloat("inventoryVolume", VolumeToDecibels(volume));
        inventoryVolumeAmount.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetFOVRaycasts(float amount)
    {
        int intAmount = Mathf.RoundToInt(amount);
        fovRaycastAmount.text = intAmount.ToString();
        fov.SetFOVRayCount(intAmount);
    }

    private float VolumeToDecibels(float volume)
    {
        if(volume == 0f)
            return -80f;
        else
            return Mathf.Log10(volume) * 20f;
    }
}
