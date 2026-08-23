using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interaction : MonoBehaviour
{
    [Header("Interaction")]
    public string interactionText = "Press E to inspect";

    [Header("Dialogue")]
    public List<string> messages = new List<string>();

    [Header("Special Events")]
    public RadioEvent radioEvent;
    public DoorController doorController;
    public ChairEvent chairEvent;

    private bool interacting = false;

    public void Interact()
    {
        if (interacting)
            return;

        // =========================
        // CHAIR EVENT
        // =========================

        if (chairEvent != null)
        {
            interacting = true;

            chairEvent.StartChairEvent();

            return;
        }

        // =========================
        // DOOR
        // =========================

        if (doorController != null)
        {
            interacting = true;

            doorController.OpenDoor();

            interacting = false;

            return;
        }

        // =========================
        // RADIO EVENT
        // =========================

        if (radioEvent != null)
        {
            interacting = true;

            radioEvent.PlayRadio();

            return;
        }

        // =========================
        // NORMAL DIALOGUE
        // =========================

        if (messages.Count > 0)
        {
            StartCoroutine(PlayDialogue());
        }
    }

    IEnumerator PlayDialogue()
    {
        interacting = true;

        DialogueUI dialogueUI = FindAnyObjectByType<DialogueUI>();

        if (dialogueUI == null)
        {
            interacting = false;
            yield break;
        }

        foreach (string message in messages)
        {
            if (string.IsNullOrEmpty(message))
                continue;

            dialogueUI.ShowMessage(message);

            yield return new WaitUntil(() => !dialogueUI.IsShowing);

            yield return new WaitForSeconds(0.1f);
        }

        interacting = false;
    }

    public void FinishInteraction()
    {
        interacting = false;
    }
}