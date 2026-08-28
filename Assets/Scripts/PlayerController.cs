using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 12f;
    public float deceleration = 8f;
    public float airControlMultiplier = 0.4f;
    public float gravity = -15f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 horizontalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y = 1;

            if (Keyboard.current.sKey.isPressed)
                input.y = -1;

            if (Keyboard.current.aKey.isPressed)
                input.x = -1;

            if (Keyboard.current.dKey.isPressed)
                input.x = 1;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 wishDirection =
            transform.right * input.x +
            transform.forward * input.y;

        Vector3 wishVelocity = wishDirection * speed;

        bool grounded = controller.isGrounded;
        float rate = wishVelocity.sqrMagnitude > 0.01f ? acceleration : deceleration;

        if (!grounded)
            rate *= airControlMultiplier;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, wishVelocity, rate * Time.deltaTime);

        controller.Move(horizontalVelocity * Time.deltaTime);

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}