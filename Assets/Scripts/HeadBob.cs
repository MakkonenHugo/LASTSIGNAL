using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBob : MonoBehaviour
{
    public float bobFrequency = 8f;
    public float bobAmount = 0.025f;
    public float sideBobAmount = 0.012f;
    public float smoothness = 10f;
    public float jitter = 0.15f;

    private Vector3 startPosition;
    private float timer;
    private float currentJitter;

    void Start()
    {
        startPosition = transform.localPosition;
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

        if (isMoving)
        {
            timer += Time.deltaTime * bobFrequency;
            currentJitter = Mathf.Lerp(currentJitter, Random.Range(-jitter, jitter), Time.deltaTime * 5f);

            float bobY = Mathf.Sin(timer) * bobAmount;
            float bobX = Mathf.Cos(timer * 0.5f) * sideBobAmount;

            Vector3 targetPosition = startPosition;
            targetPosition.y += bobY;
            targetPosition.x += bobX + (currentJitter * bobAmount * 0.1f);

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * smoothness
            );
        }
        else
        {
            timer = 0f;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPosition,
                Time.deltaTime * smoothness
            );
        }
    }
}