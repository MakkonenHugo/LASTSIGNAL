using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HackingPanel : MonoBehaviour
{
    public GameObject panelRoot;
    public RectTransform barTrack;
    public RectTransform movingIndicator;
    public RectTransform successZone;
    public TMP_Text feedbackText;
    public TMP_Text streakText;
    public TMP_Text logText;
    public Button closeButton;

    [Header("Always-visible lockout HUD")]
    public GameObject lockoutHudRoot;
    public TMP_Text lockoutHudText;

    [Header("Success Behaviour")]
    public bool useDirectDoorUnlock = true;
    public DoorController doorController;
    public int revealedCode = 63;

    public int requiredStreak = 10;
    public int easyPhaseEnd = 3;
    public int mediumPhaseEnd = 7;

    public float easySpeed = 220f;
    public float mediumSpeed = 320f;
    public float hardSpeed = 420f;

    public float easyZoneWidth = 110f;
    public float mediumZoneWidth = 90f;
    public float hardZoneWidth = 80f;

    public int maxAttempts = 3;
    public float lockoutDuration = 60f;
    public float resultMessageDuration = 1f;
    public float closeDelayOnSuccess = 0.8f;
    public float closeDelayOnCodeReveal = 2f;

    public static bool IsAnyPanelOpen { get; private set; } = false;

    private bool isOpen = false;
    private bool inputLocked = false;
    private int attemptsRemaining;
    private int currentStreak;
    private float trackHalfWidth;
    private float indicatorX;
    private int direction = 1;
    private float currentIndicatorSpeed;
    private Coroutine lockoutRoutine;
    private Coroutine feedbackRoutine;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (lockoutHudRoot != null)
            lockoutHudRoot.SetActive(false);

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

    public bool IsLockedOut => lockoutRoutine != null;

    public void OpenPanel()
    {
        if (panelRoot == null) return;
        if (lockoutRoutine != null) return;

        panelRoot.SetActive(true);
        isOpen = true;
        IsAnyPanelOpen = true;
        inputLocked = false;
        attemptsRemaining = maxAttempts;
        currentStreak = 0;

        if (barTrack != null)
            trackHalfWidth = barTrack.rect.width / 2f;

        if (feedbackText != null)
            feedbackText.text = "";

        if (logText != null)
            logText.text = "";

        SetupRound();
        UpdateStreakText();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    void SetupRound()
    {
        float speed = GetSpeedForStreak(currentStreak);
        float zoneWidth = GetZoneWidthForStreak(currentStreak);

        if (successZone != null)
        {
            successZone.sizeDelta = new Vector2(zoneWidth, successZone.sizeDelta.y);
            float zoneOffset = Random.Range(-trackHalfWidth + zoneWidth, trackHalfWidth - zoneWidth);
            successZone.anchoredPosition = new Vector2(zoneOffset, successZone.anchoredPosition.y);
        }

        indicatorX = -trackHalfWidth;
        direction = 1;
        currentIndicatorSpeed = speed;

        if (movingIndicator != null)
            movingIndicator.anchoredPosition = new Vector2(indicatorX, movingIndicator.anchoredPosition.y);
    }

    float GetSpeedForStreak(int streak)
    {
        if (streak < easyPhaseEnd)
            return easySpeed;

        if (streak < mediumPhaseEnd)
            return mediumSpeed;

        return hardSpeed;
    }

    float GetZoneWidthForStreak(int streak)
    {
        if (streak < easyPhaseEnd)
            return easyZoneWidth;

        if (streak < mediumPhaseEnd)
            return mediumZoneWidth;

        return hardZoneWidth;
    }

    void Update()
    {
        if (!isOpen)
            return;

        if (inputLocked)
            return;

        indicatorX += direction * currentIndicatorSpeed * Time.unscaledDeltaTime;

        if (indicatorX >= trackHalfWidth)
        {
            indicatorX = trackHalfWidth;
            direction = -1;
        }
        else if (indicatorX <= -trackHalfWidth)
        {
            indicatorX = -trackHalfWidth;
            direction = 1;
        }

        if (movingIndicator != null)
            movingIndicator.anchoredPosition = new Vector2(indicatorX, movingIndicator.anchoredPosition.y);

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TryHit();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
        }
    }

    void TryHit()
    {
        bool hit = IsIndicatorInZone();

        if (hit)
        {
            HandleHit();
        }
        else
        {
            HandleMiss();
        }
    }

    bool IsIndicatorInZone()
    {
        if (successZone == null)
            return false;

        float zoneCenter = successZone.anchoredPosition.x;
        float zoneHalf = successZone.sizeDelta.x / 2f;

        return indicatorX >= zoneCenter - zoneHalf && indicatorX <= zoneCenter + zoneHalf;
    }

    void HandleHit()
    {
        currentStreak++;
        AppendLogLine();
        UpdateStreakText();

        if (currentStreak >= requiredStreak)
        {
            HandleFullSuccess();
            return;
        }

        ShowFeedback("HIT", true);
        SetupRound();
    }

    void HandleFullSuccess()
    {
        inputLocked = true;

        if (useDirectDoorUnlock)
        {
            ShowFeedback("ACCESS GRANTED", true);

            if (doorController != null)
            {
                doorController.UnlockDoor();
                doorController.OpenDoor();
            }

            StartCoroutine(CloseAfterDelay(closeDelayOnSuccess));
        }
        else
        {
            ShowFeedback("ACCESS GRANTED - CODE: " + revealedCode, true);

            StartCoroutine(CloseAfterDelay(closeDelayOnCodeReveal));
        }
    }

    void HandleMiss()
    {
        currentStreak = 0;
        UpdateStreakText();

        attemptsRemaining--;

        if (attemptsRemaining <= 0)
        {
            ShowFeedback("SYSTEM LOCKED", false);
            StartCoroutine(StartLockoutAfterDelay());
        }
        else
        {
            ShowFeedback("FAILED", false);
            SetupRound();
        }
    }

    void AppendLogLine()
    {
        if (logText == null)
            return;

        string newLine = "hugoAPT: password=logged (" + currentStreak + "/" + requiredStreak + ")";

        if (string.IsNullOrEmpty(logText.text))
            logText.text = newLine;
        else
            logText.text += "\n" + newLine;
    }

    void UpdateStreakText()
    {
        if (streakText != null)
            streakText.text = "Streak: " + currentStreak + "/" + requiredStreak + "   Attempts left: " + attemptsRemaining;
    }

    void ShowFeedback(string message, bool success)
    {
        if (feedbackText == null)
            return;

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackText.text = message;
        feedbackText.color = success ? new Color(0.4f, 1f, 0.5f) : new Color(1f, 0.3f, 0.3f);

        if (!success)
            feedbackRoutine = StartCoroutine(ClearFeedbackAfterDelay());
    }

    IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(resultMessageDuration);

        if (feedbackText != null)
            feedbackText.text = "";
    }

    IEnumerator StartLockoutAfterDelay()
    {
        inputLocked = true;

        yield return new WaitForSeconds(resultMessageDuration);

        ClosePanel();

        if (lockoutRoutine != null)
            StopCoroutine(lockoutRoutine);

        lockoutRoutine = StartCoroutine(LockoutCountdown());
    }

    IEnumerator LockoutCountdown()
    {
        if (lockoutHudRoot != null)
            lockoutHudRoot.SetActive(true);

        float remaining = lockoutDuration;

        while (remaining > 0f)
        {
            if (lockoutHudText != null)
                lockoutHudText.text = "System locked, retry in " + Mathf.CeilToInt(remaining) + "s";

            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (lockoutHudRoot != null)
            lockoutHudRoot.SetActive(false);

        lockoutRoutine = null;
    }

    IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePanel();
    }
}