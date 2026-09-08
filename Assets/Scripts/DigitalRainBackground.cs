using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class DigitalRainBackground : MonoBehaviour
{
    public Color rainColor = new Color(0.4f, 1f, 0.5f);
    public int columnCount = 24;
    public int charsPerColumn = 18;
    public float minFallSpeed = 80f;
    public float maxFallSpeed = 220f;
    public int fontSize = 20;
    public TMP_FontAsset font;
    [Range(0f, 1f)] public float backgroundDim = 0.85f;
    [Range(0f, 1f)] public float rainOpacity = 0.35f;

    private RectTransform container;
    private List<RainColumn> columns = new List<RainColumn>();
    private static readonly char[] CharacterSet =
        "01ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*<>/\\".ToCharArray();

    private class RainColumn
    {
        public TMP_Text text;
        public RectTransform rt;
        public float speed;
        public float yPos;
        public float columnHeight;
    }

    void Awake()
    {
        container = GetComponent<RectTransform>();
        transform.SetAsFirstSibling();
        BuildDimOverlay();
        BuildColumns();
    }

    void BuildDimOverlay()
    {
        GameObject go = new GameObject("DimOverlay", typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(container, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = go.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, backgroundDim);
        img.raycastTarget = false;
    }

    void BuildColumns()
    {
        Rect bounds = container.rect;
        float columnWidth = bounds.width / columnCount;
        float columnHeight = bounds.height + (charsPerColumn * fontSize);

        for (int i = 0; i < columnCount; i++)
        {
            GameObject go = new GameObject("RainColumn_" + i, typeof(RectTransform));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(container, false);
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);

            float xPos = (i * columnWidth) + (columnWidth * 0.5f);
            rt.sizeDelta = new Vector2(columnWidth, columnHeight);

            TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
            txt.text = BuildRandomString();
            txt.fontSize = fontSize;
            Color colWithAlpha = rainColor;
            colWithAlpha.a = rainOpacity;
            txt.color = colWithAlpha;
            txt.alignment = TextAlignmentOptions.Top;
            txt.raycastTarget = false;
            txt.enableWordWrapping = false;

            if (font != null)
                txt.font = font;

            float startY = Random.Range(0f, columnHeight);
            rt.anchoredPosition = new Vector2(xPos, startY);

            RainColumn col = new RainColumn
            {
                text = txt,
                rt = rt,
                speed = Random.Range(minFallSpeed, maxFallSpeed),
                yPos = startY,
                columnHeight = columnHeight
            };

            columns.Add(col);
        }
    }

    string BuildRandomString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < charsPerColumn; i++)
        {
            sb.Append(CharacterSet[Random.Range(0, CharacterSet.Length)]);
            sb.Append("\n");
        }

        return sb.ToString();
    }

    void Update()
    {
        foreach (RainColumn col in columns)
        {
            col.yPos -= col.speed * Time.unscaledDeltaTime;

            if (col.yPos < -col.columnHeight)
            {
                col.yPos = 0f;
                col.text.text = BuildRandomString();
            }

            Vector2 pos = col.rt.anchoredPosition;
            pos.y = col.yPos;
            col.rt.anchoredPosition = pos;

            if (Random.value < 0.02f)
            {
                col.text.text = BuildRandomString();
            }
        }
    }

    public void SetColor(Color newColor)
    {
        rainColor = newColor;

        foreach (RainColumn col in columns)
        {
            col.text.color = newColor;
        }
    }
}