using UnityEngine;

public class UIMouseControl : MonoBehaviour
{
    void OnEnable()
    {
        // 1. Mở khóa chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. khóa nhân vật lại, game tạm dừng
        Time.timeScale = 0f;
    }

    void Update()
    {
        // Ép chuột phải hiện ra liên tục
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnDisable()
    {
        // 1. Khóa chuột lại
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 2. Rã đông thời gian, game chạy tiếp bình thường
        Time.timeScale = 1f;
    }
}