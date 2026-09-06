using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class PixelatePass : ScriptableRenderPass
{
    private Material pixelateMaterial;
    private int pixelWidth;
    private int pixelHeight;

    public PixelatePass(Material material)
    {
        pixelateMaterial = material;
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public void Setup(int width, int height)
    {
        pixelWidth = width;
        pixelHeight = height;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
    {
        if (pixelateMaterial == null)
            return;

        UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();

        if (resourceData.isActiveTargetBackBuffer)
            return;

        TextureHandle sourceTexture = resourceData.activeColorTexture;

        TextureDesc lowResDesc = sourceTexture.GetDescriptor(renderGraph);
        lowResDesc.name = "_PixelateTempTexture";
        lowResDesc.width = pixelWidth;
        lowResDesc.height = pixelHeight;
        lowResDesc.filterMode = FilterMode.Point;
        lowResDesc.clearBuffer = false;

        TextureHandle lowResTexture = renderGraph.CreateTexture(lowResDesc);

        RenderGraphUtils.BlitMaterialParameters downscaleParams = new(sourceTexture, lowResTexture, pixelateMaterial, 0);
        renderGraph.AddBlitPass(downscaleParams, "Pixelate Downscale");

        RenderGraphUtils.BlitMaterialParameters upscaleParams = new(lowResTexture, sourceTexture, pixelateMaterial, 0);
        renderGraph.AddBlitPass(upscaleParams, "Pixelate Upscale");
    }
}