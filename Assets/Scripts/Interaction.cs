using UnityEngine;

public class Interaction : MonoBehaviour
{
    public string interactionText = "Press E to inspect";
    public string message = "It's just a table.";

    public RadioEvent radioEvent;

    public void Interact()
    {
        if (radioEvent != null)
        {
            radioEvent.PlayRadio();
            return;
        }

        DialogueUI dialogueUI = FindFirstObjectByType<DialogueUI>();

        if (dialogueUI != null)
        {
            dialogueUI.ShowMessage(message);
        }
    }
}