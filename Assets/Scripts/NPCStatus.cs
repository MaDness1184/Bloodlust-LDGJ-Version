using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStatus : EntityStatus
{
    [Header("NPC settings")]
    [SerializeField] private float speedBuffDuration = 1;
    [SerializeField] private LayerMask bedLayer;

    private NPCController controller;
    private AIPath aiPath;
    private bool escalated = false;
    private float maxSpeedOriginal;

    private Coroutine speedBuffCoroutine;

    private float healCdr;
    private float nextHeal;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<NPCController>();
        aiPath = GetComponent<AIPath>();
        maxSpeedOriginal = aiPath.maxSpeed;
        healCdr = 60 / GameManager.main.timeScale;
    }

    public override void OnHit()
    {
        if (speedBuffCoroutine != null)
            StopCoroutine(speedBuffCoroutine);

        speedBuffCoroutine = StartCoroutine(SpeedBuffCoroutine());

        if (!escalated)
        {
            controller.TurnSuspicious();
            controller.AlertNearbyPeople();
            escalated = true;
        }
    }

    protected virtual void Update()
    {
        if(Time.time > nextHeal)
        {
            Heal(1);
            nextHeal = Time.time + healCdr;
        }
    }

    private IEnumerator SpeedBuffCoroutine()
    {
        aiPath.maxSpeed = maxSpeedOriginal * 2;
        yield return new WaitForSeconds(speedBuffDuration);
        aiPath.maxSpeed = maxSpeedOriginal;
    }

    public void SetEscalated(bool value)
    {
        escalated = value;
    }
}
