using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2f;
    public float smoothing = 18f;
    public float maxDeltaPerFrame = 20f;

    private float xRotation = 0f;
    private Vector2 currentMouseDelta;
    private Vector2 targetMouseDelta;
    private bool firstFrameSkipped = false;

    void Awake()
    {
        xRotation = transform.localEulerAngles.x;
        if (xRotation > 180f)
            xRotation -= 360f;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentMouseDelta = Vector2.zero;
        targetMouseDelta = Vector2.zero;
        firstFrameSkipped = false;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (Mouse.current == null)
            return;

        Vector2 rawDelta = Mouse.current.delta.ReadValue();

        if (!firstFrameSkipped)
        {
            firstFrameSkipped = true;
            return;
        }

        targetMouseDelta = Vector2.ClampMagnitude(rawDelta, maxDeltaPerFrame);

        currentMouseDelta = Vector2.Lerp(currentMouseDelta, targetMouseDelta, Time.deltaTime * smoothing);

        float mouseX = currentMouseDelta.x * sensitivity * 0.1f;
        float mouseY = currentMouseDelta.y * sensitivity * 0.1f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (transform.parent != null)
        {
            transform.parent.Rotate(
                Vector3.up * mouseX,
                Space.World
            );
        }
    }
}