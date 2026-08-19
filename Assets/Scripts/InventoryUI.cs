using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Main Inventory")]
    public InventoryManager inventoryManager;
    public Transform itemsParent;
    public GameObject slotPrefab;

    [Header("Detail Panel")]
    public GameObject detailPanel;
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailDescription;

    [Header("Drag & Drop")]
    public Image dragIcon; // Bóng ma

    private List<InventorySlotUI> slots = new List<InventorySlotUI>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (detailPanel != null) detailPanel.SetActive(false);
        if (dragIcon != null) dragIcon.gameObject.SetActive(false);
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        for (int i = 0; i < inventoryManager.inventoryCapacity; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemsParent);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();
            slotUI.slotIndex = i; // Đánh số thứ tự cho ô này
            slots.Add(slotUI);

            slotUI.UpdateSlot(inventoryManager.inventory[i]);
        }
    }

    public void ShowItemDetails(ItemData item)
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(true);
            detailIcon.sprite = item.icon;
            detailName.text = item.itemName;
            detailDescription.text = item.description;
        }
    }
}