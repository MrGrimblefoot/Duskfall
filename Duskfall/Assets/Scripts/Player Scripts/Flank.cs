using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : MonoBehaviour
{
    public bool isOccupied;

    private void Update() { transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z); }
}
