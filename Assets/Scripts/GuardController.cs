using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : CombatantController
{
    [Header("Guard Settings")]
    [SerializeField] private float patrolDuration = 120;

    private bool patrolling = false;
    private bool crimeNotified = false;

    private Coroutine patrolCoroutine;
    private Transform cachedCrimeScene;
    private CivilianController cachedNpc;

    protected override void Update()
    {
        if (hostile || patrolling) return;

        UpdateSchedule();
    }

    protected override void OnPlayerFound()
    {
        SetHostile(true);

        if(patrolling)
        {
            StopCoroutine(patrolCoroutine);
            patrolling = false;
        }

        StopSeekPlayer();
        SloppyAIMode(true);
    }

    protected override void OnPlayerNotFound()
    {
        SetHostile(false);

        if(crimeNotified)
            SetPatrol(cachedCrimeScene, cachedNpc);
        else
        {
            TrySeekPlayer();
            SloppyAIMode(false);
        }
            
    }

    public void SetPatrol(Transform crimeScene, CivilianController npcCallback)
    {
        if (patrolling) return;

        Debug.Log(gameObject.name + "Crime notified");

        crimeNotified = true;
        SloppyAIMode(true);

        cachedCrimeScene = crimeScene;
        cachedNpc = npcCallback;

        patrolCoroutine = StartCoroutine(Patrol(crimeScene, npcCallback));
    }

    private IEnumerator Patrol(Transform crimeScene, CivilianController npcCallback)
    {
        // Move to crime scene
        patrolling = true;
        destinationSetter.target = crimeScene;

        UpdateAnimDirection(npcCallback.transform.position - transform.position);
        yield return new WaitForSeconds(2);
        
        while (!aiPath.reachedDestination || aiPath.destination != crimeScene.position)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Crime scene reached = " + GameManager.main.currentMinuteRaw);

        // Patrol until time run out
        float endTime = GameManager.main.currentMinuteRaw + patrolDuration;
        float timeScale = GameManager.main.timeScale;
        Debug.Log("Patrol started = " + GameManager.main.currentMinuteRaw);
        Debug.Log("Patrol endeing = " + endTime);

        while (GameManager.main.currentMinuteRaw <= endTime)
        {
            yield return new WaitForSeconds(1 / timeScale);
        }

        Debug.Log("Patrol ended = " + GameManager.main.currentMinuteRaw);

        // Go back to normal
        destinationSetter.target = previousWaypoint;
        npcCallback.SetUpset(false);

        cachedCrimeScene = null;
        cachedNpc = null;

        patrolling = false;
        crimeNotified = false;

        SloppyAIMode(false);
    }
}
