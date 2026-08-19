using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    public Transform player;
    public Transform driverSeat;
    public Transform exitPoint;
    public GameObject carCamera;
    public GameObject playerCamera;

    private PlayerCarController carController;
    private bool isInCar = false;
    private float interactionDistance = 3f;

    void Start()
    {
        carController = GetComponent<PlayerCarController>();
        if (carCamera != null) carCamera.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.F))
        {
            if (!isInCar)
            {
                EnterCar();
            }
            else
            {
                ExitCar();
            }
        }
    }

    void EnterCar()
    {
        isInCar = true;
        carController.isDriving = true;

        player.gameObject.SetActive(false);
        player.SetParent(transform);

        if (carCamera != null) carCamera.SetActive(true);
        if (playerCamera != null) playerCamera.SetActive(false);
    }

    void ExitCar()
    {
        isInCar = false;
        carController.isDriving = false;

        player.SetParent(null);
        if (exitPoint != null)
        {
            player.position = exitPoint.position;
        }
        else
        {
            player.position = transform.position + transform.right * 2f;
        }

        player.gameObject.SetActive(true);

        if (carCamera != null) carCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);
    }
}