using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityInteraction : MonoBehaviour
{
    [Header("Player Interaction Settings")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private float scanCdr = 0.1f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask interactBlockLayer;

    [Header("Debugs")]
    [SerializeField] private bool currentColliderFound = false;
    [SerializeField] private Interactable closetInteractable;
    [SerializeField] private Interactable currentInteractable;

    private float nextScan;
    private float closetSqrDistance = 99f;
    private Collider2D closetCollider;
    private Collider2D currentCollider;

    PlayerController playerController;
    PlayerStatus playerStatus;


    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStatus = GetComponent<PlayerStatus>();
    }

    private void FixedUpdate()
    {
        ScanInteractable();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (currentInteractable == null) return;

            if (currentInteractable.TryGetComponent(out WeaponInteractable weaponInteractable))
            {
                if(playerController.GetCurrentWeapn() != null)
                {
                    Instantiate(playerController.GetCurrentWeapn().gameObject,
                        weaponInteractable.transform.position, Quaternion.identity);
                }

                playerController.SetWeapon(weaponInteractable.weapon);
                weaponInteractable.Interact(this);
            }
            else if(currentInteractable.TryGetComponent(out ItemInteractable itemInteractable))
            {
                if(itemInteractable.type == ItemInteractableType.Corpse)
                {
                    playerStatus.FillBlood(itemInteractable.value);
                }

                itemInteractable.Interact(this);
            }
        }
    }

    private void ScanInteractable()
    {
        if (Time.time < nextScan) return;
        nextScan = Time.time + scanCdr;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,
            interactionRadius, interactableLayer);

        if (colliders.Length <= 0)
        {
            ClearClosetInteractable();
            return;
        }

        closetSqrDistance = 99f;
        closetCollider = null;

        foreach (Collider2D collider in colliders)
        {
            //if (collider.transform == transform) continue;

            var sqrDistanceScan =
                (transform.position - collider.transform.position).sqrMagnitude;

            if (sqrDistanceScan > closetSqrDistance) continue;

            closetSqrDistance = sqrDistanceScan;
            closetCollider = collider;
        }


        // Check if the collider is already highlighted
        if (currentCollider != null)
        {
            if (currentCollider == closetCollider) return;
        }
/*        else
        {
            return;
        }*/

        // Check if wall is in the way
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                closetCollider.transform.position - transform.position,
                Vector2.Distance(closetCollider.transform.position, transform.position),
                interactBlockLayer);
        if (hit.collider != null) return;

        if (closetCollider.TryGetComponent(out Interactable interactable))
        {
            currentColliderFound = true;
            currentCollider = closetCollider;

            currentInteractable?.LocalHighlight(false);
            currentInteractable = interactable;
            currentInteractable.LocalHighlight(true);
        }

        // If currentInteractable is not found when scanning, assumes player is out of range
        //  and remove it from cache
        if (!currentColliderFound)
            ClearClosetInteractable();
        else
            currentColliderFound = false;
    }

    private void ClearClosetInteractable()
    {
        //if (holdingPlaceable) return;
        if (currentInteractable != null)
        {
            currentInteractable?.LocalHighlight(false);
            currentInteractable = null;
        }
            
        currentCollider = null;
    }
}
