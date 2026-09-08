using UnityEngine;
using System.Collections;

public class PixelateResetOnLoad : MonoBehaviour
{
    public bool effectEnabled = true;
    public int targetPixelWidth = 320;
    public int targetPixelHeight = 180;
    public int framesToEnforce = 10;

    void Awake()
    {
        StartCoroutine(EnforceSettings());
    }

    IEnumerator EnforceSettings()
    {
        for (int i = 0; i < framesToEnforce; i++)
        {
            if (PixelateFeature.ActiveInstance != null)
            {
                PixelateFeature.ActiveInstance.SetEnabled(effectEnabled);
                PixelateFeature.ActiveInstance.SetResolution(targetPixelWidth, targetPixelHeight);
            }

            yield return null;
        }
    }
}