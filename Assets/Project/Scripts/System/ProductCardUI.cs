using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductCardUI : MonoBehaviour
{
    [Header("Data References")]
    public ItemData itemData;

    [Header("UI - Left Section (Info)")]
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI brandText;
    public Image iconImage;
    public TextMeshProUGUI displayTypeText;
    public TextMeshProUGUI unitPriceText;
    public TextMeshProUGUI shelfCapacityText;
    public TextMeshProUGUI boxCapacityText;

    [Header("UI - Right Section (Action)")]
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI totalPriceText;

    private int currentBoxAmount = 1;
    private float pricePerBox;

    void Start()
    {
        if (itemData != null)
        {
            SetupCard(itemData);
        }
    }

    public void SetupCard(ItemData data)
    {
        itemData = data;

        // Calculate price per box = unit price * items per box
        pricePerBox = itemData.unitPrice * itemData.itemsPerBox;

        // Fill left UI info
        if (productNameText != null) productNameText.text = itemData.itemName;
        if (brandText != null) brandText.text = itemData.brand;
        if (iconImage != null && itemData.icon != null) iconImage.sprite = itemData.icon;

        if (displayTypeText != null) displayTypeText.text = itemData.displayType;
        if (unitPriceText != null) unitPriceText.text = "$" + itemData.unitPrice.ToString("F2");
        if (shelfCapacityText != null) shelfCapacityText.text = itemData.shelfCapacity.ToString();
        if (boxCapacityText != null) boxCapacityText.text = itemData.itemsPerBox.ToString();

        currentBoxAmount = 1;
        UpdateActionUI();
    }

    // Hook this to the [+] Button OnClick()
    public void IncreaseAmount()
    {
        currentBoxAmount++;
        UpdateActionUI();
    }

    // Hook this to the [-] Button OnClick()
    public void DecreaseAmount()
    {
        if (currentBoxAmount > 1)
        {
            currentBoxAmount--;
            UpdateActionUI();
        }
    }

    private void UpdateActionUI()
    {
        if (amountText != null) amountText.text = currentBoxAmount.ToString();

        float total = pricePerBox * currentBoxAmount;
        if (totalPriceText != null) totalPriceText.text = "$" + total.ToString("F2");
    }

    // Hook this to the [Add To Cart] Button OnClick()
    public void OnAddToCartClicked()
    {
        MarketManager manager = FindObjectOfType<MarketManager>();
        if (manager != null && itemData != null)
        {
            manager.AddToCart(itemData, currentBoxAmount);
        }

        // Reset amount back to 1 after adding to cart
        currentBoxAmount = 1;
        UpdateActionUI();
    }
}