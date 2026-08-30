using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionDetector : MonoBehaviour
{
    public float interactionDistance = 3f;
    public int loseTargetFrameThreshold = 5;

    public GameObject interactionPrompt;
    public TMP_Text promptText;

    private Camera playerCamera;
    private Interaction currentInteraction;
    private int framesWithoutTarget = 0;

    void Awake()
    {
        playerCamera = GetComponent<Camera>();

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        Interaction detectedInteraction = null;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            Interaction interaction =
                hit.collider.GetComponentInParent<Interaction>();

            if (interaction != null && interaction.enabled)
            {
                detectedInteraction = interaction;
            }
        }

        if (detectedInteraction != null)
        {
            framesWithoutTarget = 0;

            if (detectedInteraction != currentInteraction)
            {
                currentInteraction = detectedInteraction;

                if (interactionPrompt != null)
                    interactionPrompt.SetActive(true);

                if (promptText != null)
                    promptText.text = currentInteraction.interactionText;
            }
        }
        else if (currentInteraction != null)
        {
            framesWithoutTarget++;

            if (framesWithoutTarget >= loseTargetFrameThreshold)
            {
                currentInteraction = null;

                if (interactionPrompt != null)
                    interactionPrompt.SetActive(false);
            }
        }

        if (currentInteraction != null)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                currentInteraction.Interact();
            }
        }
    }

    public void HidePrompt()
    {
        currentInteraction = null;
        framesWithoutTarget = 0;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}