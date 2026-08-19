using System.Collections.Generic;
using UnityEngine;

public class CarTraffic : MonoBehaviour
{
    [Header("Path & Speed Settings")]
    public List<Transform> waypoints;
    public float maxMoveSpeed = 15f;
    public float turnSpeed = 5f;
    public float reachDistance = 2f;
    public bool loopPath = true;

    [Header("Safety Brake Sensor")]
    public float sensorLength = 6f;
    public Vector3 sensorOffset = new Vector3(0, 0.6f, 0);
    [Tooltip("Assign the layers this car should brake for (e.g., Default, Player, Vehicles)")]
    public LayerMask obstacleLayer;

    private int currentPoint = 0;
    private float currentSpeed;

    // Cached squared distance to save CPU power
    private float sqrReachDistance;

    void Start()
    {
        currentSpeed = maxMoveSpeed;

        // OPTIMIZATION 1: Pre-calculate the squared reach distance
        sqrReachDistance = reachDistance * reachDistance;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        // --- 1. SENSOR & BRAKING LOGIC ---
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + transform.rotation * sensorOffset;

        // OPTIMIZATION 2: Only draw the debug ray while working in the Unity Editor
#if UNITY_EDITOR
        Debug.DrawRay(sensorStartPos, transform.forward * sensorLength, Color.red);
#endif

        // BUG FIX: Passed 'obstacleLayer' into the Raycast. 
        // Now the physics engine ignores useless objects and only brakes for real obstacles!
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength, obstacleLayer))
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 5f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxMoveSpeed, Time.deltaTime * 2f);
        }

        // --- 2. MOVEMENT & STEERING LOGIC ---
        if (currentSpeed > 0.1f)
        {
            Transform target = waypoints[currentPoint];
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Keep rotation strictly horizontal

            // OPTIMIZATION 3: Use sqrMagnitude instead of magnitude
            if (direction.sqrMagnitude < sqrReachDistance)
            {
                currentPoint++;
                if (currentPoint >= waypoints.Count)
                {
                    currentPoint = loopPath ? 0 : waypoints.Count - 1;
                }
            }
            else
            {
                // Steer towards the waypoint
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        // Move forward
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }
}