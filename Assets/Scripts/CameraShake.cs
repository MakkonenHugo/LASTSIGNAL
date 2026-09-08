using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public enum ShakeSourceType
    {
        None,
        Sprint,
        Chase,
        Other
    }

    public float intensity = 0.15f;
    public float frequency = 25f;

    private Vector3 basePosition;
    private bool shaking = false;
    private ShakeSourceType currentSource = ShakeSourceType.None;
    private Coroutine shakeRoutine;
    private Coroutine pulseRoutine;
    private Vector3 pulseOffset = Vector3.zero;

    void Awake()
    {
        basePosition = transform.localPosition;
    }

    public void StartShake(ShakeSourceType source = ShakeSourceType.Other)
    {
        if (shaking)
            return;

        shaking = true;
        currentSource = source;
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    public void StopShake()
    {
        shaking = false;
        currentSource = ShakeSourceType.None;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        if (pulseRoutine == null)
        {
            transform.localPosition = basePosition;
        }
    }

    public bool IsShaking()
    {
        return shaking;
    }

    public ShakeSourceType ShakeSource()
    {
        return currentSource;
    }

    public void TriggerPulse(float pulseIntensity, float duration)
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
        }

        pulseRoutine = StartCoroutine(PulseRoutine(pulseIntensity, duration));
    }

    IEnumerator ShakeRoutine()
    {
        float timer = 0f;

        while (shaking)
        {
            timer += Time.deltaTime * frequency;

            float offsetX = (Mathf.PerlinNoise(timer, 0f) - 0.5f) * 2f * intensity;
            float offsetY = (Mathf.PerlinNoise(0f, timer) - 0.5f) * 2f * intensity;

            Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0f);
            transform.localPosition = basePosition + shakeOffset + pulseOffset;

            yield return null;
        }

        if (pulseRoutine == null)
        {
            transform.localPosition = basePosition;
        }
    }

    IEnumerator PulseRoutine(float pulseIntensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float falloff = 1f - t;

            float offsetX = (Random.value - 0.5f) * 2f * pulseIntensity * falloff;
            float offsetY = (Random.value - 0.5f) * 2f * pulseIntensity * falloff;

            pulseOffset = new Vector3(offsetX, offsetY, 0f);

            if (!shaking)
            {
                transform.localPosition = basePosition + pulseOffset;
            }

            yield return null;
        }

        pulseOffset = Vector3.zero;

        if (!shaking)
        {
            transform.localPosition = basePosition;
        }

        pulseRoutine = null;
    }

    public void SetBasePosition(Vector3 position)
    {
        basePosition = position;
    }
}