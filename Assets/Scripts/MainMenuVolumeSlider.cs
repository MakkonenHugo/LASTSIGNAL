using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public string volumeParameterName = "MasterVolume";

    void Start()
    {
        if (volumeSlider == null)
            volumeSlider = GetComponent<Slider>();

        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
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
}