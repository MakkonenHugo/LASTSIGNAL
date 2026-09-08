using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level6Controller : MonoBehaviour
{
    [Header("Trigger")]
    public bool triggerOnStart = false;

    [Header("Flickering Lights")]
    public float flickerStartDelay = 0f;
    public List<Light> flickeringLights = new List<Light>();
    public float flickerDuration = 4f;
    public float flickerMinInterval = 0.05f;
    public float flickerMaxInterval = 0.3f;

    [Header("Doors")]
    public float doorSlamStartDelay = 1.5f;
    public List<DoorController> doorsToSlam = new List<DoorController>();

    [Header("Distant Sounds")]
    public float distantSoundStartDelay = 2f;
    public AudioSource distantSoundSource;
    public List<AudioClip> distantSounds = new List<AudioClip>();
    public float distantSoundInterval = 4f;
    public int distantSoundCount = 3;

    [Header("Brief Glimpse")]
    public float glimpseStartDelay = 5f;
    public GameObject glimpseObject;
    public float glimpseVisibleDuration = 0.15f;

    [Header("Moving Objects")]
    public float objectMoveStartDelay = 6f;
    public List<Transform> objectsToMove = new List<Transform>();
    public float objectMoveDistance = 0.5f;
    public float objectMoveDuration = 0.3f;

    [Header("Ambient Change")]
    public float ambientChangeStartDelay = 0.5f;
    public AudioSource ambientSource;
    public AudioClip disturbedAmbientClip;

    private bool hasTriggered = false;

    void Start()
    {
        if (triggerOnStart)
        {
            TriggerSequence();
        }
    }

    public void TriggerSequence()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        StartCoroutine(RunDelayed(flickerStartDelay, FlickerLightsRoutine()));
        StartCoroutine(RunDelayed(ambientChangeStartDelay, SwitchAmbientRoutine()));
        StartCoroutine(RunDelayed(distantSoundStartDelay, DistantSoundsRoutine()));
        StartCoroutine(RunDelayed(doorSlamStartDelay, SlamDoorsRoutine()));
        StartCoroutine(RunDelayed(glimpseStartDelay, GlimpseRoutine()));
        StartCoroutine(RunDelayed(objectMoveStartDelay, MoveObjectsRoutine()));
    }

    IEnumerator RunDelayed(float delay, IEnumerator routine)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        yield return StartCoroutine(routine);
    }

    IEnumerator FlickerLightsRoutine()
    {
        if (flickeringLights.Count == 0)
            yield break;

        float elapsed = 0f;

        List<bool> originalStates = new List<bool>();
        foreach (Light light in flickeringLights)
        {
            originalStates.Add(light != null && light.enabled);
        }

        while (elapsed < flickerDuration)
        {
            float waitTime = Random.Range(flickerMinInterval, flickerMaxInterval);
            yield return new WaitForSeconds(waitTime);
            elapsed += waitTime;

            foreach (Light light in flickeringLights)
            {
                if (light != null)
                {
                    light.enabled = !light.enabled;
                }
            }
        }

        for (int i = 0; i < flickeringLights.Count; i++)
        {
            if (flickeringLights[i] != null)
            {
                flickeringLights[i].enabled = originalStates[i];
            }
        }
    }

    IEnumerator SwitchAmbientRoutine()
    {
        if (ambientSource != null && disturbedAmbientClip != null)
        {
            ambientSource.clip = disturbedAmbientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }

        yield break;
    }

    IEnumerator SlamDoorsRoutine()
    {
        foreach (DoorController door in doorsToSlam)
        {
            if (door != null)
            {
                door.ForceClose();
            }
        }

        yield break;
    }

    IEnumerator DistantSoundsRoutine()
    {
        if (distantSoundSource == null || distantSounds.Count == 0)
            yield break;

        for (int i = 0; i < distantSoundCount; i++)
        {
            AudioClip clip = distantSounds[Random.Range(0, distantSounds.Count)];
            distantSoundSource.PlayOneShot(clip);

            yield return new WaitForSeconds(distantSoundInterval);
        }
    }

    IEnumerator GlimpseRoutine()
    {
        if (glimpseObject == null)
            yield break;

        glimpseObject.SetActive(true);
        yield return new WaitForSeconds(glimpseVisibleDuration);
        glimpseObject.SetActive(false);
    }

    IEnumerator MoveObjectsRoutine()
    {
        foreach (Transform obj in objectsToMove)
        {
            if (obj != null)
            {
                StartCoroutine(NudgeObject(obj));
            }
        }

        yield return null;
    }

    IEnumerator NudgeObject(Transform obj)
    {
        Vector3 startPos = obj.position;
        Vector3 randomOffset = new Vector3(
            Random.Range(-objectMoveDistance, objectMoveDistance),
            0f,
            Random.Range(-objectMoveDistance, objectMoveDistance)
        );
        Vector3 targetPos = startPos + randomOffset;

        float elapsed = 0f;

        while (elapsed < objectMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / objectMoveDuration;
            obj.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        obj.position = targetPos;
    }
}