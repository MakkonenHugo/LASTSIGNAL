using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class WalkingSounds : MonoBehaviour
{
    [Header("Walking Sound")]
    public AudioClip walkingSound;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float volume = 0.5f;

    public float pitch = 1f;

    [Header("Random Start")]
    public bool randomStartPosition = true;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = walkingSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        audioSource.volume = volume;
        audioSource.pitch = pitch;

        
        audioSource.spatialBlend = 0f;
    }

    public void StartWalking()
    {
        if (walkingSound == null)
            return;

        if (!audioSource.isPlaying)
        {
            if (randomStartPosition)
            {
                audioSource.time = Random.Range(
                    0f,
                    Mathf.Max(0f, walkingSound.length - 0.1f)
                );
            }

            audioSource.Play();
        }
    }

    public void StopWalking()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}