using UnityEngine;

public class WebDemoNotice : MonoBehaviour
{
    void Awake()
    {
#if UNITY_WEBGL
        gameObject.SetActive(true);
#else
        gameObject.SetActive(false);
#endif
    }
}