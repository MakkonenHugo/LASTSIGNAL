using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class DialogueBoxVisuals : MonoBehaviour
{
    public Image background;

    public Color backgroundColor = new Color(0.07f, 0.08f, 0.1f, 0.92f);

    public float openDuration = 0.25f;
    public float closeDuration = 0.18f;
    public AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float startScaleY = 0.85f;
    public Vector2 startOffset = new Vector2(0f, -12f);

    private RectTransform rt;
    private CanvasGroup canvasGroup;
    private Vector2 targetPosition;
    private Coroutine animRoutine;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        targetPosition = rt.anchoredPosition;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (background != null)
            background.color = backgroundColor;
    }

    void OnEnable()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(AnimateOpen());
    }

    void OnDisable()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);
    }

    IEnumerator AnimateOpen()
    {
        float elapsed = 0f;

        rt.localScale = new Vector3(1f, startScaleY, 1f);
        rt.anchoredPosition = targetPosition + startOffset;
        canvasGroup.alpha = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);
            float curved = openCurve.Evaluate(t);

            rt.localScale = new Vector3(1f, Mathf.Lerp(startScaleY, 1f, curved), 1f);
            rt.anchoredPosition = Vector2.Lerp(targetPosition + startOffset, targetPosition, curved);
            canvasGroup.alpha = curved;

            yield return null;
        }

        rt.localScale = Vector3.one;
        rt.anchoredPosition = targetPosition;
        canvasGroup.alpha = 1f;
    }
}