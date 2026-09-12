using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BurnItDownTrigger : MonoBehaviour
{
    public FakeOSTerminal fakeOSTerminal;
    public string burnSceneName = "Level10Burning";
    public float fadeOutDuration = 1.5f;
    public Image fadeImage;
    public InteractionDetector interactionDetector;

    private Interaction interaction;
    private bool triggered = false;
    private bool wasUnlocked = false;

    void Awake()
    {
        interaction = GetComponent<Interaction>();
    }

    void Update()
    {
        if (fakeOSTerminal == null)
            return;

        bool unlocked = fakeOSTerminal.AreFilesDeleted();

        if (interaction != null)
        {
            interaction.interactionText = unlocked ? "BURN IT DOWN" : "";
            interaction.enabled = unlocked;
        }

        if (unlocked && !wasUnlocked)
        {
            wasUnlocked = true;

            if (interactionDetector != null)
            {
                interactionDetector.HidePrompt();
            }
        }
    }

    public void OnBurnInteract()
    {
        if (triggered)
            return;

        if (fakeOSTerminal == null || !fakeOSTerminal.AreFilesDeleted())
            return;

        triggered = true;
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        if (fadeImage != null)
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

        SceneManager.LoadScene(burnSceneName);
    }
}