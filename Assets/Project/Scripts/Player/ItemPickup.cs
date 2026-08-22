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
        // 1. KHI TAY ĐANG TRỐNG (Không cầm gì)
        if (currentItem == null)
        {
            // Bấm E hoặc Click Chuột Trái (0) đều thực hiện lệnh Tương Tác
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                TryPickUp(); // Gọi hàm này để bắn tia ngắm kiểm tra đồ vật hoặc máy tính
            }
        }
        // 2. KHI ĐANG CẦM THÙNG HÀNG TRÊN TAY
        else
        {
            // Bấm E để vứt thùng hàng xuống đất
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropItem();
            }
            // Click Chuột Trái để đặt sản phẩm lên kệ
            else if (Input.GetMouseButtonDown(0))
            {
                TryPlaceItemOnShelf();
            }
        }
    }

    // Custom function to handle placing logic
    private void TryPlaceItemOnShelf()
    {
        // Cancel if the held item has no products or is empty
        if (currentItem.productPrefab == null || currentItem.productCount <= 0) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, interactableLayer))
        {
            Shelf targetShelf = hit.collider.GetComponent<Shelf>();
            if (targetShelf != null)
            {
                Transform spot = targetShelf.GetAvailableSlot();
                if (spot != null) // Proceed if there is an empty slot
                {
                    // Decrease product count inside the crate
                    currentItem.productCount--;

                    // Cập nhật hiển thị trong thùng
                    currentItem.UpdateVisuals();

                    // Spawn the product and assign it to the shelf with animation
                    GameObject newProduct = Instantiate(currentItem.productPrefab);

                    // Truyền thêm currentItem.itemData vào để lấy tỷ lệ (scale) riêng biệt
                    targetShelf.AddItemToShelf(newProduct, holdPosition, currentItem.itemData);

                    Debug.Log("Item placed successfully! Remaining in crate: " + currentItem.productCount);
                }
            }
        }
    }

    private void TryPickUp()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            // 1. Kiểm tra xem thứ mình đang nhìn có phải là cái Máy Tính không?
            ComputerInteract computer = hit.collider.GetComponent<ComputerInteract>();
            if (computer != null)
            {
                computer.OpenComputer(); // Mở UI máy tính
                return; // Dừng lại luôn
            }

            // 2. Nếu không phải máy tính thì kiểm tra xem có phải Thùng Hàng không?
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