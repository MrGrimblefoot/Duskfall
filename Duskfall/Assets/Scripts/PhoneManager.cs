using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneManager : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Transform player;
    [SerializeField] private float turnOnDelay;
    private WeaponSystem weaponSystem;
    private PlayerRigidbodyMovement movement;
    private BasicInputActions basicInputActions;
    private CameraLook camLook;
    [HideInInspector] public bool isUsing;
    [SerializeField] private MeshRenderer mat;

    void Awake()
    {
        anim = GetComponent<Animator>();
        weaponSystem = player.GetComponent<WeaponSystem>();
        movement = player.GetComponent<PlayerRigidbodyMovement>();
        camLook = GetComponentInParent<CameraLook>();

        basicInputActions = new BasicInputActions();
        basicInputActions.Player.Phone.performed += TogglePhone;
        basicInputActions.Player.Phone.Enable();
    }

    void TogglePhone(InputAction.CallbackContext context)
    {
        if (!weaponSystem.aiming && !weaponSystem.shooting)
        {
            anim.SetBool("InUse", !isUsing);
            if (!isUsing) { weaponSystem.Stow(); }
            else { weaponSystem.Draw(); }
            TurnOnOffPhone();
            weaponSystem.enabled = isUsing;
            movement.enabled = isUsing;
            camLook.canLook = isUsing;
            isUsing = !isUsing;
        }
    }

    void TurnOnOffPhone()
    {
        if (!isUsing) { Invoke("ActivatePhone", turnOnDelay); }
        else { mat.material.SetColor("_EmissionColor", Color.white * 0); }
    }

    void ActivatePhone() { mat.material.SetColor("_EmissionColor", Color.white * 1); }
}
