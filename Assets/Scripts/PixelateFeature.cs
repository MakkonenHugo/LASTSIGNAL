using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateFeature : ScriptableRendererFeature
{
    public Shader pixelateShader;
    [Range(16, 640)] public int pixelWidth = 320;
    [Range(9, 360)] public int pixelHeight = 180;

    private Material pixelateMaterial;
    private PixelatePass pixelatePass;

    public override void Create()
    {
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
        if (pixelateMaterial == null)
            return;

        pixelatePass.Setup(pixelWidth, pixelHeight);
        renderer.EnqueuePass(pixelatePass);
    }

    protected override void Dispose(bool disposing)
    {
        if (pixelateMaterial != null)
        {
            CoreUtils.Destroy(pixelateMaterial);
        }
    }
}