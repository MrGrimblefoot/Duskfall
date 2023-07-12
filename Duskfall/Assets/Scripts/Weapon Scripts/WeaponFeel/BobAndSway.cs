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

    [SerializeField] private Transform hipPosObject;
    [SerializeField] private Transform aimPosObject;
    private Vector3 hipPos;
    private Vector3 aimPos;
    Quaternion originalRot;

    void Start()
    {
        camlook = GetComponentInParent<CameraLook>();
        movement = FindObjectOfType<PlayerRigidbodyMovement>();
        rb = movement.GetComponent<Rigidbody>();
        weaponSystem = movement.GetComponent<WeaponSystem>();
        weaponData = weaponSystem.currentWeaponData;
        originalRot = transform.localRotation;
        hipPos = hipPosObject.transform.localPosition;
        aimPos = weaponSystem.hasSight ?
            new Vector3(
                aimPosObject.transform.localPosition.x, 
                aimPosObject.transform.localPosition.y + weaponData.sightOffset, 
                aimPosObject.transform.localPosition.z)
                                       : aimPosObject.transform.localPosition;
    }

    void Update()
    {
        GetInput();

        PositionalSway();
        RotationalSway();

        if (usePositionalBob || useRotationalBob)
        {
            speedCurve += Time.deltaTime * (movement.isGrounded ? rb.velocity.magnitude : 1f) + (movement.isMoving ? 0.01f * weaponData.movementBobSpeed : 0.01f * weaponData.idleBobSpeed);
        }

        PositionalBob();
        RotationalBob();

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

        Vector3 invertLook = lookInput * (weaponSystem.aiming ? (weaponData.rotationalAimSwayIntensity * 0.01f) : (weaponData.rotationalSwayIntensity * 0.01f));
        invertLook.x = Mathf.Clamp(invertLook.x, -weaponData.maxSwayRotation, weaponData.maxSwayRotation);
        invertLook.y = Mathf.Clamp(invertLook.y, -weaponData.maxSwayRotation, weaponData.maxSwayRotation);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void PositionalBob()
    {
        if (!usePositionalBob) { bobPos = Vector3.zero; return; }

        bobPos.x = (curveCos * weaponData.bobLimit.x * (movement.isGrounded ? 1 : 0)) - (moveInput.x * weaponData.travelLimit.x); //bob - input offser

        bobPos.y = (curveSin * weaponData.bobLimit.y) - (rb.velocity.y * weaponData.travelLimit.y);//bob - y velocity offset

        bobPos.z = -(moveInput.y * weaponData.travelLimit.z); //- input offset
    }

    void RotationalBob()
    {
        if (!useRotationalBob) { bobEulerRot = Vector3.zero; return; }

        bobEulerRot.x = (moveInput != Vector2.zero ? weaponData.bobRotationalMultiplier.x * (Mathf.Sin(2 * speedCurve)) : weaponData.bobRotationalMultiplier.x * (Mathf.Sin(2 * speedCurve) / 2)); //pitch
        bobEulerRot.y = (moveInput != Vector2.zero ? weaponData.bobRotationalMultiplier.y * curveCos : 0); //yaw
        bobEulerRot.z = (moveInput != Vector2.zero ? weaponData.bobRotationalMultiplier.z * curveCos * moveInput.x : 0); //roll
    }

    void CompositePositionRotation()
    {
        transform.localPosition =
            Vector3.Lerp(transform.localPosition,
            swayPos + (!weaponData.isMelee ? (weaponSystem.aiming ? bobPos / weaponData.ADSPositionalBobDampening : bobPos)/* + (weaponSystem.aiming ? aimPos : hipPos)*/ : bobPos),
            Time.deltaTime * weaponData.ADSSpeed);

        //transform.localPosition =
        //    Vector3.Lerp(transform.localPosition,
        //    swayPos + (weaponSystem.aiming ? aimPos : hipPos),
        //    Time.deltaTime * weaponData.adsSpeed);

        transform.localRotation =
            Quaternion.Slerp(transform.localRotation,
            originalRot * (Quaternion.Euler(swayEulerRot)
            * (!weaponData.isMelee ? (weaponSystem.aiming ? Quaternion.Euler(bobEulerRot / weaponData.ADSRotationalBobDampening)
                                                          : Quaternion.Euler(bobEulerRot))
                                   : Quaternion.Euler(bobEulerRot))),
            Time.deltaTime * weaponData.rotationalSmooth);
    }
}
