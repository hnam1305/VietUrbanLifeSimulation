using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCSmartWander : MonoBehaviour
{
    public float walkSpeed = 1.5f;
    public float stopRadius = 0.5f;
    public float stuckTimeout = 2f;
    public List<PointOfInterest> availablePOIs = new List<PointOfInterest>();

    private NavMeshAgent agent;
    private Animator animator;
    private bool isBusy = false;
    private PointOfInterest currentPOI;
    private float stuckTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.speed = walkSpeed;
        agent.stoppingDistance = stopRadius;

        if (availablePOIs.Count == 0)
        {
            availablePOIs = new List<PointOfInterest>(FindObjectsByType<PointOfInterest>(FindObjectsSortMode.None));
        }

        MoveToNextPOI();
    }

    void Update()
    {
        if (isBusy || currentPOI == null) return;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    StartCoroutine(PerformActionAtPOI());
                }
            }
            else if (agent.hasPath)
            {
                if (agent.velocity.sqrMagnitude < 0.1f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer >= stuckTimeout)
                    {
                        stuckTimer = 0f;
                        MoveToNextPOI();
                    }
                }
                else
                {
                    stuckTimer = 0f;
                }
            }
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void MoveToNextPOI()
    {
        if (availablePOIs.Count == 0) return;

        PointOfInterest nextPOI = currentPOI;
        int attempts = 0;

        while (nextPOI == currentPOI && attempts < 10)
        {
            nextPOI = availablePOIs[Random.Range(0, availablePOIs.Count)];
            attempts++;
        }

        currentPOI = nextPOI;

        agent.isStopped = false;
        agent.SetDestination(currentPOI.transform.position);
        isBusy = false;
        stuckTimer = 0f;
    }

    private IEnumerator PerformActionAtPOI()
    {
        isBusy = true;
        agent.isStopped = true;
        stuckTimer = 0f;

        transform.rotation = currentPOI.transform.rotation;

        if (animator != null && !string.IsNullOrEmpty(currentPOI.animTriggerName))
        {
            animator.SetTrigger(currentPOI.animTriggerName);
        }

        float waitTime = Random.Range(currentPOI.minWaitTime, currentPOI.maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        MoveToNextPOI();
    }
}