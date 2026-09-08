using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlarmSequence : MonoBehaviour
{
    [Header("Trigger")]
    public bool triggerOnStart = false;

    [Header("Alarm Lights")]
    public List<Light> alarmLights = new List<Light>();
    public Color alarmLightColor = new Color(1f, 0.1f, 0.1f);
    public float lightPulseSpeed = 2f;
    public float lightMinIntensity = 0.3f;
    public float lightMaxIntensity = 2f;

    [Header("Alarm Sound")]
    public AudioSource sirenSource;
    public AudioClip sirenLoop;

    [Header("Sudden Loud Sounds")]
    public AudioSource loudSoundsSource;
    public AudioClip[] suddenSounds;
    public float minSoundInterval = 3f;
    public float maxSoundInterval = 7f;

    [Header("Camera Shake")]
    public CameraShake cameraShake;
    public float alarmShakeIntensity = 0.04f;

    [Header("Warning HUD")]
    public GameObject alarmHudRoot;

    private bool active = false;
    private List<float> lightBaseIntensities = new List<float>();
    private Coroutine loudSoundsRoutine;
    private Coroutine lightPulseRoutine;

    void Start()
    {
        if (triggerOnStart)
        {
            StartAlarm();
        }
    }

    public void StartAlarm()
    {
        if (active)
            return;

        active = true;

        if (alarmHudRoot != null)
            alarmHudRoot.SetActive(true);

        SetupLights();
        lightPulseRoutine = StartCoroutine(LightPulseRoutine());

        if (sirenSource != null && sirenLoop != null)
        {
            sirenSource.clip = sirenLoop;
            sirenSource.loop = true;
            sirenSource.Play();
        }

        if (loudSoundsSource != null && suddenSounds != null && suddenSounds.Length > 0)
        {
            loudSoundsRoutine = StartCoroutine(LoudSoundsRoutine());
        }

        if (cameraShake != null)
        {
            cameraShake.intensity = alarmShakeIntensity;
            cameraShake.StartShake(CameraShake.ShakeSourceType.Chase);
        }
    }

    public void StopAlarm()
    {
        if (!active)
            return;

        active = false;

        if (alarmHudRoot != null)
            alarmHudRoot.SetActive(false);

        if (lightPulseRoutine != null)
        {
            StopCoroutine(lightPulseRoutine);
            lightPulseRoutine = null;
        }

        RestoreLights();

        if (sirenSource != null)
        {
            sirenSource.Stop();
        }

        if (loudSoundsRoutine != null)
        {
            StopCoroutine(loudSoundsRoutine);
            loudSoundsRoutine = null;
        }

        if (cameraShake != null && cameraShake.ShakeSource() == CameraShake.ShakeSourceType.Chase)
        {
            cameraShake.StopShake();
        }
    }

    void SetupLights()
    {
        lightBaseIntensities.Clear();

        foreach (Light light in alarmLights)
        {
            if (light != null)
            {
                lightBaseIntensities.Add(light.intensity);
                light.color = alarmLightColor;
            }
            else
            {
                lightBaseIntensities.Add(1f);
            }
        }
    }

    void RestoreLights()
    {
        for (int i = 0; i < alarmLights.Count; i++)
        {
            if (alarmLights[i] != null && i < lightBaseIntensities.Count)
            {
                alarmLights[i].color = Color.white;
                alarmLights[i].intensity = lightBaseIntensities[i];
            }
        }
    }

    IEnumerator LightPulseRoutine()
    {
        float t = 0f;

        while (active)
        {
            t += Time.deltaTime * lightPulseSpeed;
            float pulse = (Mathf.Sin(t * Mathf.PI) + 1f) * 0.5f;
            float intensity = Mathf.Lerp(lightMinIntensity, lightMaxIntensity, pulse);

            foreach (Light light in alarmLights)
            {
                if (light != null)
                {
                    light.intensity = intensity;
                }
            }

            yield return null;
        }
    }

    IEnumerator LoudSoundsRoutine()
    {
        while (active)
        {
            float wait = Random.Range(minSoundInterval, maxSoundInterval);
            yield return new WaitForSeconds(wait);

            if (!active)
                yield break;

            AudioClip clip = suddenSounds[Random.Range(0, suddenSounds.Length)];
            loudSoundsSource.PlayOneShot(clip);
        }
    }

    public bool IsActive()
    {
        return active;
    }
}