using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string firstLevelScene = "FirstRoom";

    public SettingsPanelTransition settingsPanel;
    public SettingsPanelTransition creditsPanel;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public float musicVolume = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false;

        
        if (backgroundMusic != null)
        {
            audioSource.Play();
        }
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(firstLevelScene);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.Open();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.Close();
    }

    public void OpenCredits()
    {
        if (creditsPanel != null)
            creditsPanel.Open();
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.Close();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}