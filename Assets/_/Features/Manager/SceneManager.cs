using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager _instance;
    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameSceneManager");
                _instance = go.AddComponent<GameSceneManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("<color=orange><b><size=15>Scene Names</size></b></color>")]
    [SerializeField] private string _gameScene = "SampleScene";
    [SerializeField] private string _mainMenuScene = "MainMenu";
    [SerializeField] private string _gameOverScene = "GameOver";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        EnemyManager.Instance?.StartSpawning();
        SceneManager.LoadScene(_gameScene);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        EnemyManager.Instance?.StopSpawning();
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void LoadGameOver()
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

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        SceneManager.LoadScene(sceneName);
    }
}
