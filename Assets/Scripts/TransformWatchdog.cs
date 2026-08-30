using UnityEngine;

public class TransformWatchdog : MonoBehaviour
{
    public Transform playerRoot;
    public Camera cam;
    public float rotationJumpThreshold = 3f;
    public float positionJumpThreshold = 0.05f;
    public float playerPositionJumpThreshold = 0.1f;
    public float fovJumpThreshold = 2f;

    private Quaternion lastCameraLocalRotation;
    private Quaternion lastPlayerRotation;
    private Vector3 lastCameraLocalPosition;
    private Vector3 lastPlayerPosition;
    private float lastFov;
    private bool initialized = false;

    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        lastCameraLocalRotation = transform.localRotation;
        lastCameraLocalPosition = transform.localPosition;

        if (playerRoot != null)
        {
            lastPlayerRotation = playerRoot.rotation;
            lastPlayerPosition = playerRoot.position;
        }
        else
        {
            Debug.LogWarning("WATCHDOG: playerRoot is not assigned! Player position/rotation will not be tracked.");
        }

        if (cam != null)
            lastFov = cam.fieldOfView;
        else
            Debug.LogWarning("WATCHDOG: camera reference is missing! FOV will not be tracked.");

        initialized = true;

        Debug.Log("WATCHDOG initialized successfully. Tracking camera: " + (cam != null) + " Tracking playerRoot: " + (playerRoot != null));
    }

    void LateUpdate()
    {
        if (!initialized)
            return;

        float camAngle = Quaternion.Angle(lastCameraLocalRotation, transform.localRotation);
        if (camAngle > rotationJumpThreshold)
        {
            Debug.Log("WATCHDOG ROT (camera local): jumped " + camAngle + " deg at " + Time.time +
                       " from " + lastCameraLocalRotation.eulerAngles + " to " + transform.localRotation.eulerAngles);
        }

        Vector3 posDelta = transform.localPosition - lastCameraLocalPosition;
        if (posDelta.magnitude > positionJumpThreshold)
        {
            Debug.Log("WATCHDOG POS (camera local): jumped " + posDelta.magnitude + " units at " + Time.time +
                       " from " + lastCameraLocalPosition + " to " + transform.localPosition);
        }

        if (playerRoot != null)
        {
            float playerAngle = Quaternion.Angle(lastPlayerRotation, playerRoot.rotation);
            if (playerAngle > rotationJumpThreshold)
            {
                Debug.Log("WATCHDOG ROT (player): jumped " + playerAngle + " deg at " + Time.time +
                           " from " + lastPlayerRotation.eulerAngles + " to " + playerRoot.rotation.eulerAngles);
            }

            float playerPosDelta = Vector3.Distance(lastPlayerPosition, playerRoot.position);
            if (playerPosDelta > playerPositionJumpThreshold)
            {
                Debug.Log("WATCHDOG POS (player world): jumped " + playerPosDelta + " units at " + Time.time +
                           " from " + lastPlayerPosition + " to " + playerRoot.position);
            }

            lastPlayerRotation = playerRoot.rotation;
            lastPlayerPosition = playerRoot.position;
        }

        if (cam != null)
        {
            float fovDelta = Mathf.Abs(cam.fieldOfView - lastFov);
            if (fovDelta > fovJumpThreshold)
            {
                Debug.Log("WATCHDOG FOV: jumped " + fovDelta + " at " + Time.time +
                           " from " + lastFov + " to " + cam.fieldOfView);
            }

            lastFov = cam.fieldOfView;
        }

        lastCameraLocalRotation = transform.localRotation;
        lastCameraLocalPosition = transform.localPosition;
    }
}