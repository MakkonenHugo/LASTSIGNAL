using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interaction : MonoBehaviour
{
    [Header("Dialogue")]
    public List<string> messages = new List<string>();

    [Header("Special Events")]
    public RadioEvent radioEvent;
    public DoorController doorController;
    public ChairEvent chairEvent;
    public VanishWhenUnobserved vanishOnFinish;
    public SpawnWhenTriggered spawnOnFinish;

    [Header("Completion")]
    public InteractionMessageManager messageManager;

    private const string lockedDoorMessage = "The door is locked";

    private bool interacting = false;

    public void Interact()
    {
        if (interacting)
            return;

        if (chairEvent != null)
        {
            interacting = true;

            chairEvent.StartChairEvent();

            return;
        }

        if (doorController != null)
        {
            if (!doorController.IsUnlocked)
            {
                StartCoroutine(PlayLockedDoorMessage());
                return;
            }

            interacting = true;

            doorController.OpenDoor();

            interacting = false;

            return;
        }

        if (radioEvent != null)
        {
            interacting = true;

            radioEvent.PlayRadio();

            return;
        }

        if (messages.Count > 0)
        {
            StartCoroutine(PlayDialogue());
        }
    }

    IEnumerator PlayLockedDoorMessage()
    {
        interacting = true;

        DialogueUI dialogueUI = FindAnyObjectByType<DialogueUI>();

        if (dialogueUI == null)
        {
            interacting = false;
            yield break;
        }

        dialogueUI.ShowMessage(lockedDoorMessage);

        yield return new WaitUntil(() => !dialogueUI.IsShowing);

        interacting = false;
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

        if (messageManager != null)
{
    messageManager.InteractionCompleted();
}

        if (vanishOnFinish != null)
        {
            vanishOnFinish.Arm();
        }

        if (spawnOnFinish != null)
        {
            spawnOnFinish.Trigger();
        }
    }

    public void FinishInteraction()
    {
        interacting = false;
    }
}