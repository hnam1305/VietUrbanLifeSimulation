using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float normalSpeed = 15f;
    public float boostSpeed = 25f;
    public float turnSpeed = 100f;
    public float acceleration = 10f;
    public float brakeForce = 20f;

    [Header("System")]
    public bool isDriving = false;
    public TextMeshProUGUI speedText;
    public GameObject[] leftSignals;
    public GameObject[] rightSignals;
    public float blinkRate = 0.4f;

    [Header("Effects & Audio")]
    public TrailRenderer[] skidMarks;
    public AudioSource engineAudio;
    public float minEnginePitch = 0.8f;
    public float maxEnginePitch = 2.5f;
    public AudioSource skidAudio;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float turnInput;
    private bool isBraking;

    private bool leftSignalOn = false;
    private bool rightSignalOn = false;
    private float blinkTimer;
    private bool blinkState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!isDriving)
        {
            if (speedText != null) speedText.text = "";
            HandleAudio(false);
            return;
        }

        if (speedText != null)
        {
            float speedKMH = rb.linearVelocity.magnitude * 3.6f;
            speedText.text = Mathf.RoundToInt(speedKMH) + " KM/H";
        }

        isBraking = Input.GetKey(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            leftSignalOn = !leftSignalOn;
            rightSignalOn = false;
            SetSignals(rightSignals, false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            rightSignalOn = !rightSignalOn;
            leftSignalOn = false;
            SetSignals(leftSignals, false);
        }

        if (leftSignalOn || rightSignalOn)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkRate)
            {
                blinkTimer = 0f;
                blinkState = !blinkState;
                if (leftSignalOn) SetSignals(leftSignals, blinkState);
                if (rightSignalOn) SetSignals(rightSignals, blinkState);
            }
        }
        else
        {
            SetSignals(leftSignals, false);
            SetSignals(rightSignals, false);
        }
    }

    void FixedUpdate()
    {
        if (!isDriving)
        {
            // Xe đang đỗ: Trượt từ từ cho đến khi dừng hẳn
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);

            if (currentSpeed <= 0.1f)
            {
                // Khi xe đã dừng hẳn -> Khóa cứng xe thành "bức tường"
                currentSpeed = 0f;
                rb.isKinematic = true;
            }
            else
            {
                Vector3 stopVel = transform.forward * currentSpeed;
                rb.linearVelocity = new Vector3(stopVel.x, rb.linearVelocity.y, stopVel.z);
            }

            HandleSkidMarks(false);

            if (engineAudio != null)
            {
                engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, 0f, Time.fixedDeltaTime * 2f);
                engineAudio.volume = Mathf.Lerp(engineAudio.volume, 0f, Time.fixedDeltaTime * 2f);
                if (engineAudio.volume < 0.05f && engineAudio.isPlaying) engineAudio.Stop();
            }
            return;
        }

        // --- KHI ĐANG LÁI XE ---
        // Mở khóa vật lý nếu xe đang bị đóng băng
        if (rb.isKinematic) rb.isKinematic = false;

        float verticalInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal") * turnSpeed;
        bool isBoosting = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isBoosting ? boostSpeed : normalSpeed;

        if (isBraking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed * verticalInput, acceleration * Time.fixedDeltaTime);
        }

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float direction = Mathf.Sign(currentSpeed);
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * direction * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        Vector3 moveVelocity = transform.forward * currentSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

        bool isSkidding = isBraking && Mathf.Abs(currentSpeed) > 2f;
        HandleSkidMarks(isSkidding);
        HandleAudio(isSkidding);
    }

    void HandleSkidMarks(bool state)
    {
        foreach (TrailRenderer trail in skidMarks)
        {
            if (trail != null) trail.emitting = state;
        }
    }

    void HandleAudio(bool isSkidding)
    {
        if (engineAudio != null)
        {
            if (!engineAudio.isPlaying) engineAudio.Play();

            engineAudio.volume = Mathf.Lerp(engineAudio.volume, 1f, Time.deltaTime * 5f);

            float speedRatio = Mathf.Abs(currentSpeed) / boostSpeed;
            float targetPitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, speedRatio);
            engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetPitch, Time.deltaTime * 5f);
        }

        if (skidAudio != null)
        {
            if (isSkidding)
            {
                if (!skidAudio.isPlaying) skidAudio.Play();
            }
            else
            {
                if (skidAudio.isPlaying) skidAudio.Stop();
            }
        }
    }

    void SetSignals(GameObject[] signals, bool state)
    {
        foreach (GameObject signal in signals)
        {
            if (signal != null) signal.SetActive(state);
        }
    }
}