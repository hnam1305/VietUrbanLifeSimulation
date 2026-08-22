using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MarketManager : MonoBehaviour
{
    [Header("Player Money & UI")]
    public float playerMoney = 100f;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI cartTotalText;

    [Header("Delivery Settings")]
    public Transform deliveryPoint;
    public float fixedGroundY = 1.5f; // Độ cao cố định so với mặt đất để thùng hàng không bị tụt âm sâu

    // Cart dictionary: stores ItemData and the number of boxes ordered
    private Dictionary<ItemData, int> cart = new Dictionary<ItemData, int>();
    private float totalCartPrice = 0f;

    void Start()
    {
        UpdateUI();
    }

    public void AddToCart(ItemData item, int boxAmount)
    {
        if (item == null || boxAmount <= 0)
        {
            Debug.LogWarning("AddToCart failed: Item is null or boxAmount <= 0");
            return;
        }

        if (cart.ContainsKey(item))
            cart[item] += boxAmount;
        else
            cart[item] = boxAmount;

        CalculateTotal();
        Debug.Log($"[Cart] Added {boxAmount} box(es) of {item.itemName}. Total types in cart: {cart.Count}");
    }

    private void CalculateTotal()
    {
        totalCartPrice = 0f;
        foreach (var kvp in cart)
        {
            ItemData item = kvp.Key;
            int boxes = kvp.Value;
            float pricePerBox = item.unitPrice * item.itemsPerBox;
            totalCartPrice += pricePerBox * boxes;
        }
        UpdateUI();
    }

    // Hook this to your Checkout Button OnClick()
    public void Checkout()
    {
        Debug.Log("[Checkout] Nút Checkout đã được bấm!");

        if (cart.Count == 0)
        {
            Debug.Log("[Checkout] Thất bại: Giỏ hàng đang trống trơn!");
            return;
        }

        if (playerMoney >= totalCartPrice)
        {
            Debug.Log($"[Checkout] Đủ tiền ({playerMoney} >= {totalCartPrice}). Tiến hành spawn thùng hàng...");
            playerMoney -= totalCartPrice;

            int spawnIndex = 0;
            foreach (var kvp in cart)
            {
                ItemData item = kvp.Key;
                int boxes = kvp.Value;

                for (int i = 0; i < boxes; i++)
                {
                    if (item.cratePrefab != null && deliveryPoint != null)
                    {
                        // Dùng tọa độ X, Z của DeliveryPoint, nhưng ép độ cao Y bằng fixedGroundY cộng dồn
                        Vector3 spawnPos = new Vector3(
                            deliveryPoint.position.x,
                            fixedGroundY + (spawnIndex * 0.6f),
                            deliveryPoint.position.z
                        );

                        Instantiate(item.cratePrefab, spawnPos, deliveryPoint.rotation);
                        spawnIndex++;
                        Debug.Log($"[Checkout] Đã spawn thành công thùng: {item.itemName} tại vị trí {spawnPos}");
                    }
                    else
                    {
                        if (item.cratePrefab == null)
                            Debug.LogError($"[Checkout] Lỗi: Sản phẩm '{item.itemName}' chưa được kéo Prefab thùng hàng vào ô Crate Prefab!");
                        if (deliveryPoint == null)
                            Debug.LogError("[Checkout] Lỗi: Chưa kéo điểm Delivery Point vào GameManager!");
                    }
                }
            }

            // Clear the cart after a successful purchase
            cart.Clear();
            CalculateTotal();
            Debug.Log("[Checkout] Thanh toán thành công và đã làm sạch giỏ hàng!");
        }
        else
        {
            Debug.Log($"[Checkout] Không đủ tiền! Cần {totalCartPrice}, nhưng bạn chỉ có {playerMoney}");
        }
    }

    public void UpdateUI()
    {
        if (moneyText != null) moneyText.text = "$" + playerMoney.ToString("F2");
        if (cartTotalText != null) cartTotalText.text = "$" + totalCartPrice.ToString("F2");
    }
}