using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    #region Singleton
    private static GameSceneManager _instance;
    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameSceneManager");
                _instance = go.AddComponent<GameSceneManager>();
            }
            return _instance;
        }
    }

    #endregion

    #region Inspector Settings
    [Header("<color=orange><b><size=15>Scene Names</size></b></color>")] 
    [SerializeField] private string _gameScene = "SampleScene";
    [SerializeField] private string _mainMenuScene = "MainMenu";
    [SerializeField] private string _gameOverScene = "GameOver";
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
    #endregion

    #region Scene Loading
    /// <summary>Start a new game session - enables spawning and loads game scene.</summary>
    public void PlayGame() // Call this method to start the game by loading the game scene
    {
        Time.timeScale = 1f;
        EnemyManager.Instance?.StartSpawning();
        SceneManager.LoadScene(_gameScene);
    }

    public void LoadMenu() // Call this method to load the main menu scene
    {
        Time.timeScale = 1f;
        EnemyManager.Instance?.StopSpawning();
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void LoadGameOver() // Call this method when the player dies to load the game over scene
    {
        Time.timeScale = 1f;
        EnemyManager.Instance?.StopSpawning();
        SceneManager.LoadScene(_gameOverScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>Load any scene by name - utility for debug buttons or future features.</summary>
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        SceneManager.LoadScene(sceneName);
    }
    #endregion
}
