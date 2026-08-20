using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;

    public InventorySlot(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int value) { amount += value; }
    public void RemoveAmount(int value) { amount -= value; }
    public bool IsEmpty() { return item == null; }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory Settings")]
    public List<InventorySlot> inventory = new List<InventorySlot>();
    public int inventoryCapacity = 20;

    [Header("UI Reference")]
    public GameObject inventoryUI;
    public CanvasGroup inventoryCanvasGroup;
    public RectTransform inventoryRect;
    public bool isInventoryOpen = false;
    public float animDuration = 0.3f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Pre-allocate empty slots to prevent memory garbage during gameplay
        for (int i = 0; i < inventoryCapacity; i++)
        {
            inventory.Add(new InventorySlot(null, 0));
        }
    }

    private void Start()
    {
        if (inventoryUI != null)
        {
            isInventoryOpen = false;
            inventoryCanvasGroup.alpha = 0f;
            inventoryRect.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            inventoryUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryUI == null) return;

        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            inventoryUI.SetActive(true);

            // Show and unlock cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            inventoryCanvasGroup.DOFade(1f, animDuration).SetUpdate(true);
            inventoryRect.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }
        else
        {
            // Hide and lock cursor back to the center
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            inventoryCanvasGroup.DOFade(0f, animDuration).SetUpdate(true);
            inventoryRect.DOScale(new Vector3(0.8f, 0.8f, 0.8f), animDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                inventoryUI.SetActive(false);
            });
        }
    }

    public bool AddItem(ItemData itemToAdd, int amountToAdd)
    {
        bool hasChanged = false;

        // 1. Try to fill existing stacks first
        if (itemToAdd.isStackable)
        {
            foreach (InventorySlot slot in inventory)
            {
                if (!slot.IsEmpty() && slot.item == itemToAdd && slot.amount < itemToAdd.maxStack)
                {
                    int spaceLeft = itemToAdd.maxStack - slot.amount;
                    if (spaceLeft >= amountToAdd)
                    {
                        slot.AddAmount(amountToAdd);
                        amountToAdd = 0; // Successfully added everything
                        hasChanged = true;
                        break;
                    }
                    else
                    {
                        slot.AddAmount(spaceLeft);
                        amountToAdd -= spaceLeft; // Keep looking for space for the remaining amount
                        hasChanged = true;
                    }
                }
            }
        }

        // 2. If there are still items to add, find empty slots
        if (amountToAdd > 0)
        {
            for (int i = 0; i < inventoryCapacity; i++)
            {
                if (inventory[i].IsEmpty())
                {
                    inventory[i].item = itemToAdd;
                    inventory[i].amount = amountToAdd;
                    amountToAdd = 0;
                    hasChanged = true;
                    break;
                }
            }
        }

        // OPTIMIZATION: Only update UI once at the very end, preventing lag spikes
        if (hasChanged) RefreshUI();

        // Return true only if we successfully managed to add ALL items
        return amountToAdd == 0;
    }

    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        // BUG FIX: First, calculate if the player actually has enough of this item.
        int totalAvailable = 0;
        foreach (InventorySlot slot in inventory)
        {
            if (!slot.IsEmpty() && slot.item == itemToRemove)
            {
                totalAvailable += slot.amount;
            }
        }

        // If they don't have enough, stop immediately before deleting anything
        if (totalAvailable < amountToRemove)
        {
            Debug.LogWarning("Not enough items to remove!");
            return;
        }

        bool hasChanged = false;

        // Proceed to remove items safely
        for (int i = 0; i < inventory.Count; i++)
        {
            if (!inventory[i].IsEmpty() && inventory[i].item == itemToRemove)
            {
                if (inventory[i].amount >= amountToRemove)
                {
                    inventory[i].RemoveAmount(amountToRemove);
                    if (inventory[i].amount <= 0) // Safety clear
                    {
                        inventory[i].item = null;
                        inventory[i].amount = 0;
                    }
                    hasChanged = true;
                    break; // Done removing
                }
                else
                {
                    amountToRemove -= inventory[i].amount;
                    inventory[i].item = null;
                    inventory[i].amount = 0;
                    hasChanged = true;
                }
            }
        }

        // OPTIMIZATION: Only update UI once at the very end
        if (hasChanged) RefreshUI();
    }

    public void SwapItems(int indexA, int indexB)
    {
        // This logic is completely fine! Swapping references in a list is very lightweight.
        InventorySlot temp = inventory[indexA];
        inventory[indexA] = inventory[indexB];
        inventory[indexB] = temp;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.UpdateUI();
        }
    }
}