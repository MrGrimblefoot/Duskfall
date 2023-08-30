using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndSway : MonoBehaviour
{
    CameraLook camlook;
    PlayerRigidbodyMovement movement;
    WeaponSystem weaponSystem;
    Rigidbody rb;
    Weapon weaponData;

    [Header("Settings")]
    [SerializeField] private bool usePositionalSway;
    [SerializeField] private bool useRotationalSway;
    [SerializeField] private bool usePositionalBob;
    [SerializeField] private bool useRotationalBob;

    Vector3 swayPos;
    Vector3 swayEulerRot;
    private float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }
    Vector3 bobPos;
    Vector3 bobEulerRot;

    Vector2 moveInput;
    Vector2 lookInput;

    Quaternion originalRot;

    private float bobTime;
    bool isMoving;

    //// Add a new variable to track the starting time of the movement
    //private float moveStartTime;

    //// Add new variables for random bobbing offset
    //private float randomBobOffset;
    //[SerializeField] private float maxRandomBobOffset = 1f;
    //[SerializeField] private float initialBobMultiplier = 0.1f;

    void Start()
    {
        camlook = GetComponentInParent<CameraLook>();
        movement = FindObjectOfType<PlayerRigidbodyMovement>();
        rb = movement.GetComponent<Rigidbody>();
        weaponSystem = movement.GetComponent<WeaponSystem>();
        weaponData = weaponSystem.currentWeaponData;
        originalRot = transform.localRotation;

        //// Initialize bobTime to zero at the start
        //bobTime = 0f;

        //// Initialize moveStartTime to the current time at the start
        //moveStartTime = Time.time;

        //// Initialize randomBobOffset to a random value between 0 and maxRandomBobOffset at the start
        //randomBobOffset = Random.Range(0f, maxRandomBobOffset);
    }

    void Update()
    {
        GetInput();

        PositionalSway();
        RotationalSway();

        // Check if the player is moving
        //bool wasMoving = isMoving;
        //isMoving = moveInput.magnitude > 0.1f;

        //if (isMoving && (usePositionalBob || useRotationalBob))
        //{
        //    if (!wasMoving)
        //    {
        //        // If the player starts moving again, update the moveStartTime and generate a new random offset
        //        moveStartTime = Time.time;
        //        randomBobOffset = Random.Range(0f, maxRandomBobOffset);
        //    }
        //    bobTime = (Time.time - moveStartTime + randomBobOffset) * (movement.isGrounded ? rb.velocity.magnitude : 1f) * (movement.isMoving ? weaponData.movementBobSpeed : weaponData.idleBobSpeed);
        //}
        //else
        //{
        //    // If the player is not moving, decrease bobTime gradually to zero to center the gun smoothly
        //    if (bobTime > 0f)
        //    {
        //        float decreaseRate = weaponData.movementBobSpeed * initialBobMultiplier;
        //        bobTime = Mathf.Max(bobTime - Time.deltaTime * decreaseRate, 0f);
        //    }
        //}

        //PositionalBob();
        //RotationalBob();

        CompositePositionRotation();
    }

    //void PositionalBob()
    //{
    //    if (!usePositionalBob) { bobPos = Vector3.zero; return; }

    //    // Calculate positional bob values directly based on bobTime
    //    bobPos.x = Mathf.Sin(bobTime) * weaponData.bobLimit.x;
    //    bobPos.y = Mathf.Cos(bobTime * 2f) * weaponData.bobLimit.y - (rb.velocity.y * weaponData.travelLimit.y);
    //    bobPos.z = -moveInput.y * weaponData.travelLimit.z;
    //}

    //void RotationalBob()
    //{
    //    if (!useRotationalBob) { bobEulerRot = Vector3.zero; return; }

    //    // Calculate rotational bob values directly based on bobTime
    //    bobEulerRot.x = Mathf.Sin(bobTime * 2f) * weaponData.bobRotationalMultiplier.x;
    //    bobEulerRot.y = Mathf.Sin(bobTime) * weaponData.bobRotationalMultiplier.y;
    //    bobEulerRot.z = Mathf.Sin(bobTime) * weaponData.bobRotationalMultiplier.z * moveInput.x;
    //}

    void GetInput()
    {
        moveInput.x = movement.currentX;
        moveInput.y = movement.currentY;
        moveInput = moveInput.normalized;

        lookInput.x = camlook.mouseInput.x;
        lookInput.y = camlook.mouseInput.y;
    }

    void PositionalSway()
    {
        if (!usePositionalSway) { swayPos = Vector3.zero; return; }

        Vector3 invertLook = lookInput * (weaponSystem.aiming ? (weaponData.positionalAimSwayIntensity * 0.01f) : (weaponData.positionalSwayIntensity * 0.01f));
        invertLook.x = Mathf.Clamp(invertLook.x, -weaponData.maxSwayMovement, weaponData.maxSwayMovement);
        invertLook.y = Mathf.Clamp(invertLook.y, -weaponData.maxSwayMovement, weaponData.maxSwayMovement);

        swayPos = invertLook;
    }

    void RotationalSway()
    {
        if (!useRotationalSway) { swayEulerRot = Vector3.zero; return; }

        // Use Time.deltaTime to control the rotation speed
        float swayIntensity = weaponSystem.aiming ? (weaponData.rotationalAimSwayIntensity) : (weaponData.rotationalSwayIntensity);
        Vector3 invertLook = lookInput * swayIntensity * Time.deltaTime;
        invertLook.x = Mathf.Clamp(invertLook.x, -weaponData.maxSwayRotation, weaponData.maxSwayRotation);
        invertLook.y = Mathf.Clamp(invertLook.y, -weaponData.maxSwayRotation, weaponData.maxSwayRotation);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        transform.localPosition =
            Vector3.Lerp(transform.localPosition,
            swayPos + (!weaponData.isMelee ? (weaponSystem.aiming ? bobPos / weaponData.ADSPositionalBobDampening : bobPos) : bobPos),
            Time.deltaTime * weaponData.ADSSpeed);

        transform.localRotation =
            Quaternion.Slerp(transform.localRotation,
            originalRot * (Quaternion.Euler(swayEulerRot)
            * (!weaponData.isMelee ? (weaponSystem.aiming ? Quaternion.Euler(bobEulerRot / weaponData.ADSRotationalBobDampening)
                                                          : Quaternion.Euler(bobEulerRot))
                                   : Quaternion.Euler(bobEulerRot))),
            Time.deltaTime * weaponData.rotationalSmooth);
    }
}
