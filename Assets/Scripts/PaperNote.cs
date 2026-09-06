using UnityEngine;

public class PaperNote : MonoBehaviour
{
    public int number = 17;
    public string messagePrefix = "A note with a number scrawled on it: ";

    private Interaction interaction;

    void Awake()
    {
        interaction = GetComponent<Interaction>();

        if (interaction != null && interaction.messages.Count == 0)
        {
            interaction.messages.Add(messagePrefix + number.ToString());
        }
    }
}