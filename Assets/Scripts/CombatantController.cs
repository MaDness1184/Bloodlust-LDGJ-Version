using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatantController : CivilianController
{
    [Header("Combatant Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCdr = 2f;
    [SerializeField] private float seekPlayerDuration = 120;

    private Coroutine seekPlayerCoroutine;
    private float nextAttack;
    private float attackRangeSqr;
    protected bool seekingPlayer;

    protected override void Start()
    {
        base.Start();
        attackRangeSqr = attackRange * attackRange;
    }
    protected override void Update()
    {
        if (hostile || upset || seekingPlayer) return;

        UpdateSchedule();
    }

    private void SeekPlayerCompleted()
    {
        StopSeekPlayer();
        Debug.Log("Cannot find player, return to schedule");
        SetHostile(false);
    }

    protected override void OnPlayerFound()
    {
        if (status.currentHP > status.maxHP / 2)
        {
            SetHostile(true);
            StopSeekPlayer();
            SloppyAIMode(true);
        }
        else
        {
            if (!upset)
            {
                SetHostile(false);
                FindClosetGuard();
                SetUpset(true);
            }
        }
    }

    protected override void OnPlayerNotFound()
    {
        Debug.Log("Player not found");
        SetHostile(false);

        if (status.currentHP > status.maxHP / 2)
        {
            Debug.Log("Seek player");
            TrySeekPlayer();
            SloppyAIMode(false);
        }
    }

    protected void TrySeekPlayer()
    {
        if (seekPlayerCoroutine == null)
        {
            seekingPlayer = true;
            Debug.Log("Player out of range, start seeking for player");
            seekPlayerCoroutine = StartCoroutine(GameManager.main.SetAlarm(seekPlayerDuration, SeekPlayerCompleted));
        }
    }

    protected void StopSeekPlayer()
    {
        if (seekPlayerCoroutine != null)
        {
            seekingPlayer = false;
            Debug.Log("Player in range, stop seeking for player");
            StopCoroutine(seekPlayerCoroutine);
            seekPlayerCoroutine = null;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        if (!hostile || upset) return;

        if (!player || playerHiding) return;
        var direction = player.position - transform.position;

        if (Time.time > nextAttack)
        {
            if (direction.sqrMagnitude < attackRangeSqr)
            {
                UpdateAnimDirection(direction);
                animator.SetTrigger("attackTrigger");
                PlayAttackVfx();

                player.GetComponent<PlayerStatus>().RecieveDamage(1);
                AlertNearbyPeople();

                nextAttack = Time.time + attackCdr;
            }
        }
    }
}
