using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class SettingsPanelTransition : MonoBehaviour
{
    public float openDuration = 0.4f;
    public float closeDuration = 0.3f;
    public AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float startScale = 0.85f;
    public float overshootScale = 1.03f;
    public Vector2 startOffset = new Vector2(0f, -20f);
    public CanvasGroup canvasGroup;

    private RectTransform rt;
    private Vector2 targetPosition;
    private Coroutine activeRoutine;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        targetPosition = rt.anchoredPosition;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(AnimateOpen());
    }

    public void Close()
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(AnimateClose());
    }

    IEnumerator AnimateOpen()
    {
        float elapsed = 0f;

        rt.localScale = Vector3.one * startScale;
        rt.anchoredPosition = targetPosition + startOffset;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        while (elapsed < openDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);
            float curved = openCurve.Evaluate(t);

            float scale;
            if (t < 0.7f)
                scale = Mathf.Lerp(startScale, overshootScale, curved / 0.7f);
            else
                scale = Mathf.Lerp(overshootScale, 1f, (curved - 0.7f) / 0.3f);

            rt.localScale = Vector3.one * scale;
            rt.anchoredPosition = Vector2.Lerp(targetPosition + startOffset, targetPosition, curved);
            canvasGroup.alpha = curved;

            yield return null;
        }

        rt.localScale = Vector3.one;
        rt.anchoredPosition = targetPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    IEnumerator AnimateClose()
    {
        float elapsed = 0f;
        canvasGroup.interactable = false;

        Vector3 startScaleVec = rt.localScale;
        Vector2 startPos = rt.anchoredPosition;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < closeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / closeDuration);
            float curved = closeCurve.Evaluate(t);

            rt.localScale = Vector3.Lerp(startScaleVec, Vector3.one * startScale, curved);
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPosition + startOffset, curved);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, curved);

            yield return null;
        }

        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}