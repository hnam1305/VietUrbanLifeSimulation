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

        // Chuẩn bị sẵn các chỗ ngồi trống để lát đổi chỗ
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

            // Hiện con trỏ chuột để thao tác UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            inventoryCanvasGroup.DOFade(1f, animDuration).SetUpdate(true);
            inventoryRect.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }
        else
        {
            // Giấu và khóa con trỏ chuột lại vào giữa màn hình khi đóng UI
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
                        RefreshUI();
                        return true;
                    }
                    else
                    {
                        slot.AddAmount(spaceLeft);
                        amountToAdd -= spaceLeft;
                    }
                }
            }
        }

        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (inventory[i].IsEmpty())
            {
                inventory[i].item = itemToAdd;
                inventory[i].amount = amountToAdd;
                RefreshUI();
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (!inventory[i].IsEmpty() && inventory[i].item == itemToRemove)
            {
                if (inventory[i].amount >= amountToRemove)
                {
                    inventory[i].RemoveAmount(amountToRemove);
                    if (inventory[i].amount == 0)
                    {
                        inventory[i].item = null;
                        inventory[i].amount = 0;
                    }
                    RefreshUI();
                    return;
                }
                else
                {
                    amountToRemove -= inventory[i].amount;
                    inventory[i].item = null;
                    inventory[i].amount = 0;
                }
            }
        }
    }

    public void SwapItems(int indexA, int indexB)
    {
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