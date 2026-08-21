using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public string nextSceneName = "Level2";

    public GameObject lostSignalScreen;

    public AudioSource transitionSound;

    public float delayBeforeScreen = 1.5f;
    public float delayBeforeLevel2 = 3f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        StartCoroutine(Transition());
    }

    IEnumerator Transition()
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

        // Odotetaan ennen Level 2:ta
        yield return new WaitForSeconds(delayBeforeLevel2);

        SceneManager.LoadScene(nextSceneName);
    }
}