using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionDetector : MonoBehaviour
{
    public float interactionDistance = 3f;
    public TextMeshProUGUI interactionPrompt;

    private Camera playerCamera;
    private Interaction currentInteraction;

    void Start()
    {
        playerCamera = GetComponent<Camera>();

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        DetectInteraction();

        if (currentInteraction != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interaction interaction = currentInteraction;

            currentInteraction = null;

            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(false);
            }

            interaction.Interact();
        }
    }

    void DetectInteraction()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        Interaction detectedInteraction = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            detectedInteraction = hit.collider.GetComponent<Interaction>();

            if (detectedInteraction == null)
            {
                detectedInteraction = hit.collider.GetComponentInParent<Interaction>();
            }
        }

        if (detectedInteraction == currentInteraction)
        {
            return;
        }

        currentInteraction = detectedInteraction;

        if (interactionPrompt != null)
        {
            if (currentInteraction != null)
            {
                interactionPrompt.text = currentInteraction.interactionText;
                interactionPrompt.gameObject.SetActive(true);
            }
            else
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }
    }

    public void HidePrompt()
    {
        currentInteraction = null;

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }
}