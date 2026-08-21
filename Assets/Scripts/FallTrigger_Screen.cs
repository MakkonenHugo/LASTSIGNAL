using UnityEngine;
using UnityEngine.UI;

public class FallTrigger_Screen : MonoBehaviour
{
    [SerializeField] private Image fullscreenImage;
    [SerializeField] private Sprite imageToShow;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float displayDuration = 3f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        if (fullscreenImage != null)
        {
            fullscreenImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (fullscreenImage == null) return;
        if (imageToShow == null) return;

        fullscreenImage.sprite = imageToShow;
        fullscreenImage.gameObject.SetActive(true);
        fullscreenImage.transform.SetAsLastSibling();

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        fullscreenImage.gameObject.SetActive(false);
    }
}