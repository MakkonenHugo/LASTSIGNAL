using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorLevelTransition : MonoBehaviour
{
    public DoorController doorController;
    public string targetSceneName = "";

    [Header("Sound")]
    public AudioSource transitionSound;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeOutDuration = 1f;
    public float delayAfterDoorOpens = 0.5f;

    private bool triggered = false;

    public bool CanOpen => doorController != null && doorController.IsUnlocked;

    public void TryOpenAndTransition()
    {
        if (triggered)
            return;

        if (doorController == null)
        {
            Debug.LogWarning("DoorLevelTransition: doorController on tyhja objektilla " + gameObject.name);
            return;
        }

        if (!doorController.IsUnlocked)
            return;

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("DoorLevelTransition: targetSceneName on tyhja objektilla " + gameObject.name);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError("DoorLevelTransition: scenea '" + targetSceneName + "' ei loydy Build Settingsista.");
            return;
        }

        triggered = true;

        StartCoroutine(OpenAndTransition());
    }

    IEnumerator OpenAndTransition()
    {
        doorController.OpenDoor();

        yield return new WaitForSeconds(delayAfterDoorOpens);

        if (transitionSound != null)
        {
            transitionSound.Play();
        }

        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        SceneManager.LoadScene(targetSceneName);
    }

    IEnumerator FadeOut()
    {
        RectTransform rt = fadeImage.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        fadeImage.transform.SetAsLastSibling();
        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeOutDuration);

            color.a = t;
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }
}