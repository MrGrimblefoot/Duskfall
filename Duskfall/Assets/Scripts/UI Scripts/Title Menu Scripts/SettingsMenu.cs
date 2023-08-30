using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject GameplayMenu;
    [SerializeField] private GameObject VideoMenu;
    [SerializeField] private GameObject AudioMenu;

    [Header("Toggles")]
    [SerializeField] private Toggle FullscreenToggle;
    [SerializeField] private Toggle VSyncToggle;
    [SerializeField] private Toggle invertYToggle;

    [Header("Resolution")]
    [SerializeField] private ResolutionType[] resolutions;
    [SerializeField] private TextMeshProUGUI resText;
    [HideInInspector] public int selectedRes;
    [HideInInspector] public int screenWidth;
    [HideInInspector] public int screenHeight;

    [Header("Audio")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private TextMeshProUGUI ambianceVolumeText;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider ambianceVolumeSlider;
    [HideInInspector] public float masterVolume;
    [HideInInspector] public float musicVolume;
    [HideInInspector] public float sfxVolume;
    [HideInInspector] public float ambianceVolume;

    [Header("Gameplay")]
    [SerializeField] private TextMeshProUGUI sensitivityText;
    [SerializeField] private Slider sensitivitySlider;
    [HideInInspector] public float sensitivity;
    [HideInInspector] public int invertY;
    [HideInInspector] public int useVSync;
    [HideInInspector] public int isFullscreen;

    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        LoadSettingsData();

        UpdateSensitivityText(1.2f);
    }

    private void LoadSettingsData()
    {
        SettingsData data = SaveSystem.LoadSettings();
        if(data != null)
        {
            //set the resolution to the saved value
            //Screen.SetResolution(data.screenWidth, data.screenHeight, FullscreenToggle.isOn);
            Screen.SetResolution(data.screenWidth, data.screenHeight, FullscreenToggle.isOn);
            //make the text match the saved resolution
            resText.text = data.screenWidth + " x " + data.screenHeight;
            selectedRes = data.selectedRes;

            //setting the game volume to what the saved data is.
            masterMixer.SetFloat("MasterVolume", data.masterVolume);
            //making the slider match the volume.
            masterVolumeSlider.value = data.masterVolume;
            //making the text match the saved data is.
            masterVolumeText.text = data.masterVolume.ToString("0.00");

            //setting the game volume to what the saved data is.
            masterMixer.SetFloat("MusicVolume", data.musicVolume);
            //making the slider match the volume.
            musicVolumeSlider.value = data.musicVolume;
            //making the text match the saved data is.
            musicVolumeText.text = data.musicVolume.ToString("0.00");

            //setting the game volume to what the saved data is.
            masterMixer.SetFloat("SFXVolume", data.sfxVolume);
            //making the slider match the volume.
            sfxVolumeSlider.value = data.sfxVolume;
            //making the text match the saved data is.
            sfxVolumeText.text = data.sfxVolume.ToString("0.00");

            //setting the game volume to what the saved data is.
            masterMixer.SetFloat("AmbianceVolume", data.ambianceVolume);
            //making the slider match the volume.
            ambianceVolumeSlider.value = data.ambianceVolume;
            //making the text match the saved data is.
            ambianceVolumeText.text = data.ambianceVolume.ToString("0.00");

            //setting the game manager's sensitivity to what the saved data is.
            gameManager.sensitivity = data.sensitivity;
            //making the slider match the sensitivity.
            sensitivitySlider.value = data.sensitivity;
            //making the text match the sensitivity.
            sensitivityText.text = data.sensitivity.ToString("0.00");

            //if invertY is true
            if (data.invertY == 1)
            {
                //make Y inverted
                gameManager.invertY = true;
                //turn the toggle on
                invertYToggle.isOn = true;
            }
            //if invertY is false
            else
            {
                //make Y not inverted
                gameManager.invertY = false;
                //turn the toggle off
                invertYToggle.isOn = false;
            }

            //if useVSync is true
            if (data.useVSync == 1)
            {
                //turn on vsync
                QualitySettings.vSyncCount = 1; 
                //turn the toggle on
                VSyncToggle.isOn = true;
            }
            //if useVSync is false
            else
            {
                //turn off vsync
                QualitySettings.vSyncCount = 0;
                //turn toggle off
                VSyncToggle.isOn = false;
            }

            //if isFullscreen is true
            if (data.isFullscreen == 1)
            {
                //make the screen fullscreen
                Screen.fullScreen = true;
                //turn toggle on
                FullscreenToggle.isOn = true;            
            }
            //if isFullscreen is false
            else
            {
                //make the screen windowed
                Screen.fullScreen = false;
                //turn toggle off
                FullscreenToggle.isOn = false; ;            
            }

        }
    }

    public void OpenGameplayMenu()
    {
        GameplayMenu.SetActive(true);
        VideoMenu.SetActive(false);
        AudioMenu.SetActive(false);
    }

    public void OpenVideoMenu()
    {
        GameplayMenu.SetActive(false);
        VideoMenu.SetActive(true);
        AudioMenu.SetActive(false);
    }

    public void OpenAudioMenu()
    {
        GameplayMenu.SetActive(false);
        VideoMenu.SetActive(false);
        AudioMenu.SetActive(true);
    }

    public void ResolutionLeft()
    {
        if(selectedRes > 0) { selectedRes--; }

        UpdateResolutionText();
    }

    public void ResolutionRight()
    {
        if (selectedRes < resolutions.Length - 1) { selectedRes++; }

        UpdateResolutionText();
    }

    void UpdateResolutionText()
    {
        resText.text = resolutions[selectedRes].width + " x " + resolutions[selectedRes].height;
    }

    void SetResolution()
    {
        Screen.SetResolution(resolutions[selectedRes].width, resolutions[selectedRes].height, FullscreenToggle.isOn);
        screenWidth = resolutions[selectedRes].width;
        screenHeight = resolutions[selectedRes].height;
    }

    void ToggleFullscreen()
    {
        Screen.fullScreen = FullscreenToggle.isOn;
        if (Screen.fullScreen) { isFullscreen = 1; }
        else { isFullscreen = 0; }
    }

    void ToggleVSync()
    {
        if (VSyncToggle.isOn) { QualitySettings.vSyncCount = 1; useVSync = 1; }
        else { QualitySettings.vSyncCount = 0; useVSync = 0; }
    }

    public void UpdateSensitivityText(float sens)
    {
        sensitivityText.text = sens.ToString("0.00");
        gameManager.sensitivity = sens;
        sensitivity = sens;
    }

    public void ApplyChanges()
    {
        SetResolution();
        ToggleFullscreen();
        ToggleVSync();
        gameManager.invertY = invertYToggle.isOn;
        if (invertYToggle.isOn) { invertY = 1; } else { invertY = 0; }
        SaveSystem.SaveSettings(this);
    }

    public void ClosePanel() { gameObject.SetActive(false); }

    public void SetMasterVolume (float volume) 
    { 
        masterMixer.SetFloat("MasterVolume", /*Mathf.Log10(volume) * 20*/volume); 
        masterVolume = volume;
        masterVolumeText.text = volume.ToString("0.00");
    }

    public void SetMusicVolume (float volume) 
    {
        masterMixer.SetFloat("MusicVolume", /*Mathf.Log10(volume) * 20*/volume);
        musicVolume = volume;
        musicVolumeText.text = volume.ToString("0.00");
    }

    public void SetSFXVolume (float volume) 
    {
        masterMixer.SetFloat("SFXVolume", /*Mathf.Log10(volume) * 20*/volume);
        sfxVolume = volume;
        sfxVolumeText.text = volume.ToString("0.00");
    }

    public void SetAmbianceVolume (float volume) 
    {
        masterMixer.SetFloat("AmbianceVolume", /*Mathf.Log10(volume) * 20*/volume);
        ambianceVolume = volume;
        ambianceVolumeText.text = volume.ToString("0.00");
    }

}
