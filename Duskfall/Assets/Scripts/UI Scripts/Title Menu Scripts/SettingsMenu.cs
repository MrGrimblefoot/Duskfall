using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ResolutionLeft()
    {

    }

    public void ResolutionRight()
    {

    }

    public void ApplyChanges()
    {
        ClosePanel();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
