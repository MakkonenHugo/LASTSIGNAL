using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2f;
    public float maxDeltaPerFrame = 20f;
    public int smoothingFrameCount = 3;

    private float xRotation = 0f;
    private bool firstFrameSkipped = false;

    private Vector2[] deltaHistory;
    private int historyIndex = 0;
    private int historyFilled = 0;

    private bool yawLocked = false;
    private float yawLockCenter = 0f;
    private float yawLockRange = 90f;
    private float currentYaw = 0f;

    void Awake()
    {
        xRotation = transform.localEulerAngles.x;
        if (xRotation > 180f)
            xRotation -= 360f;

        deltaHistory = new Vector2[Mathf.Max(1, smoothingFrameCount)];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        firstFrameSkipped = false;
        historyIndex = 0;
        historyFilled = 0;

        for (int i = 0; i < deltaHistory.Length; i++)
            deltaHistory[i] = Vector2.zero;

        if (transform.parent != null)
        {
            currentYaw = transform.parent.eulerAngles.y;
        }
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

        rawDelta = Vector2.ClampMagnitude(rawDelta, maxDeltaPerFrame);

        deltaHistory[historyIndex] = rawDelta;
        historyIndex = (historyIndex + 1) % deltaHistory.Length;
        historyFilled = Mathf.Min(historyFilled + 1, deltaHistory.Length);

        Vector2 averagedDelta = Vector2.zero;
        for (int i = 0; i < historyFilled; i++)
            averagedDelta += deltaHistory[i];
        averagedDelta /= historyFilled;

        float mouseX = averagedDelta.x * sensitivity * 0.1f;
        float mouseY = averagedDelta.y * sensitivity * 0.1f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (transform.parent != null)
        {
            if (yawLocked)
            {
                currentYaw += mouseX;

                float minYaw = yawLockCenter - yawLockRange;
                float maxYaw = yawLockCenter + yawLockRange;
                currentYaw = Mathf.Clamp(currentYaw, minYaw, maxYaw);

                Vector3 euler = transform.parent.eulerAngles;
                transform.parent.rotation = Quaternion.Euler(euler.x, currentYaw, euler.z);
            }
            else
            {
                currentYaw += mouseX;

                transform.parent.Rotate(
                    Vector3.up * mouseX,
                    Space.World
                );
            }
        }
    }

    public void LockYaw(float range = 90f)
    {
        yawLocked = true;
        yawLockRange = range;

        if (transform.parent != null)
        {
            yawLockCenter = transform.parent.eulerAngles.y;
            currentYaw = yawLockCenter;
        }
    }

    public void UnlockYaw()
    {
        yawLocked = false;
    }
}