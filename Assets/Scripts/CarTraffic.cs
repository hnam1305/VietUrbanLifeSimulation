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
    public LayerMask obstacleLayer;

    private int currentPoint = 0;
    private float currentSpeed;

    void Start()
    {
        currentSpeed = maxMoveSpeed;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + transform.rotation * sensorOffset;

        Debug.DrawRay(sensorStartPos, transform.forward * sensorLength, Color.red);

        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 5f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxMoveSpeed, Time.deltaTime * 2f);
        }

        if (currentSpeed > 0.1f)
        {
            Transform target = waypoints[currentPoint];
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.magnitude < reachDistance)
            {
                currentPoint++;
                if (currentPoint >= waypoints.Count)
                {
                    if (loopPath) currentPoint = 0;
                    else currentPoint = waypoints.Count - 1;
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }
}