using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP : MonoBehaviour
{
    CameraLook camlook;
    PlayerRigidbodyMovement movement;
    WeaponSystem weaponSystem;
    Weapon weaponData;

    [Header("Settings")]
    [SerializeField] private bool useRotationalSway;
    [SerializeField] private float rotationalSwaySpeed = 30f;

    Vector3 swayEulerRot;
    Vector3 bobEulerRot;
    private Vector3 rotationalSwayVelocity;

    Vector2 moveInput;
    Vector2 lookInput;

    Quaternion originalRot;

    void Start()
    {
        camlook = GetComponentInParent<CameraLook>();
        movement = FindObjectOfType<PlayerRigidbodyMovement>();
        weaponSystem = movement.GetComponent<WeaponSystem>();
        weaponData = weaponSystem.currentWeaponData;
        originalRot = transform.localRotation;
    }

    void Update()
    {
        GetInput();
        RotationalSway();
        CompositePositionRotation();
    }

    void GetInput()
    {
        moveInput.x = movement.currentX;
        moveInput.y = movement.currentY;
        moveInput = moveInput.normalized;

        lookInput.x = camlook.mouseInput.x;
        lookInput.y = camlook.mouseInput.y;
    }

    void RotationalSway()
    {
        if (!useRotationalSway) { swayEulerRot = Vector3.zero; return; }

        // Use Time.deltaTime to control the rotation speed
        float swayIntensity = weaponSystem.aiming ? (weaponData.rotationalAimSwayIntensity * 0.01f) : (weaponData.rotationalSwayIntensity * 0.01f);
        Vector3 invertLook = lookInput * (swayIntensity * rotationalSwaySpeed);
        invertLook.x = Mathf.Clamp(invertLook.x, -weaponData.maxSwayRotation, weaponData.maxSwayRotation);
        invertLook.y = Mathf.Clamp(invertLook.y, -weaponData.maxSwayRotation, weaponData.maxSwayRotation);

        // Apply damping to rotational sway
        swayEulerRot = Vector3.SmoothDamp(swayEulerRot, new Vector3(invertLook.y, invertLook.x, invertLook.x), ref rotationalSwayVelocity, 1f / rotationalSwaySpeed);
    }

    void CompositePositionRotation()
    {
        // Calculate the rotation change per frame based on time
        float rotationSpeed = Time.deltaTime * weaponData.rotationalSmooth * rotationalSwaySpeed;

        // Interpolate the current rotation with the target sway rotation
        Quaternion targetRotation = originalRot * Quaternion.Euler(swayEulerRot);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed);
    }
}
