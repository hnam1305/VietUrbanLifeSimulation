using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWander : MonoBehaviour
{
    [Header("Wander Settings")]
    [Tooltip("How far the NPC can wander in one go")]
    public float wanderRadius = 15f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 4f;

    private NavMeshAgent agent;
    private float waitTimer;
    private bool isWaiting;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
    }

    void Update()
    {
        // Safe check to see if the NPC has arrived at its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = Random.Range(minWaitTime, maxWaitTime);
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    SetNewDestination();
                }
            }
        }
    }

    void SetNewDestination()
    {
        Vector3 bestTarget = transform.position;
        bool foundValidPoint = false;

        // OPTIMIZATION 1: Drop the loop to 5 max attempts. 
        // 30 calls to SamplePosition per NPC will crash your frame rate in crowds.
        int maxAttempts = 5;

        for (int i = 0; i < maxAttempts; i++)
        {
            // OPTIMIZATION 2: Use insideUnitCircle for ground-based NPCs. 
            // This prevents generating random points high in the sky or deep underground.
            Vector2 randomCircle = Random.insideUnitCircle;
            Vector3 randomDirection;

            // Add a slight forward bias for the first 2 attempts so they don't look robotic
            if (i < 2)
            {
                randomDirection = transform.forward + new Vector3(randomCircle.x, 0, randomCircle.y);
            }
            else
            {
                randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
            }

            Vector3 randomPoint = transform.position + (randomDirection.normalized * wanderRadius);
            NavMeshHit hit;

            // OPTIMIZATION 3: SamplePosition is heavy. We only do this a maximum of 5 times now.
            if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, agent.areaMask))
            {
                bestTarget = hit.position;
                foundValidPoint = true;
                break;
            }
        }

        // Fallback: If it genuinely can't find a spot (e.g., stuck in a tight corner)
        if (!foundValidPoint)
        {
            // Instead of forcing a physical rotation that fights the NavMeshAgent, 
            // just set a destination slightly behind the NPC to help them back out safely.
            bestTarget = transform.position - (transform.forward * 2f);
        }

        agent.SetDestination(bestTarget);
        isWaiting = false;
    }
}