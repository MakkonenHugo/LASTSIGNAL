using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class GlitchButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color buttonBaseColor = new Color(0.16f, 0.16f, 0.18f);

    public float glitchDuration = 0.55f;
    public Vector2 stepIntervalRange = new Vector2(0.05f, 0.14f);
    public AnimationCurve intensityOverTime = AnimationCurve.EaseInOut(0, 0.7f, 1, 0.3f);

    public float positionJitter = 2.5f;
    public float scaleJitter = 0.015f;
    [Range(0f, 45f)] public float rotationJitter = 0.8f;

    public bool useRgbSplit = true;
    public Color channelColorA = new Color(1f, 0.15f, 0.35f);
    public Color channelColorB = new Color(0.1f, 0.9f, 1f);
    public Color channelColorC = new Color(0.7f, 1f, 0.2f);
    [Range(0f, 20f)] public float rgbSplitOffset = 3f;
    [Range(0f, 1f)] public float ghostAlpha = 0.5f;

    [Range(0f, 1f)] public float colorFlickerChance = 0.25f;
    public Color solidFlickerColor = new Color(0.6f, 0.9f, 1f);

    public bool useEdgeStreaks = true;
    public int maxStreaks = 3;
    [Range(0f, 1f)] public float streakSpawnChance = 0.35f;
    public float streakThickness = 2f;
    public float streakLengthMin = 8f;
    public float streakLengthMax = 30f;
    public Color streakColor = new Color(0.85f, 0.95f, 1f, 0.8f);

    public bool idleGlitchEnabled = false;
    public Vector2 idleIntervalRange = new Vector2(3f, 8f);
    public float idleGlitchDuration = 0.2f;

    private RectTransform rectTransform;
    private Image image;
    private TMP_Text text;

    private Vector2 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Color originalTextColor;

    private Coroutine glitchRoutine;
    private Coroutine idleRoutine;

    private RectTransform ghostAImage, ghostBImage, ghostCImage;
    private Image ghostAImageComp, ghostBImageComp, ghostCImageComp;
    private TMP_Text ghostAText, ghostBText, ghostCText;
    private bool ghostsBuilt = false;

    private RectTransform streakContainer;
    private List<RectTransform> streakPool = new List<RectTransform>();
    private List<Image> streakPoolImages = new List<Image>();

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        text = GetComponentInChildren<TMP_Text>();

        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;

        if (image != null)
            image.color = buttonBaseColor;

        if (text != null)
            originalTextColor = text.color;
    }

    void OnEnable()
    {
        if (idleGlitchEnabled)
            idleRoutine = StartCoroutine(IdleGlitchLoop());
    }

    void OnDisable()
    {
        if (idleRoutine != null) StopCoroutine(idleRoutine);
        if (glitchRoutine != null) StopCoroutine(glitchRoutine);
        ResetVisuals();
        SetGhostsActive(false);
        ClearStreaks();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (idleRoutine != null) { StopCoroutine(idleRoutine); idleRoutine = null; }
        if (glitchRoutine != null) StopCoroutine(glitchRoutine);

        EnsureGhostsBuilt();
        EnsureStreakPoolBuilt();
        glitchRoutine = StartCoroutine(Glitch(glitchDuration, loop: true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (glitchRoutine != null)
            StopCoroutine(glitchRoutine);

        ResetVisuals();
        SetGhostsActive(false);
        ClearStreaks();

        if (idleGlitchEnabled)
            idleRoutine = StartCoroutine(IdleGlitchLoop());
    }

    IEnumerator IdleGlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(idleIntervalRange.x, idleIntervalRange.y));

            EnsureGhostsBuilt();
            EnsureStreakPoolBuilt();
            yield return Glitch(idleGlitchDuration, loop: false);
            ResetVisuals();
            SetGhostsActive(false);
            ClearStreaks();
        }
    }

    IEnumerator Glitch(float duration, bool loop)
    {
        SetGhostsActive(useRgbSplit);
        float elapsed = 0f;

        while (loop || elapsed < duration)
        {
            float t = Mathf.Repeat(elapsed, duration) / duration;
            float intensity = intensityOverTime.Evaluate(t);

            ApplyGlitchStep(intensity);

            float wait = Random.Range(stepIntervalRange.x, stepIntervalRange.y);
            yield return new WaitForSecondsRealtime(wait);
            elapsed += wait;
        }

        ResetVisuals();
        SetGhostsActive(false);
        ClearStreaks();
    }

    void ApplyGlitchStep(float intensity)
    {
        Vector2 offset = new Vector2(
            Random.Range(-positionJitter, positionJitter),
            Random.Range(-positionJitter, positionJitter)
        ) * intensity;
        rectTransform.anchoredPosition = originalPosition + offset;

        rectTransform.localScale = originalScale * (1f + Random.Range(-scaleJitter, scaleJitter) * intensity);
        rectTransform.localRotation = originalRotation * Quaternion.Euler(0, 0, Random.Range(-rotationJitter, rotationJitter) * intensity);

        bool flicker = Random.value < colorFlickerChance * intensity;
        if (image != null)
            image.color = flicker ? solidFlickerColor : buttonBaseColor;
        if (text != null)
            text.color = flicker ? solidFlickerColor : originalTextColor;

        if (useRgbSplit && ghostsBuilt)
        {
            Vector2 dirA = Random.insideUnitCircle.normalized * rgbSplitOffset * intensity;
            Vector2 dirB = Random.insideUnitCircle.normalized * rgbSplitOffset * intensity;
            Vector2 dirC = Random.insideUnitCircle.normalized * rgbSplitOffset * intensity;

            PositionGhost(ghostAImage, offset + dirA);
            PositionGhost(ghostBImage, offset + dirB);
            PositionGhost(ghostCImage, offset + dirC);

            float a = ghostAlpha * intensity;
            SetGhostColor(ghostAImageComp, ghostAText, channelColorA, a);
            SetGhostColor(ghostBImageComp, ghostBText, channelColorB, a);
            SetGhostColor(ghostCImageComp, ghostCText, channelColorC, a);

            ghostAImage.gameObject.SetActive(Random.value > 0.1f);
            ghostBImage.gameObject.SetActive(Random.value > 0.1f);
            ghostCImage.gameObject.SetActive(Random.value > 0.1f);
        }

        if (useEdgeStreaks)
            UpdateStreaks(intensity);
    }

    void PositionGhost(RectTransform ghost, Vector2 offset)
    {
        if (ghost == null) return;
        ghost.anchoredPosition = originalPosition + offset;
    }

    void SetGhostColor(Image img, TMP_Text txt, Color c, float alpha)
    {
        if (img != null)
        {
            Color col = c;
            col.a = alpha;
            img.color = col;
        }
        if (txt != null)
        {
            Color col = c;
            col.a = alpha;
            txt.color = col;
        }
    }

    void ResetVisuals()
    {
        if (rectTransform == null) return;

        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;

        if (image != null) image.color = buttonBaseColor;
        if (text != null) text.color = originalTextColor;
    }

    void EnsureGhostsBuilt()
    {
        if (ghostsBuilt || !useRgbSplit) return;

        ghostAImage = BuildGhost("Glitch_Ghost_A", out ghostAImageComp, out ghostAText);
        ghostBImage = BuildGhost("Glitch_Ghost_B", out ghostBImageComp, out ghostBText);
        ghostCImage = BuildGhost("Glitch_Ghost_C", out ghostCImageComp, out ghostCText);

        ghostsBuilt = true;
        SetGhostsActive(false);
    }

    RectTransform BuildGhost(string name, out Image imgComp, out TMP_Text txtComp)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(rectTransform.parent, worldPositionStays: false);
        rt.SetSiblingIndex(rectTransform.GetSiblingIndex());
        rt.anchorMin = rectTransform.anchorMin;
        rt.anchorMax = rectTransform.anchorMax;
        rt.pivot = rectTransform.pivot;
        rt.sizeDelta = rectTransform.sizeDelta;
        rt.anchoredPosition = originalPosition;
        rt.localScale = originalScale;

        imgComp = null;
        txtComp = null;

        if (image != null)
        {
            imgComp = go.AddComponent<Image>();
            imgComp.sprite = image.sprite;
            imgComp.type = image.type;
            imgComp.raycastTarget = false;
            imgComp.material = image.material;
        }

        if (text != null)
        {
            GameObject textGo = new GameObject("GhostText", typeof(RectTransform));
            RectTransform textRt = textGo.GetComponent<RectTransform>();
            textRt.SetParent(rt, false);
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            txtComp = textGo.AddComponent<TextMeshProUGUI>();
            txtComp.text = text.text;
            txtComp.font = text.font;
            txtComp.fontSize = text.fontSize;
            txtComp.alignment = text.alignment;
            txtComp.raycastTarget = false;
        }

        CanvasGroup cg = go.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        return rt;
    }

    void SetGhostsActive(bool active)
    {
        if (!ghostsBuilt) return;
        ghostAImage.gameObject.SetActive(active);
        ghostBImage.gameObject.SetActive(active);
        ghostCImage.gameObject.SetActive(active);
    }

    void EnsureStreakPoolBuilt()
    {
        if (streakContainer != null) return;

        GameObject containerGo = new GameObject("Glitch_Streaks", typeof(RectTransform));
        streakContainer = containerGo.GetComponent<RectTransform>();
        streakContainer.SetParent(rectTransform, false);
        streakContainer.anchorMin = Vector2.zero;
        streakContainer.anchorMax = Vector2.one;
        streakContainer.offsetMin = Vector2.zero;
        streakContainer.offsetMax = Vector2.zero;

        for (int i = 0; i < maxStreaks; i++)
        {
            GameObject streakGo = new GameObject("Streak_" + i, typeof(RectTransform));
            RectTransform streakRt = streakGo.GetComponent<RectTransform>();
            streakRt.SetParent(streakContainer, false);
            streakRt.pivot = new Vector2(0f, 0.5f);

            Image streakImg = streakGo.AddComponent<Image>();
            streakImg.raycastTarget = false;
            streakImg.color = streakColor;

            streakGo.SetActive(false);

            streakPool.Add(streakRt);
            streakPoolImages.Add(streakImg);
        }
    }

    void UpdateStreaks(float intensity)
    {
        if (streakPool.Count == 0) return;

        Rect bounds = rectTransform.rect;

        for (int i = 0; i < streakPool.Count; i++)
        {
            bool spawn = Random.value < streakSpawnChance * intensity;
            streakPool[i].gameObject.SetActive(spawn);

            if (!spawn) continue;

            bool leftEdge = Random.value > 0.5f;
            float y = Random.Range(bounds.yMin, bounds.yMax);
            float length = Random.Range(streakLengthMin, streakLengthMax);

            float x = leftEdge ? bounds.xMin : bounds.xMax - length;

            streakPool[i].anchoredPosition = new Vector2(x, y);
            streakPool[i].sizeDelta = new Vector2(length, streakThickness);

            Color c = streakColor;
            c.a = streakColor.a * intensity;
            streakPoolImages[i].color = c;
        }
    }

    void ClearStreaks()
    {
        for (int i = 0; i < streakPool.Count; i++)
            streakPool[i].gameObject.SetActive(false);
    }
}