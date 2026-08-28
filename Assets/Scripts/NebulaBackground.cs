using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NebulaBackground : MonoBehaviour
{
    public Color colorA = new Color(0.05f, 0.05f, 0.12f);
    public Color colorB = new Color(0.12f, 0.06f, 0.18f);
    public Color colorC = new Color(0.04f, 0.09f, 0.15f);

    public float cycleSpeed = 0.05f;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        float t = Time.unscaledTime * cycleSpeed;

        float phase = Mathf.Repeat(t, 3f);
        Color result;

        if (phase < 1f)
            result = Color.Lerp(colorA, colorB, phase);
        else if (phase < 2f)
            result = Color.Lerp(colorB, colorC, phase - 1f);
        else
            result = Color.Lerp(colorC, colorA, phase - 2f);

        image.color = result;
    }
}