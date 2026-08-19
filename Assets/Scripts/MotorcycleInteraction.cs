using UnityEngine;

public class MotorcycleInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform driverSeat;
    public Transform exitPoint;
    public GameObject motorcycleCamera;
    public GameObject playerCamera;

    [Header("Settings")]
    public float interactionDistance = 3.0f;

    private bool isPlayerMounted = false;
    private PlayerCarController bikeController;

    void Start()
    {
        // Automatically find the controller on this object
        bikeController = GetComponent<PlayerCarController>();

        // Ensure the bike camera is disabled at start
        if (motorcycleCamera != null) motorcycleCamera.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Check for 'F' key input when within interaction range
        if (distance <= interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isPlayerMounted)
                {
                    MountBike();
                }
                else
                {
                    DismountBike();
                }
            }
        }
    }

    void MountBike()
    {
        isPlayerMounted = true;

        if (player != null)
        {
            player.gameObject.SetActive(false);
            player.SetParent(transform);
        }

        if (bikeController != null)
        {
            bikeController.isDriving = true;
        }

        if (playerCamera != null) playerCamera.SetActive(false);
        if (motorcycleCamera != null) motorcycleCamera.SetActive(true);

        Debug.Log("Mounted the motorcycle.");
    }

    void DismountBike()
    {
        isPlayerMounted = false;

        if (bikeController != null)
        {
            bikeController.isDriving = false;
        }

        if (player != null)
        {
            player.SetParent(null);
            if (exitPoint != null)
            {
                player.position = exitPoint.position;
            }
            player.gameObject.SetActive(true);
        }

        if (motorcycleCamera != null) motorcycleCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);

        Debug.Log("Dismounted the motorcycle.");
    }
}