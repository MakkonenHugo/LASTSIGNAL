using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitPortalTransition : MonoBehaviour
{
    public string targetSceneName = "RealWorld";
    public string playerTag = "Player";

    [Header("Pixelate Collapse")]
    public int startPixelWidth = 320;
    public int startPixelHeight = 180;
    public int collapsedPixelWidth = 16;
    public int collapsedPixelHeight = 9;
    public float collapseDuration = 2.5f;
    public AnimationCurve collapseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Light Glow")]
    public Light[] portalLights;
    public float glowStartIntensity = 2f;
    public float glowPeakIntensity = 15f;

    [Header("Camera Shake")]
    public Transform cameraTransform;
    public float shakeAmount = 0.05f;

    [Header("Audio")]
    public AudioSource portalSound;

    [Header("Player Control")]
    public MouseLook playerMouseLook;
    public MonoBehaviour playerMovementScript;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.transform.root.CompareTag(playerTag))
            return;

        if (string.IsNullOrEmpty(targetSceneName))
            return;

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
            return;

        triggered = true;

        StartCoroutine(CollapseSequence());
    }

    IEnumerator CollapseSequence()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerMouseLook != null)
            playerMouseLook.LockYaw(5f);

        if (portalSound != null)
            portalSound.Play();

        Vector3 cameraOriginalPos = cameraTransform != null ? cameraTransform.localPosition : Vector3.zero;

        float elapsed = 0f;

        while (elapsed < collapseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / collapseDuration);
            float curved = collapseCurve.Evaluate(t);

            if (PixelateFeature.ActiveInstance != null)
            {
                int width = Mathf.RoundToInt(Mathf.Lerp(startPixelWidth, collapsedPixelWidth, curved));
                int height = Mathf.RoundToInt(Mathf.Lerp(startPixelHeight, collapsedPixelHeight, curved));
                PixelateFeature.ActiveInstance.SetResolution(width, height);
            }

            foreach (Light light in portalLights)
            {
                if (light != null)
                {
                    light.intensity = Mathf.Lerp(glowStartIntensity, glowPeakIntensity, curved);
                }
            }

            if (cameraTransform != null)
            {
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-shakeAmount, shakeAmount),
                    Random.Range(-shakeAmount, shakeAmount),
                    0f
                ) * curved;

                cameraTransform.localPosition = cameraOriginalPos + shakeOffset;
            }

            yield return null;
        }

        if (cameraTransform != null)
            cameraTransform.localPosition = cameraOriginalPos;

        SceneManager.LoadScene(targetSceneName);
    }
}