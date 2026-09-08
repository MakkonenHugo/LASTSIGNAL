using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ChaseWarningText : MonoBehaviour
{
    public TMP_Text warningText;
    public List<string> warningMessages = new List<string>()
    {
        "RUN",
        "ITS CHASING YOU",
        "DONT LOOK BACK",
        "GO",
        "IT'S HERE"
    };

    public Color warningColor = new Color(0.9f, 0.05f, 0.05f, 1f);
    public float minFlashInterval = 0.4f;
    public float maxFlashInterval = 1.2f;
    public float flashVisibleDuration = 0.25f;

    private bool running = false;
    private Coroutine routine;

    void Awake()
    {
        if (warningText != null)
        {
            warningText.color = warningColor;
            SetAlpha(0f);
        }
    }

    public void StartWarnings()
    {
        if (running)
            return;

        running = true;
        routine = StartCoroutine(FlashRoutine());
    }

    public void StopWarnings()
    {
        running = false;

        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        SetAlpha(0f);
    }

    IEnumerator FlashRoutine()
    {
        while (running)
        {
            float wait = Random.Range(minFlashInterval, maxFlashInterval);
            yield return new WaitForSeconds(wait);

            if (!running)
                yield break;

            if (warningText != null && warningMessages.Count > 0)
            {
                warningText.text = warningMessages[Random.Range(0, warningMessages.Count)];
            }

            SetAlpha(1f);
            yield return new WaitForSeconds(flashVisibleDuration);
            SetAlpha(0f);
        }
    }

    void SetAlpha(float alpha)
    {
        if (warningText == null)
            return;

        Color c = warningText.color;
        c.a = alpha;
        warningText.color = c;
    }
}