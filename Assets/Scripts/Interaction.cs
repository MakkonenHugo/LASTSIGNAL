using UnityEngine;
public class Interaction : MonoBehaviour
{
    public string interactionText = "Press E to inspect";
    public string message = "It's just a table.";
    public RadioEvent radioEvent;
    public DoorController doorController;
    public void Interact()
    {
        if (doorController != null)
        {
            doorController.OpenDoor();
            return;
        }
        if (radioEvent != null)
        {
            radioEvent.PlayRadio();
            return;
        }
        DialogueUI dialogueUI = FindAnyObjectByType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.ShowMessage(message);
        }
    }
}