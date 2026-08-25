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

        // BỎ QUA KIỂM TRA ITEMDATA, ÉP THẲNG TỶ LỆ VỀ 1 1 1 CHUẨN (Bù trừ độ méo của kệ)
        product.transform.localScale = new Vector3(
            1f / targetSpot.lossyScale.x,
            1f / targetSpot.lossyScale.y,
            1f / targetSpot.lossyScale.z
        );

        // Start the flying animation (0.25 seconds duration)
        StartCoroutine(MoveItemSmoothly(product.transform, startPoint, targetSpot, 0.25f));

        currentItemCount++;
    }
 
    public GameObject RemoveItemFromShelf()
    {
        // Kiểm tra xem trên kệ có đồ không
        if (currentItemCount > 0)
        {
            // Lùi lại 1 vị trí (chỉ vào slot chứa item cuối cùng vừa xếp)
            currentItemCount--;
            Transform targetSpot = slots[currentItemCount];

            // Nếu slot này thực sự đang chứa hộp sữa (có object con)
            if (targetSpot.childCount > 0)
            {
                // Lấy hộp sữa ra
                GameObject product = targetSpot.GetChild(0).gameObject;

                // Tách nó khỏi cái kệ
                product.transform.SetParent(null);

                // Quan trọng: Trả lại tỷ lệ chuẩn 1 1 1 khi cầm trên tay (thoát khỏi sự kìm kẹp của cái kệ)
                product.transform.localScale = Vector3.one;

                // Trả về cái hộp sữa để đoạn code của nhân vật đón lấy
                return product;
            }
        }

        // Nếu kệ đang trống không, trả về null
        return null;
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