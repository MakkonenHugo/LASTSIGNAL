using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level7ChaseController : MonoBehaviour
{
    [Header("References")]
    public RadioEvent radioEvent;
    public MouseLook mouseLook;
    public HeadBob headBob;
    public CameraShake cameraShake;
    public ChaseWarningText warningText;
    public PlayerController playerController;

    [Header("Chase Settings")]
    public float yawLockRange = 90f;
    public float chaseSpeedMultiplier = 1.6f;

    [Header("Time Limit")]
    public float timeLimit = 50f;
    public bool enableTimeLimit = true;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip chaseSoundtrack;
    public AudioSource loudSoundsSource;
    public AudioClip[] suddenLoudSounds;
    public float minLoudSoundInterval = 2f;
    public float maxLoudSoundInterval = 5f;

    private bool chaseActive = false;
    private float originalSpeed;
    private Coroutine loudSoundsRoutine;
    private Coroutine timeLimitRoutine;

    void Start()
    {
        if (radioEvent != null)
        {
            radioEvent.OnRadioFinished += TriggerChase;
        }
    }

    void OnDestroy()
    {
        if (radioEvent != null)
        {
            radioEvent.OnRadioFinished -= TriggerChase;
        }
    }

    public void TriggerChase()
    {
        if (chaseActive)
            return;

        chaseActive = true;
        StartCoroutine(ChaseSequence());
    }

    IEnumerator ChaseSequence()
    {
        if (mouseLook != null)
        {
            mouseLook.LockYaw(yawLockRange);
        }

        if (headBob != null)
        {
            headBob.enabled = false;
        }

        if (cameraShake != null)
        {
            cameraShake.StartShake();
        }

        if (warningText != null)
        {
            warningText.StartWarnings();
        }

        if (playerController != null)
        {
            originalSpeed = playerController.speed;
            playerController.speed = originalSpeed * chaseSpeedMultiplier;
        }

        if (musicSource != null && chaseSoundtrack != null)
        {
            musicSource.clip = chaseSoundtrack;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (loudSoundsSource != null && suddenLoudSounds != null && suddenLoudSounds.Length > 0)
        {
            loudSoundsRoutine = StartCoroutine(LoudSoundsRoutine());
        }

        if (enableTimeLimit)
        {
            timeLimitRoutine = StartCoroutine(TimeLimitRoutine());
        }

        yield return null;
    }

    IEnumerator TimeLimitRoutine()
    {
        yield return new WaitForSeconds(timeLimit);

        if (chaseActive)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }

    IEnumerator LoudSoundsRoutine()
    {
        while (chaseActive)
        {
            float wait = Random.Range(minLoudSoundInterval, maxLoudSoundInterval);
            yield return new WaitForSeconds(wait);

            if (!chaseActive)
                yield break;

            AudioClip clip = suddenLoudSounds[Random.Range(0, suddenLoudSounds.Length)];
            loudSoundsSource.PlayOneShot(clip);
        }
    }

    public void EndChase()
    {
        if (!chaseActive)
            return;

        chaseActive = false;

        if (mouseLook != null)
        {
            mouseLook.UnlockYaw();
        }

        if (headBob != null)
        {
            headBob.enabled = true;
        }

        if (cameraShake != null)
        {
            cameraShake.StopShake();
        }

        if (warningText != null)
        {
            warningText.StopWarnings();
        }

        if (playerController != null)
        {
            playerController.speed = originalSpeed;
        }

        if (loudSoundsRoutine != null)
        {
            StopCoroutine(loudSoundsRoutine);
            loudSoundsRoutine = null;
        }

        if (timeLimitRoutine != null)
        {
            StopCoroutine(timeLimitRoutine);
            timeLimitRoutine = null;
        }

        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public bool IsChaseActive()
    {
        return chaseActive;
    }
}