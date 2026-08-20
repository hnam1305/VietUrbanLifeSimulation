using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPosition;
    public float pickupRange = 3f;
    public LayerMask interactableLayer;

    private Interactable currentItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentItem == null)
            {
                TryPickUp();
            }
            else
            {
                DropItem();
            }
        }
    }

    private void TryPickUp()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            Interactable item = hit.collider.GetComponent<Interactable>();
            if (item != null)
            {
                currentItem = item;
                currentItem.OnPickedUp(holdPosition);
            }
        }
    }

    private void DropItem()
    {
        if (currentItem != null)
        {
            currentItem.OnDropped();
            currentItem = null;
        }
    }
}