using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateFeature : ScriptableRendererFeature
{
    public Shader pixelateShader;
    [Range(16, 640)] public int pixelWidth = 320;
    [Range(9, 360)] public int pixelHeight = 180;
    public bool effectEnabled = true;

    private Material pixelateMaterial;
    private PixelatePass pixelatePass;
    public static PixelateFeature ActiveInstance { get; private set; }
    public override void Create()
    {
        ActiveInstance = this;

        if (pixelateShader == null)
        {
            pixelateShader = Shader.Find("Hidden/PixelateBlit");
        }

        if (pixelateShader != null)
        {
            pixelateMaterial = CoreUtils.CreateEngineMaterial(pixelateShader);
        }

        pixelatePass = new PixelatePass(pixelateMaterial);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pixelateMaterial == null || !effectEnabled)
            return;

        pixelatePass.Setup(pixelWidth, pixelHeight);
        renderer.EnqueuePass(pixelatePass);
    }

    public void SetResolution(int width, int height)
    {
        pixelWidth = Mathf.Clamp(width, 16, 640);
        pixelHeight = Mathf.Clamp(height, 9, 360);
    }
    public void SetEnabled(bool enabled)
    {
        effectEnabled = enabled;
    }
    protected override void Dispose(bool disposing)
    {
        if (ActiveInstance == this)
            ActiveInstance = null;

        if (pixelateMaterial != null)
        {
            CoreUtils.Destroy(pixelateMaterial);
        }
    }
}