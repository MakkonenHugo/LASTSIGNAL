using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionDetector : MonoBehaviour
{
    public float interactionDistance = 3f;

    public GameObject interactionPrompt;
    public TMP_Text promptText;

    private Camera playerCamera;
    private Interaction currentInteraction;

    void Awake()
    {
        // Koska tämä scripti on kamerassa,
        // otetaan kamera automaattisesti tästä objektista.
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

        if (detectedInteraction != currentInteraction)
        {
            currentInteraction = detectedInteraction;

            if (currentInteraction != null)
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(true);

                if (promptText != null)
                    promptText.text =
                        currentInteraction.interactionText;
            }
            else
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(false);
            }
        }

        if (currentInteraction != null)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
{
    Debug.Log("E PAINETTU!");
    currentInteraction.Interact();
}
        }
    }

    public void HidePrompt()
    {
        currentInteraction = null;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}