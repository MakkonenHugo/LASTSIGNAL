using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionDetector : MonoBehaviour
{
    public float interactionDistance = 3f;
    public int loseTargetFrameThreshold = 5;
    public int targetConfirmFrameThreshold = 3;
    public float detectionRadius = 0.15f;

    public GameObject interactionPrompt;
    public TMP_Text promptText;

    private Camera playerCamera;
    private Interaction currentInteraction;
    private Interaction detectedInteraction;

    private int framesWithoutTarget = 0;
    private int framesWithNewTarget = 0;

    private bool promptSuppressed = false;

    void Awake()
    {
        playerCamera = GetComponent<Camera>();

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    static bool AnyBlockingPanelOpen()
    {
        return DoorCodePanel.IsAnyPanelOpen || HackingPanel.IsAnyPanelOpen;
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        if (AnyBlockingPanelOpen())
        {
            if (currentInteraction != null || detectedInteraction != null)
            {
                currentInteraction = null;
                detectedInteraction = null;
                framesWithoutTarget = 0;
                framesWithNewTarget = 0;

                if (interactionPrompt != null)
                    interactionPrompt.SetActive(false);
            }

            return;
        }

        if (currentInteraction != null &&
            !currentInteraction.isActiveAndEnabled)
        {
            currentInteraction = null;
            detectedInteraction = null;
            framesWithoutTarget = 0;
            framesWithNewTarget = 0;

            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        Interaction newInteraction = null;

        if (Physics.SphereCast(
            ray,
            detectionRadius,
            out hit,
            interactionDistance))
        {
            Interaction interaction =
                hit.collider.GetComponentInParent<Interaction>();

            if (interaction != null && interaction.enabled)
            {
                newInteraction = interaction;
            }
        }

        if (newInteraction != null)
        {
            framesWithoutTarget = 0;

            if (newInteraction == currentInteraction)
            {
                detectedInteraction = newInteraction;
                framesWithNewTarget = 0;

                if (!promptSuppressed)
                {
                    if (interactionPrompt != null)
                        interactionPrompt.SetActive(true);

                    if (promptText != null)
                        promptText.text = currentInteraction.interactionText;
                }
            }
            else
            {
                if (newInteraction == detectedInteraction)
                {
                    framesWithNewTarget++;
                }
                else
                {
                    detectedInteraction = newInteraction;
                    framesWithNewTarget = 1;
                }

                if (framesWithNewTarget >= targetConfirmFrameThreshold)
                {
                    currentInteraction = detectedInteraction;

                    if (!promptSuppressed)
                    {
                        if (interactionPrompt != null)
                            interactionPrompt.SetActive(true);

                        if (promptText != null)
                            promptText.text =
                                currentInteraction.interactionText;
                    }
                }
            }
        }
        else
        {
            detectedInteraction = null;
            framesWithNewTarget = 0;

            if (currentInteraction != null)
            {
                framesWithoutTarget++;

                if (framesWithoutTarget >= loseTargetFrameThreshold)
                {
                    currentInteraction = null;

                    if (interactionPrompt != null)
                        interactionPrompt.SetActive(false);
                }
            }
        }

        if (currentInteraction != null &&
            !promptSuppressed &&
            currentInteraction.isActiveAndEnabled)
        {
            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                currentInteraction.Interact();
            }
        }
    }

    public void SuppressPrompt()
    {
        promptSuppressed = true;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    public void ResumePrompt()
    {
        promptSuppressed = false;

        if (currentInteraction != null &&
            currentInteraction.isActiveAndEnabled)
        {
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);

            if (promptText != null)
                promptText.text =
                    currentInteraction.interactionText;
        }
    }

    public void HidePrompt()
    {
        currentInteraction = null;
        detectedInteraction = null;

        framesWithoutTarget = 0;
        framesWithNewTarget = 0;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}