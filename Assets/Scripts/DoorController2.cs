using UnityEngine;
using System.Collections;

public class DoorController2 : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen;
    private bool isMoving;

    public void OpenDoor()
    {
        if (isMoving)
            return;

        if (isOpen)
        {
            StartCoroutine(Close());
        }
        else
        {
            StartCoroutine(Open());
        }
    }

    IEnumerator Open()
    {
        isMoving = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, openAngle, 0f);

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

    IEnumerator Close()
    {
        isMoving = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, -openAngle, 0f);

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

        isOpen = false;
        isMoving = false;
    }

    public void ForceClose()
    {
        if (!isOpen)
            return;

        StopAllCoroutines();
        StartCoroutine(Close());
    }
}