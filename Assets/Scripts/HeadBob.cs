using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBob : MonoBehaviour
{
    public float bobFrequency = 8f;
    public float bobAmount = 0.025f;
    public float sideBobAmount = 0.012f;
    public float smoothness = 10f;
    public float blendSpeed = 6f;

    private Vector3 startPosition;
    private float timer;
    private float moveBlend;

    void Start()
    {
        startPosition = transform.localPosition;
        timer = 0f;
        moveBlend = 0f;
    }

    void Update()
    {
        bool isMoving = false;

        if (Keyboard.current != null)
        {
            isMoving =
                Keyboard.current.wKey.isPressed ||
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.sKey.isPressed ||
                Keyboard.current.dKey.isPressed;
        }

        moveBlend = Mathf.MoveTowards(moveBlend, isMoving ? 1f : 0f, Time.deltaTime * blendSpeed);

        timer += Time.deltaTime * bobFrequency;

        float bobY = Mathf.Sin(timer) * bobAmount * moveBlend;
        float bobX = Mathf.Cos(timer * 0.5f) * sideBobAmount * moveBlend;

        Vector3 targetPosition = startPosition + new Vector3(bobX, bobY, 0f);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * smoothness
        );
    }
}