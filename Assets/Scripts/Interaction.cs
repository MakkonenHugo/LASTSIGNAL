using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Interaction : MonoBehaviour
{
    [Header("Interaction")]
    public string interactionText = "Press E to interact";

    [Header("Interact Sound")]
    public AudioClip interactSound;
    [Range(0f, 1f)] public float interactSoundVolume = 1f;

    [Header("Dialogue")]
    public List<string> messages = new List<string>();

    [Header("Special Events")]
    public RadioEvent radioEvent;
    public Level8Controller level8Controller;
    public DoorController doorController;
    public DoorLevelTransition doorLevelTransition;
    public DoorController2 doorController2;
    public HackingPanel hackingPanel;
    public ChairEvent chairEvent;
    public SurveillanceMonitorPanel monitorPanel;
    public PasswordMemoryPanel passwordPanel;
    public FakeOSTerminal fakeOSTerminal;
    public BurnItDownTrigger burnTrigger;
    public VanishWhenUnobserved vanishOnFinish;
    public SpawnWhenTriggered spawnOnFinish;

    [Header("Completion")]
    public InteractionMessageManager messageManager;
    public UnityEvent onMessagesFinished;

    private const string lockedDoorMessage = "The door is locked";
    private const string systemLockedMessage = "System locked, try again later";

    private bool interacting = false;

    public void Interact()
    {
        if (interacting)
            return;

        PlayInteractSound();

        if (vanishOnFinish != null)
        {
            vanishOnFinish.Arm();
        }

        if (chairEvent != null)
        {
            interacting = true;
            chairEvent.StartChairEvent();
            return;
        }

        if (monitorPanel != null)
        {
            monitorPanel.OpenPanel();
            return;
        }

        if (passwordPanel != null)
        {
            passwordPanel.OpenPanel();
            return;
        }

        if (fakeOSTerminal != null)
        {
            fakeOSTerminal.OpenPanel();
            return;
        }

        if (burnTrigger != null)
        {
            burnTrigger.OnBurnInteract();
            return;
        }

        if (doorController2 != null)
        {
            interacting = true;
            doorController2.OpenDoor();
            interacting = false;
            return;
        }

        if (doorLevelTransition != null)
        {
            if (!doorLevelTransition.CanOpen)
            {
                StartCoroutine(PlayMessage(lockedDoorMessage));
                return;
            }

            doorLevelTransition.TryOpenAndTransition();
            return;
        }

        if (doorController != null)
        {
            if (!doorController.IsUnlocked)
            {
                if (hackingPanel != null)
                {
                    if (hackingPanel.IsLockedOut)
                    {
                        StartCoroutine(PlayMessage(systemLockedMessage));
                        return;
                    }

                    hackingPanel.OpenPanel();
                    return;
                }

                StartCoroutine(PlayMessage(lockedDoorMessage));
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

        if (level8Controller != null)
        {
            interacting = true;
            level8Controller.PlayJeffMessage();
            return;
        }

        if (messages.Count > 0)
        {
            StartCoroutine(PlayDialogue());
        }
    }

    void PlayInteractSound()
    {
        if (interactSound == null)
            return;

        AudioSource.PlayClipAtPoint(interactSound, transform.position, interactSoundVolume);
    }

    IEnumerator PlayMessage(string message)
    {
        interacting = true;

        DialogueUI dialogueUI = FindAnyObjectByType<DialogueUI>();

        if (dialogueUI == null)
        {
            interacting = false;
            yield break;
        }

        InteractionDetector detector = FindAnyObjectByType<InteractionDetector>();

        if (detector != null)
            detector.SuppressPrompt();

        dialogueUI.ShowMessage(message);

        yield return new WaitUntil(() => !dialogueUI.IsShowing);

        if (detector != null)
            detector.ResumePrompt();

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

        if (vanishOnFinish != null)
        {
            vanishOnFinish.ArmAfter();
        }

        if (detector != null)
            detector.ResumePrompt();

        interacting = false;

        if (messageManager != null)
        {
            messageManager.InteractionCompleted();
        }

        if (spawnOnFinish != null)
        {
            spawnOnFinish.Trigger();
        }

        onMessagesFinished?.Invoke();
    }

    public void FinishInteraction()
    {
        interacting = false;
    }
}