using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level6Controller : MonoBehaviour
{
    [Header("Trigger")]
    public bool triggerOnStart = false;

    [Header("Flickering Lights")]
    public List<Light> flickeringLights = new List<Light>();
    public float flickerDuration = 4f;
    public float flickerMinInterval = 0.05f;
    public float flickerMaxInterval = 0.3f;

    [Header("Doors")]
    public List<DoorController> doorsToSlam = new List<DoorController>();
    public float doorSlamDelay = 1.5f;

    [Header("Distant Sounds")]
    public AudioSource distantSoundSource;
    public List<AudioClip> distantSounds = new List<AudioClip>();
    public float distantSoundStartDelay = 2f;
    public float distantSoundInterval = 4f;
    public int distantSoundCount = 3;

    [Header("Brief Glimpse")]
    public GameObject glimpseObject;
    public float glimpseDelay = 5f;
    public float glimpseVisibleDuration = 0.15f;

    [Header("Moving Objects")]
    public List<Transform> objectsToMove = new List<Transform>();
    public float objectMoveDelay = 6f;
    public float objectMoveDistance = 0.5f;
    public float objectMoveDuration = 0.3f;

    [Header("Ambient Change")]
    public AudioSource ambientSource;
    public AudioClip disturbedAmbientClip;
    public float ambientChangeDelay = 0.5f;

    [Header("Timing")]
    public float totalSequenceDuration = 12f;

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
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        StartCoroutine(FlickerLightsRoutine());

        yield return new WaitForSeconds(ambientChangeDelay);
        SwitchAmbient();

        yield return new WaitForSeconds(distantSoundStartDelay - ambientChangeDelay);
        StartCoroutine(DistantSoundsRoutine());

        yield return new WaitForSeconds(doorSlamDelay - distantSoundStartDelay);
        SlamDoors();

        yield return new WaitForSeconds(glimpseDelay - doorSlamDelay);
        StartCoroutine(GlimpseRoutine());

        yield return new WaitForSeconds(objectMoveDelay - glimpseDelay);
        StartCoroutine(MoveObjectsRoutine());
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

    void SwitchAmbient()
    {
        if (ambientSource != null && disturbedAmbientClip != null)
        {
            ambientSource.clip = disturbedAmbientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    void SlamDoors()
    {
        foreach (DoorController door in doorsToSlam)
        {
            if (door != null)
            {
                door.ForceClose();
            }
        }
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