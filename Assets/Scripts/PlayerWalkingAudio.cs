using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingAudio : MonoBehaviour
{
    public CharacterController controller;
    public int loseSurfaceFrameThreshold = 5;

    private WalkingSounds currentSurface;
    private int framesWithoutSurface = 0;

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
            }

            return;
        }

        WalkingSounds surface = FindSurface();

        if (surface != null)
        {
            framesWithoutSurface = 0;

            if (currentSurface != surface)
            {
                StopCurrentSurface();

                currentSurface = surface;
                currentSurface.StartWalking();
            }
        }
        else
        {
            framesWithoutSurface++;

            if (framesWithoutSurface >= loseSurfaceFrameThreshold)
            {
                StopCurrentSurface();
            }
        }
    }

    WalkingSounds FindSurface()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(
            origin,
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