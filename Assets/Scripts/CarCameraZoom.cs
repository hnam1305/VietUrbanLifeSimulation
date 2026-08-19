using UnityEngine;

public class CarCameraZoom : MonoBehaviour
{
    [Header("Camera Positions")]
    public Vector3 fpPos = new Vector3(-0.3f, 0.86f, 0.17f);
    public float tpDistance = 6f;
    public float tpHeight = 2f;

    [Header("Controls")]
    public float mouseSensitivity = 3f;
    public float cameraSmoothness = 10f; // Tôi thêm biến này ra ngoài để bạn dễ chỉnh độ mượt trên Inspector

    private float yaw = 0f;
    private float pitch = 0f;
    private bool isThirdPerson = false;

    // Biến lưu trữ Input
    private float mouseX;
    private float mouseY;

    void OnEnable()
    {
        yaw = 0f;
        pitch = 0f;
    }

    void Update()
    {
        // 1. Chỉ đọc Input (Phím và Chuột) ở Update để đảm bảo nhạy bén nhất
        if (Input.GetKeyDown(KeyCode.V))
        {
            isThirdPerson = !isThirdPerson;
        }

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    // 2. Toàn bộ logic xoay, trượt Camera được dời xuống LateUpdate
    void LateUpdate()
    {
        // Tính toán góc xoay dựa trên Input đã lấy ở trên
        if (Mathf.Abs(mouseX) > 0.05f || Mathf.Abs(mouseY) > 0.05f)
        {
            yaw += mouseX * mouseSensitivity;
            pitch -= mouseY * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            // Giữ góc yaw xoay vòng 360 độ an toàn
            if (yaw > 180f) yaw -= 360f;
            if (yaw < -180f) yaw += 360f;
        }

        // Áp dụng góc xoay vào Camera
        Quaternion currentRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.localRotation = currentRotation;

        // Tính toán vị trí khi ở góc nhìn thứ 3
        Vector3 tpOffset = new Vector3(0f, tpHeight, -tpDistance);
        Vector3 fullTpPos = fpPos + (currentRotation * tpOffset);

        // Chọn vị trí mục tiêu dựa trên phím V
        Vector3 targetPos = isThirdPerson ? fullTpPos : fpPos;

        // Trượt Camera mượt mà tới vị trí mục tiêu
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * cameraSmoothness);
    }
}