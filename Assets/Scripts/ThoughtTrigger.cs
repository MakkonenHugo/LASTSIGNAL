using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ThoughtTrigger : MonoBehaviour
{
    public string playerTag = "Player";

    public List<string> messages = new List<string>();

    public bool triggerOnce = true;

    private bool hasTriggered = false;
    private bool playing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (playing)
            return;

        if (triggerOnce && hasTriggered)
            return;

        if (!other.transform.root.CompareTag(playerTag))
            return;

        if (messages.Count == 0)
            return;

        hasTriggered = true;

        StartCoroutine(PlayThought());
    }

    IEnumerator PlayThought()
    {
        playing = true;

        DialogueUI dialogueUI = FindAnyObjectByType<DialogueUI>();

        if (dialogueUI == null)
        {
            playing = false;
            yield break;
        }

        InteractionDetector detector = FindAnyObjectByType<InteractionDetector>();

        if (detector != null)
            detector.SuppressPrompt();

        foreach (string message in messages)
        {
            if (string.IsNullOrEmpty(message))
                continue;

            dialogueUI.ShowMessage(message);

            yield return new WaitUntil(() => !dialogueUI.IsShowing);

            yield return new WaitForSeconds(0.1f);
        }

        if (detector != null)
            detector.ResumePrompt();

        playing = false;
    }
}