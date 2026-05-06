using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private TMP_Text _finalScoreText;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (_restartButton != null)
            _restartButton.onClick.AddListener(() => GameSceneManager.Instance.PlayGame());

        if (_menuButton != null)
            _menuButton.onClick.AddListener(() => GameSceneManager.Instance.LoadMenu());

        if (_finalScoreText != null)
            _finalScoreText.raycastTarget = false;
        else
            Debug.LogWarning("GameOverUI: Final score text is not assigned.");

    }

    private void Start()
    {
        if (_finalScoreText != null && GameManager.Instance != null)
        {
            int kills = GameManager.Instance.KillCount;
            int difficulty = GameManager.Instance.DifficultyLevel;
            System.TimeSpan time = GameManager.Instance.Timer;
            Debug.Log($"<color=green><b>GameOverUI:</b></color> Displaying final score - Kills: {kills}, Difficulty: {difficulty}, Time: {time:mm\\:ss}");

            _finalScoreText.text =
                $"<color=yellow><b>Final Score : </b></color> " +
                $"<color=red>Kills:</color> {kills} " +
                $"<color=#ff8c00>Difficulty:</color> {difficulty} " +
                $"<color=green>Time:</color> {time:mm\\:ss}";
        }
    }
}
