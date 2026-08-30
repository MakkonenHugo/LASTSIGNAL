using UnityEngine;

public class Level3RadioEvent : MonoBehaviour
{
    public GameObject radio;
    public GameObject strangeObject;
    public GameObject trigger;

    [Header("Hallway Objects (RadioActivated)")]
    public GameObject strangeObject2;
    public GameObject strangeObject3;

    private bool activated;

    public void ActivateEvent()
    {
        if (activated)
            return;

        activated = true;

        if (radio != null)
            radio.SetActive(true);

        if (strangeObject != null)
            strangeObject.SetActive(true);

        if (trigger != null)
            trigger.SetActive(false);
    }

    public void RadioActivated()
    {
        if (!activated)
            return;

        if (strangeObject != null)
            strangeObject.SetActive(false);

        if (strangeObject2 != null)
            strangeObject2.SetActive(true);

        if (strangeObject3 != null)
            strangeObject3.SetActive(true);
    }
}