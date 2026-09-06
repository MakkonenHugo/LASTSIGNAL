using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class StarfieldBackground : MonoBehaviour
{
    public int starCount = 90;
    public float minStarSize = 1.5f;
    public float maxStarSize = 3.5f;
    public Color starColor = new Color(0.75f, 0.85f, 1f);

    public float driftSpeed = 4f;
    public float twinkleSpeedMin = 0.5f;
    public float twinkleSpeedMax = 1.8f;
    public float twinkleMinAlpha = 0.15f;
    public float twinkleMaxAlpha = 1f;
    public int shootingStarChanceRolls = 1;
    [Range(0f, 1f)] public float shootingStarChancePerSecond = 0.15f;
    public float shootingStarSpeed = 700f;
    public float shootingStarLength = 90f;

    private RectTransform container;
    private List<StarData> stars = new List<StarData>();
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
        BuildStars();
        BuildShootingStar();
    }

    void BuildStars()
    {
        Rect bounds = container.rect;

        for (int i = 0; i < starCount; i++)
        {
            GameObject go = new GameObject("Star_" + i, typeof(RectTransform));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(container, false);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            float size = Random.Range(minStarSize, maxStarSize);
            rt.sizeDelta = new Vector2(size, size);

            Vector2 pos = new Vector2(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax)
            );
            rt.anchoredPosition = pos;

            Image img = go.AddComponent<Image>();
            img.color = starColor;
            img.raycastTarget = false;

            StarData data = new StarData
            {
                rt = rt,
                img = img,
                baseOffset = pos,
                twinkleSpeed = Random.Range(twinkleSpeedMin, twinkleSpeedMax),
                twinklePhase = Random.Range(0f, Mathf.PI * 2f)
            };

            stars.Add(data);
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

        go.SetActive(true);
    }

    void Update()
    {
        float t = Time.unscaledTime;
        Rect bounds = container.rect;

        for (int i = 0; i < stars.Count; i++)
        {
            StarData s = stars[i];

            float alpha = Mathf.Lerp(twinkleMinAlpha, twinkleMaxAlpha, (Mathf.Sin(t * s.twinkleSpeed + s.twinklePhase) + 1f) * 0.5f);
            Color c = starColor;
            c.a = alpha;
            s.img.color = c;

            float driftX = Mathf.Sin(t * 0.15f + s.twinklePhase) * driftSpeed;
            float driftY = Mathf.Cos(t * 0.1f + s.twinklePhase) * driftSpeed;
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
            duration = shootingStarLength / shootingStarSpeed * 10f,
            start = start,
            end = end
        };
    }
}