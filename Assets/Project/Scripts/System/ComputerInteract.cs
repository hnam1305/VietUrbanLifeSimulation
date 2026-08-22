using UnityEngine;

public class ComputerInteract : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject computerUI; // Kéo ComputerUI vào đây

    void Update()
    {
        // Tắt máy tính khi bấm phím ESC (nếu UI đang mở)
        if (computerUI != null && computerUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseComputer();
            }
        }
    }

    // Gọi hàm này khi nhân vật tương tác (bấm phím E) vào cái máy tính 3D
    public void OpenComputer()
    {
        if (computerUI != null)
        {
            computerUI.SetActive(true); // Hiện UI

            // Mở khóa và hiện con chuột để người chơi bấm nút
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Hàm này dùng để tắt UI
    public void CloseComputer()
    {
        if (computerUI != null)
        {
            computerUI.SetActive(false); // Ẩn UI

            // Khóa chuột và ẩn chuột đi để tiếp tục điều khiển góc nhìn nhân vật (nếu game bạn là góc nhìn thứ nhất)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}