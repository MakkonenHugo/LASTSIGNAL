using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class IntroSequence : MonoBehaviour
{
    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Logo")]
    public Image logoImage;
    public float logoFadeInDuration = 1f;
    public float logoHoldDuration = 1.5f;
    public float logoFadeOutDuration = 1f;

    [Header("Terminal Text")]
    public TMP_Text terminalText;
    public List<string> terminalLines = new List<string>
    {
        "LASTSIGNAL",
        "Finding signal...",
        "Signal lost"
    };
    public float typeSpeed = 0.04f;
    public float lineHoldDuration = 0.8f;
    public float terminalFadeOutDuration = 0.4f;

    [Header("Glitch")]
    public RectTransform glitchTarget;
    public int glitchStepCount = 6;
    public float glitchStepDuration = 0.05f;
    public float glitchPositionJitter = 8f;
    public Color glitchFlickerColorA = new Color(1f, 0.15f, 0.35f);
    public Color glitchFlickerColorB = new Color(0.1f, 0.9f, 1f);

    [Header("Sound")]
    public AudioSource introStartSound;
    public AudioSource typingLoopSound;
    public AudioSource glitchSound;
    public float typingSoundStopBeforeEnd = 0.3f;

    [Header("Black Fade")]
    public Image blackOverlay;
    public float finalFadeOutDuration = 0.6f;

    void Start()
    {
        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        if (introStartSound != null)
            introStartSound.Play();

        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;
            c.a = 1f;
            blackOverlay.color = c;
        }

        if (logoImage != null)
        {
            SetImageAlpha(logoImage, 0f);
            logoImage.gameObject.SetActive(true);
        }

        if (terminalText != null)
            terminalText.gameObject.SetActive(false);

        if (blackOverlay != null)
            yield return StartCoroutine(FadeImage(blackOverlay, 1f, 0f, 0.5f));

        if (logoImage != null)
        {
            yield return StartCoroutine(FadeImage(logoImage, 0f, 1f, logoFadeInDuration));

            yield return new WaitForSeconds(logoHoldDuration);

            yield return StartCoroutine(FadeImage(logoImage, 1f, 0f, logoFadeOutDuration));

            logoImage.gameObject.SetActive(false);
        }

        if (terminalText != null)
        {
            terminalText.gameObject.SetActive(true);
            terminalText.color = new Color(terminalText.color.r, terminalText.color.g, terminalText.color.b, 1f);

            if (typingLoopSound != null)
                typingLoopSound.Play();

            for (int i = 0; i < terminalLines.Count; i++)
            {
                bool isLastLine = i == terminalLines.Count - 1;

                if (isLastLine && typingLoopSound != null && typingLoopSound.isPlaying)
                {
                    StartCoroutine(StopTypingSoundBeforeLineEnds(terminalLines[i]));
                }

                yield return StartCoroutine(TypeLine(terminalLines[i]));
                yield return new WaitForSeconds(lineHoldDuration);
            }

            if (typingLoopSound != null && typingLoopSound.isPlaying)
                typingLoopSound.Stop();

            if (glitchTarget != null)
            {
                if (glitchSound != null)
                    glitchSound.Play();

                yield return StartCoroutine(PlayGlitch());
            }

            yield return StartCoroutine(FadeText(terminalText, 1f, 0f, terminalFadeOutDuration));

            terminalText.gameObject.SetActive(false);
        }

        if (blackOverlay != null)
            yield return StartCoroutine(FadeImage(blackOverlay, 0f, 1f, finalFadeOutDuration));

        SceneManager.LoadScene(mainMenuSceneName);
    }

    IEnumerator TypeLine(string line)
    {
        terminalText.text = "";

        foreach (char letter in line)
        {
            terminalText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator StopTypingSoundBeforeLineEnds(string line)
    {
        float lineDuration = line.Length * typeSpeed;
        float waitTime = Mathf.Max(0f, lineDuration - typingSoundStopBeforeEnd);

        yield return new WaitForSeconds(waitTime);

        if (typingLoopSound != null && typingLoopSound.isPlaying)
            typingLoopSound.Stop();
    }

    IEnumerator PlayGlitch()
    {
        Vector2 originalPosition = glitchTarget.anchoredPosition;
        Color originalColor = terminalText.color;

        for (int i = 0; i < glitchStepCount; i++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-glitchPositionJitter, glitchPositionJitter),
                Random.Range(-glitchPositionJitter, glitchPositionJitter)
            );

            glitchTarget.anchoredPosition = originalPosition + offset;

            terminalText.color = Random.value > 0.5f ? glitchFlickerColorA : glitchFlickerColorB;

            yield return new WaitForSeconds(glitchStepDuration);
        }

        glitchTarget.anchoredPosition = originalPosition;
        terminalText.color = originalColor;
    }

    IEnumerator FadeImage(Image image, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            SetImageAlpha(image, Mathf.Lerp(from, to, t));

            yield return null;
        }

        SetImageAlpha(image, to);
    }

    IEnumerator FadeText(TMP_Text text, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color baseColor = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float alpha = Mathf.Lerp(from, to, t);
            text.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            yield return null;
        }

        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, to);
    }

    void SetImageAlpha(Image image, float alpha)
    {
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}