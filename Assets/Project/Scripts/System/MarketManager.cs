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
    public float fixedGroundY = 1.5f; // Fixed height above the ground to prevent crates from sinking

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
        Debug.Log("[Checkout] Checkout button was clicked!");

        if (cart.Count == 0)
        {
            Debug.Log("[Checkout] Failed: Cart is empty!");
            return;
        }

        if (playerMoney >= totalCartPrice)
        {
            Debug.Log($"[Checkout] Sufficient funds ({playerMoney} >= {totalCartPrice}). Spawning crates...");
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
                        // Use X and Z from DeliveryPoint, but stack Y using fixedGroundY + offset
                        Vector3 spawnPos = new Vector3(
                            deliveryPoint.position.x,
                            fixedGroundY + (spawnIndex * 0.6f),
                            deliveryPoint.position.z
                        );

                        // Spawn the crate prefab directly. 
                        // Note: Configure inside-crate items' scale directly inside the Crate Prefab to avoid position offset bugs.
                        Instantiate(item.cratePrefab, spawnPos, deliveryPoint.rotation);
                        spawnIndex++;

                        Debug.Log($"[Checkout] Successfully spawned crate: {item.itemName} at position {spawnPos}");
                    }
                    else
                    {
                        if (item.cratePrefab == null)
                            Debug.LogError($"[Checkout] Error: Product '{item.itemName}' is missing its Crate Prefab reference!");
                        if (deliveryPoint == null)
                            Debug.LogError("[Checkout] Error: Delivery Point is not assigned in GameManager!");
                    }
                }
            }

            // Clear the cart after a successful purchase
            cart.Clear();
            CalculateTotal();
            Debug.Log("[Checkout] Purchase successful and cart cleared!");
        }
        else
        {
            Debug.Log($"[Checkout] Not enough money! Required: {totalCartPrice}, but player has: {playerMoney}");
        }
    }

    public void UpdateUI()
    {
        if (moneyText != null) moneyText.text = "$" + playerMoney.ToString("F2");
        if (cartTotalText != null) cartTotalText.text = "$" + totalCartPrice.ToString("F2");
    }
}