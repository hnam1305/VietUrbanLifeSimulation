using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorModel;
    [Tooltip("Góc muốn mở TƯƠNG ĐỐI so với lúc đóng (VD: nhập Y = 90 hoặc -90)")]
    public Vector3 openRotationOffset = new Vector3(0, 90, 0);
    [Tooltip("Thời gian mở cửa (giây)")]
    public float openDuration = 0.5f;

    private Vector3 closedRotation;
    private Vector3 actualOpenRotation; // Góc mở thực tế sau khi tính toán
    private bool isOpen = false;
    private Coroutine animationCoroutine;

    void Start()
    {
        if (doorModel != null)
        {
            // Ghi nhớ góc đóng
            closedRotation = doorModel.localEulerAngles;
            // Tự động tính toán góc mở chuẩn xác dù cái nhà có bị xoay đi hướng nào
            actualOpenRotation = closedRotation + openRotationOffset;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateDoor(actualOpenRotation));
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateDoor(closedRotation));
            isOpen = false;
        }
    }

    private IEnumerator AnimateDoor(Vector3 targetRot)
    {
        Quaternion startRot = doorModel.localRotation;
        Quaternion endRot = Quaternion.Euler(targetRot);
        float elapsed = 0;

        while (elapsed < openDuration)
        {
            // Dùng Slerp thay vì Lerp để cửa xoay tròn trịa và tự nhiên hơn ở các góc cong
            doorModel.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / openDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        doorModel.localRotation = endRot;
    }
}