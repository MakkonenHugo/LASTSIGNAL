using UnityEngine;
using System.Collections;

public class ChairEvent : MonoBehaviour
{
    [Header("Player")]
    public Camera playerCamera;

    [Header("Chair Positions")]
    public Transform position1;
    public Transform position2;
    public Transform position3;

    [Header("Settings")]
    public float lookAwayAngle = 70f;
    public float moveDelay = 0.5f;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;

    private bool eventStarted = false;
    private bool waitingForPlayerToLookAway = false;
    private bool moved = false;

    private Interaction interaction;

    void Start()
    {
        interaction = GetComponent<Interaction>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    public void StartChairEvent()
    {
        if (eventStarted || moved)
            return;

        eventStarted = true;

        if (interaction != null)
            interaction.enabled = false;

        StartCoroutine(ChairSequence());
    }

    IEnumerator ChairSequence()
    {
        // Ensimmäinen dialogi
        if (dialogueUI != null)
        {
            dialogueUI.ShowMessage("Huh...");

            yield return new WaitUntil(() => !dialogueUI.IsShowing);

            yield return new WaitForSeconds(0.2f);

            dialogueUI.ShowMessage("Something feels wrong about this chair.");

            yield return new WaitUntil(() => !dialogueUI.IsShowing);
        }

        // Nyt odotetaan että pelaaja KATSOO POIS
        waitingForPlayerToLookAway = true;

        while (waitingForPlayerToLookAway)
        {
            if (PlayerIsLookingAway())
            {
                waitingForPlayerToLookAway = false;
            }

            yield return null;
        }

        // Pieni viive, jotta teleportti ei tunnu välittömältä
        yield return new WaitForSeconds(moveDelay);

        MoveChair();

        moved = true;
    }
void FinishInteraction()
{
    Interaction interaction = GetComponent<Interaction>();

    if (interaction != null)
    {
        interaction.FinishInteraction();
    }
}
    bool PlayerIsLookingAway()
    {
        if (playerCamera == null)
            return false;

        Vector3 directionToChair =
            (transform.position - playerCamera.transform.position).normalized;

        float angle = Vector3.Angle(
            playerCamera.transform.forward,
            directionToChair
        );

        return angle > lookAwayAngle;
    }

    void MoveChair()
    {
        Transform[] positions =
        {
            position1,
            position2,
            position3
        };

        // Valitaan satunnainen paikka
        Transform selectedPosition = null;

        for (int i = 0; i < 20; i++)
        {
            Transform candidate =
                positions[Random.Range(0, positions.Length)];

            if (candidate == null)
                continue;

            // Ei nykyiseen paikkaan
            if (Vector3.Distance(
                transform.position,
                candidate.position
            ) < 0.1f)
                continue;

            // Varmistetaan että pelaaja EI katso sinne
            Vector3 direction =
                candidate.position -
                playerCamera.transform.position;

            float angle = Vector3.Angle(
                playerCamera.transform.forward,
                direction
            );

            if (angle <= playerCamera.fieldOfView / 2f)
                continue;

            selectedPosition = candidate;
            break;
        }

        
        if (selectedPosition == null)
        {
            foreach (Transform candidate in positions)
            {
                if (candidate == null)
                    continue;

                if (Vector3.Distance(
                    transform.position,
                    candidate.position
                ) < 0.1f)
                    continue;

                selectedPosition = candidate;
                break;
            }
        }

        if (selectedPosition == null)
            return;

        transform.position = selectedPosition.position;
        transform.rotation = selectedPosition.rotation;

        Debug.Log("CHAIR MOVED!");
    }
}