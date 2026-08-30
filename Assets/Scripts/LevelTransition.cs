using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public string targetSceneName = "Level kääknagen on paras ou ou ou äää Hugsa mugsa jne HEHEH";
    public string playerTag = "Player";

    [Header("Lost Signal Screen ")]
    public bool useLostSignalScreen = false;
    public GameObject lostSignalScreen;
    public float delayBeforeScreen = 1.5f;
    public float delayBeforeLevel = 3f;

    [Header("Soundrh")]
    public AudioSource transitionSound;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.transform.root.CompareTag(playerTag))
            return;

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("LevelTransition: targetSceneName on tyhja objektilla " + gameObject.name);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError("LevelTransition: scenea '" + targetSceneName + "' ei loydy Build Settingsista.");
            return;
        }

        triggered = true;

        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        if (useLostSignalScreen)
        {
            yield return new WaitForSeconds(delayBeforeScreen);

            if (transitionSound != null)
            {
                transitionSound.Play();
            }

            if (lostSignalScreen != null)
            {
                lostSignalScreen.SetActive(true);
            }

            yield return new WaitForSeconds(delayBeforeLevel);
        }
        else if (transitionSound != null)
        {
            transitionSound.Play();
        }

        SceneManager.LoadScene(targetSceneName);
    }
}