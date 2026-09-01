using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float fadeInDuration = 1f;

    void Start()
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);

            color.a = 1f - t;
            fadeImage.color = color;

            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }
}