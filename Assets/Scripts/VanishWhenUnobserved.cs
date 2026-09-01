using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VanishWhenUnobserved : MonoBehaviour
{
    public List<GameObject> objectsToVanish = new List<GameObject>();

    public float vanishDelay = 1f;
    public float viewAngleThreshold = 15f;

    private Camera playerCamera;
    private Interaction interaction;
    private bool armed = false;
    private List<VanishState> states = new List<VanishState>();

    private class VanishState
    {
        public GameObject obj;
        public bool vanished;
        public Coroutine routine;
    }

    void Start()
    {
        interaction = GetComponent<Interaction>();
        playerCamera = Camera.main;

        foreach (GameObject obj in objectsToVanish)
        {
            if (obj != null)
            {
                states.Add(new VanishState { obj = obj, vanished = false, routine = null });
            }
        }
    }

    public void Arm()
    {
        armed = true;
    }

    void Update()
    {
        if (!armed || playerCamera == null)
            return;

        foreach (VanishState state in states)
        {
            if (state.vanished || state.obj == null)
                continue;

            bool isBeingViewed = IsObjectBeingViewed(state.obj);

            if (!isBeingViewed && state.routine == null)
            {
                state.routine = StartCoroutine(VanishAfterDelay(state));
            }
            else if (isBeingViewed && state.routine != null)
            {
                StopCoroutine(state.routine);
                state.routine = null;
            }
        }
    }

    bool IsObjectBeingViewed(GameObject obj)
    {
        Vector3 directionToObject = (obj.transform.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToObject);

        if (angle > viewAngleThreshold)
            return false;

        RaycastHit hit;
        Vector3 origin = playerCamera.transform.position;
        Vector3 target = obj.transform.position;
        float distance = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, (target - origin).normalized, out hit, distance))
        {
            if (hit.collider.gameObject != obj && hit.collider.transform.root.gameObject != obj)
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator VanishAfterDelay(VanishState state)
    {
        yield return new WaitForSeconds(vanishDelay);

        if (state.obj != null)
        {
            state.obj.SetActive(false);
        }

        state.vanished = true;
        state.routine = null;
    }
}