using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PasswordMemoryPanel : MonoBehaviour
{
    public GameObject panelRoot;
    public TMP_Text displayText;
    public TMP_Text inputText;
    public TMP_Text feedbackText;
    public TMP_Text roundText;
    public InteractionDetector interactionDetector;

    public DialogueUI dialogueUI;
    public SpawnWhenTriggered rewardSpawn;

    [Header("Passwords (in order, easy to hard)")]
    public List<string> passwords = new List<string>()
    {
        "4271",
        "9K3P",
        "L02M9",
        "XR7-42Q",
        "J3FF-LIVES"
    };

    public float showDuration = 2.5f;
    public float resultMessageDuration = 1f;

    public static bool IsAnyPanelOpen { get; private set; } = false;

    private bool isOpen = false;
    private bool acceptingInput = false;
    private int currentRound = 0;
    private string currentInput = "";
    private bool rewardGiven = false;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
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
        if (isOpen) return;

        panelRoot.SetActive(true);
        isOpen = true;
        IsAnyPanelOpen = true;
        currentRound = 0;

        if (feedbackText != null)
            feedbackText.text = "";

        if (interactionDetector != null)
            interactionDetector.SuppressPrompt();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(RoundRoutine());
    }

    public void ClosePanel()
    {
        if (panelRoot == null) return;

        StopAllCoroutines();

        panelRoot.SetActive(false);
        isOpen = false;
        IsAnyPanelOpen = false;
        acceptingInput = false;

        if (interactionDetector != null)
            interactionDetector.ResumePrompt();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator RoundRoutine()
    {
        UpdateRoundText();

        string target = passwords[currentRound];

        if (displayText != null)
            displayText.text = target;

        if (inputText != null)
            inputText.text = "";

        currentInput = "";
        acceptingInput = false;

        yield return new WaitForSecondsRealtime(showDuration);

        if (displayText != null)
            displayText.text = "?????";

        acceptingInput = true;
    }

    void Update()
    {
        if (!isOpen)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
            return;
        }

        if (!acceptingInput)
            return;

        HandleTyping();

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SubmitAnswer();
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame && currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);

            if (inputText != null)
                inputText.text = currentInput;
        }
    }

    void HandleTyping()
    {
        foreach (KeyControl key in Keyboard.current.allKeys)
        {
            if (!key.wasPressedThisFrame)
                continue;

            char? c = KeyToChar(key.keyCode);

            if (c.HasValue)
            {
                currentInput += c.Value;

                if (inputText != null)
                    inputText.text = currentInput;
            }
        }
    }

    char? KeyToChar(Key key)
    {
        bool shift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

        switch (key)
        {
            case Key.A: return shift ? 'A' : 'a';
            case Key.B: return shift ? 'B' : 'b';
            case Key.C: return shift ? 'C' : 'c';
            case Key.D: return shift ? 'D' : 'd';
            case Key.E: return shift ? 'E' : 'e';
            case Key.F: return shift ? 'F' : 'f';
            case Key.G: return shift ? 'G' : 'g';
            case Key.H: return shift ? 'H' : 'h';
            case Key.I: return shift ? 'I' : 'i';
            case Key.J: return shift ? 'J' : 'j';
            case Key.K: return shift ? 'K' : 'k';
            case Key.L: return shift ? 'L' : 'l';
            case Key.M: return shift ? 'M' : 'm';
            case Key.N: return shift ? 'N' : 'n';
            case Key.O: return shift ? 'O' : 'o';
            case Key.P: return shift ? 'P' : 'p';
            case Key.Q: return shift ? 'Q' : 'q';
            case Key.R: return shift ? 'R' : 'r';
            case Key.S: return shift ? 'S' : 's';
            case Key.T: return shift ? 'T' : 't';
            case Key.U: return shift ? 'U' : 'u';
            case Key.V: return shift ? 'V' : 'v';
            case Key.W: return shift ? 'W' : 'w';
            case Key.X: return shift ? 'X' : 'x';
            case Key.Y: return shift ? 'Y' : 'y';
            case Key.Z: return shift ? 'Z' : 'z';
            case Key.Digit0: return '0';
            case Key.Digit1: return '1';
            case Key.Digit2: return '2';
            case Key.Digit3: return '3';
            case Key.Digit4: return '4';
            case Key.Digit5: return '5';
            case Key.Digit6: return '6';
            case Key.Digit7: return '7';
            case Key.Digit8: return '8';
            case Key.Digit9: return '9';
            case Key.Minus: return '-';
            default: return null;
        }
    }

    void SubmitAnswer()
    {
        acceptingInput = false;

        bool correct = string.Equals(currentInput, passwords[currentRound], System.StringComparison.OrdinalIgnoreCase);

        if (correct)
        {
            currentRound++;

            if (currentRound >= passwords.Count)
            {
                StartCoroutine(HandleFullSuccess());
            }
            else
            {
                ShowFeedback("CORRECT", true);
                StartCoroutine(NextRoundAfterDelay());
            }
        }
        else
        {
            ShowFeedback("WRONG - RESTARTING", false);
            currentRound = 0;
            StartCoroutine(NextRoundAfterDelay());
        }
    }

    IEnumerator NextRoundAfterDelay()
    {
        yield return new WaitForSecondsRealtime(resultMessageDuration);
        StartCoroutine(RoundRoutine());
    }

    IEnumerator HandleFullSuccess()
    {
        ShowFeedback("ACCESS GRANTED", true);

        yield return new WaitForSecondsRealtime(resultMessageDuration);

        ClosePanel();

        if (!rewardGiven && rewardSpawn != null)
        {
            rewardGiven = true;
            rewardSpawn.Trigger();
        }
    }

    void ShowFeedback(string message, bool success)
    {
        if (feedbackText == null)
            return;

        feedbackText.text = message;
        feedbackText.color = success ? new Color(0.4f, 1f, 0.5f) : new Color(1f, 0.3f, 0.3f);
    }

    void UpdateRoundText()
    {
        if (roundText != null)
            roundText.text = "Password " + (currentRound + 1) + " / " + passwords.Count;
    }
}