using UnityEngine;
using UnityEngine.AI;

public class NPCWander : MonoBehaviour
{
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

        for (int i = 0; i < 30; i++)
        {
            Vector3 randomDirection;

            if (i < 15)
            {
                randomDirection = (transform.forward + Random.insideUnitSphere * 0.8f).normalized * wanderRadius;
            }
            else
            {
                randomDirection = Random.insideUnitSphere * wanderRadius;
            }

            Vector3 randomPoint = transform.position + randomDirection;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, 1.5f, agent.areaMask))
            {
                bestTarget = hit.position;
                foundValidPoint = true;
                break;
            }
        }

        if (!foundValidPoint)
        {
            transform.Rotate(0, Random.Range(90f, 180f), 0);
        }

        agent.SetDestination(bestTarget);
        isWaiting = false;
    }
}