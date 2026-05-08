using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Panel</size></b></color>")]
    [SerializeField] private GameObject _pausePanel;

    [Header("<color=cyan><b><size=15>Buttons</size></b></color>")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _menuButton;
    #endregion

    #region State
    private bool _isPaused;
    #endregion

    #region Constants
    private const KeyCode PauseGamepadButton = KeyCode.JoystickButton7;
    #endregion

    #region Initialization
    private void Awake()
    {
        EnsureEventSystemInputModule();
        ConfigureNavigation(_resumeButton);
        ConfigureNavigation(_menuButton);

        if (_resumeButton != null)
            _resumeButton.onClick.AddListener(Resume);

        if (_menuButton != null)
            _menuButton.onClick.AddListener(GoToMenu);

        if (_pausePanel != null)
            _pausePanel.SetActive(false);
    }
    #endregion

    #region Input Handling
    private void EnsureEventSystemInputModule() // Utility method to ensure there is an EventSystem with an input module in the scene for UI interaction, adds one if missing
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        BaseInputModule inputModule = eventSystem.GetComponent<BaseInputModule>();
        if (inputModule == null)
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
    }

    private static void ConfigureNavigation(Selectable selectable)
    {
        if (selectable == null)
            return;

        Navigation navigation = selectable.navigation;
        navigation.mode = Navigation.Mode.Automatic;
        selectable.navigation = navigation;
    }

    private static void SelectButton(Button button)
    {
        if (button == null || EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(PauseGamepadButton))
            TogglePause();

        if (_isPaused && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            SelectButton(_resumeButton != null ? _resumeButton : _menuButton);
    }

    private void TogglePause()
    {
        if (_isPaused) Resume();
        else Pause();
    }
    #endregion

    #region State Management
    private void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        if (_pausePanel != null)
            _pausePanel.SetActive(true);
        SelectButton(_resumeButton != null ? _resumeButton : _menuButton);
        Debug.Log("<color=green><b>PauseUI:</b></color> Paused");
    }
            
    private void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
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
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
    #endregion
}
