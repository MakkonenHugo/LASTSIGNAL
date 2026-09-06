using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VanishWhenUnobserved : MonoBehaviour
{
    [Header("Objects That Vanish Immediately")]
    public List<GameObject> objectsThatVanishImmediately = new List<GameObject>();

    [Header("Objects That Vanish After")]
    public List<GameObject> objectsThatVanishAfter = new List<GameObject>();

    [Header("Vanish Settings")]
    public float vanishDelay = 1f;
    public float viewAngleThreshold = 15f;

    private Camera playerCamera;

    private bool immediateArmed = false;
    private bool afterArmed = false;

    private List<VanishState> immediateStates = new List<VanishState>();
    private List<VanishState> afterStates = new List<VanishState>();

    private class VanishState
    {
        public GameObject obj;
        public bool vanished;
        public Coroutine routine;
    }

    void Start()
    {
        playerCamera = Camera.main;

        foreach (GameObject obj in objectsThatVanishImmediately)
        {
            if (obj != null)
            {
                immediateStates.Add(new VanishState
                {
                    obj = obj,
                    vanished = false,
                    routine = null
                });
            }
        }

        foreach (GameObject obj in objectsThatVanishAfter)
        {
            if (obj != null)
            {
                afterStates.Add(new VanishState
                {
                    obj = obj,
                    vanished = false,
                    routine = null
                });
            }
        }
    }

    public void Arm()
    {
        immediateArmed = true;
    }

    public void ArmAfter()
    {
        afterArmed = true;
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        if (immediateArmed)
        {
            foreach (VanishState state in immediateStates)
            {
                CheckVanishState(state);
            }
        }

        if (afterArmed)
        {
            foreach (VanishState state in afterStates)
            {
                CheckVanishState(state);
            }
        }
    }

    void CheckVanishState(VanishState state)
    {
        if (state.vanished || state.obj == null)
            return;

        bool isBeingViewed = IsObjectBeingViewed(state.obj);

        if (isBeingViewed)
        {
            if (state.routine != null)
            {
                StopCoroutine(state.routine);
                state.routine = null;
            }
        }
        else
        {
            if (state.routine == null)
            {
                state.routine = StartCoroutine(
                    VanishAfterDelay(state)
                );
            }
        }
    }

    bool IsObjectBeingViewed(GameObject obj)
    {
        Vector3 directionToObject =
            (obj.transform.position -
             playerCamera.transform.position).normalized;

        float angle =
            Vector3.Angle(
                playerCamera.transform.forward,
                directionToObject
            );

        if (angle > viewAngleThreshold)
            return false;

        RaycastHit hit;

        Vector3 origin =
            playerCamera.transform.position;

        Vector3 target =
            obj.transform.position;

        float distance =
            Vector3.Distance(origin, target);

        if (Physics.Raycast(
            origin,
            (target - origin).normalized,
            out hit,
            distance))
        {
            if (hit.collider.gameObject != obj &&
                hit.collider.transform.root.gameObject != obj)
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