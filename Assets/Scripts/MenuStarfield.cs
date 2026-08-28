using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class MenuStarfield : MonoBehaviour
{
    [System.Serializable]
    public class StarLayer
    {
        public int count = 40;
        public float minSize = 1f;
        public float maxSize = 2f;
        public float driftSpeed = 3f;
        public float twinkleSpeedMin = 0.3f;
        public float twinkleSpeedMax = 1.2f;
        public float minAlpha = 0.2f;
        public float maxAlpha = 0.9f;
        public Color color = new Color(0.8f, 0.85f, 1f);
    }

    public StarLayer backLayer = new StarLayer { count = 50, minSize = 0.8f, maxSize = 1.5f, driftSpeed = 1.5f, minAlpha = 0.1f, maxAlpha = 0.5f };
    public StarLayer midLayer = new StarLayer { count = 35, minSize = 1.3f, maxSize = 2.3f, driftSpeed = 3f, minAlpha = 0.2f, maxAlpha = 0.75f };
    public StarLayer frontLayer = new StarLayer { count = 20, minSize = 2f, maxSize = 3.2f, driftSpeed = 5f, minAlpha = 0.3f, maxAlpha = 1f };

    [Range(0f, 1f)] public float shootingStarChancePerSecond = 0.08f;
    public float shootingStarDuration = 1f;
    public float shootingStarLength = 110f;

    private RectTransform container;
    private List<StarData> allStars = new List<StarData>();
    private ShootingStarData activeShootingStar;
    private RectTransform shootingStarRt;
    private Image shootingStarImg;

    private class StarData
    {
        public RectTransform rt;
        public Image img;
        public Vector2 baseOffset;
        public float twinkleSpeed;
        public float twinklePhase;
        public float driftSpeed;
        public float minAlpha;
        public float maxAlpha;
        public Color baseColor;
    }

    private class ShootingStarData
    {
        public float elapsed;
        public float duration;
        public Vector2 start;
        public Vector2 end;
    }

    void Awake()
    {
        container = GetComponent<RectTransform>();

        BuildLayer(backLayer, "Back");
        BuildLayer(midLayer, "Mid");
        BuildLayer(frontLayer, "Front");

        BuildShootingStar();
    }

    void BuildLayer(StarLayer layer, string layerName)
    {
        Rect bounds = container.rect;

        for (int i = 0; i < layer.count; i++)
        {
            GameObject go = new GameObject("Star_" + layerName + "_" + i, typeof(RectTransform));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(container, false);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            float size = Random.Range(layer.minSize, layer.maxSize);
            rt.sizeDelta = new Vector2(size, size);

            Vector2 pos = new Vector2(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax)
            );
            rt.anchoredPosition = pos;

            Image img = go.AddComponent<Image>();
            img.color = layer.color;
            img.raycastTarget = false;

            StarData data = new StarData
            {
                rt = rt,
                img = img,
                baseOffset = pos,
                twinkleSpeed = Random.Range(layer.twinkleSpeedMin, layer.twinkleSpeedMax),
                twinklePhase = Random.Range(0f, Mathf.PI * 2f),
                driftSpeed = layer.driftSpeed,
                minAlpha = layer.minAlpha,
                maxAlpha = layer.maxAlpha,
                baseColor = layer.color
            };

            allStars.Add(data);
        }
    }

    void BuildShootingStar()
    {
        GameObject go = new GameObject("ShootingStar", typeof(RectTransform));
        shootingStarRt = go.GetComponent<RectTransform>();
        shootingStarRt.SetParent(container, false);
        shootingStarRt.anchorMin = new Vector2(0.5f, 0.5f);
        shootingStarRt.anchorMax = new Vector2(0.5f, 0.5f);
        shootingStarRt.pivot = new Vector2(0f, 0.5f);
        shootingStarRt.sizeDelta = new Vector2(shootingStarLength, 2f);

        shootingStarImg = go.AddComponent<Image>();
        shootingStarImg.color = new Color(1f, 1f, 1f, 0f);
        shootingStarImg.raycastTarget = false;
    }

    void Update()
    {
        float t = Time.unscaledTime;
        Rect bounds = container.rect;

        for (int i = 0; i < allStars.Count; i++)
        {
            StarData s = allStars[i];

            float alpha = Mathf.Lerp(s.minAlpha, s.maxAlpha, (Mathf.Sin(t * s.twinkleSpeed + s.twinklePhase) + 1f) * 0.5f);
            Color c = s.baseColor;
            c.a = alpha;
            s.img.color = c;

            float driftX = Mathf.Sin(t * 0.08f + s.twinklePhase) * s.driftSpeed;
            float driftY = Mathf.Cos(t * 0.05f + s.twinklePhase) * s.driftSpeed * 0.6f;
            s.rt.anchoredPosition = s.baseOffset + new Vector2(driftX, driftY);
        }

        UpdateShootingStar(t, bounds);
    }

    void UpdateShootingStar(float t, Rect bounds)
    {
        if (activeShootingStar == null)
        {
            if (Random.value < shootingStarChancePerSecond * Time.unscaledDeltaTime)
                SpawnShootingStar(bounds);
            return;
        }

        activeShootingStar.elapsed += Time.unscaledDeltaTime;
        float pct = activeShootingStar.elapsed / activeShootingStar.duration;

        if (pct >= 1f)
        {
            activeShootingStar = null;
            shootingStarImg.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        Vector2 pos = Vector2.Lerp(activeShootingStar.start, activeShootingStar.end, pct);
        shootingStarRt.anchoredPosition = pos;

        Vector2 dir = (activeShootingStar.end - activeShootingStar.start).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        shootingStarRt.localRotation = Quaternion.Euler(0, 0, angle);

        float fade = pct < 0.15f ? pct / 0.15f : (1f - pct) / 0.85f;
        shootingStarImg.color = new Color(1f, 1f, 1f, Mathf.Clamp01(fade) * 0.9f);
    }

    void SpawnShootingStar(Rect bounds)
    {
        Vector2 start = new Vector2(Random.Range(bounds.xMin, bounds.xMax * 0.3f), Random.Range(bounds.yMax * 0.4f, bounds.yMax));
        Vector2 dir = new Vector2(1f, -0.6f).normalized;
        Vector2 end = start + dir * (bounds.width * 0.8f);

        activeShootingStar = new ShootingStarData
        {
            elapsed = 0f,
            duration = shootingStarDuration,
            start = start,
            end = end
        };
    }
}