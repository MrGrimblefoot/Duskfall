using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionUpdater : MonoBehaviour
{
    [SerializeField] private GameObject cam;

    void Update() { cam.transform.position = transform.position; }
}
