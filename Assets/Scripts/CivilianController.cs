using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianController : NPCController
{
    [Header("Civilian Settings")]
    [SerializeField] private float findGuardRadius = 500f;

    protected bool upset = false;
    protected Transform crimeScene;

    protected override void Update()
    {
        if (upset) return;

        UpdateSchedule();
    }

    protected override void OnPlayerFound()
    {
        if (!upset)
        {
            FindClosetGuard();
            SetUpset(true);
        }
    }

    protected void FindClosetGuard()
    {
        // Create a crime scene
        GameObject crimeSceneInit = new GameObject("Crime Scene");
        crimeSceneInit.transform.position = transform.position;
        crimeScene = crimeSceneInit.transform;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, findGuardRadius, npcLayerMask);

        if (hits.Length <= 0) return;

        foreach (Collider2D hit in hits)
        {
            if (hit.tag == "Guard")
            {
                StartCoroutine(ReportCrime(hit.transform));
                break;
            }
        }
    }

    private IEnumerator ReportCrime(Transform guard)
    {
        destinationSetter.target = guard;
        //(transform.position - guard.position).sqrMagnitude > 4 &&
        while (!aiPath.reachedDestination)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Crime reported");

        var guardController = guard.GetComponent<GuardController>();
        guardController.TurnSuspicious();
        guardController.SetPatrol(crimeScene, this);
    }

    public void SetUpset(bool value)
    {
        Debug.Log(gameObject.name + " upset = " + true);

        upset = value;

        if (!upset)
            destinationSetter.target = previousWaypoint;

        SloppyAIMode(value);
    }
}
