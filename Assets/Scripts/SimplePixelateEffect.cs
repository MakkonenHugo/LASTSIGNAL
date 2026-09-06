using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class SimplePixelateEffect : MonoBehaviour
{
    public Material pixelateMaterial;
    public Shader pixelateShader;

    [Range(16, 640)] public int pixelWidth = 320;
    [Range(9, 360)] public int pixelHeight = 180;

    private RenderTexture lowResTexture;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        if (pixelateShader == null)
            pixelateShader = Shader.Find("Hidden/PixelateBlit");

        if (pixelateShader != null && pixelateMaterial == null)
            pixelateMaterial = new Material(pixelateShader);
    }

    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;

        if (lowResTexture != null)
        {
            lowResTexture.Release();
            lowResTexture = null;
        }
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera renderedCamera)
    {
        if (renderedCamera != cam || pixelateMaterial == null)
            return;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelateMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (lowResTexture == null || lowResTexture.width != pixelWidth || lowResTexture.height != pixelHeight)
        {
            if (lowResTexture != null)
                lowResTexture.Release();

            lowResTexture = new RenderTexture(pixelWidth, pixelHeight, 0);
            lowResTexture.filterMode = FilterMode.Point;
        }

        Graphics.Blit(source, lowResTexture);
        Graphics.Blit(lowResTexture, destination, pixelateMaterial);
    }
}