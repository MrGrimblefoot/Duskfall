using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private GameObject SettingsMenu;

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
}
