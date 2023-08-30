using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    public bool canLook;

    public float fieldOfView;
    [SerializeField] private float lookSensitivity;
    private float currentLookSensitivity;
    [SerializeField] private float smoothing;
    [SerializeField] private float lookMax;
    [SerializeField] private float lookMin;
    [SerializeField] private bool lockCursor = true;

    private int weaponAngle;
    private Vector3 weaponPos;

    [SerializeField] private Transform weapon;
    [SerializeField] private WeaponSystem weaponController;
    [SerializeField] private Transform orientation;

    private Vector2 smoothedVelocity;
    //private Vector2 currentLookingPos;
    private float xRotation;
    private float yRotation;

    private BasicInputActions basicInputActions;
    [HideInInspector] public Vector2 mouseInput;
    private GameManager gameManager;

    private void Start()
    {
        currentLookSensitivity = lookSensitivity;
        canLook = true;
        gameManager = FindObjectOfType<GameManager>();

        #region InputActions
        basicInputActions = new BasicInputActions();
        basicInputActions.Player.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        basicInputActions.Player.MouseX.Enable();
        basicInputActions.Player.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        basicInputActions.Player.MouseY.Enable();
        #endregion
    }

    private void Update()
    {
        if (canLook)
        {
            if (lockCursor == true) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
            else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }

            if (weaponController.currentWeapon != null)
            {
                if (weaponController.aiming) { currentLookSensitivity = lookSensitivity / weaponController.currentWeaponData.aimSensitivityMultiplier; }
                else { currentLookSensitivity = lookSensitivity; }
            }

            //calling the RotateCamera function
            RotateCamera();
        }
    }

    void RotateCamera()
    {
        //storing input
        Vector2 inputValues = new Vector2(mouseInput.x, gameManager != null ? (gameManager.invertY ? -mouseInput.y : mouseInput.y) : mouseInput.y);

        //smoothing
        inputValues = Vector2.Scale(inputValues, new Vector2(currentLookSensitivity * smoothing, currentLookSensitivity * smoothing));
        smoothedVelocity.x = Mathf.Lerp(smoothedVelocity.x, inputValues.x, 1f / smoothing);
        smoothedVelocity.y = Mathf.Lerp(smoothedVelocity.y, inputValues.y, 1f / smoothing);

        //taking current position and adding the new one
        xRotation += smoothedVelocity.x;
        yRotation += smoothedVelocity.y;

        //restricting the camera movement
        yRotation = Mathf.Clamp(yRotation, lookMax, lookMin);

        //move the camera on the y axis
        transform.rotation = Quaternion.Euler(-yRotation, xRotation, 0);
        orientation.rotation = Quaternion.Euler(0, -yRotation, 0);

        weapon.rotation = transform.rotation;
    }
}
