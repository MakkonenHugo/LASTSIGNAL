using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Crosshair : MonoBehaviour
{
    public Color crosshairColor = new Color(1f, 1f, 1f, 0.8f);
    public float size = 6f;
    public float thickness = 2f;
    public float gap = 3f;
    public bool showCenterDot = true;
    public float centerDotSize = 2f;

    void Awake()
    {
        Build();
    }

    void Build()
    {
        RectTransform root = GetComponent<RectTransform>();
        root.anchorMin = new Vector2(0.5f, 0.5f);
        root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0.5f, 0.5f);
        root.anchoredPosition = Vector2.zero;

        CreateLine(root, new Vector2(0f, gap + size * 0.5f), new Vector2(thickness, size));
        CreateLine(root, new Vector2(0f, -(gap + size * 0.5f)), new Vector2(thickness, size));
        CreateLine(root, new Vector2(gap + size * 0.5f, 0f), new Vector2(size, thickness));
        CreateLine(root, new Vector2(-(gap + size * 0.5f), 0f), new Vector2(size, thickness));

        if (showCenterDot)
        {
            CreateLine(root, Vector2.zero, new Vector2(centerDotSize, centerDotSize));
        }
    }

    void CreateLine(RectTransform parent, Vector2 offset, Vector2 lineSize)
    {
        GameObject go = new GameObject("CrosshairPart", typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = offset;
        rt.sizeDelta = lineSize;

        Image img = go.AddComponent<Image>();
        img.color = crosshairColor;
        img.raycastTarget = false;
    }
}