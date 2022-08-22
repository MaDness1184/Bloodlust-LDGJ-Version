using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct WaypointEvent
{
    public int time;
    public Transform waypoint;
}

public class NPCController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float scanCdr = 0.5f;
    [SerializeField] private float interactionRadius = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private AudioClip[] attackVfxs;

    [Header("Suspicion Settings")]
    [SerializeField] protected LayerMask npcLayerMask;
    [SerializeField] private float alertRadius = 5f;
    [SerializeField] private float playerScanCdr = 0.1f;
    [SerializeField] private float detectDistance = 4;

    [Header("Schedule Settings")]
    [SerializeField] private float updateCdr = 0.1f;
    [SerializeField] protected Transform defaultPosition;
    [SerializeField] private WaypointEvent[] waypointSchedule;

    [SerializeField] protected bool suspicious = false;
    [SerializeField] protected bool hostile = false;

    private int cachedScheduleHour;
    private float detectDistanceSqr;

    private Dictionary<int, Transform> internalSchedule = new Dictionary<int, Transform>();
    protected AIDestinationSetter destinationSetter;
    protected AIPath aiPath;
    protected NPCStatus status;
    protected Animator animator;
    protected Transform player;
    private AudioSource audioSource;

    private float nextScheduleUpdate;
    private float nextPlayerScan;
    private float nextInteractableScan;
    protected Transform previousWaypoint;

    protected bool playerHiding = false;

    protected virtual void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        status = GetComponent<NPCStatus>();
        animator = GetComponent<Animator>();
        player = GameManager.main.GetPlayer();
        audioSource = GetComponent<AudioSource>();

        detectDistanceSqr = detectDistance * detectDistance;
        previousWaypoint = defaultPosition;

        foreach (WaypointEvent waypointEvent in waypointSchedule)
        {
            if (!internalSchedule.ContainsKey(waypointEvent.time))
                internalSchedule.Add(waypointEvent.time, waypointEvent.waypoint);
        }


    }

    protected virtual void Update()
    {
        UpdateSchedule();
    }

    protected virtual void FixedUpdate()
    {
        ScanInteractable();
        ScanForPlayer();
        SetMovementAnimation();
    }
    protected void PlayAttackVfx()
    {
        if(attackVfxs != null && attackVfxs.Length > 0)
            audioSource.PlayOneShot(attackVfxs[Random.Range(0, attackVfxs.Length)]);
    }

    //Collider2D hitCached;

    private void ScanForPlayer()
    {
        if (!suspicious || !player) return;

        var direction = player.position - transform.position;

        if (Time.time > nextPlayerScan)
        {
            if (direction.sqrMagnitude < detectDistanceSqr)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 20f, playerLayer);
                
                //hitCached = hit.collider;
                if (hit)
                {
                    if (playerHiding)
                        OnPlayerNotFound();
                    else
                        OnPlayerFound();
                }
                else
                {
                    OnPlayerNotFound();
                }
            }
            else
            {
                OnPlayerNotFound();
                playerHiding = false;
            }

            nextPlayerScan = Time.time + playerScanCdr;
        }
    }

    protected virtual void OnPlayerFound()
    {

    }

    protected virtual void OnPlayerNotFound()
    {

    }

    private void SetMovementAnimation()
    {
        var velocity = aiPath.desiredVelocity;
        if (aiPath.desiredVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            //animator.SetBool("isInteracting", false);
            animator.SetBool("isMoving", true);
            UpdateAnimDirection(velocity);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    protected void UpdateAnimDirection(Vector2 direction)
    {
        direction.Normalize();
        animator.SetFloat("directionX", direction.x);
        animator.SetFloat("directionY", direction.y);
    }

    //[SerializeField] Collider2D[] cachedHits;

    private void ScanInteractable()
    {
        if (Time.time > nextInteractableScan)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,
                interactionRadius, interactableLayer);
            //cachedHits = hits;

            if (hits.Length > 1)
            {
                foreach (Collider2D hit in hits)
                {
                    if (hit.transform == transform) continue;
                    if (hit.tag == "Interactable")
                    {
                        //animator.SetBool("isInteracting", true);
                    }
                    else
                    {
                        speechBubble.SetActive(true);
                    }
                }
            }
            else
            {
                speechBubble.SetActive(false);
            }

            nextInteractableScan = Time.time + scanCdr;
        }
    }

    protected void UpdateSchedule()
    {
        if (Time.time < nextScheduleUpdate) return;

        var currentTime = (int)GameManager.main.currentHour;
        if (currentTime == cachedScheduleHour)
        {
            nextScheduleUpdate = Time.time + updateCdr;
        }
        else
        {
            if (internalSchedule.ContainsKey(currentTime))
            {
                previousWaypoint = destinationSetter.target;
                destinationSetter.target = internalSchedule[currentTime];
            }
        }
    }

    public virtual void TurnSuspicious()
    {
        suspicious = true;
    }

    public virtual void SetHostile(bool value)
    {
        hostile = value;

        if (hostile)
        {
            if (destinationSetter.target != player)
                destinationSetter.target = player;
        }
        else
        {
            if (destinationSetter.target == player)
                destinationSetter.target = null;
        }
    }

    public void AlertNearbyPeople()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, alertRadius, npcLayerMask);

        if (hits.Length <= 0) return;

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out NPCController controller))
                controller.TurnSuspicious();
        }
    }

    protected void SloppyAIMode(bool sloppy)
    {
        //Debug.Log(gameObject.name + " sloppy = " + sloppy);
        if (sloppy)
        {
            aiPath.endReachedDistance = 2;
            aiPath.whenCloseToDestination = CloseToDestinationMode.Stop;
        }
        else
        {
            aiPath.endReachedDistance = 0.01f;
            aiPath.whenCloseToDestination = CloseToDestinationMode.Stop;
        }
    }

    public void SetPlayerHiding(bool value)
    {
        playerHiding = value;
        Debug.Log("player hiding = " + value);
    }
}
