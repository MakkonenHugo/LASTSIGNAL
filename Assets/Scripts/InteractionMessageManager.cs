using UnityEngine;
using System.Collections;

public class InteractionMessageManager : MonoBehaviour
{
    [Header("Music")]
    public AudioSource audioSource;
    public AudioClip musicToPlay;
    public float musicDelay = 0f;

    private bool completed = false;
    private bool musicStarted = false;

    public void InteractionCompleted()
    {
        if (completed)
            return;

        completed = true;

        StartCoroutine(PlayMusicAfterDelay());
    }

    IEnumerator PlayMusicAfterDelay()
    {
        if (musicStarted)
            yield break;

        musicStarted = true;

        if (musicDelay > 0f)
        {
            yield return new WaitForSeconds(musicDelay);
        }

        if (audioSource == null)
        {
            Debug.LogWarning("InteractionMessageManager: AudioSource is missing.");
            yield break;
        }

        if (musicToPlay == null)
        {
            Debug.LogWarning("InteractionMessageManager: Music AudioClip is missing.");
            yield break;
        }

        audioSource.clip = musicToPlay;
        audioSource.Play();

        Debug.Log("All messages in this Interaction completed. Music started.");
    }
}