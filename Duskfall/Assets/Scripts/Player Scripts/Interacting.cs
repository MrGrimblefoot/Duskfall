using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interacting : MonoBehaviour
{
    [SerializeField] private LayerMask isInteractable;
    public BasicInputActions basicInputActions;
    private PlayerRigidbodyMovement movement;
    Interactable interactable;

    void Start()
    {
        movement = GetComponent<PlayerRigidbodyMovement>();

        basicInputActions = new BasicInputActions();
        basicInputActions.Player.Interact.performed += CheckForInteractable;
        basicInputActions.Player.Interact.Enable();
    }

    void CheckForInteractable(InputAction.CallbackContext context)
    {
        RaycastHit hit;

        if (Physics.Raycast(movement.orientation.position, movement.orientation.forward, out hit, movement.interactRange, isInteractable))
        {
            if(interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
            {
                interactable = hit.collider.GetComponent<Interactable>();
            }

            if (hit.collider.GetComponent<Interactable>() != false)
            {
                interactable.OnInteract.Invoke();
            }
        }
    }
}
