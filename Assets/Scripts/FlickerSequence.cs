using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FlickerStep
{
    public bool visible;
    public float duration;
}

public class FlickerSequence : MonoBehaviour
{
    public GameObject targetObject;
    public bool startVisible = false;
    public bool loop = true;

    public List<FlickerStep> steps = new List<FlickerStep>
    {
        new FlickerStep { visible = false, duration = 0.1f },
        new FlickerStep { visible = true, duration = 0.1f },
        new FlickerStep { visible = false, duration = 0.15f },
        new FlickerStep { visible = true, duration = 1.5f },
        new FlickerStep { visible = false, duration = 5f }
    };

    void OnEnable()
    {
        if (targetObject != null)
            targetObject.SetActive(startVisible);

        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        do
        {
            foreach (FlickerStep step in steps)
            {
                if (targetObject != null)
                    targetObject.SetActive(step.visible);

                yield return new WaitForSeconds(step.duration);
            }
        }
        while (loop);
    }
}
