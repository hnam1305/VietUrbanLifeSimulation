using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float dayDuration = 60f;

    void Update()
    {
        float rotationAngle = 360f / dayDuration * Time.deltaTime;
        transform.Rotate(Vector3.right, rotationAngle);
    }
}