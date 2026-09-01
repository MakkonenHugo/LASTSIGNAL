using UnityEngine;

public class SpawnWhenTriggered : MonoBehaviour
{
    public GameObject objectToSpawn;
    public bool disableThisObjectToo = false;

    public void Trigger()
    {
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(true);
        }

        if (disableThisObjectToo)
        {
            gameObject.SetActive(false);
        }
    }
}