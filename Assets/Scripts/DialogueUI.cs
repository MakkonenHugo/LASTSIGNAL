using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("Typing Sound")]
    public AudioSource typingAudioSource;
    public AudioClip typingBlipSound;
    [Range(0f, 1f)] public float typingBlipVolume = 0.5f;
    public Vector2 pitchRandomRange = new Vector2(0.95f, 1.05f);
    public bool skipSoundOnWhitespace = true;

    bool isShowing;
    bool justOpened;
    bool isTyping;

    string currentMessage;
    Coroutine typingCoroutine;

    public bool IsShowing => isShowing;

    void Start()
    {
        dialogueBox.SetActive(false);
        isShowing = false;
    }

    void Update()
    {
        if (justOpened)
        {
            justOpened = false;
            return;
        }

        if (isShowing && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isTyping)
            {
                FinishTyping();
            }
            else
            {
                HideMessage();
            }
        }
    }

    public void ShowMessage(string message)
    {
        currentMessage = message;

        dialogueBox.SetActive(true);
        isShowing = true;
        justOpened = true;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeMessage());
    }

    IEnumerator TypeMessage()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in currentMessage)
        {
            dialogueText.text += letter;
            PlayTypingBlip(letter);
            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    void PlayTypingBlip(char letter)
    {
        if (typingAudioSource == null || typingBlipSound == null)
            return;

        if (skipSoundOnWhitespace && char.IsWhiteSpace(letter))
            return;

        typingAudioSource.pitch = Random.Range(pitchRandomRange.x, pitchRandomRange.y);
        typingAudioSource.PlayOneShot(typingBlipSound, typingBlipVolume);
    }

    void FinishTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = currentMessage;
        isTyping = false;
    }

    public void HideMessage()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueBox.SetActive(false);
        isShowing = false;
        isTyping = false;
    }
}