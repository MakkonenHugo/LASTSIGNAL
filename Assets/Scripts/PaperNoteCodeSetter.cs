using UnityEngine;

public class PaperNoteCodeSetter : MonoBehaviour
{
    public DoorCodePanel doorCodePanel;

    void Start()
    {
        if (doorCodePanel == null)
            return;

        PaperNote[] notes = FindObjectsByType<PaperNote>(FindObjectsSortMode.None);

        if (notes.Length == 0)
            return;

        int highest = int.MinValue;

        foreach (PaperNote note in notes)
        {
            if (note.number > highest)
                highest = note.number;
        }

        doorCodePanel.correctCode = highest;
    }
}