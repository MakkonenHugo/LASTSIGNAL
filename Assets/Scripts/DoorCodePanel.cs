using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorCodePanel : MonoBehaviour
{
    public GameObject panelRoot;
    public TMP_Text codeDisplayText;
    public TMP_Text feedbackText;
    public Button closeButton;

    public int correctCode = 63;
    public int maxDigits = 3;

    public DoorController doorController;

    public float wrongCodeMessageDuration = 1.2f;
    public float cursorBlinkSpeed = 2f;

    private string enteredDigits = "";
    private bool isOpen = false;
    private Coroutine feedbackRoutine;
    private float blinkTimer = 0f;
    private bool blinkVisible = true;

    public static bool IsAnyPanelOpen { get; private set; } = false;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    void OnDisable()
    {
        if (isOpen)
        {
            isOpen = false;
            IsAnyPanelOpen = false;
        }
    }

    public void OpenPanel()
    {
        if (panelRoot == null) return;

        panelRoot.SetActive(true);
        isOpen = true;
        IsAnyPanelOpen = true;
        enteredDigits = "";
        blinkTimer = 0f;
        blinkVisible = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateDisplay();

        if (feedbackText != null)
            feedbackText.text = "";
    }

    public void ClosePanel()
    {
        if (panelRoot == null) return;

        panelRoot.SetActive(false);
        isOpen = false;
        IsAnyPanelOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isOpen)
            return;

        blinkTimer += Time.unscaledDeltaTime;
        if (blinkTimer >= 1f / cursorBlinkSpeed)
        {
            blinkTimer = 0f;
            blinkVisible = !blinkVisible;
            UpdateDisplay();
        }

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            SubmitCode();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
            return;
        }

        CheckDigitKey(Keyboard.current.digit0Key, '0');
        CheckDigitKey(Keyboard.current.digit1Key, '1');
        CheckDigitKey(Keyboard.current.digit2Key, '2');
        CheckDigitKey(Keyboard.current.digit3Key, '3');
        CheckDigitKey(Keyboard.current.digit4Key, '4');
        CheckDigitKey(Keyboard.current.digit5Key, '5');
        CheckDigitKey(Keyboard.current.digit6Key, '6');
        CheckDigitKey(Keyboard.current.digit7Key, '7');
        CheckDigitKey(Keyboard.current.digit8Key, '8');
        CheckDigitKey(Keyboard.current.digit9Key, '9');

        CheckDigitKey(Keyboard.current.numpad0Key, '0');
        CheckDigitKey(Keyboard.current.numpad1Key, '1');
        CheckDigitKey(Keyboard.current.numpad2Key, '2');
        CheckDigitKey(Keyboard.current.numpad3Key, '3');
        CheckDigitKey(Keyboard.current.numpad4Key, '4');
        CheckDigitKey(Keyboard.current.numpad5Key, '5');
        CheckDigitKey(Keyboard.current.numpad6Key, '6');
        CheckDigitKey(Keyboard.current.numpad7Key, '7');
        CheckDigitKey(Keyboard.current.numpad8Key, '8');
        CheckDigitKey(Keyboard.current.numpad9Key, '9');
    }

    void CheckDigitKey(KeyControl key, char digit)
    {
        if (key == null)
            return;

        if (key.wasPressedThisFrame && enteredDigits.Length < maxDigits)
        {
            enteredDigits += digit;
            blinkTimer = 0f;
            blinkVisible = true;
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        if (codeDisplayText == null)
            return;

        string display = enteredDigits;

        int remaining = maxDigits - enteredDigits.Length;
        for (int i = 0; i < remaining; i++)
        {
            bool isNextSlot = i == 0;
            if (isNextSlot)
                display += blinkVisible ? "_" : " ";
            else
                display += "_";
        }

        codeDisplayText.text = display;
    }

    void SubmitCode()
    {
        if (int.TryParse(enteredDigits, out int enteredValue) && enteredValue == correctCode)
        {
            ShowFeedback("ACCESS GRANTED", true);

            if (doorController != null)
                doorController.UnlockDoor();

            StartCoroutine(CloseAfterDelay(0.8f));
        }
        else
        {
            ShowFeedback("ACCESS DENIED", false);
            enteredDigits = "";
            UpdateDisplay();
        }
    }

    void ShowFeedback(string message, bool success)
    {
        if (feedbackText == null)
            return;

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackText.text = message;
        feedbackText.color = success ? new Color(0.4f, 1f, 0.5f) : new Color(1f, 0.3f, 0.3f);

        feedbackRoutine = StartCoroutine(ClearFeedbackAfterDelay());
    }

    IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(wrongCodeMessageDuration);

        if (feedbackText != null)
            feedbackText.text = "";
    }

    IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePanel();
    }
}