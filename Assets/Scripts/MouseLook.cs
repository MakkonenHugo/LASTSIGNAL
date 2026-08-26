using UnityEngine;
using UnityEngine.InputSystem;
public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2f;
    private float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        xRotation = transform.localEulerAngles.x;
        if (xRotation > 180f)
            xRotation -= 360f;
    }
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;
        if (Mouse.current == null)
            return;
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensitivity * 0.1f;
        float mouseY = mouseDelta.y * sensitivity * 0.1f;
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