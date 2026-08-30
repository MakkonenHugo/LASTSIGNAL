using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class CameraOverlayHUD : MonoBehaviour
{
    public Color overlayColor = new Color(1f, 1f, 1f, 0.85f);
    public Color recColor = new Color(1f, 0.15f, 0.15f);
    public int fontSize = 22;
    public TMP_FontAsset font;

    public bool showRecIndicator = true;
    public bool showTimer = true;
    public bool showBatteryIcon = true;
    public Color batteryOutlineColor = new Color(1f, 1f, 1f, 0.85f);
    public Color batteryFillColor = new Color(0.15f, 0.15f, 0.15f, 0.95f);
    public bool showCornerBrackets = true;
    public bool showVignetteAndScanlines = true;

    public float recBlinkSpeed = 1.5f;

    [Range(0f, 1f)] public float batteryLevel = 0.72f;

    public float bracketLength = 30f;
    public float bracketThickness = 3f;
    public float bracketMargin = 16f;

    public float hudMargin = 52f;

    [Range(0f, 1f)] public float vignetteStrength = 0.55f;
    [Range(0f, 0.15f)] public float scanlineAlpha = 0.05f;
    public int scanlineCount = 120;

    private TMP_Text recText;
    private Image recDot;
    private TMP_Text timerText;
    private Image batteryOutline;
    private Image batteryFill;

    private float elapsedTime = 0f;

    void Awake()
    {
        BuildRecIndicator();
        BuildTimer();
        BuildBatteryIcon();
        BuildCornerBrackets();
        BuildVignetteAndScanlines();
    }

    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;

        if (showRecIndicator && recDot != null)
        {
            float pulse = (Mathf.Sin(Time.unscaledTime * recBlinkSpeed * Mathf.PI) + 1f) * 0.5f;
            Color c = recColor;
            c.a = Mathf.Lerp(0.3f, 1f, pulse);
            recDot.color = c;
        }

        if (showTimer && timerText != null)
        {
            int totalSeconds = Mathf.FloorToInt(elapsedTime);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        if (showBatteryIcon && batteryFill != null)
        {
            batteryFill.fillAmount = batteryLevel;
        }
    }

    RectTransform CreateUIObject(string name, RectTransform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        return rt;
    }

    TMP_Text CreateText(RectTransform parent, string name, string content, TextAlignmentOptions alignment)
    {
        RectTransform rt = CreateUIObject(name, parent);
        TextMeshProUGUI txt = rt.gameObject.AddComponent<TextMeshProUGUI>();
        txt.text = content;
        txt.fontSize = fontSize;
        txt.color = overlayColor;
        txt.alignment = alignment;
        txt.raycastTarget = false;

        if (font != null)
            txt.font = font;

        return txt;
    }

    void BuildRecIndicator()
    {
        if (!showRecIndicator) return;

        RectTransform container = CreateUIObject("REC_Indicator", GetComponent<RectTransform>());
        container.anchorMin = new Vector2(0f, 1f);
        container.anchorMax = new Vector2(0f, 1f);
        container.pivot = new Vector2(0f, 1f);
        container.anchoredPosition = new Vector2(hudMargin, -hudMargin);
        container.sizeDelta = new Vector2(160f, 30f);

        RectTransform dotRt = CreateUIObject("REC_Dot", container);
        dotRt.anchorMin = new Vector2(0f, 0.5f);
        dotRt.anchorMax = new Vector2(0f, 0.5f);
        dotRt.pivot = new Vector2(0f, 0.5f);
        dotRt.anchoredPosition = new Vector2(0f, 0f);
        dotRt.sizeDelta = new Vector2(14f, 14f);

        recDot = dotRt.gameObject.AddComponent<Image>();
        recDot.color = recColor;
        recDot.raycastTarget = false;

        recText = CreateText(container, "REC_Text", "REC", TextAlignmentOptions.MidlineLeft);
        RectTransform textRt = recText.rectTransform;
        textRt.anchorMin = new Vector2(0f, 0.5f);
        textRt.anchorMax = new Vector2(0f, 0.5f);
        textRt.pivot = new Vector2(0f, 0.5f);
        textRt.anchoredPosition = new Vector2(24f, 0f);
        textRt.sizeDelta = new Vector2(100f, 30f);
    }

    void BuildTimer()
    {
        if (!showTimer) return;

        RectTransform container = CreateUIObject("Timer_Container", GetComponent<RectTransform>());
        container.anchorMin = new Vector2(0f, 1f);
        container.anchorMax = new Vector2(0f, 1f);
        container.pivot = new Vector2(0f, 1f);
        container.anchoredPosition = new Vector2(hudMargin, -(hudMargin + 34f));
        container.sizeDelta = new Vector2(200f, 30f);

        timerText = CreateText(container, "Timer_Text", "00:00:00", TextAlignmentOptions.MidlineLeft);
        RectTransform textRt = timerText.rectTransform;
        textRt.anchorMin = new Vector2(0f, 0.5f);
        textRt.anchorMax = new Vector2(0f, 0.5f);
        textRt.pivot = new Vector2(0f, 0.5f);
        textRt.anchoredPosition = Vector2.zero;
        textRt.sizeDelta = container.sizeDelta;
    }

    void BuildBatteryIcon()
    {
        if (!showBatteryIcon) return;

        RectTransform container = CreateUIObject("Battery_Container", GetComponent<RectTransform>());
        container.anchorMin = new Vector2(1f, 1f);
        container.anchorMax = new Vector2(1f, 1f);
        container.pivot = new Vector2(1f, 1f);
        container.anchoredPosition = new Vector2(-hudMargin, -hudMargin);
        container.sizeDelta = new Vector2(50f, 24f);

        RectTransform outlineRt = CreateUIObject("Battery_Outline", container);
        outlineRt.anchorMin = new Vector2(0f, 0.5f);
        outlineRt.anchorMax = new Vector2(0f, 0.5f);
        outlineRt.pivot = new Vector2(0f, 0.5f);
        outlineRt.anchoredPosition = Vector2.zero;
        outlineRt.sizeDelta = new Vector2(42f, 20f);

        batteryOutline = outlineRt.gameObject.AddComponent<Image>();
        batteryOutline.color = batteryOutlineColor;
        batteryOutline.raycastTarget = false;
        batteryOutline.type = Image.Type.Sliced;
        batteryOutline.sprite = BuildSolidSprite();

        RectTransform tipRt = CreateUIObject("Battery_Tip", container);
        tipRt.anchorMin = new Vector2(0f, 0.5f);
        tipRt.anchorMax = new Vector2(0f, 0.5f);
        tipRt.pivot = new Vector2(0f, 0.5f);
        tipRt.anchoredPosition = new Vector2(42f, 0f);
        tipRt.sizeDelta = new Vector2(4f, 10f);

        Image tipImg = tipRt.gameObject.AddComponent<Image>();
        tipImg.color = batteryOutlineColor;
        tipImg.sprite = BuildSolidSprite();
        tipImg.raycastTarget = false;

        RectTransform fillRt = CreateUIObject("Battery_Fill", outlineRt);
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(1f, 1f);
        fillRt.offsetMin = new Vector2(3f, 3f);
        fillRt.offsetMax = new Vector2(-3f, -3f);
        fillRt.pivot = new Vector2(0.5f, 0.5f);

        batteryFill = fillRt.gameObject.AddComponent<Image>();
        batteryFill.color = batteryFillColor;
        batteryFill.raycastTarget = false;
        batteryFill.sprite = BuildSolidSprite();
        batteryFill.type = Image.Type.Filled;
        batteryFill.fillMethod = Image.FillMethod.Horizontal;
        batteryFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        batteryFill.fillAmount = batteryLevel;
    }

    void BuildCornerBrackets()
    {
        if (!showCornerBrackets) return;

        RectTransform root = GetComponent<RectTransform>();

        BuildOneBracket(root, new Vector2(0f, 0f), new Vector2(bracketMargin, bracketMargin), true, true);
        BuildOneBracket(root, new Vector2(1f, 0f), new Vector2(-bracketMargin, bracketMargin), false, true);
        BuildOneBracket(root, new Vector2(0f, 1f), new Vector2(bracketMargin, -bracketMargin), true, false);
        BuildOneBracket(root, new Vector2(1f, 1f), new Vector2(-bracketMargin, -bracketMargin), false, false);
    }

    void BuildOneBracket(RectTransform root, Vector2 anchor, Vector2 offset, bool faceRight, bool faceUp)
    {
        RectTransform container = CreateUIObject("Bracket_" + anchor.x + "_" + anchor.y, root);
        container.anchorMin = anchor;
        container.anchorMax = anchor;
        container.pivot = anchor;
        container.anchoredPosition = offset;
        container.sizeDelta = new Vector2(bracketLength, bracketLength);

        RectTransform horizontal = CreateUIObject("H", container);
        horizontal.anchorMin = anchor;
        horizontal.anchorMax = anchor;
        horizontal.pivot = new Vector2(faceRight ? 0f : 1f, faceUp ? 0f : 1f);
        horizontal.anchoredPosition = Vector2.zero;
        horizontal.sizeDelta = new Vector2(bracketLength, bracketThickness);

        Image hImg = horizontal.gameObject.AddComponent<Image>();
        hImg.color = overlayColor;
        hImg.raycastTarget = false;

        RectTransform vertical = CreateUIObject("V", container);
        vertical.anchorMin = anchor;
        vertical.anchorMax = anchor;
        vertical.pivot = new Vector2(faceRight ? 0f : 1f, faceUp ? 0f : 1f);
        vertical.anchoredPosition = Vector2.zero;
        vertical.sizeDelta = new Vector2(bracketThickness, bracketLength);

        Image vImg = vertical.gameObject.AddComponent<Image>();
        vImg.color = overlayColor;
        vImg.raycastTarget = false;
    }

    void BuildVignetteAndScanlines()
    {
        if (!showVignetteAndScanlines) return;

        RectTransform root = GetComponent<RectTransform>();

        RectTransform vignetteRt = CreateUIObject("Vignette", root);
        vignetteRt.anchorMin = Vector2.zero;
        vignetteRt.anchorMax = Vector2.one;
        vignetteRt.offsetMin = Vector2.zero;
        vignetteRt.offsetMax = Vector2.zero;
        vignetteRt.SetAsFirstSibling();

        Image vignetteImg = vignetteRt.gameObject.AddComponent<Image>();
        vignetteImg.raycastTarget = false;
        vignetteImg.color = Color.white;
        vignetteImg.sprite = BuildVignetteSprite();
        vignetteImg.type = Image.Type.Simple;

        RectTransform scanlineRt = CreateUIObject("Scanlines", root);
        scanlineRt.anchorMin = Vector2.zero;
        scanlineRt.anchorMax = Vector2.one;
        scanlineRt.offsetMin = Vector2.zero;
        scanlineRt.offsetMax = Vector2.zero;

        Image scanlineImg = scanlineRt.gameObject.AddComponent<Image>();
        scanlineImg.raycastTarget = false;
        scanlineImg.color = Color.white;
        scanlineImg.sprite = BuildScanlineSprite();
        scanlineImg.type = Image.Type.Tiled;
    }

    Sprite BuildSolidSprite()
    {
        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));
    }

    Sprite BuildVignetteSprite()
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float maxDist = center.magnitude;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                float alpha = Mathf.Clamp01(Mathf.Pow(dist, 2.2f)) * vignetteStrength;
                tex.SetPixel(x, y, new Color(0f, 0f, 0f, alpha));
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    Sprite BuildScanlineSprite()
    {
        int height = Mathf.Max(2, scanlineCount);
        Texture2D tex = new Texture2D(1, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        for (int y = 0; y < height; y++)
        {
            float alpha = (y % 2 == 0) ? scanlineAlpha : 0f;
            tex.SetPixel(0, y, new Color(0f, 0f, 0f, alpha));
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, height), new Vector2(0.5f, 0.5f), height);
    }
}