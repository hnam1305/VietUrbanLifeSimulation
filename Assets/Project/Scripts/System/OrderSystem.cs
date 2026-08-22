using UnityEngine;
using TMPro; // Dùng để update text tiền

public class OrderSystem : MonoBehaviour
{
    [Header("Money System")]
    public int playerMoney = 97000; // Số tiền khởi điểm
    public TextMeshProUGUI moneyText; // Kéo Text "Money: 97000VND" của bạn vào đây

    [Header("Order Settings")]
    public Transform deliveryPoint; // Điểm rơi của thùng hàng
    public GameObject computerUIPanel; // Khung giao diện máy tính

    // Định nghĩa cấu trúc 1 món hàng bán trên máy tính
    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public GameObject cratePrefab; // Thùng hàng chứa món này (VD: Thùng sữa)
        public int price; // Giá nhập 1 thùng
    }

    [Header("Shop Database")]
    public ShopItem[] shopItems; // Danh sách các mặt hàng

    void Start()
    {
        UpdateMoneyUI();
        computerUIPanel.SetActive(false); // Ẩn màn hình lúc mới vào game
    }

    // Hàm gọi khi bấm nút "Mua" trên UI
    public void BuyItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= shopItems.Length) return;

        ShopItem selectedItem = shopItems[itemIndex];

        // Kiểm tra xem có đủ tiền không
        if (playerMoney >= selectedItem.price)
        {
            playerMoney -= selectedItem.price;
            UpdateMoneyUI();

            // Sinh ra cái thùng hàng ở vị trí giao hàng
            Instantiate(selectedItem.cratePrefab, deliveryPoint.position, deliveryPoint.rotation);
            Debug.Log("Đã giao thành công: " + selectedItem.itemName);
        }
        else
        {
            Debug.Log("Không đủ tiền để mua!");
        }
    }

    // Cập nhật lại UI hiển thị tiền
    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money:\n" + playerMoney + "VND";
        }
    }

    // Bật/Tắt giao diện máy tính
    public void ToggleComputerUI()
    {
        bool isActive = !computerUIPanel.activeSelf;
        computerUIPanel.SetActive(isActive);

        // Mở UI thì hiện chuột để bấm, tắt UI thì khóa chuột lại để đi dạo
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}