using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingAudio : MonoBehaviour
{
    public CharacterController controller;

    private WalkingSounds currentSurface;

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
            StopCurrentSurface();
            return;
        }

        WalkingSounds surface = FindSurface();

        if (surface != null)
        {
            if (currentSurface != surface)
            {
                StopCurrentSurface();

                currentSurface = surface;
                currentSurface.StartWalking();
            }
            else
            {
                currentSurface.StartWalking();
            }
        }
        else
        {
            StopCurrentSurface();
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
    }
}