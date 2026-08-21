using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FallTrigger_SFX : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag(playerTag)) return;

        if (audioSource.clip == null)
            return;

        audioSource.Play();
    }
}