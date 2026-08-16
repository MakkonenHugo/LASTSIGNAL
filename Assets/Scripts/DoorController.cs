using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen;
    private bool isMoving;

    public void OpenDoor()
    {
        if (isOpen || isMoving)
            return;

        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        isMoving = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, openAngle, 0f);

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * openSpeed;

            transform.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                time
            );

            yield return null;
        }

        transform.localRotation = targetRotation;

        isOpen = true;
        isMoving = false;
    }
}