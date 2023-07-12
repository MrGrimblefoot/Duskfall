using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using Photon.Pun;

public class Sway : MonoBehaviour/*PunCallbacks*/
{
    #region Variables
    private Quaternion originRotation;
    [SerializeField] private WeaponSystem weaponSystem;

    [SerializeField] private float targetXMouse = 0f;
    [SerializeField] private float targetYMouse = 0f;

    private PlayerInput playerInput;
    private BasicInputActions basicInputActions;
    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
        originRotation = transform.localRotation;
        weaponSystem = FindObjectOfType<WeaponSystem>();

        #region InputActions
        basicInputActions = new BasicInputActions();
        basicInputActions.Player.MouseX.performed += ctx => targetXMouse = ctx.ReadValue<float>();
        basicInputActions.Player.MouseX.Enable();
        basicInputActions.Player.MouseY.performed += ctx => targetYMouse = ctx.ReadValue<float>();
        basicInputActions.Player.MouseY.Enable();
        #endregion

    }
    void Update()
    {
        UpdateSway();
    }
    #endregion

    #region Private Methods
    private void UpdateSway()
    {
        //calculate target rotation
        //Quaternion tempXAdj = Quaternion.AngleAxis((!weaponSystem.aiming ? (weaponSystem.currentWeaponData.swayIntensity / 50) * weaponSystem.currentWeaponData.rotKickReturnSpeed : (weaponSystem.currentWeaponData.aimSwayIntensity / 50) * weaponSystem.currentWeaponData.aimRotKickReturnSpeed) * targetXMouse, Vector3.up);
        //Quaternion tempYAdj = Quaternion.AngleAxis((!weaponSystem.aiming ? (weaponSystem.currentWeaponData.swayIntensity / 50) * weaponSystem.currentWeaponData.rotKickReturnSpeed : (weaponSystem.currentWeaponData.aimSwayIntensity / 50) * weaponSystem.currentWeaponData.aimRotKickReturnSpeed) * targetYMouse, Vector3.right);
        //Quaternion tempZAdj = Quaternion.AngleAxis((!weaponSystem.aiming ? (weaponSystem.currentWeaponData.swayIntensity / 50) * weaponSystem.currentWeaponData.rotKickReturnSpeed : (weaponSystem.currentWeaponData.aimSwayIntensity / 50) * weaponSystem.currentWeaponData.aimRotKickReturnSpeed) * targetYMouse, Vector3.right);
        //Quaternion targetRotation = originRotation * tempXAdj * tempYAdj * tempZAdj;

        //rotate towards target rotation
        //if (!weaponSystem.aiming) { transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * weaponSystem.currentWeaponData.rotKickReturnSpeed); }
        //else { transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * weaponSystem.currentWeaponData.aimRotKickReturnSpeed); }
    }
    #endregion
}
