using UnityEngine;

public class Level3Trigger : MonoBehaviour
{
    public Level3RadioEvent level3Event;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("LEVEL 3 TRIGGER: " + other.gameObject.name);
        Debug.Log("LEVEL 3 TRIGGER: level3Event kentta osoittaa GameObjectiin: " +
            (level3Event != null ? level3Event.gameObject.name : "NULL"));

        if (level3Event == null)
        {
            Debug.LogError("LEVEL 3 TRIGGER: Level3RadioEvent is NOT assigned!");
            return;
        }

        Debug.Log("LEVEL 3 TRIGGER: Activating Level3RadioEvent!");

        level3Event.ActivateEvent();
    }
}