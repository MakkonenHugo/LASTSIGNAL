using UnityEngine;
using UnityEngine.SceneManagement;

public class FallTrigger_LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneName = "SecondRoom";
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag(playerTag)) return;

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}