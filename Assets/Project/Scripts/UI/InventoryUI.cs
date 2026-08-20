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
    public Image dragIcon; // Ghost icon for dragging

    private List<InventorySlotUI> slots = new List<InventorySlotUI>();
    private bool isUIInitialized = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (detailPanel != null) detailPanel.SetActive(false);
        if (dragIcon != null) dragIcon.gameObject.SetActive(false);

        // Create the UI slots once at startup
        InitializeUI();
        UpdateUI();
    }

    private void InitializeUI()
    {
        // 1. Clear any placeholder items you might have left in the Scene for testing
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        // 2. Pre-instantiate all slots based on the manager's capacity
        for (int i = 0; i < inventoryManager.inventoryCapacity; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemsParent);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

            slotUI.slotIndex = i; // Assign index for drag and drop logic later
            slots.Add(slotUI);
        }

        isUIInitialized = true;
    }

    public void UpdateUI()
    {
        // Failsafe: Prevent errors if UpdateUI is called before Start finishes
        if (!isUIInitialized || inventoryManager == null) return;

        // OPTIMIZATION: Do not Destroy and Instantiate! 
        // Just update the data inside the existing UI elements.
        for (int i = 0; i < inventoryManager.inventoryCapacity; i++)
        {
            if (i < slots.Count)
            {
                slots[i].UpdateSlot(inventoryManager.inventory[i]);
            }
        }
    }

    public void ShowItemDetails(ItemData item)
    {
        if (detailPanel != null && item != null)
        {
            detailPanel.SetActive(true);
            detailIcon.sprite = item.icon;
            detailName.text = item.itemName;
            detailDescription.text = item.description;
        }
    }

    // BONUS: Added a quick method to hide details (useful for when you click an empty slot)
    public void HideItemDetails()
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }
    }
}