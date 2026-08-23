using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger_LoadScene3 : MonoBehaviour
{
    [SerializeField] private string sceneName = "ThirdRoom";
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