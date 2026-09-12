using UnityEngine;
using TMPro;

public class HideTextAfterDelay : MonoBehaviour
{
    public float delay = 5f;

    void Start()
    {
        Invoke(nameof(HideText), delay);
    }

    void HideText()
    {
        GetComponent<TMP_Text>().enabled = false;
    }
}