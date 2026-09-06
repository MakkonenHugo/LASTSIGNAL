using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingAudio : MonoBehaviour
{
    public CharacterController controller;
    public int loseSurfaceFrameThreshold = 5;
    public int switchSurfaceFrameThreshold = 4;
    public float surfaceCheckRadius = 0.25f;

    private WalkingSounds currentSurface;
    private WalkingSounds pendingSurface;
    private int framesWithoutSurface = 0;
    private int framesOnPendingSurface = 0;

    void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (controller == null || Keyboard.current == null)
            return;

        bool moving =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed;

        if (!moving || !controller.isGrounded)
        {
            framesWithoutSurface++;

            if (framesWithoutSurface >= loseSurfaceFrameThreshold)
            {
                StopCurrentSurface();
                pendingSurface = null;
                framesOnPendingSurface = 0;
            }

            return;
        }

        WalkingSounds surface = FindSurface();

        if (surface != null)
        {
            framesWithoutSurface = 0;

            if (surface == currentSurface)
            {
                pendingSurface = null;
                framesOnPendingSurface = 0;
                return;
            }

            if (surface != pendingSurface)
            {
                pendingSurface = surface;
                framesOnPendingSurface = 0;
            }

            framesOnPendingSurface++;

            if (framesOnPendingSurface >= switchSurfaceFrameThreshold)
            {
                StopCurrentSurface();
                currentSurface = pendingSurface;
                currentSurface.StartWalking();
                pendingSurface = null;
                framesOnPendingSurface = 0;
            }
        }
        else
        {
            framesWithoutSurface++;

            if (framesWithoutSurface >= loseSurfaceFrameThreshold)
            {
                StopCurrentSurface();
                pendingSurface = null;
                framesOnPendingSurface = 0;
            }
        }
    }

    WalkingSounds FindSurface()
    {
        Vector3 origin = transform.position + Vector3.up * 0.3f;

        if (Physics.SphereCast(
            origin,
            surfaceCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            3f))
        {
            return hit.collider.GetComponentInParent<WalkingSounds>();
        }

        return null;
    }

    void StopCurrentSurface()
    {
        if (currentSurface != null)
        {
            currentSurface.StopWalking();
            currentSurface = null;
        }

        framesWithoutSurface = 0;
    }
}