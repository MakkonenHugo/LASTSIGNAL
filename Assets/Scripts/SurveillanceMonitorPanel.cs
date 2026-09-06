using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class SurveillanceMonitorPanel : MonoBehaviour
{
    public static bool IsAnyPanelOpen = false;

    public GameObject panelUI;
    public RawImage screenImage;
    public TMP_Text cameraLabel;
    public InteractionDetector interactionDetector;

    public List<Texture> cameraFeeds = new List<Texture>();
    public List<string> cameraLabels = new List<string>();

    private int currentIndex = 0;
    private bool isOpen = false;
    private bool justOpened = false;

    void Awake()
    {
        if (panelUI != null)
            panelUI.SetActive(false);

        if (interactionDetector == null)
            interactionDetector = FindAnyObjectByType<InteractionDetector>();
    }

    void Update()
    {
        if (!isOpen)
            return;

        if (justOpened)
        {
            justOpened = false;
            return;
        }

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            NextCamera();
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            PreviousCamera();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
        }
    }

    public void OpenPanel()
    {
        if (cameraFeeds.Count == 0)
            return;

        isOpen = true;
        justOpened = true;
        IsAnyPanelOpen = true;
        currentIndex = 0;

        if (panelUI != null)
            panelUI.SetActive(true);

        if (interactionDetector != null)
            interactionDetector.SuppressPrompt();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowCurrentCamera();
    }

    public void ClosePanel()
    {
        isOpen = false;
        IsAnyPanelOpen = false;

        if (panelUI != null)
            panelUI.SetActive(false);

        if (interactionDetector != null)
            interactionDetector.ResumePrompt();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void NextCamera()
    {
        currentIndex = (currentIndex + 1) % cameraFeeds.Count;
        ShowCurrentCamera();
    }

    void PreviousCamera()
    {
        currentIndex = (currentIndex - 1 + cameraFeeds.Count) % cameraFeeds.Count;
        ShowCurrentCamera();
    }

    void ShowCurrentCamera()
    {
        if (screenImage != null && cameraFeeds.Count > 0)
        {
            screenImage.texture = cameraFeeds[currentIndex];
        }

        if (cameraLabel != null)
        {
            string label = (currentIndex < cameraLabels.Count && !string.IsNullOrEmpty(cameraLabels[currentIndex]))
                ? cameraLabels[currentIndex]
                : "CAM 0" + (currentIndex + 1);

            cameraLabel.text = label;
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}