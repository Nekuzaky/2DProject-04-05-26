using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>Panel</size></b></color>")]
    [SerializeField] private GameObject _pausePanel;

    [Header("<color=cyan><b><size=15>Buttons</size></b></color>")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _menuButton;

    private bool _isPaused;

    private void Awake()
    {
        EnsureEventSystemInputModule();

        if (_resumeButton != null)
            _resumeButton.onClick.AddListener(Resume);

        if (_menuButton != null)
            _menuButton.onClick.AddListener(GoToMenu);

        if (_pausePanel != null)
            _pausePanel.SetActive(false);
    }


    private void EnsureEventSystemInputModule()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
            return;

        BaseInputModule inputModule = eventSystem.GetComponent<BaseInputModule>();
        if (inputModule == null)
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void TogglePause()
    {
        if (_isPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        if (_pausePanel != null)
            _pausePanel.SetActive(true);
        Debug.Log("<color=green><b>PauseUI:</b></color> Paused");
    }

    private void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (_pausePanel != null)
            _pausePanel.SetActive(false);
        Debug.Log("<color=green><b>PauseUI:</b></color> Resumed");
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadMenu();
        Debug.Log("<color=green><b>PauseUI:</b></color> Returning to Main Menu");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
