using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRigidbodyMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private WeaponSystem weaponSystem;
    public Transform orientation;
    private BasicInputActions basicInputActions;
    
    [Header("Bools")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool isSprinting => canSprint && sprintingInput && !weaponSystem.aiming;
    private bool sprintingInput;
    [SerializeField] private bool isCrouching;
    public bool isMoving;
    public bool isGrounded;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 7.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] private float groundDrag;
    private float currentSpeed = 0.0f;
    public float currentX;
    public float currentY;
    Vector3 moveDirection;

    [Header("Jumping & Air Control")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float gravity;
    bool readyToJump;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckObj;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Mantling")]
    [SerializeField] private float playerHeight = 1.8288f;
    [SerializeField] private float playerRadius = 0.18f;
    public float interactRange = 1f;
    [SerializeField] private LayerMask mantleLayer;
    private Transform cam;

    [Header("Climbing")]
    [SerializeField] private float climbBoostHeight = 1f;
    [SerializeField] private float climbDuration = 0.6f;
    
    [Header("Vaulting")]
    [SerializeField] private float vaultHeight = 0.6f;
    [SerializeField] private float vaultBoostHeight = 1f;
    [SerializeField] private float vaultBoostDistance = 1f;
    [SerializeField] private float vaultDuration = 0.6f;
    [SerializeField] private Vector2 vaultBoostDampening;

    [Header("Debug")]
    [SerializeField] private Transform debugObject;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        weaponSystem = GetComponent<WeaponSystem>();
        cam = Camera.main.transform;

        #region InputActions
        basicInputActions = new BasicInputActions();
        basicInputActions.Player.Jump.performed += Jump;
        basicInputActions.Player.Jump.Enable();
        basicInputActions.Player.Mantle.performed += DoMantleCheck;
        basicInputActions.Player.Mantle.Enable();
        //basicInputActions.Player.Crouch.performed += Crouch;
        //basicInputActions.Player.Crouch.Enable();
        basicInputActions.Player.Sprint.Enable();
        basicInputActions.Player.Sprint.started += _ => sprintingInput = true;
        basicInputActions.Player.Sprint.canceled += _ => sprintingInput = false;
        basicInputActions.Player.Movement.Enable();
        #endregion

        currentSpeed = walkSpeed;

        readyToJump = true;
    }

    private void Update()
    {
        if (canMove)
        {
            GetInput();
            HandleSpeed();
            LimitVelocity();
            CheckGrounded();
            ApplyGravity();
        }
    }

    private void FixedUpdate() { MovePlayer(); }

    void GetInput()
    {
        currentX = basicInputActions.Player.Movement.ReadValue<Vector2>().x;
        currentY = basicInputActions.Player.Movement.ReadValue<Vector2>().y;
        if (currentX == 0 && currentY == 0) { isMoving = false; } else { isMoving = true; }
    }

    private void HandleSpeed()
    {
        if (isCrouching) { currentSpeed = crouchSpeed; }
        else if (isSprinting && !isCrouching) { currentSpeed = sprintSpeed; }
        else if (!isSprinting && !isCrouching) { currentSpeed = walkSpeed; }
    }

    private void LimitVelocity()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * currentY + orientation.right * currentX;
        moveDirection.y = 0;

        if (isGrounded) { rb.AddForce(moveDirection.normalized * (currentSpeed * 10), ForceMode.Force); }
        else { rb.AddForce(moveDirection.normalized * (currentSpeed * 10 * airMultiplier), ForceMode.Force); }
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundCheckObj.position, Vector3.down, 0.2f, whatIsGround);

        if (isGrounded) { rb.drag = groundDrag; }
        else { rb.drag = 0; }
    }

    void Jump(InputAction.CallbackContext context)
    {
        if(readyToJump && isGrounded)
        {
            readyToJump = false;

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump() { readyToJump = true; }

    void DoMantleCheck(InputAction.CallbackContext context) { CheckForMantle(); }

    void CheckForMantle()
    {
        if (Physics.Raycast(orientation.transform.position, orientation.transform.forward, out var firstHit, interactRange, mantleLayer))
        {
            debugObject.transform.position = firstHit.point;
            ClimbObject climbObject = firstHit.transform.gameObject.GetComponentInParent<ClimbObject>();
            if (climbObject.climbOrVault == true)
            {
                Vault(climbObject);
            }
            else
            {
                if (Physics.Raycast(firstHit.point + (orientation.transform.forward * playerRadius) + (Vector3.up * vaultHeight * playerHeight), Vector3.down, out var secondHit, playerHeight))
                    StartCoroutine(LerpClimb(new Vector3(secondHit.point.x, secondHit.point.y + climbBoostHeight, secondHit.point.z), climbDuration));
            }
        }
    }


    void Vault(ClimbObject collider)
    {
        StartCoroutine(DisableCollider(vaultDuration));
        rb.AddForce(isMoving ? transform.up * (vaultBoostHeight / vaultBoostDampening.y) : transform.up * vaultBoostHeight, ForceMode.Impulse);
        rb.AddForce(isMoving ? orientation.transform.forward * (vaultBoostDistance / vaultBoostDampening.x) : orientation.transform.forward * vaultBoostDistance, ForceMode.Impulse);
    }

    IEnumerator DisableCollider(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            playerCollider.enabled = false;
            time += Time.deltaTime;
            yield return null;
        }
        playerCollider.enabled = true;
    }

    IEnumerator LerpClimb(Vector3 targetPos, float duration)
    {
        float time = 0;
        Vector3 startPos = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Slerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    void ApplyGravity() { if (!isGrounded) { rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.deltaTime, rb.velocity.z); } }
}
