using UnityEngine;
using System.Collections;

public class RadioEvent : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public InteractionDetector interactionDetector;

    public GameObject jeff;
    public Collider doorCollider;

    private Interaction interaction;
    private bool hasPlayed = false;
    private bool playing = false;

    void Start()
    {
        interaction = GetComponent<Interaction>();
    }

    public void PlayRadio()
    {
        if (hasPlayed || playing)
            return;

        hasPlayed = true;
        playing = true;

        if (interaction != null)
        {
            interaction.enabled = false;
        }

        if (interactionDetector != null)
        {
            interactionDetector.HidePrompt();
        }

        StartCoroutine(StartRadio());
    }

    IEnumerator StartRadio()
    {
        yield return null;

        dialogueUI.ShowMessage("It's not plugged in...");

        yield return new WaitUntil(() => !dialogueUI.IsShowing);

        yield return new WaitForSeconds(0.3f);

        dialogueUI.ShowMessage("HELLOOO!! Is anyone getting this signal?");

        yield return new WaitUntil(() => !dialogueUI.IsShowing);

        yield return new WaitForSeconds(0.1f);

        dialogueUI.ShowMessage("If you are in a gray room, GET OUT OF THERE!!");

        yield return new WaitUntil(() => !dialogueUI.IsShowing);

        yield return new WaitForSeconds(0.1f);

        dialogueUI.ShowMessage("The door should be unlocked!");

        yield return new WaitUntil(() => !dialogueUI.IsShowing);

        yield return new WaitForSeconds(0.5f);

        if (jeff != null)
        {
            jeff.SetActive(false);
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        playing = false;
    }
}