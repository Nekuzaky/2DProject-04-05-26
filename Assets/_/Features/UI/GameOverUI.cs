using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
    #region Inspector Settings
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private TMP_Text _finalScoreText;
    #endregion

    #region UI Setup
    private void Awake()
    {
        Time.timeScale = 1f;
        EnsureEventSystemInputModule();
        ConfigureNavigation(_restartButton);
        ConfigureNavigation(_menuButton);

        if (_restartButton != null)
            _restartButton.onClick.AddListener(() => GameSceneManager.Instance.PlayGame()); // Restart the game by loading the game scene
            
        if (_menuButton != null)
            _menuButton.onClick.AddListener(() => GameSceneManager.Instance.LoadMenu()); // Load the main menu scene

        if (_finalScoreText != null)
            _finalScoreText.raycastTarget = false;
        else
            GameLogger.LogWarning("GameOverUI: Final score text is not assigned.");
    }

    private void Update()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            SelectButton(_restartButton != null ? _restartButton : _menuButton);
    }
    #endregion

    #region Score Display
    private void Start()
    {
        SelectButton(_restartButton != null ? _restartButton : _menuButton);

        if (_finalScoreText == null)
            return;

        int kills;
        int difficulty;
        System.TimeSpan time;

        if (GameRunSummary.HasData)
        {
            kills = GameRunSummary.Kills;
            difficulty = GameRunSummary.Difficulty;
            time = GameRunSummary.Time;
        }
        else if (GameManager.Instance != null)
        {
            kills = GameManager.Instance.KillCount; 
            difficulty = GameManager.Instance.DifficultyLevel;
            time = GameManager.Instance.Timer; // Fallback to GameManager data if GameRunSummary is not available
        }
        else
        {
            kills = 0;
            difficulty = 0;
            time = System.TimeSpan.Zero;
        }

        GameLogger.Log($"<color=green><b>GameOverUI:</b></color> Displaying final score - Kills: {kills}, Difficulty: {difficulty}, Time: {time:mm\\:ss}");

        _finalScoreText.text =
            $"<color=#ff2222><b>Final Score</b></color>\n"  +
            $"<color=#cc0000>Kills</color> <color=#ffffff>{kills}</color>    " +
            $"<color=#cc0000>Difficulty</color> <color=#ffffff>{difficulty}</color>    " +
            $"<color=#cc0000>Time</color> <color=#ffffff>{time:mm\\:ss}</color>";
    }

    private void EnsureEventSystemInputModule()
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
    #endregion
}
