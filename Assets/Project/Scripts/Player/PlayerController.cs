using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera; // Sẽ kéo thả Camera vào đây

    private float cameraPitch = 0f; // Trục ngước lên/cúi xuống của Camera
    private Rigidbody rb;
    private Animator animator;

    private float moveX;
    private float moveZ;
    private bool isSprinting;
    private bool jumpRequested;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // Khóa con trỏ chuột vào giữa màn hình và ẩn nó đi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. NHÌN BẰNG CHUỘT (MOUSE LOOK)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Xoay cơ thể nhân vật sang trái/phải
        transform.Rotate(Vector3.up * mouseX);

        // Gật gù Camera lên/xuống
        if (playerCamera != null)
        {
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f); // Khóa góc, không cho gập cổ ra sau
            playerCamera.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        // 2. NHẬN LỆNH DI CHUYỂN
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpRequested = true;
        }

        // Cập nhật Animation
        if (animator != null)
        {
            float currentSpeed = new Vector2(moveX, moveZ).magnitude;
            animator.SetFloat("Speed", currentSpeed > 0f ? (isSprinting ? 1.0f : 0.3f) : 0.0f);
        }
    }

    void FixedUpdate()
    {
        // 3. DI CHUYỂN VẬT LÝ
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Di chuyển tương đối theo hướng nhìn của cơ thể
        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        Vector3 moveVelocity = moveDirection * currentSpeed;

        moveVelocity.y = rb.linearVelocity.y; // Giữ nguyên trọng lực rơi
        rb.linearVelocity = moveVelocity;

        // Nhảy
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (animator != null) animator.SetTrigger("Jump");
            jumpRequested = false;
        }
    }

    private bool IsGrounded()
    {
        float rayLength = 0.3f;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(rayOrigin, Vector3.down, rayLength);
    }
}