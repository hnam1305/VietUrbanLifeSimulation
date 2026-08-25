using UnityEngine;

public enum ItemRarity
{
    Normal,
    Good,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [TextArea(3, 5)]
    public string description;

    public ItemRarity rarity;

    public bool isStackable;
    public int maxStack;

    [Header("Display Settings")]
    public Vector3 itemScale = Vector3.one; // Tỷ lệ riêng khi hiển thị
    public Vector3 shelfScale = Vector3.one; // Tỷ lệ chuẩn khi đặt lên kệ (Scale 1,1,1 hoặc to đẹp như gốc)

    [Header("Market UI Settings")]
    public string brand;            // Brand name (e.g., Bio, Eco, Bourlait...)
    public string displayType;      // Shelf type (e.g., "Shelf" or "Fridge")
    public float unitPrice;         // Unit price for a single box (e.g., 1.99)
    public int itemsPerBox;         // Number of boxes inside one crate (e.g., 12)
    public int shelfCapacity;       // Max capacity of boxes on a shelf (e.g., 16)

    [Header("Market Delivery Settings")]
    public GameObject cratePrefab;  // Prefab of the crate spawned when ordered
}