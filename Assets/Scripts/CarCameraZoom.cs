using UnityEngine;

public class CarCameraZoom : MonoBehaviour
{
    [Header("Camera Positions")]
    public Vector3 fpPos = new Vector3(-0.3f, 0.86f, 0.17f);
    public float tpDistance = 6f;
    public float tpHeight = 2f;

    [Header("Controls")]
    public float mouseSensitivity = 3f;

    private float yaw = 0f;
    private float pitch = 0f;
    private bool isThirdPerson = false;

    void OnEnable()
    {
        yaw = 0f;
        pitch = 0f;
    }

    void Update()
    {
        // Bấm V để chuyển đổi góc nhìn
        if (Input.GetKeyDown(KeyCode.V))
        {
            isThirdPerson = !isThirdPerson;
        }

        // Xử lý xoay Camera tự do
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) > 0.05f || Mathf.Abs(mouseY) > 0.05f)
        {
            yaw += mouseX * mouseSensitivity;
            pitch -= mouseY * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            if (yaw > 180f) yaw -= 360f;
            if (yaw < -180f) yaw += 360f;
        }

        Quaternion currentRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.localRotation = currentRotation;

        // Tính toán vị trí góc nhìn 3
        Vector3 tpOffset = new Vector3(0f, tpHeight, -tpDistance);
        Vector3 fullTpPos = fpPos + (currentRotation * tpOffset);

        // Trượt camera dựa trên trạng thái của phím V
        Vector3 targetPos = isThirdPerson ? fullTpPos : fpPos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 10f);
    }
}