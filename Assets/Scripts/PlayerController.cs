using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 720f;
    public float jumpForce = 5f;
    public LayerMask groundLayer; // Assign your ground layer here in the Inspector!

    [Header("Interaction & Carry")]
    public float interactRange = 2f;
    public Transform holdPoint;
    public float throwForce = 15f;

    private Rigidbody rb;
    private Animator animator;
    private Camera mainCamera;
    private Rigidbody heldItemRb;

    // Input caching
    private float moveX;
    private float moveZ;
    private bool isSprinting;
    private bool jumpRequested;
    private Vector3 movementDirection;

    // Memory optimization: Pre-allocate an array for the interact scanner (max 5 items at a time)
    private Collider[] interactColliders = new Collider[5];

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // Cache the camera so we don't search for it every frame
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. GATHER INPUT
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpRequested = true;
        }

        // Calculate movement direction relative to camera
        if (mainCamera != null)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            movementDirection = (camForward.normalized * moveZ + camRight.normalized * moveX).normalized;
        }

        // Update Animations smoothly in Update (not FixedUpdate)
        if (animator != null)
        {
            float currentSpeed = movementDirection.magnitude;
            animator.SetFloat("Speed", currentSpeed > 0f ? (isSprinting ? 1.0f : 0.3f) : 0.0f);
        }

        // 2. HANDLE INTERACTION
        HandleInteraction();
    }

    void FixedUpdate()
    {
        // 3. APPLY PHYSICS
        Vector3 moveVelocity = movementDirection * moveSpeed;
        moveVelocity.y = rb.linearVelocity.y; // Keep the existing falling/jumping velocity
        rb.linearVelocity = moveVelocity;

        // Player Rotation
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.fixedDeltaTime);
        }

        // Player Jump
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (animator != null) animator.SetTrigger("Jump");
            jumpRequested = false; // Reset the jump request
        }
    }

    private void HandleInteraction()
    {
        if (heldItemRb == null)
        {
            // OPTIMIZATION: Use NonAlloc to prevent Garbage Collection (lag spikes)
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, interactRange, interactColliders);
            Interactable targetItem = null;

            // Loop only through the actual hits
            for (int i = 0; i < hitCount; i++)
            {
                Interactable interactable = interactColliders[i].GetComponent<Interactable>();
                if (interactable != null)
                {
                    targetItem = interactable;
                    break;
                }
            }

            if (targetItem != null)
            {
                GameManager.Instance.ShowInteractText(targetItem.promptMessage);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.Instance.HideInteractText();
                    PickUpItem(targetItem.gameObject);
                }
            }
            else
            {
                // Only hide if we actually have GameManager instantiated
                if (GameManager.Instance != null) GameManager.Instance.HideInteractText();
            }
        }
        else
        {
            GameManager.Instance.ShowInteractText("Press E to Drop | Left Click to Throw");

            if (Input.GetKeyDown(KeyCode.E))
            {
                DropItem();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                ThrowItem();
            }
        }
    }

    // Fix: A much safer way to check if the player is on the ground
    private bool IsGrounded()
    {
        // Bắn một tia raycast xuống dưới chân, loại trừ collider của chính nhân vật để không bị nhận diện nhầm
        float rayLength = 0.3f;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        // Sử dụng Raycast thông thường (bỏ qua LayerMask để test cho nhanh)
        if (Physics.Raycast(rayOrigin, Vector3.down, rayLength))
        {
            return true;
        }
        return false;
    }

    private void PickUpItem(GameObject item)
    {
        heldItemRb = item.GetComponent<Rigidbody>();
        if (heldItemRb != null)
        {
            heldItemRb.useGravity = false;
            heldItemRb.isKinematic = true;

            Collider itemCollider = heldItemRb.GetComponent<Collider>();
            if (itemCollider != null) itemCollider.enabled = false;

            heldItemRb.transform.position = holdPoint.position;
            heldItemRb.transform.parent = holdPoint;
        }
    }

    private void DropItem()
    {
        if (heldItemRb != null)
        {
            heldItemRb.useGravity = true;
            heldItemRb.isKinematic = false;

            Collider itemCollider = heldItemRb.GetComponent<Collider>();
            if (itemCollider != null) itemCollider.enabled = true;

            heldItemRb.transform.parent = null;
            heldItemRb = null;
            GameManager.Instance.HideInteractText();
        }
    }

    private void ThrowItem()
    {
        if (heldItemRb != null)
        {
            heldItemRb.useGravity = true;
            heldItemRb.isKinematic = false;

            Collider itemCollider = heldItemRb.GetComponent<Collider>();
            if (itemCollider != null) itemCollider.enabled = true;

            heldItemRb.transform.parent = null;

            // Throw based on where the camera is looking
            Vector3 throwDirection = mainCamera.transform.forward;
            heldItemRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

            heldItemRb = null;
            GameManager.Instance.HideInteractText();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}