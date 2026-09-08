using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level8Controller : MonoBehaviour
{
    [Header("Radio Dialogue")]
    public DialogueUI dialogueUI;
    public InteractionDetector interactionDetector;

    [TextArea(2, 4)]
    public List<string> jeffMessages = new List<string>
    {
        "this is my LAST SIGNAL for you, find an way out",
        "and destroy this place before anybody else comes here,",

        "i wont see you anymore so i might aswell say it...",
        "Im JEFF, i cheated the system but i didnt succeed,",

        "so i desided with my last efforts,",
        "help YOU succeed,",

        "but after the administrators catched me,",
        "they turned ME into an twisted experiement,",

        "and now im forced to stand still and watch new people get tormented,",
        "i only WANTED TO HELP"
    };

    public float delayBetweenMessages = 0.4f;

    [Header("Jeff2 Glimpse")]
    public GameObject jeff2Object;
    public int glimpseAfterMessageIndex = 4;
    public float glimpseVisibleDuration = 0.2f;

    [Header("Atmosphere")]
    public List<Light> lightsToDim;
    public float dimmedIntensity = 0.15f;
    public float dimTransitionDuration = 2f;
    public AudioSource ambientSource;
    [Range(0f, 1f)] public float ambientDuckVolume = 0.2f;

    [Header("Camera Lock")]
    public MouseLook playerMouseLook;
    public bool lockCameraDuringSpeech = true;
    public float cameraLockRange = 15f;

    private bool hasPlayed = false;
    private bool playing = false;
    private List<float> originalLightIntensities = new List<float>();
    private float originalAmbientVolume = 1f;

    void Start()
    {
        if (jeff2Object != null)
            jeff2Object.SetActive(false);

        foreach (Light light in lightsToDim)
        {
            originalLightIntensities.Add(light != null ? light.intensity : 0f);
        }

        if (ambientSource != null)
            originalAmbientVolume = ambientSource.volume;
    }

    public void PlayJeffMessage()
    {
        if (hasPlayed || playing)
            return;

        hasPlayed = true;
        playing = true;

        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        if (interactionDetector != null)
            interactionDetector.SuppressPrompt();

        if (lockCameraDuringSpeech && playerMouseLook != null)
            playerMouseLook.LockYaw(cameraLockRange);

        StartCoroutine(DimLightsRoutine());
        StartCoroutine(DuckAmbientRoutine());

        yield return StartCoroutine(PlayMessagesRoutine());

        yield return StartCoroutine(RestoreLightsRoutine());
        yield return StartCoroutine(RestoreAmbientRoutine());

        if (lockCameraDuringSpeech && playerMouseLook != null)
            playerMouseLook.UnlockYaw();

        if (interactionDetector != null)
            interactionDetector.ResumePrompt();

        playing = false;
    }

    IEnumerator PlayMessagesRoutine()
    {
        if (dialogueUI == null)
            yield break;

        for (int i = 0; i < jeffMessages.Count; i++)
        {
            string message = jeffMessages[i];

            if (string.IsNullOrEmpty(message))
                continue;

            if (i == glimpseAfterMessageIndex)
            {
                StartCoroutine(GlimpseJeff2Routine());
            }

            dialogueUI.ShowMessage(message);

            yield return new WaitUntil(() => !dialogueUI.IsShowing);

            yield return new WaitForSeconds(delayBetweenMessages);
        }
    }

    IEnumerator GlimpseJeff2Routine()
    {
        if (jeff2Object == null)
            yield break;

        jeff2Object.SetActive(true);
        yield return new WaitForSeconds(glimpseVisibleDuration);
        jeff2Object.SetActive(false);
    }

    IEnumerator DimLightsRoutine()
    {
        if (lightsToDim.Count == 0)
            yield break;

        float elapsed = 0f;

        while (elapsed < dimTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dimTransitionDuration;

            for (int i = 0; i < lightsToDim.Count; i++)
            {
                if (lightsToDim[i] != null)
                {
                    lightsToDim[i].intensity = Mathf.Lerp(
                        originalLightIntensities[i],
                        dimmedIntensity,
                        t
                    );
                }
            }

            yield return null;
        }
    }

    IEnumerator RestoreLightsRoutine()
    {
        if (lightsToDim.Count == 0)
            yield break;

        float elapsed = 0f;

        List<float> startIntensities = new List<float>();

        foreach (Light light in lightsToDim)
        {
            startIntensities.Add(light != null ? light.intensity : 0f);
        }

        while (elapsed < dimTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dimTransitionDuration;

            for (int i = 0; i < lightsToDim.Count; i++)
            {
                if (lightsToDim[i] != null)
                {
                    lightsToDim[i].intensity = Mathf.Lerp(
                        startIntensities[i],
                        originalLightIntensities[i],
                        t
                    );
                }
            }

            yield return null;
        }
    }

    IEnumerator DuckAmbientRoutine()
    {
        if (ambientSource == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < dimTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dimTransitionDuration;

            ambientSource.volume = Mathf.Lerp(
                originalAmbientVolume,
                ambientDuckVolume,
                t
            );

            yield return null;
        }
    }

    IEnumerator RestoreAmbientRoutine()
    {
        if (ambientSource == null)
            yield break;

        float elapsed = 0f;
        float startVolume = ambientSource.volume;

        while (elapsed < dimTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dimTransitionDuration;

            ambientSource.volume = Mathf.Lerp(
                startVolume,
                originalAmbientVolume,
                t
            );

            yield return null;
        }
    }
}