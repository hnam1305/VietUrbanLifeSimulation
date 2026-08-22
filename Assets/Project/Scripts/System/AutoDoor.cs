using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorModel; // Mô hình cánh cửa sẽ xoay
    public Vector3 openRotation = new Vector3(0, 90, 0); // Góc mở cửa (trục Y xoay 90 độ)
    public float openSpeed = 0.5f;

    private Vector3 closedRotation;
    private bool isOpen = false;
    private Coroutine animationCoroutine;

    void Start()
    {
        // Ghi nhớ góc đóng ban đầu
        if (doorModel != null) closedRotation = doorModel.localEulerAngles;
    }

    // Khi có người bước vào vùng cảm biến
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateDoor(openRotation));
            isOpen = true;
        }
    }

    // Khi người đi khỏi vùng cảm biến
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

        while (elapsed < openSpeed)
        {
            doorModel.localRotation = Quaternion.Lerp(startRot, endRot, elapsed / openSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        doorModel.localRotation = endRot;
    }
}