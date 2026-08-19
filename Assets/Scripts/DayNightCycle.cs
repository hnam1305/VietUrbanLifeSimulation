using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Time in seconds for a full day to pass")]
    public float dayDuration = 60f;

    private float rotationSpeed;

    void Start()
    {
        // Calculate the rotation speed once at startup to save CPU cycles
        CalculateRotationSpeed();
    }

    void Update()
    {
        // Use multiplication instead of division for better performance
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }

    public void CalculateRotationSpeed()
    {
        if (dayDuration > 0)
        {
            rotationSpeed = 360f / dayDuration;
        }
        else
        {
            rotationSpeed = 0f;
        }
    }
}