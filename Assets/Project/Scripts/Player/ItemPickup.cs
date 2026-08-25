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
                TryPickUp();
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
            // =====================================
            // MỚI: Click Chuột Phải để rút đồ từ kệ về thùng
            // =====================================
            else if (Input.GetMouseButtonDown(1))
            {
                TryTakeItemFromShelf();
            }
        }
    }

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
                if (spot != null)
                {
                    currentItem.productCount--;
                    currentItem.UpdateVisuals();

                    GameObject newProduct = Instantiate(currentItem.productPrefab);
                    targetShelf.AddItemToShelf(newProduct, holdPosition, currentItem.itemData);

                    Debug.Log("Đã xếp lên kệ! Trong thùng còn: " + currentItem.productCount);
                }
            }
        }
    }

    // =====================================
    // HÀM MỚI: XỬ LÝ LẤY ĐỒ TỪ KỆ VỀ THÙNG
    // =====================================
    private void TryTakeItemFromShelf()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, interactableLayer))
        {
            Shelf targetShelf = hit.collider.GetComponent<Shelf>();
            if (targetShelf != null)
            {
                // Kiểm tra xem thùng có còn chỗ chứa không (Dựa vào số lượng tối đa của visualItems)
                if (currentItem.productCount < currentItem.visualItems.Length)
                {
                    // Lấy item 3D từ trên kệ xuống
                    GameObject retrievedItem = targetShelf.RemoveItemFromShelf();

                    if (retrievedItem != null)
                    {
                        // Phá hủy object 3D vừa lấy xuống vì nó đã chui tọt vào thùng
                        Destroy(retrievedItem);

                        // Tăng số lượng hàng trong thùng lên và cập nhật hình ảnh
                        currentItem.productCount++;
                        currentItem.UpdateVisuals();

                        Debug.Log("Đã rút về thùng! Số lượng hiện tại: " + currentItem.productCount);
                    }
                    else
                    {
                        Debug.Log("Kệ đang trống, không có gì để lấy!");
                    }
                }
                else
                {
                    Debug.Log("Thùng đã đầy, không thể chứa thêm!");
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
            ComputerInteract computer = hit.collider.GetComponent<ComputerInteract>();
            if (computer != null)
            {
                computer.OpenComputer();
                return;
            }

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