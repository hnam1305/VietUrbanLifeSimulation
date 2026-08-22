using UnityEngine;
using System.Collections; // Required for Coroutines

public class Shelf : MonoBehaviour
{
    [Header("Shelf Slots")]
    public Transform[] slots;
    private int currentItemCount = 0;

    public Transform GetAvailableSlot()
    {
        if (currentItemCount < slots.Length) return slots[currentItemCount];
        return null;
    }

    // Đã cập nhật nhận thêm itemData để lấy custom scale
    public void AddItemToShelf(GameObject product, Transform startPoint, ItemData itemData)
    {
        Transform targetSpot = slots[currentItemCount];
        product.transform.SetParent(targetSpot);

        // Áp dụng tỷ lệ riêng của item ngay khi đặt lên kệ
        if (itemData != null)
        {
            product.transform.localScale = itemData.itemScale;
        }

        // Start the flying animation (0.25 seconds duration)
        StartCoroutine(MoveItemSmoothly(product.transform, startPoint, targetSpot, 0.25f));

        currentItemCount++;
    }

    // Coroutine to animate the product moving smoothly
    private IEnumerator MoveItemSmoothly(Transform item, Transform startPoint, Transform targetSpot, float duration)
    {
        float elapsedTime = 0f;

        // Set initial position at the player's hand
        item.position = startPoint.position;
        item.rotation = startPoint.rotation;

        // Convert world position to local position for smooth Lerping
        Vector3 startLocalPos = item.localPosition;
        Quaternion startLocalRot = item.localRotation;

        while (elapsedTime < duration)
        {
            // Interpolate position and rotation over time
            item.localPosition = Vector3.Lerp(startLocalPos, Vector3.zero, elapsedTime / duration);
            item.localRotation = Quaternion.Lerp(startLocalRot, Quaternion.identity, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to exact target at the end to prevent slight offsets
        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;
    }
}