using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ParkourFallCatcher : MonoBehaviour
{
    public float fallCatchHeight = -15f;
    public Transform startingRespawnPoint;
    private CharacterController controller;
    private Transform currentCheckpoint;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentCheckpoint = startingRespawnPoint;
    }

    void Update()
    {
        if (transform.position.y < fallCatchHeight)
        {
            Respawn();
        }
    }
    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    void Respawn()
    {
        if (currentCheckpoint == null)
            return;

        controller.enabled = false;
        transform.position = currentCheckpoint.position;
        transform.rotation = currentCheckpoint.rotation;
        controller.enabled = true;
    }
}