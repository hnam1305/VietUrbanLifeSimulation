using UnityEngine;

public class CarLights : MonoBehaviour
{
    public Transform sun;
    public GameObject[] headLights;

    void Update()
    {
        if (sun == null) return;

        bool isNight = sun.forward.y > 0;

        foreach (GameObject light in headLights)
        {
            if (light.activeSelf != isNight)
            {
                light.SetActive(isNight);
            }
        }
    }
}