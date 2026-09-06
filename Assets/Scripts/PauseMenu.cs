using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Image backgroundImage;
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.7f);
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public string volumeParameterName = "MasterVolume";

    public Toggle crosshairToggle;
    public Crosshair crosshair;

    public Slider sensitivitySlider;
    public MouseLook mouseLook;
    public float minSensitivity = 0.5f;
    public float maxSensitivity = 10f;

    public Toggle headBobToggle;
    public HeadBob headBob;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (backgroundImage != null)
            backgroundImage.color = backgroundColor;

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (crosshair == null)
            crosshair = FindAnyObjectByType<Crosshair>();

        bool savedCrosshairEnabled = PlayerPrefs.GetInt("CrosshairEnabled", 1) == 1;
        ApplyCrosshairState(savedCrosshairEnabled);

        if (crosshairToggle != null)
        {
            crosshairToggle.isOn = savedCrosshairEnabled;
            crosshairToggle.onValueChanged.AddListener(SetCrosshairEnabled);
        }

        if (mouseLook == null)
            mouseLook = FindAnyObjectByType<MouseLook>();

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;

            float defaultSensitivity = mouseLook != null ? mouseLook.sensitivity : 2f;
            float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

            sensitivitySlider.value = savedSensitivity;
            SetSensitivity(savedSensitivity);
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }

        if (headBob == null)
            headBob = FindAnyObjectByType<HeadBob>();

        bool savedHeadBobEnabled = PlayerPrefs.GetInt("HeadBobEnabled", 1) == 1;
        ApplyHeadBobState(savedHeadBobEnabled);

        if (headBobToggle != null)
        {
            headBobToggle.isOn = savedHeadBobEnabled;
            headBobToggle.onValueChanged.AddListener(SetHeadBobEnabled);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);

        if (audioMixer != null)
        {
            float dB = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
            audioMixer.SetFloat(volumeParameterName, dB);
        }
        else
        {
            AudioListener.volume = value;
        }
    }

    public void SetCrosshairEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("CrosshairEnabled", enabled ? 1 : 0);
        ApplyCrosshairState(enabled);
    }

    void ApplyCrosshairState(bool enabled)
    {
        if (crosshair != null)
            crosshair.gameObject.SetActive(enabled);
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);

        if (mouseLook != null)
            mouseLook.sensitivity = value;
    }

    public void SetHeadBobEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("HeadBobEnabled", enabled ? 1 : 0);
        ApplyHeadBobState(enabled);
    }

    void ApplyHeadBobState(bool enabled)
    {
        if (headBob != null)
            headBob.enabled = enabled;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    void OnValidate()
    {
        if (backgroundImage != null)
            backgroundImage.color = backgroundColor;
    }
}