using UnityEngine;

public class RoomStretchEvent : MonoBehaviour
{
    public Transform room;
    public float targetScaleZ = 3f;
    public float stretchDuration = 1f;

    private Vector3 startScale;
    private bool stretching;

    public void StretchRoom()
    {
        if (stretching || room == null)
            return;

        startScale = room.localScale;
        StartCoroutine(Stretch());
    }

    System.Collections.IEnumerator Stretch()
    {
        stretching = true;

        float timer = 0f;
        Vector3 targetScale = new Vector3(
            startScale.x,
            startScale.y,
            targetScaleZ
        );

        while (timer < stretchDuration)
        {
            timer += Time.deltaTime;

            float t = timer / stretchDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            room.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                t
            );

            yield return null;
        }

        room.localScale = targetScale;

        stretching = false;
    }
}