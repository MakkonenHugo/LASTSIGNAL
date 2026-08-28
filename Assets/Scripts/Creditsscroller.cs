using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(ScrollRect))]
public class CreditsScroller : MonoBehaviour
{
    public ScrollRect scrollRect;
    public TMP_Text creditsText;

    public float scrollSpeed = 25f;
    public float startDelay = 1f;
    public float endDelay = 2f;
    public bool loop = true;

    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.35f;
    public bool musicLoop = true;

    public bool ambientGlitchEnabled = true;
    public Vector2 glitchIntervalRange = new Vector2(2.5f, 6f);
    public float glitchStepDuration = 0.12f;
    public int glitchStepCount = 3;

    public float positionJitter = 3f;
    [Range(0f, 1f)] public float colorFlickerChance = 0.3f;
    public Color flickerColorA = new Color(1f, 0.15f, 0.35f);
    public Color flickerColorB = new Color(0.1f, 0.9f, 1f);

    RectTransform content;
    RectTransform viewport;
    RectTransform textRect;
    AudioSource audioSource;

    Vector2 startPosition;
    Vector2 textStartPosition;
    Color originalColor;

    float timer;
    float endTimer;
    float maxScroll;

    bool scrolling;
    bool waitingAtEnd;
    bool initialized;

    float nextGlitchTime;
    bool glitching;
    int glitchStepsDone;
    float glitchStepTimer;

    void Awake()
    {
        scrollRect = scrollRect == null ? GetComponent<ScrollRect>() : scrollRect;

        if (creditsText == null)
            creditsText = GetComponentInChildren<TMP_Text>();

        content = scrollRect.content;
        viewport = scrollRect.viewport != null
            ? scrollRect.viewport
            : scrollRect.GetComponent<RectTransform>();

        if (creditsText != null)
        {
            textRect = creditsText.rectTransform;
            originalColor = creditsText.color;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = musicLoop;
        audioSource.volume = musicVolume;

        scrollRect.horizontal = false;
        scrollRect.vertical = false;
        scrollRect.inertia = false;
    }

    void OnEnable()
    {
        StopAllCoroutines();

        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = musicLoop;
            audioSource.Play();
        }

        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        initialized = false;
        scrolling = false;
        waitingAtEnd = false;
        timer = 0f;
        endTimer = 0f;
        glitching = false;

        yield return null;
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (content == null || viewport == null || textRect == null)
            yield break;

        creditsText.ForceMeshUpdate();

        float viewportHeight = viewport.rect.height;
        float textHeight = creditsText.preferredHeight;

        textRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            textHeight
        );

        float contentHeight = textHeight + viewportHeight;

        content.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            contentHeight
        );

        startPosition = content.anchoredPosition;
        textStartPosition = textRect.anchoredPosition;

        maxScroll = Mathf.Max(0f, contentHeight - viewportHeight);

        content.anchoredPosition = startPosition;
        textRect.anchoredPosition = textStartPosition;

        ResetTextVisuals();
        ScheduleNextGlitch();

        initialized = true;
    }

    void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();

        ResetTextVisuals();

        if (content != null)
            content.anchoredPosition = startPosition;
    }

    void Update()
    {
        if (!initialized)
            return;

        UpdateScroll();

        if (ambientGlitchEnabled)
            UpdateAmbientGlitch();
    }

    void UpdateScroll()
    {
        if (waitingAtEnd)
        {
            endTimer += Time.unscaledDeltaTime;

            if (endTimer >= endDelay)
            {
                if (loop)
                {
                    content.anchoredPosition = startPosition;
                    waitingAtEnd = false;
                    endTimer = 0f;
                }
                else
                {
                    scrolling = false;
                }
            }

            return;
        }

        if (!scrolling)
        {
            timer += Time.unscaledDeltaTime;

            if (timer < startDelay)
                return;

            scrolling = true;
        }

        if (maxScroll <= 0f)
            return;

        Vector2 position = content.anchoredPosition;

        position.y -= scrollSpeed * Time.unscaledDeltaTime;

        float minimumY = startPosition.y - maxScroll;

        if (position.y <= minimumY)
        {
            position.y = minimumY;
            waitingAtEnd = true;
            endTimer = 0f;
        }

        content.anchoredPosition = position;
    }

    void ScheduleNextGlitch()
    {
        nextGlitchTime =
            Time.unscaledTime +
            Random.Range(glitchIntervalRange.x, glitchIntervalRange.y);
    }

    void UpdateAmbientGlitch()
    {
        if (textRect == null)
            return;

        if (!glitching)
        {
            if (Time.unscaledTime >= nextGlitchTime)
            {
                glitching = true;
                glitchStepsDone = 0;
                glitchStepTimer = 0f;
            }

            return;
        }

        glitchStepTimer += Time.unscaledDeltaTime;

        if (glitchStepTimer >= glitchStepDuration)
        {
            glitchStepTimer = 0f;
            glitchStepsDone++;

            if (glitchStepsDone >= glitchStepCount)
            {
                glitching = false;
                ResetTextVisuals();
                ScheduleNextGlitch();
                return;
            }

            ApplyGlitchStep();
        }
    }

    void ApplyGlitchStep()
    {
        Vector2 offset = new Vector2(
            Random.Range(-positionJitter, positionJitter),
            Random.Range(-positionJitter, positionJitter)
        );

        textRect.anchoredPosition =
            textStartPosition + offset;

        if (Random.value < colorFlickerChance)
        {
            creditsText.color =
                Random.value > 0.5f
                ? flickerColorA
                : flickerColorB;
        }
        else
        {
            creditsText.color = originalColor;
        }
    }

    void ResetTextVisuals()
    {
        if (textRect == null)
            return;

        textRect.anchoredPosition = textStartPosition;
        creditsText.color = originalColor;
    }
}