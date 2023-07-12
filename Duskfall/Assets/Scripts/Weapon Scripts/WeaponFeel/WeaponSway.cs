using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private float amount;
    [SerializeField] private float maxAmountX;
    [SerializeField] private float maxAmountY;
    [SerializeField] private float smooth;
    private Vector3 initialPosition;
    private CameraLook camLook;

    void Start()
    {
        camLook = GetComponentInParent<CameraLook>();
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        PositionalSway();
    }

    void PositionalSway()
    {
        float movementX = -camLook.mouseInput.x * (amount * 0.01f);
        float movementY = -camLook.mouseInput.y * (amount * 0.01f);

        movementX = Mathf.Clamp(movementX, -maxAmountX, maxAmountX);
        movementY = Mathf.Clamp(movementY, -maxAmountY, maxAmountY);

        Vector3 finalPos = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPosition, Time.deltaTime * smooth);
    }
}
