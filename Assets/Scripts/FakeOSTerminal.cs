using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class FakeOSTerminal : MonoBehaviour
{
    public GameObject panelRoot;
    public GameObject desktopView;
    public GameObject programView;
    public GameObject confirmView;

    public Button simulationManagerIcon;
    public Button deleteButton;
    public Button confirmYesButton;
    public Button confirmNoButton;

    public TMP_Text statusText;
    public TMP_Text confirmText;

    public InteractionDetector interactionDetector;

    public System.Action OnFilesDeleted;

    private bool isOpen = false;
    private bool filesDeleted = false;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (simulationManagerIcon != null)
            simulationManagerIcon.onClick.AddListener(OpenProgram);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(ShowConfirm);

        if (confirmYesButton != null)
            confirmYesButton.onClick.AddListener(ConfirmDelete);

        if (confirmNoButton != null)
            confirmNoButton.onClick.AddListener(CancelConfirm);
    }

    void Update()
    {
        if (!isOpen)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
        }
    }

    public void OpenPanel()
    {
        if (panelRoot == null) return;

        panelRoot.SetActive(true);
        isOpen = true;

        ShowDesktop();

        if (deleteButton != null)
            deleteButton.interactable = !filesDeleted;

        if (confirmYesButton != null)
            confirmYesButton.interactable = !filesDeleted;

        if (confirmNoButton != null)
            confirmNoButton.interactable = !filesDeleted;

        if (interactionDetector != null)
            interactionDetector.SuppressPrompt();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePanel()
    {
        if (panelRoot == null) return;

        panelRoot.SetActive(false);
        isOpen = false;

        if (interactionDetector != null)
            interactionDetector.ResumePrompt();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ShowDesktop()
    {
        SetView(desktopView);
    }

    void OpenProgram()
    {
        if (filesDeleted)
            return;

        SetView(programView);

        if (statusText != null)
            statusText.text = "SIGNAL_ARCHIVE_00X.dat\nSIGNAL_CORE_CONFIG.sys\nEXPERIMENT_LOG_ALL.rec\n\nSTATUS: ACTIVE";
    }

    void ShowConfirm()
    {
        SetView(confirmView);

        if (confirmText != null)
            confirmText.text = "ARE YOU SURE?\nThis action cannot be undone.";
    }

    void CancelConfirm()
    {
        SetView(programView);
    }

    void ConfirmDelete()
    {
        filesDeleted = true;

        if (deleteButton != null)
            deleteButton.interactable = false;

        if (confirmYesButton != null)
            confirmYesButton.interactable = false;

        if (confirmNoButton != null)
            confirmNoButton.interactable = false;

        StartCoroutine(DeleteSequence());
    }

    IEnumerator DeleteSequence()
    {
        SetView(programView);

        if (statusText != null)
            statusText.text = "DELETING...";

        yield return new WaitForSecondsRealtime(0.6f);

        if (statusText != null)
            statusText.text = "DELETING...\nSIGNAL_ARCHIVE_00X.dat [REMOVED]";

        yield return new WaitForSecondsRealtime(0.5f);

        if (statusText != null)
            statusText.text = "DELETING...\nSIGNAL_ARCHIVE_00X.dat [REMOVED]\nSIGNAL_CORE_CONFIG.sys [REMOVED]";

        yield return new WaitForSecondsRealtime(0.5f);

        if (statusText != null)
            statusText.text = "DELETING...\nSIGNAL_ARCHIVE_00X.dat [REMOVED]\nSIGNAL_CORE_CONFIG.sys [REMOVED]\nEXPERIMENT_LOG_ALL.rec [REMOVED]\n\nSTATUS: TERMINATED";

        yield return new WaitForSecondsRealtime(1.2f);

        ClosePanel();

        OnFilesDeleted?.Invoke();
    }

    void SetView(GameObject viewToShow)
    {
        if (desktopView != null) desktopView.SetActive(viewToShow == desktopView);
        if (programView != null) programView.SetActive(viewToShow == programView);
        if (confirmView != null) confirmView.SetActive(viewToShow == confirmView);
    }

    public bool AreFilesDeleted()
    {
        return filesDeleted;
    }
}