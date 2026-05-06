using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public const string GameScene = "SampleScene";
    public const string MainMenuScene = "MainMenu";
    public const string GameOverScene = "GameOver";

    [Header("<color=orange><b><size=15>Scene Names</size></b></color>")]
    [SerializeField] private string _gameScene = GameScene;
    [SerializeField] private string _mainMenuScene = MainMenuScene;
    [SerializeField] private string _gameOverScene = GameOverScene;

    public void LoadGameScene()
    {
        LoadScene(_gameScene);
    }

    public void LoadMainMenuScene()
    {
        LoadScene(_mainMenuScene);
    }

    public void LoadGameOverScene()
    {
        LoadScene(_gameOverScene);
    }

    public void QuitGame()
    {
        GameManager.QuitGame();
    }

    private void LoadScene(string sceneName)
    {
        GameManager.LoadSceneByName(sceneName);
    }
}

