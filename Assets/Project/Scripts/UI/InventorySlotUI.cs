using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;
    public Image icon;
    public TextMeshProUGUI amountText;

    private Image slotBackground;
    private InventorySlot currentSlot;
    private Color normalColor;
    private Color hoverColor = new Color32(200, 200, 200, 255); // Màu khi lướt chuột vào (sáng hơn một chút)

    private void Awake()
    {
        slotBackground = GetComponent<Image>();
        if (slotBackground != null)
        {
            normalColor = slotBackground.color;
        }
    }

    public void UpdateSlot(InventorySlot slot)
    {
        currentSlot = slot;
        if (slot != null && slot.item != null)
        {
            if (icon != null)
            {
                icon.sprite = slot.item.icon;
                icon.enabled = true;
            }

            if (amountText != null)
            {
                amountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
                amountText.enabled = slot.amount > 1;
            }

            if (slotBackground != null)
            {
                switch (slot.item.rarity)
                {
                    case ItemRarity.Normal: normalColor = new Color32(150, 150, 150, 255); break;
                    case ItemRarity.Good: normalColor = new Color32(100, 200, 100, 255); break;
                    case ItemRarity.Rare: normalColor = new Color32(100, 150, 255, 255); break;
                    case ItemRarity.Epic: normalColor = new Color32(200, 100, 255, 255); break;
                    case ItemRarity.Legendary: normalColor = new Color32(255, 200, 50, 255); break;
                }
                slotBackground.color = normalColor;
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentSlot = null;

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (amountText != null)
        {
            amountText.text = "";
            amountText.enabled = false;
        }

        if (slotBackground != null)
        {
            normalColor = new Color32(50, 50, 50, 150);
            slotBackground.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotBackground != null)
        {
            slotBackground.color = hoverColor; // Sáng lên khi chuột trỏ vào
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slotBackground != null)
        {
            slotBackground.color = normalColor; // Trở về màu ban đầu khi chuột rời đi
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentSlot != null && currentSlot.item != null && InventoryUI.Instance != null)
        {
            InventoryUI.Instance.ShowItemDetails(currentSlot.item);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.item == null) return;

        if (InventoryUI.Instance != null && InventoryUI.Instance.dragIcon != null)
        {
            InventoryUI.Instance.dragIcon.gameObject.SetActive(true);
            InventoryUI.Instance.dragIcon.sprite = currentSlot.item.icon;
        }

        if (icon != null) icon.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.item == null) return;

        if (InventoryUI.Instance != null && InventoryUI.Instance.dragIcon != null)
        {
            InventoryUI.Instance.dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryUI.Instance != null && InventoryUI.Instance.dragIcon != null)
        {
            InventoryUI.Instance.dragIcon.gameObject.SetActive(false);
        }

        if (icon != null) icon.color = new Color(1, 1, 1, 1f);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            InventorySlotUI draggedSlot = droppedObject.GetComponent<InventorySlotUI>();
            if (draggedSlot != null && draggedSlot != this && InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SwapItems(draggedSlot.slotIndex, this.slotIndex);
            }
        }
    }
}