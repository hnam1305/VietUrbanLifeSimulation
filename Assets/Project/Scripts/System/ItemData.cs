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
    public Vector3 itemScale = Vector3.one; // Tỷ lệ riêng khi đặt lên kệ (mặc định là 1,1,1)

    [Header("Market UI Settings ")]
    public string brand;            // Nhãn hiệu (VD: Bio, Eco, Bourlait...)
    public string displayType;      // Loại kệ (Gõ chữ "Shelf" hoặc "Fridge")
    public float unitPrice;         // Giá nhập của 1 hộp lẻ (VD: 1.99)
    public int itemsPerBox;         // Số lượng hộp có trong 1 thùng (VD: 12)
    public int shelfCapacity;       // Số lượng hộp tối đa xếp được trên 1 kệ (VD: 16)

    [Header("Market Delivery Settings")]
    public GameObject cratePrefab; // Prefab của thùng hàng sẽ rớt ra ngoài vỉa hè khi mua
}