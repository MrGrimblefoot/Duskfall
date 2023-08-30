using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float sensitivity;
    public int invertY;
    public int useVSync;
    public int isFullscreen;
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float ambianceVolume;
    public int selectedRes;
    public int screenWidth;
    public int screenHeight;

    public SettingsData(SettingsMenu settings)
    {
        sensitivity = settings.sensitivity;

        invertY = settings.invertY;
        useVSync = settings.useVSync;
        isFullscreen = settings.isFullscreen;

        masterVolume = settings.masterVolume;
        musicVolume = settings.musicVolume;
        sfxVolume = settings.sfxVolume;
        ambianceVolume = settings.ambianceVolume;

        selectedRes = settings.selectedRes;
        screenWidth = settings.screenWidth;
        screenHeight = settings.screenHeight;

        //position = new float[3];
        //position[0] = player.transform.position.x;
        //position[1] = player.transform.position.y;
        //position[2] = player.transform.position.z;
    }
}
