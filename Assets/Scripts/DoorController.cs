using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen;
    private bool isMoving;
    private bool isUnlocked;

    public void UnlockDoor()
    {
        Debug.Log("[DoorController] UnlockDoor() kutsuttu objektilla: " + gameObject.name);
        isUnlocked = true;
    }

    public void OpenDoor()
    {
        Debug.Log("[DoorController] OpenDoor() kutsuttu objektilla: " + gameObject.name +
            " | isUnlocked=" + isUnlocked + " isOpen=" + isOpen + " isMoving=" + isMoving);

        if (!isUnlocked)
        {
            Debug.Log("[DoorController] Keskeytetaan: ovi ei ole unlocked");
            return;
        }

        if (isMoving)
        {
            Debug.Log("[DoorController] Keskeytetaan: ovi on jo liikkeessa");
            return;
        }

        if (isOpen)
        {
            Debug.Log("[DoorController] Suljetaan ovi");
            StartCoroutine(Close());
        }
        else
        {
            Debug.Log("[DoorController] Avataan ovi");
            StartCoroutine(Open());
        }
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
        Debug.Log("[DoorController] Ovi auki");
    }

    IEnumerator Close()
    {
        isMoving = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, -openAngle, 0f);

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
        Debug.Log("[DoorController] Ovi kiinni");
    }
}