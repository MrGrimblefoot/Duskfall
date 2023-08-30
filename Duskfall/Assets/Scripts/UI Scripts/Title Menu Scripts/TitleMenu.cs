using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private GameObject SettingsMenu;

    //private void Awake()
    //{
    //    Screen.SetResolution(1920,1080, true);
    //}

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenSettingMenu()
    {
        SettingsMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void save()
    {
        SaveSystem.SaveSettings(SettingsMenu.GetComponent<SettingsMenu>());
    }
}
