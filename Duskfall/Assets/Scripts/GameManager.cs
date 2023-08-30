using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool invertY;
    public float sensitivity;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }
}
