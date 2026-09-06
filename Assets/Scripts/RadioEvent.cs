using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadioEvent : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public InteractionDetector interactionDetector;

    [Header("Radio Messages")]
    public List<string> messages = new List<string>();

    [Header("Optional Events")]
    public GameObject jeff;
    public Collider doorCollider;
    public DoorController doorController;
    public Level3RadioEvent level3Event;

    [Header("Timing")]
    public float delayBetweenMessages = 0.3f;

    private Interaction interaction;
    private bool hasPlayed = false;
    private bool playing = false;

    void Start()
    {
        interaction = GetComponent<Interaction>();
    }

    public void PlayRadio()
    {
        if (hasPlayed || playing)
            return;

        hasPlayed = true;
        playing = true;

        if (level3Event != null)
        {
            level3Event.RadioActivated();
        }

        if (interaction != null)
        {
            interaction.enabled = false;
        }

        if (interactionDetector != null)
        {
            interactionDetector.SuppressPrompt();
        }

        StartCoroutine(PlayMessages());
    }

    IEnumerator PlayMessages()
    {
        yield return null;

        if (dialogueUI == null)
        {
            if (interactionDetector != null)
            {
                interactionDetector.ResumePrompt();
            }

            playing = false;
            yield break;
        }

        foreach (string message in messages)
        {
            if (string.IsNullOrEmpty(message))
                continue;

            dialogueUI.ShowMessage(message);

            yield return new WaitUntil(() => !dialogueUI.IsShowing);

            yield return new WaitForSeconds(delayBetweenMessages);
        }

        if (jeff != null)
        {
            jeff.SetActive(false);
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        if (doorController != null)
        {
            doorController.enabled = true;
            doorController.UnlockDoor();
        }

        if (interactionDetector != null)
        {
            interactionDetector.ResumePrompt();
        }

        playing = false;
    }
}