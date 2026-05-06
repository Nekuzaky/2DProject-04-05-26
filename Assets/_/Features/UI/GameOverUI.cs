using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        if (_restartButton != null)
            _restartButton.onClick.AddListener(() => GameSceneManager.Instance.PlayGame()); // Restart the game by loading the game scene
            
        if (_menuButton != null)
            _menuButton.onClick.AddListener(() => GameSceneManager.Instance.LoadMenu()); // Load the main menu scene

        if (_finalScoreText != null)
            _finalScoreText.raycastTarget = false;
        else
            Debug.LogWarning("GameOverUI: Final score text is not assigned.");
    }
    #endregion

    #region Score Display
    private void Start()
    {
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

        Debug.Log($"<color=green><b>GameOverUI:</b></color> Displaying final score - Kills: {kills}, Difficulty: {difficulty}, Time: {time:mm\\:ss}");

        _finalScoreText.text =
            $"<color=yellow><b>Final Score : </b></color> " +
            $"<color=red>Kills:</color> {kills} " +
            $"<color=#ff8c00>Difficulty:</color> {difficulty} " +
            $"<color=green>Time:</color> {time:mm\\:ss}";
    }
    #endregion
}
